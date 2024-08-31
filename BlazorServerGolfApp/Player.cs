using BlazorServerGolfApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Text.Json.Serialization;

namespace BlazorServerGolfApp
{
    public class Player
    {
        public string? Name { get; set; }
        public string ConnectionId { get; set; }
        public bool IsPlaying { get; set; } = false;
        public List<List<Card>>? Cards { get; set; } //2D array to mimic 2 rows of 3
        public int Points { get; set; }
        public string? Avatar { get; set; }


        [JsonConstructorAttribute]
        public Player(string name, string connectionId, string? avatar, List<List<Card>>? cards = null, bool isPlaying = true) {
            Name = name;
            ConnectionId = connectionId;
            Avatar = avatar;
            IsPlaying = isPlaying;
            Cards = cards;
            Points = 0;
        }

        public Player(string name, string? avatar = null, List<List<Card>>? cards = null, bool isPlaying = true) {
            Name = name;
            ConnectionId = null;
            Avatar = avatar;
            IsPlaying = isPlaying;
            Cards = cards;
            Points = 0;
        }
    }
}
