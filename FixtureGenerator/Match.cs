using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FixtureGenerator
{
    class Match
    {
        private Int16 _weekID;
        private Int16 _ID;
        private Team _homeTeam;
        private Team _awayTeam;
        private Int16 _homeLane;
        private Int16 _awayLane;
        private bool _IsFinalised;

        public Match(Int16 weekID)
        {
            _weekID = weekID;
        }

        public Int16 WeekID
        {
            get { return _weekID; } // Read only and must be set in the constructor
        }

        public Int16 ID
        {
            get {   return _ID; }
            set {   _ID = value;
                    _homeLane = (short)(_ID * 2 - 1);
                    _awayLane = (short)(_ID * 2);
                }
        }

        public Int16 HomeLane 
        {
          get { return _homeLane; }
        }

        public Int16 AwayLane
        {
            get { return _awayLane; }
        }

        public Team HomeTeam
        {
            get { return _homeTeam; }
            set { _homeTeam = value; }
        }

        public Team AwayTeam
        {
            get { return _awayTeam; }
            set { _awayTeam = value; }
        }

        public bool IsFinalised
        {
            get { return _IsFinalised; }
            set { _IsFinalised = value; }
        }

        public void SwapLanes()
        {
            Team workingTeam;
      
            workingTeam = _awayTeam;
            _awayTeam = _homeTeam;
            _homeTeam = workingTeam;

        }

    }
}
