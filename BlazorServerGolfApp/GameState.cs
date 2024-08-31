using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace BlazorServerGolfApp
{
    public class GameState
    {
        public List<Player>? Players { get; set; }
        //public PlayerList? _Players { get; set; }
        public Deck Deck { get; set; }
        public List<Card> DiscardPile { get; set; }
        public string DealerName { get; set; }
        public string? ActivePlayer { get; set; }
        //public string? TurnStage { get; set; }
        public TurnStages? TurnStage { get; set; }
        public string? FinalFlipper { get; set; }


        public enum TurnStages {
            Dealing,
            Flipping,
            Drawing,
            Discarding,
            InterRound
        }


        public GameState() {
            Players = new();
            //_Players = new();
            Deck = new();
            DiscardPile = new();
            DealerName = null;
            ActivePlayer = null;
            TurnStage = null;
        }

        [JsonConstructorAttribute]
        public GameState(List<Player> players, Deck deck, List<Card> discardPile, string dealerName, string activePlayer, TurnStages? turnStage, string finalFlipper) {
            Players = players;
            //_Players = players;
            Deck = deck;
            DiscardPile = discardPile;
            DealerName = dealerName;
            ActivePlayer = activePlayer;
            TurnStage = turnStage;
            FinalFlipper = finalFlipper;
        }
    }
}
