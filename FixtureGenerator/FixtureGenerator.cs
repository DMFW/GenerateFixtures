using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace FixtureGenerator
{
    public partial class frmGenerate : Form
    {

        private string _ExcelSheetPath;
        private Int16 _NumberOfTeams;
        private Int16 _NumberOfRounds;
        private Int16 _StartingWeek;
        private DateTime _FirstDate;
        private bool _InterRoundPositionNights;
        private bool _FinalPositionNight;
        List<Team> _TeamList = new List<Team>();

        private bool _ImportValid;
        private bool _DatesVerified;
        private Fixtures _Fixtures;
        

        public frmGenerate()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            _Fixtures = new Fixtures(_StartingWeek,_NumberOfTeams, _NumberOfRounds, _InterRoundPositionNights);
            btnApportion.Enabled = true;
        }

        private void btnApportion_Click(object sender, EventArgs e)
        {
            if (_Fixtures != null)
            {
                _Fixtures.ApportionLanes();
                btnCopyToClipboard.Enabled = true;
            }
            else
            {
                MessageBox.Show("Fixures must be generated first before apportioning across lanes");
            }

        }

        private void btnCopyToClipboard_Click(object sender, EventArgs e)
        {

            if (_Fixtures != null)
            {
                _Fixtures.WriteToClipboard();
            }
        }

        private void txtMasterSpreadsheet_TextChanged(object sender, EventArgs e)
        {
           _ExcelSheetPath = txtMasterSpreadsheet.Text;
        }

        private void frmGenerate_Load(object sender, EventArgs e)
        {
            txtMasterSpreadsheet.Text = Properties.Settings.Default.MasterSpreadsheet;
        }

        private void frmGenerate_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveUserSettings();
        }

        private void SaveUserSettings()
        {
            Properties.Settings.Default.MasterSpreadsheet = txtMasterSpreadsheet.Text;
            Properties.Settings.Default.Save();
        }

        private void btnSpreadsheet_Click(object sender, EventArgs e)
        {
            if (ofdSpreadsheet.ShowDialog() == DialogResult.OK)
            {
               txtMasterSpreadsheet.Text = ofdSpreadsheet.FileName;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {

            bool importValid;

            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Excel.Workbook wb = excel.Workbooks.Open(_ExcelSheetPath);

            importValid = ImportParameters(wb);

            if (importValid) { importValid = ImportTeams(wb); }

            if (importValid) { btnGenerate.Enabled = true; }
        }

        private bool ImportParameters(Excel.Workbook wb)
        {
            bool importParametersValid = true;

            Excel.Worksheet ws = wb.Sheets["Setup"];

            Excel.Range rangeNoOfRounds = ws.Range["NoOfRounds"];

            if (!Int16.TryParse(rangeNoOfRounds.Value.ToString(),out _NumberOfRounds))
            {
                lstImportValidation.Items.Add("Invalid number of rounds. Not Numeric.");
                importParametersValid = false;
            }
            else
            {
                lstImportValidation.Items.Add(_NumberOfRounds + " rounds");
            }

            Excel.Range rangeFirstWeekNo = ws.Range["FirstWeekNo"];

            if (!Int16.TryParse(rangeFirstWeekNo.Value.ToString(), out _StartingWeek))
            {
                lstImportValidation.Items.Add("Invalid first week number. Not Numeric.");
                importParametersValid = false;
            }
            else
            {
                lstImportValidation.Items.Add("First week number " + _StartingWeek);
            }

            Excel.Range rangeFirstWeekDate = ws.Range["FirstWeekDate"];

            if (!DateTime.TryParse(rangeFirstWeekDate.Value.ToString(), out _FirstDate))
            {
                lstImportValidation.Items.Add("Invalid first week date. Not a date.");
                importParametersValid = false;
            }
            else
            {
                lstImportValidation.Items.Add("First week date " + _FirstDate);
            }

            Excel.Range rangeInterroundPosition = ws.Range["InterroundPositionNights"];
            _InterRoundPositionNights = rangeInterroundPosition.Value.ToString() == "Y";
            lstImportValidation.Items.Add("Interround Position Nights " + _InterRoundPositionNights.ToString());

            Excel.Range rangeFinalPosition = ws.Range["FinalPositionNight"];
            _FinalPositionNight = rangeFinalPosition.Value.ToString() == "Y";
            lstImportValidation.Items.Add("Final Position Night " + _InterRoundPositionNights.ToString());

            return importParametersValid;

        }

        private bool ImportTeams(Excel.Workbook wb)
        {
            Team currentTeam = null;
            
            Excel.Worksheet ws = wb.Sheets["Setup"];

            Excel.Range rangeTeamCodes = ws.Range["TeamCodes"];

            int currentTeamRow = 0;
            int lastTeamRow = 0;
            foreach (Excel.Range cell in rangeTeamCodes)
            {

                currentTeamRow = cell.Row;

                if (currentTeamRow != lastTeamRow)
                {
                    if (currentTeam != null)
                    {
                        _TeamList.Add(currentTeam);
                    }
                    currentTeam = new Team();
                }

                switch (cell.Column)
                {
                    case 1:
                        currentTeam.ID = cell.Value;
                        break;
                    case 2:
                        currentTeam.Code = cell.Value.ToString();
                        break;
                    case 3:
                        currentTeam.Name = cell.Value;
                        break;
                    default:
                        break;
                }
            }
            wb.Close();
            return true;
        }
    }
}
