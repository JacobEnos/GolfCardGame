using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorServerGolfApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorServerGolfApp.Tests {
    [TestClass()]
    public class CircularLinkedListTests {

        static CircularLinkedList<Player> playerList {
            get {
                CircularLinkedList<Player> list = new CircularLinkedList<Player>();
                list.Add(new Player("A", "connection"));
                list.Add(new Player("B", "connection"));
                list.Add(new Player("C", "connection"));
                list.Add(new Player("D", "connection"));
                list.Add(new Player("E", "connection"));
                list.Add(new Player("F", "connection"));
                return list;
            }
        }
        
        
        
        
        [TestMethod()]
        public void CircularLinkedListTest() {
            if(playerList == null) {
                Assert.Fail();
            }

            if (playerList[3] == null) {
                Assert.Fail("Index 3 null");
            }

            Assert.AreEqual(playerList[3].Name, "D");
        }

        [TestMethod()]
        public void CountTest() {
            Assert.AreEqual(playerList.Count(), 6);
        }

        [TestMethod()]
        public void AddTest() {

            var testList = playerList;
            try {
                int oLength = testList.Count();
                string testPlayerName = "G";
                testList.Add(new Player(testPlayerName, "connection"));
                Assert.IsNotNull(testList[6]);
                Assert.AreEqual(testPlayerName, testList[6].Name);
                Assert.AreEqual(oLength + 1, testList.Count());
            }
            catch(Exception e) {
                Assert.Fail(e.Message);
            }
            
        }
    }
}