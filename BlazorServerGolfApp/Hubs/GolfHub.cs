using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using static BlazorServerGolfApp.GameState;
using static BlazorServerGolfApp.ScoreBoard;

namespace BlazorServerGolfApp.Hubs;


public class GolfHub : Hub {
    private readonly IJSRuntime _jsRuntime;
    private const string stateFileName = "GameState.json";
    private const int playerHandCount = 6;

    //private List<Player>? _Players { get; set; }
    private PlayerList? _PlayerList { get; set; }
    private Deck? _Deck { get; set; }
    private List<Card>? _DiscardPile { get; set; }
    private string? _DealerName { get; set; }
    private string? _ActivePlayerName { get; set; }
    private TurnStages? _TurnStage { get; set; }
    private string? _FinalFlipper { get; set; }

    public GolfHub(IJSRuntime jSRuntime) {
        _jsRuntime = jSRuntime;
    }

    /*
    public override async Task OnConnectedAsync() {
        string connectionId = Context.UserIdentifier; // Assuming you have a user identifier in the Context
        Player reconnectingUser = _PlayerList.Where(p => p.ConnectionId == connectionId).First();// = Context.ConnectionId;
        if (reconnectingUser != null) {
            reconnectingUser.ConnectionId = connectionId;
        }
        await base.OnConnectedAsync();
    }
    */

    public void ResetGame() {
        if (File.Exists(stateFileName)) {
            File.Delete(stateFileName);
            Clients.All.SendAsync("FullReset");
        }
    }

