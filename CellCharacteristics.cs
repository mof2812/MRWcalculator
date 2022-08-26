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
    public enum CELL_TYPE_T
    {
        MRWLimit = 0,
        MRW420,
        MRWcan,
    }
    public struct CELL_CHARACTERISTICS_T
    {
        public int DMSOffset;
        public float DMSGain;
        public float EModuleFactor;
        public TEMP_CHARACTERISTICS_T TempCharacteristics;
        public CELL_TYPE_T Celltype;
    }

    public partial class CellCharacteristics : TempCharacteristics
    {
        #region Members
        CELL_CHARACTERISTICS_T Params;
        public TempCharacteristics TCharacteristics = new TempCharacteristics();
        #endregion
        #region Methods
        public CellCharacteristics()
        {
            InitializeComponent();
        }
        public void Init(bool ByDefault, ref CELL_CHARACTERISTICS_T CellParams)
        {
            Params = CellParams;

            if (ByDefault == true)
            {
                Params.DMSOffset = Properties.Settings.Default.DMSOffset = Properties.Settings.Default.DMSOffset_Default;
                Params.DMSGain = Properties.Settings.Default.DMSGain = Properties.Settings.Default.DMSGain_Default;
                Params.EModuleFactor = Properties.Settings.Default.EModuleFactor = Properties.Settings.Default.EModuleFactor_Default;
                Params.Celltype = CELL_TYPE_T.MRWLimit;
            }
            else 
            { 
                Params.DMSOffset = Properties.Settings.Default.DMSOffset;
                Params.DMSGain = Properties.Settings.Default.DMSGain;
                Params.EModuleFactor = Properties.Settings.Default.EModuleFactor;
                Params.Celltype = (CELL_TYPE_T)Properties.Settings.Default.LoadCellType;
            }
            TCharacteristics.Init(Params.TempCharacteristics, ByDefault);

            CellParams = Params;

            SetData();

            TempCharacteristics::Init(ByDefault, ref CellParams);
        }
        private void SetData()
        {
            txtDMSOffset.Text = Params.DMSOffset.ToString();
            txtDMSGain.Text = Params.DMSGain.ToString();
            txtCellTemperaturInfluence_70.Text = Params.TempCharacteristics.TempCorrValue[0].ToString();
            txtCellTemperaturInfluence_Minus30.Text = Params.TempCharacteristics.TempCorrValue[20].ToString();
            txtCellEModuleFactor.Text = Params.EModuleFactor.ToString();
        }
        #endregion
        #region Events
        private void btnCellCharacteristics_Exit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCharacteristics_Click(object sender, EventArgs e)
        {
            DialogResult Result;

            Result = TCharacteristics.ShowDialog();

        }
        #endregion

        private void CellCharacteristics_Enter(object sender, EventArgs e)
        {
            SetData();
        }
    }
}
