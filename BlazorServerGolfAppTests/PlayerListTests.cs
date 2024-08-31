using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorServerGolfApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace BlazorServerGolfApp.Tests {
    [TestClass()]
    public class PlayerListTests {

        static PlayerList playerList {
            get {
                PlayerList list = new PlayerList();
                list.Add(new Player("A", "connection"));
                list.Add(new Player("B", "connection", null, false));
                list.Add(new Player("C", "connection"));
                list.Add(new Player("D", "connection", null, false));
                list.Add(new Player("E", "connection", null, false));
                list.Add(new Player("F", "connection"));
                return list;
            }
        }


        [TestMethod()]
        public void NextActivePlayerTest() {
            Assert.AreEqual("A", playerList.NextActivePlayer("F").Name);
            Assert.AreEqual("C", playerList.NextActivePlayer("A").Name);
            Assert.AreEqual("F", playerList.NextActivePlayer("C").Name);
            Assert.IsNull(playerList.NextActivePlayer(null));
        }


        [TestMethod()]
        public void Jsonify() {

            string playerListJson = JsonSerializer.Serialize(playerList.ToList());
            if(string.IsNullOrWhiteSpace(playerListJson) || playerListJson == "{}") {
                Assert.Fail();
            }
            File.WriteAllText("C:\\Users\\Jacob\\source\\repos\\BlazorServerGolfApp\\BlazorServerGolfAppTests\\PlayerListTestOutput.json", playerListJson);
        }

        [TestMethod()]
        public void DeJsonify() {

            string json = File.ReadAllText("C:\\Users\\Jacob\\source\\repos\\BlazorServerGolfApp\\BlazorServerGolfAppTests\\PlayerListTestOutput.json");
            List<Player> listOfPlayers = JsonSerializer.Deserialize<List<Player>>(json);
            PlayerList deserializedPlayerList = new PlayerList(listOfPlayers);
            Assert.AreEqual(playerList.Count(), deserializedPlayerList.Count());
        }
    }
}