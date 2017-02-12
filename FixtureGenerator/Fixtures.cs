using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FixtureGenerator
{
    class Fixtures
    {

        private string[] _teamNames = { "Quicquid", "Lane Rangers", "Turkey's Delight", "Three Amigos", "Essex Boyz", "Drambusters" };
        private string[] _teamCodes = { "QUIQUID", "LANERANG", "TURKEYSD", "XTREMEAM", "EBOYZ", "DRAMBUST" };

        private Int16 _NoOfTeams;
        private Int16 _NoOfRounds;
        private Int16 _StartingWeek;
        private Int16 _RequiredLanes;
        private bool _positionNightPerRound;
        private List<Team> _lstTeams = new List<Team>();
        private List<Week> _lstWeeks = new List<Week>();

        public Fixtures(Int16 StartingWeek, Int16 NoOfTeams, Int16 NoOfRounds, bool PositionNightPerRound)
        {
            _NoOfTeams = NoOfTeams;
            _NoOfRounds = NoOfRounds;
            _StartingWeek = StartingWeek;
            _RequiredLanes = (Int16)_teamNames.Count();
            if (_RequiredLanes % 2 != 0) _RequiredLanes--; // The required lanes are always even and one less than the team count if that is odd
            _positionNightPerRound = PositionNightPerRound;
            Generate();
        }

        private void Generate()
        {
            Queue<Team> homeTeams = new Queue<Team>();
            Queue<Team> awayTeams = new Queue<Team>();
            Team lockedHomeTeam;
            Team firstAwayTeam;

            lockedHomeTeam = new Team(1, _teamNames[0], _teamCodes[0]); // This team is not queued.

            _lstTeams.Add(lockedHomeTeam);

            for (Int16 i = _NoOfTeams; i > 1; i--)
            {
                Team realTeam = new Team(i, _teamNames[i - 1], _teamCodes[i - 1]);
                realTeam.Dummy = false;

                _lstTeams.Add(realTeam);

                if (i <= Math.Round((decimal)_NoOfTeams / 2, 0, MidpointRounding.AwayFromZero))
                {
                    homeTeams.Enqueue(realTeam);
                }
                else
                {
                    awayTeams.Enqueue(realTeam);
                }
            }

            if (awayTeams.Count > (homeTeams.Count + 1))
            {
                Team dummyTeam = new Team(_NoOfTeams + 1,"","");
                dummyTeam.Dummy = true;
                homeTeams.Enqueue(dummyTeam);
            }

            Int16 NoOfWeeks = (short)((_NoOfTeams - 1) * _NoOfRounds);

            if (_positionNightPerRound)
            {
                NoOfWeeks = (short)(NoOfWeeks + _NoOfRounds);
            }
            else
            {
                NoOfWeeks++; // Just the last position night at the end of the seaon
            }

            firstAwayTeam = awayTeams.Peek();
            bool positionNightHandledForRound = false;

            for (Int16 i = _StartingWeek; i < (NoOfWeeks + _StartingWeek - 1); i++)
            {
                Week week = new Week();
                week.ID = i;
                _lstWeeks.Add(week);

                if ((firstAwayTeam == awayTeams.Peek()) && !positionNightHandledForRound)
                {
                    // We have gone round the loop again so....
                    if ((_positionNightPerRound) && (i > _StartingWeek))
                    {
                        // Do nothing with this week (empty fixtures to be included in the database)
                        positionNightHandledForRound = true;
                        continue;
                    }
                }

                week.GenerateMatches(lockedHomeTeam, homeTeams, awayTeams);
               // Rotate the queue
               awayTeams.Enqueue(homeTeams.Dequeue());
               homeTeams.Enqueue(awayTeams.Dequeue());
               positionNightHandledForRound = false;
            }
        }

        public void ApportionLanes()
        {
            List<Match> AllMatches = new List<Match>();
            Dictionary<Int16,IGrouping<Int16, Match>> currentTeamLaneGroups = new Dictionary<Int16,IGrouping<Int16, Match>>();
            bool moreAdjmustmentsToDoForCurrentTeam;
   
            foreach (Week week in _lstWeeks)
            {
                AllMatches.AddRange(week.Matches);
            }

            foreach (Team teamToFix in _lstTeams)
            {

                 // LINQ below returns matches across all weeks grouped by lane for the current team

                 var homeCurrentTeamLaneGroups =
                    (from homeTeamMatch in AllMatches
                    where (homeTeamMatch.HomeTeam == teamToFix)
                    group homeTeamMatch by homeTeamMatch.HomeLane into homelaneGroup
                    select homelaneGroup);

                 var awayCurrentTeamLaneGroups =
                    (from awayTeamMatch in AllMatches
                    where (awayTeamMatch.AwayTeam == teamToFix)
                    group awayTeamMatch by awayTeamMatch.AwayLane into awaylaneGroup
                    select awaylaneGroup);

                 var allCurrentTeamLaneGroups = homeCurrentTeamLaneGroups.Union(awayCurrentTeamLaneGroups);

                 moreAdjmustmentsToDoForCurrentTeam = true;
                
                 do
                 {
                     Int16 sourceLaneCount = 0;
                     Int16 targetLaneCount = (Int16)AllMatches.Count();
                     Int16 sourceLane = 0;
                     Int16 targetLane = 0;

                     Int16[] teamLaneCount = new Int16[_RequiredLanes + 1];
                     currentTeamLaneGroups.Clear();

                     foreach (IGrouping<Int16, Match> laneGroup in allCurrentTeamLaneGroups)
                     {
                         currentTeamLaneGroups.Add(laneGroup.Key, laneGroup);
                         teamLaneCount[laneGroup.Key] = (Int16)laneGroup.Count<Match>();
                         Console.WriteLine("Team=" + teamToFix.Name + "; Lane=" + laneGroup.Key + "; Count=" + laneGroup.Count<Match>()); // The lane number
                     }

                     for (int laneNo = 1; laneNo <= _RequiredLanes; laneNo++)
                     {
                         if (teamLaneCount[laneNo] > sourceLaneCount)
                         {
                             sourceLaneCount = teamLaneCount[laneNo];
                             sourceLane = (Int16)laneNo;
                         }
                         if (teamLaneCount[laneNo] < targetLaneCount)
                         {
                             targetLaneCount = teamLaneCount[laneNo];
                             targetLane = (Int16)laneNo;
                         }
                     }

                     if (sourceLaneCount == targetLaneCount)
                     {
                         // It doesn't get much better than this. The team is now perfectly distributed.
                         moreAdjmustmentsToDoForCurrentTeam = false;
                         break;
                     }

                     // Now the interesting lane swapping meat of the routine!

                     // Try and find ONE match that we can adjust. We should only adjust the first one we find
                     // which is possible to adjust within this cycle then return to re-analyse the lane breakdowns
                     // and see what is the next best candidate to adjust.

                     bool adjustedOneMatch = false;

                     var unresolvedMatches =
                     (from sourceLaneMatch in currentTeamLaneGroups[sourceLane]
                      where (sourceLaneMatch.IsFinalised == false)
                      select sourceLaneMatch);

                     foreach (Match potentialMatchToAdjust in unresolvedMatches)
                     {

                         if (LanesArePaired(sourceLane, targetLane))
                         {
                             // We need to swap into a lane next to the one we are already playing in.
                             // That's lucky isn't it? Just swap the home and away roles within the match.
                             potentialMatchToAdjust.SwapLanes();
                             potentialMatchToAdjust.IsFinalised = true;
                             Console.WriteLine("Team=" + teamToFix.Name + "; Swapping lanes within a match from " + sourceLane + " to " + targetLane + " in week " + potentialMatchToAdjust.WeekID);
                             adjustedOneMatch = true;
                             break;
                         }

                         // It isn't as simple as swapping home and away, we need to find the candidate match to swap with first
                         // which is restricted to the week of the source match and the target lane and so is uniquely defined.
                         // I could do this with LINQ as well if I really wanted to but I'm finding LINQ a bit wearisome at this point
                         // so I'm sticking with old fashioned explicit loops.

                         Match potentialSwapMatch = null;

                         foreach (Match weekMatch in _lstWeeks[potentialMatchToAdjust.WeekID - _StartingWeek].Matches)
                         {
                             if (weekMatch.ID == LaneNoToId(targetLane)) 
                             {
                                 potentialSwapMatch = weekMatch;
                                 break;
                             }
                         }

                          
                         // We can't use the match if has been locked down by prior usage in apportionment.
                         if (!potentialSwapMatch.IsFinalised)
                         {
                             _lstWeeks[potentialMatchToAdjust.WeekID - _StartingWeek].SwapMatches(potentialMatchToAdjust.ID, potentialSwapMatch.ID);

                             potentialMatchToAdjust.IsFinalised = true;
                             Console.WriteLine("Team=" + teamToFix.Name + "; was swapped from " + sourceLane + " to " + targetLane + " in week " + potentialMatchToAdjust.WeekID);

                             // Just because we've swapped into the right pair of lanes doesn't mean we are necessarily on the right lane yet...
                             if (   ((potentialMatchToAdjust.HomeTeam == teamToFix) && (potentialMatchToAdjust.HomeLane != targetLane))
                                 || ((potentialMatchToAdjust.AwayTeam == teamToFix) && (potentialMatchToAdjust.AwayLane != targetLane)))
                             {
                                 Console.WriteLine("A subsequent lane swap was required.");
                                 potentialMatchToAdjust.SwapLanes();
                             }

                             adjustedOneMatch = true;
                             break;
                         }
                     }

                     if (adjustedOneMatch == false)
                     {
                         // We were unable to change any of the matches in the worst case lane.
                         // I'm making a simplifying assumption that this means we can't continue and can't do any better (just for this team)
                         // so we will pass on to the next team and rinse and repeat, leaving all "settled" matches (IsFinalised) alone.
                         // It is entirely possible that my assumptions are in fact NOT justified on strict mathematical grounds but this
                         // algorithm is the best my inadequate brain can devise and I THINK it should in practice be good enough.
                         moreAdjmustmentsToDoForCurrentTeam = false; 
                     }
                 }
                 while (moreAdjmustmentsToDoForCurrentTeam);

                // End of a team. Tage any remaining unmoved ones as resolved because we don't want to move them now.
                foreach (Match teamMatch in AllMatches) 
                {
                    if (((teamMatch.HomeTeam == teamToFix) || (teamMatch.AwayTeam == teamToFix)) && (!teamMatch.IsFinalised))
                    {
                        Console.WriteLine("Locking unmoved match in week " + teamMatch.WeekID);
                        teamMatch.IsFinalised = true;
                    }
                }
                 Console.WriteLine("Team=" + teamToFix.Name + " completed apportionment");

            }

            // Let's put some post apportionment verification in here but later...
            MessageBox.Show("Lane Apportionment Completed");

        }

        private bool LanesArePaired(Int16 sourceLane, Int16 targetLane)
        {
            if (Math.Abs(sourceLane - targetLane) > 1)
            {
                return false;
            }

            if (sourceLane % 2 != 0)
            {
                if (targetLane > sourceLane)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (targetLane < sourceLane)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        private Int16 LaneNoToId(Int16 laneNo)
        {
            return (Int16)Math.Ceiling((decimal)(laneNo / 2.0));
        }

        private Int16 IDToHomeLaneNo(Int16 ID)
        {
            return (Int16)((ID * 2) - 1);
        }

        public void WriteToClipboard()
        {
            string clipBoardOutput = "";
            foreach (Week currentWeek in _lstWeeks)
            {
                foreach (Match currentMatch in currentWeek.Matches)
                {
                    if ((currentMatch.HomeTeam != null) & (currentMatch.AwayTeam != null))
                    {
                        if ((!currentMatch.HomeTeam.Dummy) & (!currentMatch.AwayTeam.Dummy))
                        {
                            string SQL = String.Format("insert into fixtures values ({0},{1},{2},{3});" + System.Environment.NewLine, currentWeek.ID, currentMatch.ID, currentMatch.HomeTeam.ID, currentMatch.AwayTeam.ID);
                            clipBoardOutput += SQL;
                        }
                    }
                }
            }
            // Take off the final new line
            clipBoardOutput = clipBoardOutput.TrimEnd(System.Environment.NewLine.ToCharArray());
            Clipboard.SetText(clipBoardOutput);
        }
    }
}
