using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FixtureGenerator
{
    class Week
    {
        private Int16 _ID;
        private List<Match> _lstMatches = new List<Match>();

        public Int16 ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public List<Match> Matches
        {
            get { return _lstMatches; }
        }

        public void GenerateMatches(Team lockedHomeTeam, IEnumerable<Team> homeTeams, IEnumerable<Team> awayTeams)
        {

            Match newMatch = new Match(_ID);
            newMatch.ID = 1;
            newMatch.HomeTeam = lockedHomeTeam;
            newMatch.AwayTeam = awayTeams.ElementAt<Team>(0);
            _lstMatches.Add(newMatch);

            // Now match up the dynamic home teams and the reverse away teams
            for (Int16 i = 1; i < awayTeams.Count<Team>(); i++)
            {
                newMatch = new Match(_ID);
                newMatch.ID = (short)(i + 1);
                newMatch.HomeTeam = homeTeams.ElementAt<Team>(homeTeams.Count<Team>() - i);
                newMatch.AwayTeam = awayTeams.ElementAt<Team>(i);
                _lstMatches.Add(newMatch);
            }
        }

        public void SwapMatches(Int16 ID1, Int16 ID2)
        {
            Match firstMatch = null;
            Match secondMatch = null;

            var matchID1 =
            from match in _lstMatches
            where match.ID == ID1
            select match;

            var matchID2 =
            from match in _lstMatches
            where match.ID == ID2
            select match;

            foreach (var match in matchID1)
            {
                Debug.Assert (firstMatch == null); // Because there should only be one object in this LINQ
                firstMatch = match;
            }

            foreach (var match in matchID2)
            {
                Debug.Assert(secondMatch == null); // Because there should only be one object in this LINQ
                secondMatch = match;
            }

            Debug.Assert(firstMatch != null);
            Debug.Assert(secondMatch != null);

            firstMatch.ID = ID2;
            secondMatch.ID = ID1;
        }
    }
}
