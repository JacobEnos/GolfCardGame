using System.Security;
using System.Text.Json.Serialization;

namespace BlazorServerGolfApp
{
    public class Card {
        public bool isShowing { get; set; } = false;
        public string? Number { get; set; }
        public string? Suite { get; set; }
        public string? Color { get; set; }
        public static readonly Dictionary<string, string> icons = new Dictionary<string, string>{
                {"hearts", "\u2665"},
                {"diamonds", "\u2666"},
                {"clubs", "\u2663"},
                {"spades", "\u2660"}
            };

        public Card() {
        }

        public Card(string number, string suite)
        {
            Number = number;
            this.Suite = suite;
            this.Color = suite == "spades" || suite == "clubs" ? "black" : "red"; 
        }

        [JsonConstructorAttribute]
        public Card(string number, string suite, string color) {
            Number = number;
            this.Suite = suite;
            this.Color = color;
        }

        public Card(string json) {
            string[] data = json.Split(',');
            this.Number = data[0];
            this.Suite = data[1];
            this.Color = Suite == "spades" || Suite == "clubs" ? "black" : "red";
        }

        public override string ToString()
        {
            return Number + "," + Suite;
        }
    }
}
