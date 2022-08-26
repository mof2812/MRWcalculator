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
        #region Members
        TEMP_CHARACTERISTICS_T Params;
        TextBox[] TextCorrValue = new TextBox[21];
        string FileName = "";
        #endregion
        #region Methods
        public TempCharacteristics()
        {
            InitializeComponent();

            Params.Linearization = false;

            SetTextBoxLinks();
        }
        private void CalcTempCompensationTableByDefault()
        {
            Params.TempCorrValue[0] = Properties.Settings.Default.TempCorrVal[0] = Properties.Settings.Default.TempCorrVal_70_Default;
            Params.TempCorrValue[20] = Properties.Settings.Default.TempCorrVal[20] = -Properties.Settings.Default.TempCorrVal_70_Default;

            for(int Index = 1; Index < 20; Index++)
            {
                Params.TempCorrValue[Index] = Properties.Settings.Default.TempCorrVal[Index] = Properties.Settings.Default.TempCorrVal_70_Default - (int)(2 * (float)Properties.Settings.Default.TempCorrVal_70_Default/100 * (5 * Index));
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
                    Params.TempCorrValue[Index] = Properties.Settings.Default.TempCorrVal[Index] = (int)(Factor * (double)Properties.Settings.Default.TempCorrVal[Index]);
                }
                Params.MRWnumber = 0;
                Params.Linearization = false;

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
                Params.TempCorrValue[Index] = Properties.Settings.Default.TempCorrVal[Index] = 0;
            }
            Params.MRWnumber = 0;
            Params.Linearization = false;

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
                            Params.Linearization = false;

                            SaveCharacteristics();

                            SetTempCorrValues();
                        }
                    }
                }
            }

            

            return (Error);
        }
        public void Init(TEMP_CHARACTERISTICS_T TCParams, bool ByDefault)
        {
            Params = TCParams;
            //Params.TempCorrValue = new int[21];

            Chart_Init();

            if (ByDefault == true)
            {
                CalcTempCompensationTableByDefault();
            }
            else
            {
                for (int Index = 0; Index < 21; Index++)
                {
                    Params.TempCorrValue[Index] = Properties.Settings.Default.TempCorrVal[Index];
                }
            }
            SetTempCorrValues();
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
                        Params.TempCorrValue[Index] = Convert.ToInt32((string)Dataparts[7]);

                        if (Line == 0)
                        {
                            Params.MRWnumber = Convert.ToUInt32((string)Dataparts[1]);
                            Params.SerialNumber = Dataparts[2];
                            Params.Channel = Convert.ToByte((string)Dataparts[5]);
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

                    Params.MRWnumber = Convert.ToUInt32((string)Dataparts[0]);
                    Params.SerialNumber = Dataparts[1];
                    Params.Channel = Convert.ToByte((string)Dataparts[2]);

                    for (Index = 3; Index < 24; Index++)
                    {
                        Params.TempCorrValue[Index - 3] = Convert.ToInt32((string)Dataparts[Index]);
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
                            Params.Linearization = false;

                            SaveCharacteristics();

                            SetTempCorrValues();
                        }
                    }
                }
            }
        }
        private void InvertTempCompensationTable()
        {
            for (int Index = 0; Index < 21; Index++)
            {
                Params.TempCorrValue[Index] = Properties.Settings.Default.TempCorrVal[Index] = -Properties.Settings.Default.TempCorrVal[Index];
            }
            Params.MRWnumber = 0;
            Params.Linearization = false;

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
                Params.TempCorrValue[0] = Properties.Settings.Default.TempCorrVal[0];
                Params.TempCorrValue[20] = -Properties.Settings.Default.TempCorrVal[0];
                for (int Index = 1; Index < 20; Index++)
                {
                    Params.TempCorrValue[Index] = Properties.Settings.Default.TempCorrVal[Index] = Properties.Settings.Default.TempCorrVal[0] - (int)(2 * (float)Properties.Settings.Default.TempCorrVal[0] / 100 * (5 * Index));
                }
                Params.MRWnumber = 0;
                Params.Linearization = true;

                SaveCharacteristics();
                SetTempCorrValues();
            }
        }
        public void SaveCharacteristics()
        {
            for (int Index = 0; Index < 21; Index++)
            {
                Properties.Settings.Default.TempCorrVal[Index] = Params.TempCorrValue[Index];
            }
            Properties.Settings.Default.TempCorrBasedOnMRW = Params.MRWnumber;
            Properties.Settings.Default.TempCorrBasedOnSerialNb = Params.SerialNumber;
            Properties.Settings.Default.TempCorrBasedOnChannel = Params.Channel;

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
                if (Params.Linearization == false)
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
                    Params.Linearization = false;
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
                    
                }
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
                DataLine = Params.MRWnumber.ToString() + ";" + Params.SerialNumber + ";" + Params.Channel;
                for (byte Index = 0; Index < 21; Index++)
                {
                    DataLine = DataLine + ";" + Params.TempCorrValue[Index].ToString();
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
                TextCorrValue[Index].Text =  Params.TempCorrValue[Index].ToString();

                chartTempCharacteristics.Series[0].Points.AddXY(70 - 5 * Index, Params.TempCorrValue[Index]);
            }
            /* Autoscale */
            chartTempCharacteristics.ChartAreas[0].AxisY.Minimum = Double.NaN;
            chartTempCharacteristics.ChartAreas[0].AxisY.Maximum = Double.NaN;
            chartTempCharacteristics.ChartAreas[0].RecalculateAxesScale();

            if (Params.MRWnumber != 0)
            {
                grpTempCorrValBasedOn.Enabled = true;
                //txtTempCorrValBasedOnMRW.Text = Params.MRWnumber.ToString();
                //txtTempCorrValBasedOnSerialNb.Text = Params.SerialNumber;
                //txtTempCorrValBasedOnChannel.Text = Params.Channel.ToString();
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
        private void linearisierenMitEckpunktvorgabeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinearizeTempCompensationTable(true);
        }
        private void linearisierenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinearizeTempCompensationTable(false);
        }
        private void speichernToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCharacteristicsToFile(false);
        }
        private void speichernunterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCharacteristicsToFile(true);
        }
        #endregion
    }
}
