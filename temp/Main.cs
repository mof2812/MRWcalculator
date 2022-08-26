using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MRWCalculator
{
    public struct PARAMETERS_T
    {
        public CELL_CHARACTERISTICS_T CellParams;
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

            CCharacteristics.Init(ByDefault, Params.CellParams);

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
    }
}
