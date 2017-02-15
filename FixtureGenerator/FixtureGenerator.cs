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
        private short _NumberOfTeams;
        private short _NumberOfRounds;
        private short _StartingWeek;
        private DateTime _FirstDate;
        private bool _InterRoundPositionNights;
        private bool _FinalPositionNight;
        Dictionary<short, Team> _dctTeams = new Dictionary<short, Team>();
        Team _pivotTeam;
        Queue<Team> _homeTeams;
        Queue<Team> _awayTeams;

        private bool _ImportValid;
        private bool _DatesVerified;
        private Fixtures _Fixtures;
        

        public frmGenerate()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            _Fixtures = new Fixtures(_ExcelSheetPath, _StartingWeek, _NumberOfRounds, _InterRoundPositionNights, _dctTeams);
            _Fixtures.Generate(_pivotTeam, _homeTeams, _awayTeams);
            btnCopyToClipboard.Enabled = true;
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
            if (importValid) { importValid = ImportFirstWeekFixtures(wb); }

            wb.Close();

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
            bool importTeams = true;
            Team currentTeam = null;
            
            Excel.Worksheet ws = wb.Sheets["Setup"];
            try
            {
                Excel.Range rangeTeamCodes = ws.Range["TeamCodes"];

                int currentTeamRow = 0; // The relative row in the range
                int currentColumn = 0;  // The relative column in the range
                int lastTeamRow = 0;
                foreach (Excel.Range cell in rangeTeamCodes)
                {

                    currentTeamRow = cell.Row - rangeTeamCodes.Row + 1;

                    if (currentTeamRow != lastTeamRow)
                    {
                        if (currentTeam != null)
                        {
                            if (importTeams) { importTeams = AddTeam(currentTeam, cell.Row); };
                        }
                        currentTeam = new Team();
                        lastTeamRow = currentTeamRow;
                    }

                    currentColumn = cell.Column - rangeTeamCodes.Column + 1;

                    switch (currentColumn)
                    {
                        case 1:
                            currentTeam.ID = (short)cell.Value;
                            break;
                        case 2:
                            currentTeam.Code = cell.Value.ToString();
                            break;
                        case 3:
                            currentTeam.Name = cell.Value.ToString();
                            break;
                        default:
                            break;
                    }
                }

                if (importTeams) { AddTeam(currentTeam, currentTeamRow + rangeTeamCodes.Row); } // Add the last team
                lstImportValidation.Items.Add(_dctTeams.Count() + " Teams loaded");
                return importTeams;
            }
            catch(Exception e)
            {
                lstImportValidation.Items.Add("Teams could not be loaded " + e.Message);
                return false;
            }
        }

        private bool AddTeam(Team currentTeam, int row)
        {
            if (_dctTeams.ContainsKey(currentTeam.ID))
            {
                lstImportValidation.Items.Add("Team " + currentTeam.ID + " is represented at least twice. Import failed on row " + row);
                return false;
            }
            else
            {
                _dctTeams.Add(currentTeam.ID, currentTeam);
                return true;
            }
        }

        private bool ImportFirstWeekFixtures(Excel.Workbook wb)
        {
            bool firstWeekFixturesValid = true;
            Dictionary<short, Team> dctUsedTeams = new Dictionary<short, Team>();
            short teamID;

            Excel.Worksheet ws = wb.Sheets["Setup"];

            _pivotTeam = null;
            _homeTeams = new Queue<Team>();
            _awayTeams = new Queue<Team>();

            try
            {

                Excel.Range rangeFirstWeekFixtures = ws.Range["FirstWeekFixtures"];

                foreach (Excel.Range cell in rangeFirstWeekFixtures)
                {
                    if (!Int16.TryParse(cell.Value.ToString(), out teamID))
                    {
                        firstWeekFixturesValid = false;
                        lstImportValidation.Items.Add("Team ID " + cell.Value + " in first week fixtures is not numeric");
                    }
                    else
                    {
                        if ((teamID < 1) | (teamID > _dctTeams.Count))
                        {
                            firstWeekFixturesValid = false;
                            lstImportValidation.Items.Add("Team ID " + cell.Value + " in first week fixtures is out of the valid ID range");
                        }
                        else
                        {
                            if (dctUsedTeams.ContainsKey(teamID))
                            {
                                firstWeekFixturesValid = false;
                                lstImportValidation.Items.Add("Team ID " + cell.Value + " has been used more than once in the first weeks fixtures");
                            }
                            else
                            {
                                int currentColumn = cell.Column - rangeFirstWeekFixtures.Column + 1;
                                switch (currentColumn)
                                {
                                    case 1:
                                        if (_pivotTeam == null) { _pivotTeam = _dctTeams[teamID]; } else { _homeTeams.Enqueue(_dctTeams[teamID]); }
                                        break;
                                    case 2:
                                        _awayTeams.Enqueue(_dctTeams[teamID]);
                                        break;
                                    default:
                                        break;
                                }
                                dctUsedTeams.Add(teamID, _dctTeams[teamID]);
                            }
                        }
                    }
                }

                lstImportValidation.Items.Add("First week fixtures loaded. OK to proceed to generate.");
                return firstWeekFixturesValid;

            }
            catch (Exception e)
            {
                lstImportValidation.Items.Add("First week fixtures could not be loaded " + e.Message);
                return false;
            }

        }
    }
}
