using System.Collections;

namespace BlazorServerGolfApp {
    public class ScoreBoard : IEnumerable<ScoreBoard.ScoreBoardRow>{

        public class ScoreBoardRow {
            public int? roundPoints { get; set; }
            public string? playerName { get; set; }
            public int? totalPoints { get; set; }

            public ScoreBoardRow() {
                roundPoints = null;
                playerName = null;
                totalPoints = null;
            }

            public ScoreBoardRow(int roundPoints, string playerName, int totalPoints) {
                this.roundPoints = roundPoints;
                this.playerName = playerName;
                this.totalPoints = totalPoints;
            }

            public ScoreBoardRow(string serialized) {
                string[] values = serialized.Split(',');
                roundPoints = Int32.Parse(values[0]);
                playerName = values[1];
                totalPoints = Int32.Parse(values[2]);
            }

            public string ToString() {
                return $"{roundPoints},{playerName},{totalPoints}";
            }
        }

        private List<ScoreBoardRow> scoreboard;

        public ScoreBoard() { 
            scoreboard = new();
        }
        
        public void Add(int roundPoints, string playerName, int totalPoints) {
            scoreboard.Add(new ScoreBoardRow(roundPoints, playerName, totalPoints));
        }
        /*
        public IEnumerator GetEnumerator() {
            
        }
        */
        public IEnumerator<ScoreBoardRow> GetEnumerator() {
            if (scoreboard == null || scoreboard.Count < 1) {
                yield break;
            }

            foreach (ScoreBoardRow row in scoreboard) {
                yield return row;
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        
    }
}
