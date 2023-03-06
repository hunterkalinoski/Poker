using System;
using System.Collections.Generic;
using System.Text;


namespace ConsoleApp2
{
    class Card
    {
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }
        public string Image { get; private set; }

        public Card(int suit, int rank)
        {
            this.Suit = (Suit)suit;
            this.Rank = (Rank)rank;
            this.Image = PickImage();
        }

        private string PickImage()
        {
            string path = "Poker.Images.";

            switch (Rank)
            {
                case Rank.Ace:
                    path += "ace_of_";
                    break;
                case Rank.Two:
                    path += "2_of_";
                    break;
                case Rank.Three:
                    path += "3_of_";
                    break;
                case Rank.Four:
                    path += "4_of_";
                    break;
                case Rank.Five:
                    path += "5_of_";
                    break;
                case Rank.Six:
                    path += "6_of_";
                    break;
                case Rank.Seven:
                    path += "7_of_";
                    break;
                case Rank.Eight:
                    path += "8_of_";
                    break;
                case Rank.Nine:
                    path += "9_of_";
                    break;
                case Rank.Ten:
                    path += "10_of_";
                    break;
                case Rank.Jack:
                    path += "jack_of_";
                    break;
                case Rank.Queen:
                    path += "queen_of_";
                    break;
                case Rank.King:
                    path += "king_of_";
                    break;
            }

            switch (Suit)
            {
                case Suit.Clubs:
                    path += "clubs";
                    break;
                case Suit.Diamonds:
                    path += "diamonds";
                    break;
                case Suit.Hearts:
                    path += "hearts";
                    break;
                case Suit.Spades:
                    path += "spades";
                    break;
            }

            path += ".png";
            return path;
        }

        public override string ToString()
        {
            return String.Format("{0} of {1}", Rank, Suit);
        }
    }
}
