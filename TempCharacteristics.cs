using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Diagnostics;

public struct TEMP_CHARACTERISTICS_T
{
    public string SerialNumber;
    public UInt32 MRWnumber;
    public byte Channel;
    public int[] TempCorrValue;
    public bool On;
    public bool Linearization;
}

namespace MRWCalculator
{
    public partial class TempCharacteristics : Form
    {
        public enum TASK_STATE_T
        {
            Init = 0,
            Idle,
            Send,
            Receive,
            WaitForOkay,
            Error,
            Okay
        }
        private enum COMMAND_T
        {
            None,
            Power_On,
            Power_Off,
            CommMode,
            ProgMode_CH0,
            ProgMode_CH1,
        }
        private struct COMMUNICATION_TASK_T
        {
            public TASK_STATE_T TaskState;
            public COMMAND_T Command;
            public bool StayInLoop;
            public byte ReplyCnt;
            public byte[] ReceiveBuffer;
            public int ReceivePtr;
            public byte Callback;
        }

        #region Members
        CELL_CHARACTERISTICS_T Params;
        COMMUNICATION_TASK_T TaskParams;
        Stopwatch Timeout = new Stopwatch();
        SetupLoadcellInterface CSetupLoadcellInterface = new SetupLoadcellInterface();
        TextBox[] TextCorrValue = new TextBox[21];
        string FileName = "";
        #endregion
        #region Methods
        public TempCharacteristics()
        {
            InitializeComponent();

            Params.TempCharacteristics.Linearization = false;

            SetTextBoxLinks();
        }
        private void CalcTempCompensationTableByDefault()
        {
            Params.TempCharacteristics.TempCorrValue[0] = Properties.Settings.Default.TempCorrVal[0] = Properties.Settings.Default.TempCorrVal_70_Default;
            Params.TempCharacteristics.TempCorrValue[20] = Properties.Settings.Default.TempCorrVal[20] = -Properties.Settings.Default.TempCorrVal_70_Default;

            for(int Index = 1; Index < 20; Index++)
            {
                Params.TempCharacteristics.TempCorrValue[Index] = Properties.Settings.Default.TempCorrVal[Index] = Properties.Settings.Default.TempCorrVal_70_Default - (int)(2 * (float)Properties.Settings.Default.TempCorrVal_70_Default/100 * (5 * Index));
            }
        }
        private void ChangeGradientTempCompensationTable()
        {
            bool Error;
            DialogResult Result;
            string Gradient = "";
            double Factor = 1;

            Error = false;

            Result = InputBox("Werteingabe", "Um wieviel Prozent sollen die Kennlinienpunkte verändert werden?", ref Gradient);

            if ((Result == DialogResult.OK) && (Gradient.Length > 0))
            {
                try
                {
                    Factor = Convert.ToDouble(Gradient);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Auswerten ihrer Eingabe");
                    Error = true;
                }
            }
            else
            {
                Error = true;
            }

            if (Error == false)
            {
                Factor = 1 + Factor / 100;

                for (int Index = 0; Index < 21; Index++)
                {
                    Params.TempCharacteristics.TempCorrValue[Index] = Properties.Settings.Default.TempCorrVal[Index] = (int)(Factor * (double)Properties.Settings.Default.TempCorrVal[Index]);
                }
                Params.TempCharacteristics.MRWnumber = 0;
                Params.TempCharacteristics.Linearization = false;

                SaveCharacteristics();
                SetTempCorrValues();
            }
        }
        private void Chart_Init()
        {
            chartTempCharacteristics.Series.Clear();

            if (Properties.Settings.Default.TempCorrBasedOnMRW == 0)
            {
                chartTempCharacteristics.Series.Add(new System.Windows.Forms.DataVisualization.Charting.Series(""));
            }
            else
            {
                chartTempCharacteristics.Series.Add(new System.Windows.Forms.DataVisualization.Charting.Series(Properties.Settings.Default.TempCorrBasedOnMRW.ToString() + " - Kanal " + Properties.Settings.Default.TempCorrBasedOnChannel.ToString()));
            }
            chartTempCharacteristics.Series[0].Color = Color.Red;
            chartTempCharacteristics.Series[0].BorderWidth = 1;
            chartTempCharacteristics.Series[0].ChartType = SeriesChartType.Line;
            chartTempCharacteristics.ChartAreas[0].AxisX.Minimum = -30;
            chartTempCharacteristics.ChartAreas[0].AxisX.Maximum = 70;
        }
        private void ClearTempCompensationTable()
        {
            for (int Index = 0; Index < 21; Index++)
            {
                Params.TempCharacteristics.TempCorrValue[Index] = Properties.Settings.Default.TempCorrVal[Index] = 0;
            }
            Params.TempCharacteristics.MRWnumber = 0;
            Params.TempCharacteristics.Linearization = false;

            SaveCharacteristics();
            SetTempCorrValues();

            chartTempCharacteristics.ChartAreas[0].AxisY.Minimum = -1;
            chartTempCharacteristics.ChartAreas[0].AxisY.Maximum = 1;
        }
        public bool GetCharacteristicsFromFile()
        {
            bool Error;
            DialogResult Result;
            string Path;
            string Dataline;
            int Pos;
            FileStream DataStream;

            Error = false;

            OpenFileDialog FileDlg = new OpenFileDialog()
            {
                Multiselect = false,
                InitialDirectory = Properties.Settings.Default.FileOpenPath,
                Filter = "(*.csv)|*.csv|" + "Texte (*.txt)|*.txt|" + "(*.tcr) | *.tcr | " + "Alle Dateien (*.*)|*.*",
                Title = "Datei mit den zu ladenden Kennliniendaten wählen"
            };

            Result = FileDlg.ShowDialog();

            if (Result == DialogResult.OK)
            {
                FileName = FileDlg.FileName;

                if (FileName.Length > 0)
                {
                    Pos = FileName.LastIndexOf("\\");
                    if (Pos >= 0)
                    {
                        Path = FileName.Substring(0, Pos);
                        if (Path != Properties.Settings.Default.FileOpenPath)
                        {
                            Properties.Settings.Default.FileOpenPath = Path;
                            Properties.Settings.Default.Save();
                        }

                        try
                        {
                            DataStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                            StreamReader Reader = new StreamReader(DataStream);
                            bool bFirstLine = true;
                            uint Line;

                            Line = 0;

                            while ((Reader.Peek() != 0) && (Error == false) && (Line < 1))
                            {
                                Dataline = Reader.ReadLine();
                                if (bFirstLine == false)
                                {
                                    Error = EvaluateFileData(Dataline, Line++, false);
                                }
                                else
                                {
                                    bFirstLine = false;
                                }
                            }
                            Reader.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Fehler beim Öffnen der Datei (" + ex.Message + ")");
                            Error = true;
                        }
                        if (Error == false)
                        {
                            this.Text = "Temperatur - Kennlinie - " + FileName;
                            speichernToolStripMenuItem.Enabled = true;

                            Params.TempCharacteristics.Linearization = false;

                            SaveCharacteristics();

                            SetTempCorrValues();
                        }
                    }
                }
            }

            return (Error);
        }
        public System.IO.Ports.SerialPort GetSerialInterface()
        {
            return (sptLoadCell);
        }
        public int GetTempCharacteristicsValue(int Temperature)
        {
            int TempCharacteristicsValue;
            byte Index = 0;

            TempCharacteristicsValue = 0;

            if (Temperature > 70)
            {
                TempCharacteristicsValue = (int)(Params.TempCharacteristics.TempCorrValue[0] - (Params.TempCharacteristics.TempCorrValue[1] - Params.TempCharacteristics.TempCorrValue[0]) / 5 * (Temperature - 70));
            }
            else if (Temperature < -30)
            {
                TempCharacteristicsValue = (int)(Params.TempCharacteristics.TempCorrValue[20] + (Params.TempCharacteristics.TempCorrValue[19] - Params.TempCharacteristics.TempCorrValue[20]) / 5 * (Temperature + 30));
            }
            else
            {
                if ((Temperature % 5) == 0)
                {
                    Index = (byte)((70 - Temperature) / 5);
                    TempCharacteristicsValue = Params.TempCharacteristics.TempCorrValue[Index];
                }
                else
                {
                    Index = (byte)((70 - Temperature) / 5);
                    TempCharacteristicsValue = (int)(Params.TempCharacteristics.TempCorrValue[Index] + (Params.TempCharacteristics.TempCorrValue[Index + 1] - Params.TempCharacteristics.TempCorrValue[Index]) / 5 * (5 - Temperature % 5));
                }
            }

            return (TempCharacteristicsValue);
        }
        public void Init(TEMP_CHARACTERISTICS_T TCParams, bool ByDefault)
        {
            Params.TempCharacteristics = TCParams;
            //Params.TempCharacteristics.TempCorrValue = new int[21];
            Chart_Init();

            if (ByDefault == true)
            {
                CalcTempCompensationTableByDefault();
            }
            else
            {
                for (int Index = 0; Index < 21; Index++)
                {
                    Params.TempCharacteristics.TempCorrValue[Index] = Properties.Settings.Default.TempCorrVal[Index];
                }
            }
            SetTempCorrValues();

            GetTempCharacteristicsValue(69);
            GetTempCharacteristicsValue(66);
            GetTempCharacteristicsValue(70);
            GetTempCharacteristicsValue(-30);
            GetTempCharacteristicsValue(80);
            GetTempCharacteristicsValue(-40);
        }
        private bool EvaluateFileData(string Dataline, uint Line, bool Import)
        {
            bool Error;
            byte Index;

            Error = false;
            Index = 0;

            var Dataparts = Dataline.Split(';');

            if (Import == true)
            {
                if (Dataparts.Length == 8)
                {
                    Index = (byte)((70 - Convert.ToInt32((string)Dataparts[6])) / 5);

                    if ((Index >= 0) || (Index <= 20))
                    {
                        Params.TempCharacteristics.TempCorrValue[Index] = Convert.ToInt32((string)Dataparts[7]);

                        if (Line == 0)
                        {
                            Params.TempCharacteristics.MRWnumber = Convert.ToUInt32((string)Dataparts[1]);
                            Params.TempCharacteristics.SerialNumber = Dataparts[2];
                            Params.TempCharacteristics.Channel = Convert.ToByte((string)Dataparts[5]);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Datenfehler - Temperaturwert ungültig\nBitte Importdatei überprüfen", "Dateifehler", MessageBoxButtons.OK);
                        Error = true;
                    }
                }
                else
                {
                    MessageBox.Show("Anzahl der Daten im Datenstring ist nicht korrekt\nBitte Importdatei überprüfen", "Dateifehler", MessageBoxButtons.OK);
                    Error = true;
                }
            }
            else
            {
                if (Dataparts.Length == 24)
                {

                    Params.TempCharacteristics.MRWnumber = Convert.ToUInt32((string)Dataparts[0]);
                    Params.TempCharacteristics.SerialNumber = Dataparts[1];
                    Params.TempCharacteristics.Channel = Convert.ToByte((string)Dataparts[2]);

                    for (Index = 3; Index < 24; Index++)
                    {
                        Params.TempCharacteristics.TempCorrValue[Index - 3] = Convert.ToInt32((string)Dataparts[Index]);
                    }
                }
                else
                {
                    MessageBox.Show("Anzahl der Daten im Datenstring ist nicht korrekt\nBitte Datei überprüfen", "Dateifehler", MessageBoxButtons.OK);
                    Error = true;
                }
            }
            return (Error);
        }
        private void ImportCharacteristics()
        {
            bool Error;
            DialogResult Result;
            string Path;
            string Dataline;
            int Pos;
            FileStream DataStream;

            Error = false;

            OpenFileDialog FileDlg = new OpenFileDialog()
            {
                Multiselect = false,
                InitialDirectory = Properties.Settings.Default.FileOpenPathImport,
                Filter = "(*.csv)|*.csv|" + "Texte (*.txt)|*.txt|" + "Alle Dateien (*.*)|*.*",
                Title = "Datei mit den zu importierenden Kennliniendaten wählen"
                
            };

            Result = FileDlg.ShowDialog();

            if (Result == DialogResult.OK)
            {
                FileName = FileDlg.FileName;

                if (FileName.Length > 0)
                {
                    Pos = FileName.LastIndexOf("\\");
                    if (Pos >= 0)
                    {
                        Path = FileName.Substring(0, Pos);
                        if (Path != Properties.Settings.Default.FileOpenPathImport)
                        {
                            Properties.Settings.Default.FileOpenPathImport = Path;
                            Properties.Settings.Default.Save();
                        }

                        try
                        {
                            DataStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                            StreamReader Reader = new StreamReader(DataStream);
                            bool bFirstLine = true;
                            uint Line;

                            Line = 0;

                            while ((Reader.Peek() != 0) && (Error == false) && (Line <= 20))
                            {
                                Dataline = Reader.ReadLine();
                                if (bFirstLine == false)
                                {
                                    Error = EvaluateFileData(Dataline, Line++, true);
                                }
                                else
                                {
                                    bFirstLine = false;
                                }
                            }
                            Reader.Close();
                            FileName = "";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Fehler beim Öffnen der Datei (" + ex.Message + ")");
                            Error = true;
                        }
                        if (Error == false)
                        {
                            Params.TempCharacteristics.Linearization = false;

                            SaveCharacteristics();

                            SetTempCorrValues();
                        }
                    }
                }
            }
        }
        private bool ImportFromLoadCell()
        {
            bool Error;

            Error = false;

            return (Error);
        }
        public DialogResult SetupLoadCellCommunication()
        {
            DialogResult Result;

            Result = DialogResult.OK;

            Result = CSetupLoadcellInterface.ShowDialog();

            switch (Result)
            {
                case DialogResult.OK:
                    Params.Celltype = CSetupLoadcellInterface.GetCelltype();
                    if (Properties.Settings.Default.LoadCellType != (byte)Params.Celltype)
                    {
                        Properties.Settings.Default.LoadCellType = (byte)Params.Celltype;
                        Properties.Settings.Default.Save();
                    }
                    break;

                case DialogResult.Cancel:
                    Properties.Settings.Default.Reload();
                    break;

                default:
                    break;
            }

            return (Result);
        }
        private void InvertTempCompensationTable()
        {
            for (int Index = 0; Index < 21; Index++)
            {
                Params.TempCharacteristics.TempCorrValue[Index] = Properties.Settings.Default.TempCorrVal[Index] = -Properties.Settings.Default.TempCorrVal[Index];
            }
            Params.TempCharacteristics.MRWnumber = 0;
            Params.TempCharacteristics.Linearization = false;

            SaveCharacteristics();
            SetTempCorrValues();
        }
        private void LinearizeTempCompensationTable(bool AskForReferencePoint)
        {
            bool Error;
            DialogResult Result;
            string TempCorrVal70 = "";

            Error = false;

            Result = InputBox("Werteingabe", "Bitte geben sie den Temperaturkennlinienwert für 70°C ein", ref TempCorrVal70);

            if ((Result == DialogResult.OK) && (TempCorrVal70.Length > 0))
            {
                try
                {
                    Properties.Settings.Default.TempCorrVal[0] = Convert.ToInt32(TempCorrVal70);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Auswerten ihrer Eingabe");
                    Error = true;
                }
            }
            else
            {
                Error = true;
            }

            if (Error == false)
            {
                Params.TempCharacteristics.TempCorrValue[0] = Properties.Settings.Default.TempCorrVal[0];
                Params.TempCharacteristics.TempCorrValue[20] = -Properties.Settings.Default.TempCorrVal[0];
                for (int Index = 1; Index < 20; Index++)
                {
                    Params.TempCharacteristics.TempCorrValue[Index] = Properties.Settings.Default.TempCorrVal[Index] = Properties.Settings.Default.TempCorrVal[0] - (int)(2 * (float)Properties.Settings.Default.TempCorrVal[0] / 100 * (5 * Index));
                }
                Params.TempCharacteristics.MRWnumber = 0;
                Params.TempCharacteristics.Linearization = true;

                SaveCharacteristics();
                SetTempCorrValues();
            }
        }
        private void Communication_Task()
        {
            do
            {
                TaskParams.StayInLoop = false;

                switch (TaskParams.TaskState)
                {
                    case TASK_STATE_T.Init:
                        TaskParams.TaskState = TASK_STATE_T.Idle;
                        break;

                    case TASK_STATE_T.Idle:
                        if (TaskParams.Command != COMMAND_T.None)
                        {
                            TaskParams.TaskState = TASK_STATE_T.Send;
                            TaskParams.StayInLoop = true;
                            TaskParams.ReplyCnt = 0;
                        }
                        break;

                    case TASK_STATE_T.Send:
                        switch (TaskParams.Command)
                        {
                            case COMMAND_T.CommMode:
                                SendCommand("COM");
                                break;

                            case COMMAND_T.Power_Off:
                                SendCommand("PWR 0");
                                break;

                            case COMMAND_T.Power_On:
                                SendCommand("PWR 1");
                                break;

                            case COMMAND_T.ProgMode_CH0:
                                SendCommand("PROG 0");
                                break;

                            case COMMAND_T.ProgMode_CH1:
                                SendCommand("PROG 1");
                                break;

                            default:
                                TaskParams.TaskState = TASK_STATE_T.Error;
                                break;
                        }
                        break;

                    case TASK_STATE_T.Receive:
                        TaskParams.Command = COMMAND_T.None;
                        break;

                    case TASK_STATE_T.WaitForOkay:
                        string DataReceived = "";
                        RECEIVE_RETURN_T Callback;
                        Callback = ReceiveFrame(ref DataReceived);

                        switch (Callback)
                        {
                            case RECEIVE_RETURN_T.BUSY:
                                break;

                            case RECEIVE_RETURN_T.OK_FRAME:
                                TaskParams.TaskState = TASK_STATE_T.Okay;
                                TaskParams.Command = COMMAND_T.None;
                                break;

                            default:
                                TaskParams.TaskState = TASK_STATE_T.Error;
                                TaskParams.StayInLoop = true;
                                break;
                        }
                        break;

                    case TASK_STATE_T.Error:
                        if (TaskParams.ReplyCnt < Properties.Settings.Default.MaxReplies)
                        {
                            TaskParams.ReplyCnt++;
                            TaskParams.TaskState = TASK_STATE_T.Send;
                        }
                        else
                        {
                            TaskParams.Command = COMMAND_T.None;
                            TaskParams.TaskState = TASK_STATE_T.Idle;
                        }
                        break;

                    case TASK_STATE_T.Okay:
                        TaskParams.Command = COMMAND_T.None;
                        TaskParams.TaskState = TASK_STATE_T.Idle;
                        break;

                    default:
                        TaskParams.TaskState = TASK_STATE_T.Idle;
                        break;
                }
            } while (TaskParams.StayInLoop == true);
        }
        private RECEIVE_RETURN_T ReceiveFrame(ref String ReceiveData)
        {
            RECEIVE_RETURN_T RetVal;
            int BytesRead = 0;
            int i;
            bool Copy;
            byte[] Data = new byte[65];

            RetVal = RECEIVE_RETURN_T.BUSY;
            Copy = false;

            try
            {
                if (sptLoadCell.IsOpen == true)
                {
                    try
                    {
                        if (Timeout.ElapsedMilliseconds < (long)Properties.Settings.Default.TimeoutGetData)
                        {
                            BytesRead = sptLoadCell.Read(Data, 0, 63 - TaskParams.ReceivePtr);

                            if (BytesRead > 0)
                            {
                                if (TaskParams.ReceivePtr == -1)
                                {
                                    if (Data[0] == (char)0x02)
                                    {
                                        TaskParams.ReceiveBuffer[0] = 0;
                                        TaskParams.ReceivePtr = 0;
                                        Copy = true;
                                    }
                                    else if ((Data[0] == (char)0x06) && (BytesRead == 1))
                                    {
                                        ReceiveData = $"{(char)0x06}";
                                        RetVal = RECEIVE_RETURN_T.ACK_FRAME;
                                    }
                                    else if ((Data[0] == (char)0x07) && (BytesRead == 1))
                                    {
                                        ReceiveData = $"{(char)0x07}";
                                        RetVal = RECEIVE_RETURN_T.NAK_FRAME;
                                    }
                                    else
                                    {

                                    }
                                }
                                else
                                {
                                    Copy = true;
                                }

                                if (Copy == true)
                                {
                                    i = 0;
                                    while ((Data[i] != (char)0x03) && (i < BytesRead))
                                    {
                                        if (Data[i] != (char)0x02)
                                        {
                                            TaskParams.ReceiveBuffer[TaskParams.ReceivePtr++] = Data[i];
                                        }
                                        i++;
                                    }

                                    if (Data[i] == (char)0x03)
                                    {
                                        TaskParams.ReceiveBuffer[TaskParams.ReceivePtr] = 0;

                                        sptLoadCell.DiscardInBuffer();

                                        ReceiveData = System.Text.Encoding.UTF8.GetString(TaskParams.ReceiveBuffer, 0, TaskParams.ReceivePtr);

                                        if ((TaskParams.ReceiveBuffer[0] == 'O') && (TaskParams.ReceiveBuffer[1] == 'K'))
                                        {
                                            RetVal = RECEIVE_RETURN_T.OK_FRAME;
                                        }
                                        else if ((TaskParams.ReceiveBuffer[0] == 'O') && (TaskParams.ReceiveBuffer[1] >= '0') && (TaskParams.ReceiveBuffer[1] <= '9') && (TaskParams.ReceiveBuffer[2] >= '0') && (TaskParams.ReceiveBuffer[2] <= '9') && (TaskParams.ReceiveBuffer[3] >= '0') && (TaskParams.ReceiveBuffer[3] <= '9') && (TaskParams.ReceiveBuffer[4] == 0))
                                        {
                                            RetVal = RECEIVE_RETURN_T.ERROR_FRAME;
                                        }
                                        else
                                        {
                                            RetVal = RECEIVE_RETURN_T.DATA_FRAME;
                                        }

                                        TaskParams.ReceivePtr = -1;
                                        TaskParams.ReceiveBuffer[0] = 0;
                                    }
                                    Copy = false;
                                }
                            }
                        }
                        else
                        {
                            RetVal = RECEIVE_RETURN_T.TIMEOUT;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.HResult != -2146233083)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        else
                        {
                            RetVal = RECEIVE_RETURN_T.TIMEOUT;
                        }
                    }
                }
                else
                {
                    RetVal = RECEIVE_RETURN_T.PORT_CLOSED;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return (RetVal);
        }
        public void SaveCharacteristics()
        {
            for (int Index = 0; Index < 21; Index++)
            {
                Properties.Settings.Default.TempCorrVal[Index] = Params.TempCharacteristics.TempCorrValue[Index];
            }
            Properties.Settings.Default.TempCorrBasedOnMRW = Params.TempCharacteristics.MRWnumber;
            Properties.Settings.Default.TempCorrBasedOnSerialNb = Params.TempCharacteristics.SerialNumber;
            Properties.Settings.Default.TempCorrBasedOnChannel = Params.TempCharacteristics.Channel;

            Properties.Settings.Default.Save();
        }
        public bool SaveCharacteristicsToFile(bool SaveAs)
        {
            bool Error;
            FileStream DataStream;
            DialogResult Result;
            string Path;
            string DataLine = "";
            int Pos;

            Error = false;

            if ((SaveAs == true)||(FileName.Length == 0))
            {
                SaveFileDialog FileDlg = new SaveFileDialog()
                {
                    //Filter = "tcr files (*.tcr)|*.tcr|All files (*.*)|*.*",
                    Filter = "csv files (*.csv)|*.csv|(*.tcr)|*.tcr|All files (*.*)|*.*",
                    FilterIndex = 1,
                    AddExtension = true,
                    DefaultExt = ".csv",
                    InitialDirectory = Properties.Settings.Default.FileOpenPath,
                    RestoreDirectory = true,
                    Title = "Zieldatei der zu speichernden Kennliniendaten wählen"
                };
                if (Params.TempCharacteristics.Linearization == false)
                {
                    if (Properties.Settings.Default.TempCorrBasedOnMRW > 0)
                    {
                        FileDlg.FileName = "Temperaturkennlinie - " + Properties.Settings.Default.TempCorrBasedOnMRW.ToString() + " - Kanal " + Properties.Settings.Default.TempCorrBasedOnChannel.ToString();
                    }
                    else
                    {
                        FileDlg.FileName = "Temperaturkennlinie - ";
                    }
                }
                else
                {
                    Params.TempCharacteristics.Linearization = false;
                    FileDlg.FileName = "Linearisierte Temperaturkennlinie - " + Convert.ToString(Properties.Settings.Default.TempCorrVal[0] * 2) + "digits pro 100°C";
                }

                Result = FileDlg.ShowDialog();

                if (Result == DialogResult.OK)
                {
                    FileName = FileDlg.FileName;

                    if (FileName.Length > 0)
                    {
                        Pos = FileName.LastIndexOf("\\");
                        if (Pos >= 0)
                        {
                            Path = FileName.Substring(0, Pos);
                            if (Path != Properties.Settings.Default.FileOpenPath)
                            {
                                Properties.Settings.Default.FileOpenPath = Path;
                                Properties.Settings.Default.Save();
                            }
                        }
                    }
                    else 
                    {
                        Error = true;
                    }
                }
                else
                {
                    Error = true;
                }

            }
            if (Error == false)
            {
                try
                {
                    DataStream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
                    StreamWriter Writer = new StreamWriter(DataStream);

                    /* Add datalines */
                    DataLine = SetFileDataline(true);  // Header

                    Error = true;

                    if (DataLine.Length != 0)
                    {
                        Writer.WriteLine(DataLine);

                        DataLine = SetFileDataline(false);  // Header

                        if (DataLine.Length != 0)
                        {
                            Writer.WriteLine(DataLine);
                            Error = false;
                        }
                    }
                    Writer.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Öffnen der Datei (" + ex.Message + ")");
                    Error = true;
                }

                if (Error == false)
                {
                    this.Text = "Temperatur - Kennlinie - " + FileName;
                    speichernToolStripMenuItem.Enabled = true;
                }
            }
            return (Error);
        }
        private bool SendCommand(string Command)
        {
            bool Error;

            Error = false;

            if (SerialWrite(Command) == false)
            {
                Timeout.Restart();
                TaskParams.TaskState = TASK_STATE_T.WaitForOkay;
            }
            else
            {
                Error = true;
                TaskParams.TaskState = TASK_STATE_T.Error;
            }

            return (Error);
        }
        private bool SerialWrite(string Command)
        {
            bool Error = false;
            string Frame2Send;

            Frame2Send = (char)0x02 + Command + (char)0x03;

            sptLoadCell.DiscardInBuffer();

            try
            {
                sptLoadCell.Write(Frame2Send);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Error = true;
            }

            return (Error);
        }
        public string SetFileDataline(bool Header)
        {
            string DataLine;

            if (Header == true)
            { // Header
                DataLine = "MRW-Nummer;Seriennummer;Kanal;Korrekturwert bei -30°C;Korrekturwert bei -25°C;Korrekturwert bei -20°C;Korrekturwert bei -15°C;Korrekturwert bei -10°C;Korrekturwert bei -5°C;Korrekturwert bei 0°C;Korrekturwert bei 5°C;Korrekturwert bei 10°C;Korrekturwert bei 15°C;Korrekturwert bei 20°C;Korrekturwert bei 0°C;Korrekturwert bei 5°C;Korrekturwert bei 10°C;Korrekturwert bei 15°C;Korrekturwert bei 20°C;Korrekturwert bei 25°C;Korrekturwert bei 30°C;Korrekturwert bei 35°C;Korrekturwert bei 40°C;Korrekturwert bei 45°C;Korrekturwert bei 50°C;Korrekturwert bei 55°C;Korrekturwert bei 60°C;Korrekturwert bei 65°C;Korrekturwert bei 70°C";
            }
            else
            {
                DataLine = Params.TempCharacteristics.MRWnumber.ToString() + ";" + Params.TempCharacteristics.SerialNumber + ";" + Params.TempCharacteristics.Channel;
                for (byte Index = 0; Index < 21; Index++)
                {
                    DataLine = DataLine + ";" + Params.TempCharacteristics.TempCorrValue[Index].ToString();
                }
            }
            return(DataLine); 
        }
        public void SetTempCorrValues()
        {
            chartTempCharacteristics.Series[0].Points.Clear();
            chartTempCharacteristics.Series[0].Name = Properties.Settings.Default.TempCorrBasedOnMRW.ToString() + " - Kanal " + Properties.Settings.Default.TempCorrBasedOnChannel.ToString();

            for (int Index = 0; Index < 21; Index++)
            {
                TextCorrValue[Index].Text =  Params.TempCharacteristics.TempCorrValue[Index].ToString();

                chartTempCharacteristics.Series[0].Points.AddXY(70 - 5 * Index, Params.TempCharacteristics.TempCorrValue[Index]);
            }
            /* Autoscale */
            chartTempCharacteristics.ChartAreas[0].AxisY.Minimum = Double.NaN;
            chartTempCharacteristics.ChartAreas[0].AxisY.Maximum = Double.NaN;
            chartTempCharacteristics.ChartAreas[0].RecalculateAxesScale();

            if (Params.TempCharacteristics.MRWnumber != 0)
            {
                grpTempCorrValBasedOn.Enabled = true;
                txtTempCorrValBasedOnMRW.Text = Properties.Settings.Default.TempCorrBasedOnMRW.ToString();
                txtTempCorrValBasedOnSerialNb.Text = Properties.Settings.Default.TempCorrBasedOnSerialNb;
                txtTempCorrValBasedOnChannel.Text = Properties.Settings.Default.TempCorrBasedOnChannel.ToString();
            }
            else
            {
                grpTempCorrValBasedOn.Enabled = false;
                txtTempCorrValBasedOnMRW.Text = "------";
                txtTempCorrValBasedOnSerialNb.Text = "------";
                txtTempCorrValBasedOnChannel.Text = "------";
            }
        }
        public void SetTextBoxLinks()
        {
            for (int Index = 0; Index < 21; Index++)
            {
                switch(Index)
                {
                    case 0:
                        TextCorrValue[Index] = txtTempCorrValue_70;
                        break;

                    case 1:
                        TextCorrValue[Index] = txtTempCorrValue_65;
                        break;

                    case 2:
                        TextCorrValue[Index] = txtTempCorrValue_60;
                        break;

                    case 3:
                        TextCorrValue[Index] = txtTempCorrValue_55;
                        break;

                    case 4:
                        TextCorrValue[Index] = txtTempCorrValue_50;
                        break;

                    case 5:
                        TextCorrValue[Index] = txtTempCorrValue_45;
                        break;

                    case 6:
                        TextCorrValue[Index] = txtTempCorrValue_40;
                        break;

                    case 7:
                        TextCorrValue[Index] = txtTempCorrValue_35;
                        break;

                    case 8:
                        TextCorrValue[Index] = txtTempCorrValue_30;
                        break;

                    case 9:
                        TextCorrValue[Index] = txtTempCorrValue_25;
                        break;

                    case 10:
                        TextCorrValue[Index] = txtTempCorrValue_20;
                        break;

                    case 11:
                        TextCorrValue[Index] = txtTempCorrValue_15;
                        break;

                    case 12:
                        TextCorrValue[Index] = txtTempCorrValue_10;
                        break;

                    case 13:
                        TextCorrValue[Index] = txtTempCorrValue_5;
                        break;

                    case 14:
                        TextCorrValue[Index] = txtTempCorrValue_0;
                        break;

                    case 15:
                        TextCorrValue[Index] = txtTempCorrValue_Minus5;
                        break;

                    case 16:
                        TextCorrValue[Index] = txtTempCorrValue_Minus10;
                        break;

                    case 17:
                        TextCorrValue[Index] = txtTempCorrValue_Minus15;
                        break;

                    case 18:
                        TextCorrValue[Index] = txtTempCorrValue_Minus20;
                        break;

                    case 19:
                        TextCorrValue[Index] = txtTempCorrValue_Minus25;
                        break;

                    case 20:
                        TextCorrValue[Index] = txtTempCorrValue_Minus30;
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion
        #region Events
        private void btnCellCharacteristics_Exit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
        //private void btnImport_Click(object sender, EventArgs e)
        //{
        //    ImportCharacteristics();
        //}
        //private void btnSave_Click(object sender, EventArgs e)
        //{
        //    SaveCharacteristicsToFile(false);
        //}
        private void ChangeGradientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeGradientTempCompensationTable();
        }
        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearTempCompensationTable();
        }
         private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportCharacteristics();
        }
        private void InvertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvertTempCompensationTable();
        }
        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetCharacteristicsFromFile();
        }
        private void LinearizeWithReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinearizeTempCompensationTable(true);
        }
        private void LinearizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinearizeTempCompensationTable(false);
        }
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCharacteristicsToFile(false);
        }
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCharacteristicsToFile(true);
        }
        #endregion
        private void TempCharacteristics_Enter(object sender, EventArgs e)
        {
        }

        private void TempCharacteristics_Load(object sender, EventArgs e)
        {
            speichernToolStripMenuItem.Enabled = (this.Text != "Temperatur-Kennlinie");
        }
        private void ImportFromLoadCellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportFromLoadCell();
        }

        private void timer10ms_Tick(object sender, EventArgs e)
        {
            Communication_Task();
        }
    }
}
