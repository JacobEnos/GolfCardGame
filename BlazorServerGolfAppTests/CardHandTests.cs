using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorServerGolfApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Moq;
using Xunit;

namespace BlazorServerGolfApp.Tests {

    [TestClass()]
    public class CardHandTests {

        public class TestHand { 
            public int points;
            public CardHand hand;
            
            public TestHand(int _points, CardHand _hand) {
                points = _points;
                hand = _hand;
            }
        }

        static TestHand noBonus {
            get {
                List<List<Card>> stack = new List<List<Card>> {
                    new List<Card> {
                        new Card("2", "hearts"),
                        new Card("4", "hearts"),
                        new Card("6", "hearts")
                    }, //12
                    new List<Card> {
                        new Card("3", "hearts"),
                        new Card("5", "hearts"),
                        new Card("7", "hearts")
                    } //15
                };

                CardHand testHand = new CardHand(stack);
                return new TestHand(27, testHand);
            }
        }


        static TestHand commonHand1 {
            get {
                List<List<Card>> stack = new List<List<Card>> {
                    new List<Card> {
                        new Card("Q", "hearts"),
                        new Card("A", "hearts"),
                        new Card("8", "hearts")
                    }, //12
                    new List<Card> {
                        new Card("2", "hearts"),
                        new Card("A", "clubs"),
                        new Card("8", "clubs")
                    } //15
                };

                CardHand testHand = new CardHand(stack);
                return new TestHand(2, testHand);
            }
        }


        static TestHand commonHand2 {
            get {
                List<List<Card>> stack = new List<List<Card>> {
                    new List<Card> {
                        new Card("10", "hearts"),
                        new Card("7", "hearts"),
                        new Card("2", "hearts")
                    }, //12
                    new List<Card> {
                        new Card("10", "hearts"),
                        new Card("Q", "hearts"),
                        new Card("K", "hearts")
                    } //15
                };

                CardHand testHand = new CardHand(stack);
                return new TestHand(19, testHand);
            }
        }


        static TestHand stackCancel {
            get {
                List<List<Card>> stack = new List<List<Card>> {
                    new List<Card> {
                        new Card("7", "hearts"),
                        new Card("8", "spades"),
                        new Card("9", "spades")
                    },
                    new List<Card> {
                        new Card("7", "hearts"),
                        new Card("8", "spades"),
                        new Card("9", "spades")
                    }
                };

                CardHand testHand = new CardHand(stack);
                return new TestHand(0, testHand);
            }
        }

        static TestHand cornersColumnCancel {
            get {
                List<List<Card>> stack = new List<List<Card>> {
                    new List<Card> {
                        new Card("7", "hearts"),
                        new Card("8", "spades"),
                        new Card("7", "spades")
                    },
                    new List<Card> {
                        new Card("7", "diamonds"),
                        new Card("8", "clubs"),
                        new Card("7", "clubs")
                    }
                };

                CardHand testHand = new CardHand(stack);
                return new TestHand(-25, testHand);
            }
        }

        static TestHand cornersCancel {
            get {
                List<List<Card>> stack = new List<List<Card>> {
                    new List<Card> {
                        new Card("7", "hearts"),
                        new Card("8", "spades"),
                        new Card("7", "spades")
                    }, //8
                    new List<Card> {
                        new Card("7", "diamonds"),
                        new Card("9", "clubs"),
                        new Card("7", "clubs")
                    } //9
                };

                CardHand testHand = new CardHand(stack);
                return new TestHand(-8, testHand);
            }
        }

        static TestHand aceStack {
            get {
                List<List<Card>> stack = new List<List<Card>> {
                    new List<Card> {
                        new Card("A", "hearts"),
                        new Card("8", "spades"),
                        new Card("9", "spades")
                    },
                    new List<Card> {
                        new Card("A", "diamonds"),
                        new Card("8", "clubs"),
                        new Card("9", "clubs")
                    }
                };

                CardHand testHand = new CardHand(stack);
                return new TestHand(-10, testHand);
            }
        }

        static TestHand aceCorners {
            get {
                List<List<Card>> stack = new List<List<Card>> {
                    new List<Card> {
                        new Card("A", "hearts"),
                        new Card("8", "spades"),
                        new Card("A", "spades")
                    },
                    new List<Card> {
                        new Card("A", "diamonds"),
                        new Card("8", "clubs"),
                        new Card("A", "clubs")
                    }
                };

                CardHand testHand = new CardHand(stack);
                return new TestHand(-25, testHand);
            }
        }


        //Output must be object[]. Object[] is used as parameter input to test methods
        public static IEnumerable<object[]> testHandSource {
            get {
                yield return new object[] { commonHand2 };
                yield return new object[] { commonHand1 };
                yield return new object[] { noBonus }; //what "yield" do?
                yield return new object[] { aceStack };
                yield return new object[] { stackCancel };
                yield return new object[] { cornersColumnCancel };
                yield return new object[] { cornersCancel };
                yield return new object[] { aceCorners };
            }
        }

        [TestMethod()]
        [DynamicData(nameof(testHandSource), DynamicDataSourceType.Property)]
        public void CardHandGetHandPointsTest(TestHand _testHand) {
            Assert.AreEqual(_testHand.points, _testHand.hand.GetHandPoints());
        }

    }
}
