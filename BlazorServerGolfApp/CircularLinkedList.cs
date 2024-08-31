using System.Collections;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace BlazorServerGolfApp {
    public class CircularLinkedList<T> : IEnumerable<T> {

        private LinkedListNode<T>? _Head { get; set; }

        /*
        [JsonConstructorAttribute]
        public CircularLinkedList<T>(LinkedListNode<T> _head) {
            
        }
        */
        public IEnumerator<T> GetEnumerator() {
            if (_Head == null) {
                yield break;
            }

            LinkedListNode<T> current = _Head;
            do {
                yield return current.data;
                current = current.next;
            }
            while (current != null && current != _Head);
        }

        IEnumerator IEnumerable.GetEnumerator() { 
            return GetEnumerator(); 
        }


        public class LinkedListNode<T>{
            public T data;
            public LinkedListNode<T> next;
            public LinkedListNode<T> previous;

            public LinkedListNode() {
                data = default(T);
                next = null;
                previous = null;
            }

            public LinkedListNode(T data) {
                this.data = data;
            }

            public LinkedListNode(LinkedListNode<T> previous, T data, LinkedListNode<T> head) {
                this.data = data;
            }



        }
        
        public T? this[int _index]{
            get {
                //int index = _index % this.Count();
                LinkedListNode<T> node = null;
                for (int i = 0; i <= _index; i++) {

                    if (i == 0) {
                        node = _Head;
                    }
                    else {
                        node = node.next;
                    }

                    if(node == null) {
                        return default(T);
                    }

                    if (i==_index) {
                        return node.data;
                    }
                }
                return default(T);
            }
        }



        public CircularLinkedList() {
            _Head = null;
        }
        
        public CircularLinkedList(T data) {
            _Head = new LinkedListNode<T>(data);
            _Head.next = _Head;
            _Head.previous = _Head;
        }


        public CircularLinkedList(List<T> dataList) {
            _Head = null;
            foreach (T data in dataList) {
                this.Add(data);    
            }
        }

        public LinkedListNode<T> Start() {
            return _Head;
        }


        public int Count() {
            LinkedListNode<T> currentNode = _Head;
            int count = 0;
            
            if (currentNode == null) {
                return count;
            }
            else {
                count++;
            }
            
            while(currentNode.next != null && currentNode.next != _Head){
                currentNode = currentNode.next;
                count++;
            }
            return count;
        }

        public bool Add(T toAdd) {
            try {
                if (_Head == null) {
                    _Head = new LinkedListNode<T>(toAdd);
                    _Head.next = _Head;
                    _Head.previous = _Head;
                    //return _Head;
                }
                else {
                    var lastNode = _Head.previous;
                    var newNode = new LinkedListNode<T>(toAdd);
                    newNode.previous = lastNode;
                    newNode.next = _Head;

                    lastNode.next = newNode;
                    _Head.previous = newNode;
                    //return newNode;
                }
                return true;
            }
            catch (Exception e) { 
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public List<T> ToList() {
            List<T> toReturn = new List<T>();
            LinkedListNode<T> currentNode = _Head;

            while (currentNode != null && currentNode.data != null ) {
                toReturn.Add(currentNode.data);

                if(currentNode.next == null || currentNode.next == _Head) {
                    break;
                }
                currentNode = currentNode.next;
            }
            return toReturn;
        }


    }
}
