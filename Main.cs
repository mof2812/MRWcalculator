using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MRWCalculator
{
    public enum RECEIVE_RETURN_T
    {
        DATA_FRAME,
        ACK_FRAME,
        OK_FRAME,
        ERROR_FRAME,
        NAK_FRAME,
        CORRUPTED_FRAME,
        PORT_CLOSED,
        TIMEOUT,
        BUSY
    }
    enum RETURN_T : uint
    {
        OKAY = 0x00000000,
        #region Devices
        /* Devices (0x8xxx xxxx) */
        #region Serial port
        /* Serial Port (0x8xxx x0xx) */
        SERIAL_PORT_OPEN = 0x80000000,
        SERIAL_PORT_CLOSE,
        SERIAL_PORT_UNDEF,  /* undefined port */
        SERIAL_PORT_UNDEF_DEVICE,
        SERIAL_PORT_WRITE,
        #endregion
        #endregion
    }
    public struct PARAMETERS_T
    {
        public CELL_CHARACTERISTICS_T CellParams;
        public bool CommunicationOn;
    }

    public partial class Main : Form
    {
        #region Members
        const uint InitCode = 0x55AA55AB;
        PARAMETERS_T Params;
        CellCharacteristics CCharacteristics = new CellCharacteristics();

        #endregion
        #region Methods
        public Main()
        {
            bool ByDefault;

            InitializeComponent();

            Params.CellParams.TempCharacteristics.TempCorrValue = new int[21];

            ByDefault = (Properties.Settings.Default.InitCode != InitCode);

            if (ByDefault == true)
            {
                Properties.Settings.Default.TempCorrVal = new int[21];

                Properties.Settings.Default.InitCode = InitCode;
                
            }

            CCharacteristics.Init(ByDefault, ref Params.CellParams);
//            CSetupLoadcellInterface.Init(CCharacteristics.TCharacteristics.sptLoad   .TCharacteristics.sptLoadCell, Params.CellParams);

            if (ByDefault == true)
            {
                Properties.Settings.Default.Save();
            }
        }
        #endregion
        #region Events
        private void CellCharacteristicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult Result;

            Result = CCharacteristics.ShowDialog();
        }
        #endregion

        private void InterfaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CCharacteristics.SetupLoadCellCommunication();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblCellType.Text = Params.CellParams.Celltype.ToString();
            lblBaudrate.Text = Params.CellParams.TempCharacteristics.TempCorrValue[0].ToString();
        }
    }
}
