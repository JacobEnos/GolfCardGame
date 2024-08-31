using System.Collections.Generic;
using System;

namespace BlazorServerGolfApp
{
    public class Deck
    {
        public List<Card> Cards { get; set; }

        public Deck()
        {
            Cards = new List<Card>();

            string[] suites = new string[4] { "spades", "clubs", "hearts", "diamonds" };
            foreach (string s in suites)
            {
                string[] numbers = new string[] {"2","3","4","5","6","7","8","9","10","J","Q","K","A"};
                foreach (string n in numbers)
                {
                    Cards.Add(new Card(n, s));
                }
            }
        }

        public Deck(List<Card> cards)
        {
            Cards = cards;
        }

        public List<Card> Shuffle()
        {
            for (int loops=100; 0<loops; loops--)
            {
                for (int i = Cards.Count - 1; i > 0; i--) {
                    Random random = new Random();
                    int j = random.Next(i + 1);
                    Card temp = Cards[i];
                    Cards[i] = Cards[j];
                    Cards[j] = temp;
                }
            }

            return Cards;
        }

        public Card Pop() {
            Card drawn = Cards[0];
            Cards.RemoveAt(0);
            return drawn;
        }
    }
}