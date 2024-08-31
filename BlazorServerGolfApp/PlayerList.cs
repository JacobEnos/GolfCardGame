using System.Collections;
using System.Threading;
using System.Xml.Linq;

namespace BlazorServerGolfApp
{
    public class PlayerList : IEnumerable<Player> {
        private CircularLinkedList<Player> _Players;

        public IEnumerator<Player> GetEnumerator() {
            if (_Players == null) {
                yield break;
            }

            foreach (Player p in _Players) {
                yield return p;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public PlayerList() {
            _Players = new();
        }

        public PlayerList(List<Player> players) {
            _Players = new(players);
        }

        public PlayerList(Player player) {
            _Players = new(player);
            //_Players.Add(player);
        }

        public Player this[int i] {
            get {
                return _Players[i];
            }
        }

        public Player this[string name] {
            get {
                foreach (Player p in _Players) {
                    if (p.Name == name) return p;
                }
                return null;
            }
        }

        

        public int Count() {
            return _Players.Count();
        }


        public Player NextActivePlayer(string currentPlayerName) {
            if (String.IsNullOrEmpty(currentPlayerName)) {
                return null;
            }

            bool currentPlayerFound = false;
            //Foreach Enumerator will not loop the list, custom loop here
            var currentNode = _Players.Start();
            if(currentNode == null || currentNode.data == null) {
                return null;
            }


            do {
                if (currentNode.data.Name == currentPlayerName) {
                    if (currentPlayerFound) {
                        return null; //looped twice, not found
                    }

                    currentPlayerFound = true;
                    currentNode = currentNode.next;
                    continue;
                }

                if (currentPlayerFound && currentNode.data.IsPlaying) {
                    return currentNode.data;
                }

                currentNode = currentNode.next;

            } while (true);
        }


        public List<Player> GetPlayerQueue(string currentPlayerName, bool includeActivePlayer = false, string finalFlipper = null) {

            if (String.IsNullOrEmpty(currentPlayerName)) {
                return _Players.ToList();
            }

            var queue = new List<Player>();

            bool currentPlayerFound = false;
            //Foreach Enumerator will not loop the list, custom loop here
            var currentNode = _Players.Start();
            if (currentNode == null || currentNode.data == null) {
                return null;
            }


            do {
                if (currentNode.data.Name == currentPlayerName) {
                    if (currentPlayerFound) {
                        break; //looped twice, queue complete
                    }

                    if (includeActivePlayer) {
                        queue.Add(currentNode.data);
                    }
                    currentPlayerFound = true;
                    currentNode = currentNode.next;
                    continue;
                }

                if (currentPlayerFound && currentNode.data.IsPlaying /*Untested*/&& currentNode.data.Name != finalFlipper) {
                    queue.Add(currentNode.data);
                }

                currentNode = currentNode.next;

            } while (true);
            return queue;
        }


        public bool AllHandsClear() {

            //Foreach Enumerator will not loop the list, custom loop here
            var currentNode = _Players.Start();
            if (currentNode == null || currentNode.data == null) {
                return true;
            }

            do {
                if (currentNode.data.IsPlaying == true && currentNode.data.Cards != null && currentNode.data.Cards.Count != 0) {
                    return false;
                }

                currentNode = currentNode.next;

            } while (currentNode != _Players.Start());
            return true;
        }

        public bool Add(Player player) {
            try {
                _Players.Add(player);
                return true;
            }
            catch {
                return false;
            }
        }

        public bool Add(string name) {
            Player p = new(name);
            try {
                _Players.Add(p);
                return true;
            }
            catch {
                return false;
            }
        }

        public List<Player> ToList() {
            return _Players.ToList();
        }
    }
}
