using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker.Models
{
    class Deck
    {
        List<Card> cards;

        public Deck()
        {
            cards = new List<Card>();
            FillDeck();
        }

        private void FillDeck()
        {
            for (int i = 0; i < 4; i ++)
            {
                for (int j = 0; j < 13; j++)
                {
                    Card c = new Card(i, j);
                    cards.Add(c);
                }
            }
        }

        public void Shuffle(int seed)
        {
            Card[] arr = cards.ToArray();

            Random rand = new Random(seed);
            for (int i = 0; i < arr.Length; i++)
            {
                int r = rand.Next(0, arr.Length-1);
                Card temp = arr[i];
                arr[i] = arr[r];
                arr[r] = temp;
            }

            List<Card> newCards = new List<Card>();
            foreach (Card c in arr)
            {
                newCards.Add(c);
            }

            this.cards = newCards;
        }

        public Card Draw()
        {
            Card c = cards.ElementAt(cards.Count-1);
            cards.RemoveAt(cards.Count-1);
            return c;
        }

        public int Size()
        {
            return cards.Count;
        }
    }
}