    private void SaveState() {
        try {
            GameState state = new GameState(_PlayerList.ToList(), _Deck, _DiscardPile, _DealerName, _ActivePlayerName, _TurnStage, _FinalFlipper);
            string jsonString = JsonSerializer.Serialize(state);
            File.WriteAllText(stateFileName, jsonString);
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }

    private void GetState() {
        if (File.Exists(stateFileName)) {
            string jsonString = File.ReadAllText(stateFileName);
            if (!string.IsNullOrEmpty(jsonString)) {
                LoadState(JsonSerializer.Deserialize<GameState>(jsonString));
            }
        }
        else {
            File.Create(stateFileName).Close();
            _PlayerList = new();
        }
        return;
    }

    private void LoadState(GameState state) {
        _PlayerList = state.Players == null ? new() : new(state.Players);
        _Deck = state.Deck ?? new();
        _DiscardPile = state.DiscardPile ?? new();
        _DealerName = state.DealerName;
        _ActivePlayerName = state.ActivePlayer;
        _TurnStage = state.TurnStage;
        _FinalFlipper = state.FinalFlipper;
    }

    public void AvatarChosen(string playerName, string playerAvatar) {
        GetState();
        _PlayerList[playerName].Avatar = playerAvatar;
        Clients.Others.SendAsync("PlayerChoseAvatar", playerName, playerAvatar);
        SaveState();
        return;
    }


    public void StartGame() {
        GetState();

        if (!_PlayerList.Where(p => p.IsPlaying).All(p => p.Cards == null || p.Cards.Count == 0)) {
            throw new Exception("Not all players are ready! (have readied/left for the next game)");
        }

        NewRound();
        //Clients.All.SendAsync("BeginGame", activePlayer, DiscardPile.Last().ToString());
        SaveState();
        return;
    }

    private void ClearAllPoints() {
        foreach (var p in _PlayerList) {
            p.Points = 0;
        }
    }

    private void NewLobby() {
        _PlayerList = new();
        NewGame();
    }

    private void NewGame() {
        ClearAllPoints();
        _TurnStage = TurnStages.InterRound;
        NewRound();
    }


    private void NewRound() { //TODO: NewHand() rename
        
        ClearPlayerHands();
        _FinalFlipper = null;
        Deal();

        
        PlayersFlip2();
        SaveState();
        //BeginRound();
        return;
    }

    private void BeginRound() {
        _TurnStage = TurnStages.Drawing;
        Clients.All.SendAsync("BeginRound");
    }

    private int PlayerFlippedCount(Player p) {
        //Count flipped
        int flippedCount = 0;

        foreach (List<Card> cl in p.Cards) {
            foreach (Card card in cl) {

                if (card.isShowing == true) {
                    flippedCount++;
                }
            }
        }
        return flippedCount;
    }


    private bool AllPlayersFlipped() {
        bool allReady = _PlayerList.All(p => PlayerFlippedCount((Player)p) == 2);
        return allReady;
    }

    private void PlayersFlip2() {

        //Prompt players to flip
        _TurnStage = TurnStages.Flipping;
        Clients.All.SendAsync("FlipDeal", _ActivePlayerName);
        return;
    }

    private void AssignDealer() {

        if (String.IsNullOrEmpty(_ActivePlayerName) && _TurnStage == null) {
            //If new game, first player deals
            _DealerName = _PlayerList[0].Name;
            _ActivePlayerName = _PlayerList.NextActivePlayer(_DealerName).Name;
        }
        else {
            //if new round
            string newDealerName = _PlayerList.NextActivePlayer(_DealerName).Name;
            string newActivePlayerName = _PlayerList.NextActivePlayer(newDealerName).Name;
            _DealerName = newDealerName;
            _ActivePlayerName = newActivePlayerName;
        }
    }

    public void Deal() {
        try {
            AssignDealer();

            _DiscardPile = new();
            _Deck = new();
            _Deck.Shuffle();
            
            //TODO: simplify dealing loop with PlayerList.GetNextActivePlayer();
            List<Player> activePlayers = _PlayerList.Where(_ => _.IsPlaying).ToList();
            Player activePlayerObject = _PlayerList[_ActivePlayerName];
            int activePlayerIndex = activePlayers.IndexOf(activePlayerObject);

            //Deal 6 cards
            for (int i = 1; i <= 6; i++) {

                //To each player
                bool hitEnd = false;
                for (int j = activePlayerIndex; j < activePlayers.Count; j++) {

                    if (j == activePlayerIndex && hitEnd) {
                        break;
                    }

                    Player p = activePlayers.ElementAt(j);
                    Card drawn = _Deck.Pop();
                    if (p.Cards == null) {
                        p.Cards = new();
                    }
                    if (p.Cards.Count < 1) {
                        p.Cards.Add(new());
                        p.Cards.Add(new());
                    }
                    int rowIndex = i / 4;
                    p.Cards[rowIndex].Add(drawn);
                    Clients.All.SendAsync("Deal", p.Name, rowIndex);

                    if (j + 1 == _PlayerList.Count() && !hitEnd) {
                        hitEnd = true;
                        j = -1;
                    }
                }
            }
            _DiscardPile = new();
            _DiscardPile.Add(_Deck.Pop());
            Clients.All.SendAsync("TopDiscard", _DiscardPile.First().ToString());
        }
        catch (Exception ex) {

            throw ex;
        }

        return;
    }

    public void AddPlayer(string name, bool isPlaying = true) {
        GetState();
        Player joiner = new Player(name, Context.ConnectionId, null, null, isPlaying);
        //Joining between rounds && between games (no dealer)
        if (_TurnStage == null && _ActivePlayerName == null) {
            //
        }
        else {
            joiner.IsPlaying = false;
        }


        _PlayerList.Add(joiner);
        SaveState();
        Clients.Caller.SendAsync("YouJoined", _PlayerList.ToList().Where(p => p.IsPlaying));
        Clients.Others.SendAsync("PlayerJoined", joiner);
        return;
    }

    public void Draw() {
        GetState();
        Card drawn = _Deck.Pop();
        drawn.isShowing = true;
        //Clients.Caller.SendAsync("YourDraw", Deck.Pop().ToString());
        Player player = _PlayerList.Where(p => p.ConnectionId == Context.ConnectionId).FirstOrDefault();
        player.Cards.Insert(0, new List<Card> { drawn });
        Clients.All.SendAsync("Draw", drawn);
        _TurnStage = TurnStages.Discarding;
        SaveState();
        return;
    }

    public void Pickup() {
        GetState();

        //Rotate discard pile
        Card discard = _DiscardPile.LastOrDefault();
        discard.isShowing = true;
        Card newTopDiscard = null;
        if (1 < _DiscardPile.Count) {
            _DiscardPile.RemoveAt(_DiscardPile.Count - 1);
            newTopDiscard = _DiscardPile.Last();
            newTopDiscard.isShowing = true;
        }

        //Add to players hand
        Player player = _PlayerList.Where(p => p.ConnectionId == Context.ConnectionId).FirstOrDefault();
        player.Cards.Insert(0, new List<Card> { discard });
        Clients.All.SendAsync("Pickup", discard, newTopDiscard);
        _TurnStage = TurnStages.Discarding;
        SaveState();
        return;
    }

    public void Discard(Tuple<int, int> indices) {
        GetState();

        Player discarder = _PlayerList.Where(p => p.ConnectionId == Context.ConnectionId).FirstOrDefault();
        Card discarded = discarder.Cards[indices.Item1][indices.Item2];
        discarded.isShowing = true;
        discarder.Cards[indices.Item1][indices.Item2] = discarder.Cards?[0][0];
        discarder.Cards.RemoveAt(0);
        _DiscardPile.Add(discarded);

        Clients.All.SendAsync("Discard", discarder.Name, discarded, indices);
       
        NextTurn();
        return;
    }


    public bool PlayerFlippedAll(Player p = null, string playerName = null) {
        if (p == null) {
            p = _PlayerList.Where(p => p.ConnectionId == Context.ConnectionId).FirstOrDefault();
        }
        return 6 <= PlayerFlippedCount(p);
    }



    private bool CheckRoundOver() => !String.IsNullOrEmpty(_FinalFlipper) && _ActivePlayerName == _FinalFlipper;


    //public void FlipAllCards() =>  _PlayerList.ForEach(p => p.Cards.ForEach(cl => cl.ForEach(c => c.isShowing = true)));
    public void FlipAllCards() {
        foreach (Player p in _PlayerList) {
            p.Cards.ForEach(cl => cl.ForEach(c => c.isShowing = true));
        }
    }


    public void FlipCard(string playerName, Tuple<int, int> indices) {
        GetState();

        if (_TurnStage == TurnStages.InterRound || _TurnStage == TurnStages.Dealing || _TurnStage == TurnStages.Discarding) {
            throw new Exception($"Invalid call to FlipCard() during a {_TurnStage} TurnStage");
        }


        //Flip the card
        Player player = _PlayerList[playerName];
        Card card = player.Cards?[indices.Item1][indices.Item2];
        card.isShowing = true;
        Clients.All.SendAsync("PlayerFlipped", playerName, card, indices);

        if (_TurnStage == TurnStages.Drawing) {
            NextTurn();
        }


        //Check if all players have flipped 2
        if (_TurnStage == TurnStages.Flipping && AllPlayersFlipped()) {
            BeginRound();
        }
        SaveState();
    }

    


    private void ShowScoreboard() {
        //Generate list of player stats
        //Round points, Name, Total points
        //List<(int RoundPoints, string Name, int Points)> l = new();
        ScoreBoard scoreboard = new ScoreBoard();
        List<ScoreBoardRow> scores = new();

        _PlayerList.Where(p => p.IsPlaying).ToList().ForEach((p) => {
            CardHand hand = new CardHand(p.Cards);
            int roundPoints = hand.GetHandPoints();
            p.Points += roundPoints;
            scoreboard.Add(roundPoints, p.Name, p.Points);
            scores.Add(new ScoreBoardRow(roundPoints, p.Name, p.Points));
        });

        string scoreJson = JsonSerializer.Serialize(scores);

        Clients.All.SendAsync("ShowScoreboard", scoreJson);
    }

    public void YouWin(string winnerName) {

        //Send the winners name
        Clients.All.SendAsync("PlayerWon", winnerName);
        ClearPlayerHands();
        SaveState();
    }

    private void NextTurn() {
        Player activePlayer = _PlayerList[_ActivePlayerName];        
        

        //Check if this player is the first to flip all their cards
        if (String.IsNullOrEmpty(_FinalFlipper) && PlayerFlippedAll(activePlayer)) {
            _FinalFlipper = activePlayer.Name;
            FlipAllCards();

            List<KeyValuePair<string, List<List<Card>>>> playerHands = _PlayerList.Select(p => new KeyValuePair<string, List<List<Card>>>(p.Name, p.Cards)).ToList();
            Clients.All.SendAsync("FlipAll", playerHands);
        }



        string nextPlayerName = _PlayerList.NextActivePlayer(_ActivePlayerName).Name;
        if (_FinalFlipper == nextPlayerName) {
            ShowScoreboard();
        }
        else {
            NewPlayersTurn(nextPlayerName);
        }

        // Get the next playing player's name

        SaveState();
        return;
    }



    private Task NewPlayersTurn(string nextPlayer) {
        _ActivePlayerName = nextPlayer;
        _TurnStage = TurnStages.Drawing;
        return Clients.All.SendAsync("NewPlayersTurn", nextPlayer);
    }

    public void PlayerReady(string? playerName) {
        GetState();
        Player p = _PlayerList[playerName];
        p.Cards = new();


        if (_PlayerList.AllHandsClear()) {
            NewRound();
        }

        SaveState();
    }

    public void PlayerReadyForNextGame(string? playerName) {
        GetState();
        Player p = _PlayerList[playerName];
        p.Cards = new();

        //Start next game if everyone is ready
        if (_PlayerList.Where(pl => pl.IsPlaying).All(pl => pl.Cards == null || p.Cards.Count == 0)) {
            StartGame();
        }
        
        SaveState();
    }


    private void ClearPlayerHands() {
        foreach(Player p in _PlayerList) {
            p.Cards = new();
        }
    }

    public void LeaveGame(string leaverName) {
        GetState();
        Player p = _PlayerList[leaverName];
        p.IsPlaying = false;
        p.Cards = null;

        if (_PlayerList.Where(pl => pl.IsPlaying).All(pl => pl.Cards == null || p.Cards.Count == 0)) {
            StartGame();
        }
        SaveState();
        return;
    }

    private void EndGame() {
        if (File.Exists(stateFileName)) {
            File.Delete(stateFileName); 
        }
    }

    public void SendChat(string playerName, string message) {
        Clients.All.SendAsync("SendChat", playerName, message);
    }

}