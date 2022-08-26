using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace MRWCalculator
{
    public partial class SetupLoadcellInterface : Form
    {
        public struct PARAMS_T
        {
            public bool CommunicationOn;
            public CELL_CHARACTERISTICS_T CellCharacteristics;
        }
        #region Members
        SerialPort Port = new SerialPort();
        bool InitPhase = true;
        private GroupBox grpCOMSetup;
        private Label lblStoppbits;
        private ComboBox cmbStopbits;
        private Label lblDatabits;
        private ComboBox cmbDatabits;
        private Label lblCOMBaudrate;
        private ComboBox cmbBaudrate;
        private Label lblCOMPort;
        private ComboBox cmbPort;
        private Button btnOK;
        private PictureBox pictureBox1;
        private Button btnCancel;
        private GroupBox grpCellType;
        private ComboBox cmbCellType;
        PARAMS_T Params;
        #endregion
        #region Methods
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupLoadcellInterface));
            this.grpCOMSetup = new System.Windows.Forms.GroupBox();
            this.lblStoppbits = new System.Windows.Forms.Label();
            this.cmbStopbits = new System.Windows.Forms.ComboBox();
            this.lblDatabits = new System.Windows.Forms.Label();
            this.cmbDatabits = new System.Windows.Forms.ComboBox();
            this.lblCOMBaudrate = new System.Windows.Forms.Label();
            this.cmbBaudrate = new System.Windows.Forms.ComboBox();
            this.lblCOMPort = new System.Windows.Forms.Label();
            this.cmbPort = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpCellType = new System.Windows.Forms.GroupBox();
            this.cmbCellType = new System.Windows.Forms.ComboBox();
            this.grpCOMSetup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.grpCellType.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCOMSetup
            // 
            this.grpCOMSetup.Controls.Add(this.lblStoppbits);
            this.grpCOMSetup.Controls.Add(this.cmbStopbits);
            this.grpCOMSetup.Controls.Add(this.lblDatabits);
            this.grpCOMSetup.Controls.Add(this.cmbDatabits);
            this.grpCOMSetup.Controls.Add(this.lblCOMBaudrate);
            this.grpCOMSetup.Controls.Add(this.cmbBaudrate);
            this.grpCOMSetup.Controls.Add(this.lblCOMPort);
            this.grpCOMSetup.Controls.Add(this.cmbPort);
            this.grpCOMSetup.Location = new System.Drawing.Point(13, 36);
            this.grpCOMSetup.Name = "grpCOMSetup";
            this.grpCOMSetup.Size = new System.Drawing.Size(205, 293);
            this.grpCOMSetup.TabIndex = 6;
            this.grpCOMSetup.TabStop = false;
            this.grpCOMSetup.Text = "Serielle Schnittstelle";
            // 
            // lblStoppbits
            // 
            this.lblStoppbits.AutoSize = true;
            this.lblStoppbits.Location = new System.Drawing.Point(16, 221);
            this.lblStoppbits.Name = "lblStoppbits";
            this.lblStoppbits.Size = new System.Drawing.Size(77, 20);
            this.lblStoppbits.TabIndex = 7;
            this.lblStoppbits.Text = "Stoppbits";
            // 
            // cmbStopbits
            // 
            this.cmbStopbits.FormattingEnabled = true;
            this.cmbStopbits.Location = new System.Drawing.Point(17, 245);
            this.cmbStopbits.Name = "cmbStopbits";
            this.cmbStopbits.Size = new System.Drawing.Size(170, 28);
            this.cmbStopbits.TabIndex = 6;
            this.cmbStopbits.Text = "Stoppbits";
            // 
            // lblDatabits
            // 
            this.lblDatabits.AutoSize = true;
            this.lblDatabits.Location = new System.Drawing.Point(16, 161);
            this.lblDatabits.Name = "lblDatabits";
            this.lblDatabits.Size = new System.Drawing.Size(78, 20);
            this.lblDatabits.TabIndex = 5;
            this.lblDatabits.Text = "Datenbits";
            // 
            // cmbDatabits
            // 
            this.cmbDatabits.FormattingEnabled = true;
            this.cmbDatabits.Location = new System.Drawing.Point(17, 185);
            this.cmbDatabits.Name = "cmbDatabits";
            this.cmbDatabits.Size = new System.Drawing.Size(170, 28);
            this.cmbDatabits.TabIndex = 4;
            this.cmbDatabits.Text = "Datenbits";
            // 
            // lblCOMBaudrate
            // 
            this.lblCOMBaudrate.AutoSize = true;
            this.lblCOMBaudrate.Location = new System.Drawing.Point(16, 101);
            this.lblCOMBaudrate.Name = "lblCOMBaudrate";
            this.lblCOMBaudrate.Size = new System.Drawing.Size(75, 20);
            this.lblCOMBaudrate.TabIndex = 3;
            this.lblCOMBaudrate.Text = "Baudrate";
            // 
            // cmbBaudrate
            // 
            this.cmbBaudrate.FormattingEnabled = true;
            this.cmbBaudrate.Location = new System.Drawing.Point(17, 125);
            this.cmbBaudrate.Name = "cmbBaudrate";
            this.cmbBaudrate.Size = new System.Drawing.Size(170, 28);
            this.cmbBaudrate.TabIndex = 2;
            this.cmbBaudrate.Text = "Baudrate";
            // 
            // lblCOMPort
            // 
            this.lblCOMPort.AutoSize = true;
            this.lblCOMPort.Location = new System.Drawing.Point(16, 41);
            this.lblCOMPort.Name = "lblCOMPort";
            this.lblCOMPort.Size = new System.Drawing.Size(96, 20);
            this.lblCOMPort.TabIndex = 1;
            this.lblCOMPort.Text = "Schnittstelle";
            // 
            // cmbPort
            // 
            this.cmbPort.FormattingEnabled = true;
            this.cmbPort.Location = new System.Drawing.Point(17, 65);
            this.cmbPort.Name = "cmbPort";
            this.cmbPort.Size = new System.Drawing.Size(170, 28);
            this.cmbPort.Sorted = true;
            this.cmbPort.TabIndex = 0;
            this.cmbPort.Text = "COM-Port";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(510, 369);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(158, 50);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "Okay";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(305, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(364, 317);
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(346, 369);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(158, 50);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Abbrechen";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpCellType
            // 
            this.grpCellType.Controls.Add(this.cmbCellType);
            this.grpCellType.Location = new System.Drawing.Point(13, 344);
            this.grpCellType.Name = "grpCellType";
            this.grpCellType.Size = new System.Drawing.Size(205, 74);
            this.grpCellType.TabIndex = 9;
            this.grpCellType.TabStop = false;
            this.grpCellType.Text = "Zellentyp";
            // 
            // cmbCellType
            // 
            this.cmbCellType.FormattingEnabled = true;
            this.cmbCellType.Location = new System.Drawing.Point(17, 28);
            this.cmbCellType.Name = "cmbCellType";
            this.cmbCellType.Size = new System.Drawing.Size(170, 28);
            this.cmbCellType.TabIndex = 0;
            this.cmbCellType.SelectedIndexChanged += new System.EventHandler(this.cmbCellType_SelectedIndexChanged);
            // 
            // SetupLoadcellInterface
            // 
            this.ClientSize = new System.Drawing.Size(690, 443);
            this.Controls.Add(this.grpCellType);
            this.Controls.Add(this.grpCOMSetup);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCancel);
            this.Name = "SetupLoadcellInterface";
            this.grpCOMSetup.ResumeLayout(false);
            this.grpCOMSetup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.grpCellType.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        public SetupLoadcellInterface()
        {
            InitializeComponent();

            InitPhase = true;
        }
        private RETURN_T SetupLoadcellInterface_ClosePort()
        {
            RETURN_T RetVal;

            RetVal = RETURN_T.OKAY;

            try
            {
                Port.Close();
                Params.CommunicationOn = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                RetVal = RETURN_T.SERIAL_PORT_CLOSE;
            }
            return (RetVal);
        }
        public CELL_TYPE_T GetCelltype()
        {
            return (Params.CellCharacteristics.Celltype);
        }

        public void Init(SerialPort SPort, CELL_CHARACTERISTICS_T CellParams)
        {
            Params.CommunicationOn = false;
            Params.CellCharacteristics = CellParams;

            Port = SPort;

            SetupLoadcellInterface_InitPorts();
            SetupLoadcellInterface_InitBaudrate();
            SetupLoadcellInterface_InitDatabits();
            SetupLoadcellInterface_InitStopbits();

            SetupLoadcellInterface_InitCelltype();

            try
            {
                if (Port.PortName != Properties.Settings.Default.SerialPort)
                {
                    Port.PortName = Properties.Settings.Default.SerialPort;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                Port.BaudRate = Properties.Settings.Default.Baudrate;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                Port.DataBits = Properties.Settings.Default.DataBits;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                Port.StopBits = Properties.Settings.Default.StopBits;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            cmbPort.Text = Port.PortName;
            cmbBaudrate.Text = Port.BaudRate.ToString();
            cmbDatabits.Text = Port.DataBits.ToString();
            cmbStopbits.Text = SetupLoadcellInterface_StopBitsToText(Port.StopBits);

            cmbCellType.SelectedItem = (int)Params.CellCharacteristics.Celltype;
        }
        private RETURN_T SetupLoadcellInterface_InitBaudrate()
        {
            RETURN_T RetVal;

            RetVal = RETURN_T.OKAY;

            cmbBaudrate.Items.Clear();

            cmbBaudrate.Items.Add("300");
            cmbBaudrate.Items.Add("600");
            cmbBaudrate.Items.Add("1200");
            cmbBaudrate.Items.Add("2400");
            cmbBaudrate.Items.Add("4800");
            cmbBaudrate.Items.Add("9600");
            cmbBaudrate.Items.Add("19200");
            cmbBaudrate.Items.Add("38400");
            cmbBaudrate.Items.Add("56000");
            cmbBaudrate.Items.Add("57600");
            cmbBaudrate.Items.Add("115200");

            return (RetVal);
        }
        private RETURN_T SetupLoadcellInterface_InitCelltype()
        {
            RETURN_T RetVal;

            RetVal = RETURN_T.OKAY;

            cmbCellType.Items.Clear();

            cmbCellType.Items.Add("MRWlimit");
            cmbCellType.Items.Add("MRW420");
            cmbCellType.Items.Add("MRWcan");

            return (RetVal);
        }
        private RETURN_T SetupLoadcellInterface_InitDatabits(bool bUpdateOnly = true)
        {
            RETURN_T RetVal;

            RetVal = RETURN_T.OKAY;

            cmbDatabits.Items.Clear();

            cmbDatabits.Items.Add("5");
            cmbDatabits.Items.Add("6");
            cmbDatabits.Items.Add("7");
            cmbDatabits.Items.Add("8");

            if (bUpdateOnly == false)
            {
                SetupLoadcellInterface_SetDatabits(Properties.Settings.Default.DataBits);
            }
            cmbDatabits.Text = Properties.Settings.Default.DataBits.ToString();

            return (RetVal);
        }
        private RETURN_T SetupLoadcellInterface_InitPorts()
        {
            RETURN_T RetVal;

            RetVal = RETURN_T.OKAY;

            cmbPort.Items.Clear();

            string[] Ports = SerialPort.GetPortNames();

            foreach (string Port in Ports)
            {
                cmbPort.Items.Add(Port);
            }

            return (RetVal);
        }
        private RETURN_T SetupLoadcellInterface_InitStopbits()
        {
            RETURN_T RetVal;

            RetVal = RETURN_T.OKAY;

            cmbStopbits.Items.Clear();

            cmbStopbits.Items.Add("Keins");
            cmbStopbits.Items.Add("1");
            cmbStopbits.Items.Add("1.5");
            cmbStopbits.Items.Add("2");

            return (RetVal);
        }
        private RETURN_T SetupLoadcellInterface_OpenPort()
        {
            RETURN_T RetVal;

            RetVal = RETURN_T.OKAY;

            try
            {
                Port.Open();
                Params.CommunicationOn = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                Params.CommunicationOn = false;
                RetVal = RETURN_T.SERIAL_PORT_OPEN;
            }

            return (RetVal);
        }
        private RETURN_T SetupLoadcellInterface_SetBaudrate(int Baudrate)
        {
            RETURN_T RetVal;
            bool IsOpen;

            RetVal = RETURN_T.OKAY;

            if ((Baudrate >= 300) && (Baudrate <= 115200))
            {
                if (RetVal == RETURN_T.OKAY)
                {
                    IsOpen = Port.IsOpen;

                    if (IsOpen == true)
                    {
                        RetVal = SetupLoadcellInterface_ClosePort();
                    }

                    if (RetVal == RETURN_T.OKAY)
                    {
                        Port.BaudRate = Baudrate;

                        if (Baudrate != Properties.Settings.Default.Baudrate)
                        {
                            Properties.Settings.Default.Baudrate = Baudrate;
                            Properties.Settings.Default.Save();
                        }

                        if (IsOpen == true)
                        {
                            RetVal = SetupLoadcellInterface_OpenPort();
                        }
                    }
                }
            }
            return (RetVal);
        }
        private RETURN_T SetupLoadcellInterface_SetDatabits(byte Databits)
        {
            RETURN_T RetVal;
            bool IsOpen;
            SerialPort port = new SerialPort();

            RetVal = RETURN_T.OKAY;

            if ((Databits >= 5) && (Databits <= 8))
            {
                if (RetVal == RETURN_T.OKAY)
                {
                    IsOpen = port.IsOpen;

                    if (IsOpen == true)
                    {
                        RetVal = SetupLoadcellInterface_ClosePort();
                    }

                    if (RetVal == RETURN_T.OKAY)
                    {
                        port.DataBits = Databits;

                        if (Databits != Properties.Settings.Default.DataBits)
                        {
                            Properties.Settings.Default.DataBits = Databits;
                            Properties.Settings.Default.Save();
                        }

                        if (IsOpen == true)
                        {
                            RetVal = SetupLoadcellInterface_OpenPort();
                        }
                    }
                }
            }
            return (RetVal);
        }
        private RETURN_T SetupLoadcellInterface_SetStopbits(StopBits Stopbits)
        {
            RETURN_T RetVal;
            SerialPort port = new SerialPort();
            bool IsOpen;

            RetVal = RETURN_T.OKAY;

            if (RetVal == RETURN_T.OKAY)
            {
                IsOpen =Port.IsOpen;

                if (IsOpen == true)
                {
                    RetVal = SetupLoadcellInterface_ClosePort();
                }

                if (RetVal == RETURN_T.OKAY)
                {
                    port.StopBits = Stopbits;

                    if (Stopbits != Properties.Settings.Default.StopBits)
                    {
                        Properties.Settings.Default.StopBits = Stopbits;
                        Properties.Settings.Default.Save();
                    }

                    if (IsOpen == true)
                    {
                        RetVal = SetupLoadcellInterface_OpenPort();
                    }
                }
            }
            return (RetVal);
        }
        private string SetupLoadcellInterface_StopBitsToText(StopBits stopbits)
        {
            string RetString;

            if (stopbits == StopBits.None)
                RetString = "0";
            if (stopbits == StopBits.OnePointFive)
                RetString = "1.5";
            if (stopbits == StopBits.Two)
                RetString = "2";
            else
                RetString = "1";

            return (RetString);
        }
        #endregion
        #region Events
        private void btnOK_Click(object sender, EventArgs e)
        {
            bool Error;
            string Portname;
            int Baudrate;
            int Databits;
            StopBits Stopbits;

            Error = true;
            Portname = cmbPort.Text;
            Baudrate = Int32.Parse(cmbBaudrate.Text);
            Databits = Int32.Parse(cmbDatabits.Text);
            if (cmbStopbits.Text == "1")
                Stopbits = StopBits.One;
            else if (cmbStopbits.Text == "1.5")
                Stopbits = StopBits.OnePointFive;
            else Stopbits = StopBits.Two;

            if (Portname.Length > 3)
            {
                Port.Close();

                Port.PortName = Portname;
                Properties.Settings.Default.SerialPort = Port.PortName;

                if ((Baudrate >= 300) && (Baudrate <= 115200))
                {
                    Port.BaudRate = Baudrate;

                    Properties.Settings.Default.Baudrate = Port.BaudRate;

                    if ((Databits >= 5) && (Databits <= 8))
                    {
                        Port.DataBits = Databits;

                        Properties.Settings.Default.DataBits = (byte)Port.DataBits;

                        if ((Stopbits >= (StopBits)1) && (Stopbits <= (StopBits)2))
                        {
                            Port.StopBits = Stopbits;

                            Properties.Settings.Default.StopBits = Port.StopBits;

                            Error = false;
                        }
                    }
                }
            }

            if (Error == false)
            {
                Properties.Settings.Default.Save();

                try
                {
                    Port.Open();

                    DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Error = true;
                }
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        private void cmbPort_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cmbBaudrate_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
         private void cmbCellType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Params.CellCharacteristics.Celltype = (CELL_TYPE_T)cmbCellType.SelectedIndex;
        }
       private void cmbDatabits_SelectedIndexChanged(object sender, EventArgs e)
        {
       }
        private void cmbStopbits_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        #endregion
    }
}
