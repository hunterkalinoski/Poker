using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp2
{
    class Hand
    {
        List<Card> cards;

        public Hand()
        {
            cards = new List<Card>();
        }

        public void Add(Card c)
        {
            cards.Add(c);
        }

        public Card Get(int index)
        {
            return cards.ElementAt(index);
        }

        public int[] GetScore()
        {
            Sort();
            int[] score;
            /*
             * this check is not necessary, royal flush will be a straight flush that beats all other straight flushes
             * 
            score = RoyalFlush();
            if (score[0] != -1) return score;
            *
            */

            score = StraightFlush();
            if (score[0] != -1) return score;

            score = FourOfAKind();
            if (score[0] != -1) return score;

            score = FullHouse();
            if (score[0] != -1) return score;

            score = Flush();
            if (score[0] != -1) return score;

            score = Straight();
            if (score[0] != -1) return score;

            score = ThreeOfAKind();
            if (score[0] != -1) return score;

            score = TwoPair();
            if (score[0] != -1) return score;

            score = OnePair();
            if (score[0] != -1) return score;

            score = HighCard();
            return score;
        }

        //redundant
        private int[] RoyalFlush()
        {
            int[] score = new int[1];
            score[0] = 9;

            int found = -1;
            for (int i = 0; i < 3; i++)
            {
                Card c = cards.ElementAt(i);
                int a = (int)c.Rank;
                int have = 0;
                int need = 4;
                for (int j = i + 1; j < cards.Count - 1; j++)
                {
                    Card c2 = cards.ElementAt(j);
                    int b = (int)c2.Rank;
                    if (b == a - 1 - have)
                    {
                        if (c2.Suit == c.Suit)
                            have++;
                    }
                }
                if (have >= need)
                {
                    found = a;
                    break;
                }
            }

            if (found == -1)
            {
                score[0] = found;
                return score;
            }

            if (found != 12)
            {
                score[0] = -1;
                return score;
            }
            return score;
        }

        private int[] StraightFlush()
        {
            int[] score = new int[2];
            score[0] = 8;

            int found = -1;
            for (int i = 0; i < 3; i++)
            {
                Card c = cards.ElementAt(i);
                int a = (int)c.Rank;
                int have = 0;
                int need = 4;
                for (int j = i + 1; j < cards.Count - 1; j++)
                {
                    Card c2 = cards.ElementAt(j);
                    int b = (int)c2.Rank;
                    if (b == a - 1 - have)
                    {
                        if (c2.Suit == c.Suit)
                            have++;
                    }
                }
                if (have >= need)
                {
                    found = a;
                    break;
                }
            }

            if (found == -1)
            {
                score[0] = found;
                return score;
            }

            score[1] = found;
            return score;
        }

        private int[] FourOfAKind()
        {
            int[] score = new int[3];
            score[0] = 7;

            int found = -1;
            for (int i = 0; i < 3; i++)
            {
                int a = (int)cards.ElementAt(i).Rank;
                int b = (int)cards.ElementAt(i + 1).Rank;
                int c = (int)cards.ElementAt(i + 2).Rank;
                int d = (int)cards.ElementAt(i + 3).Rank;
                if (a == b && a == c && a == d)
                {
                    found = a;
                    break;
                }
            }

            if (found == -1)
            {
                score[0] = -1;
                return score;
            }

            score[1] = found;

            int index = 0;
            while (index < cards.Count - 1)
            {
                int value = (int)cards.ElementAt(index).Rank;

                index++;
                if (value == found) continue;
                else
                {
                    score[2] = value;
                    break;
                }
            }

            return score;
        }

        private int[] FullHouse()
        {
            int[] score = new int[3];
            score[0] = 6;

            int found = -1;
            for (int i = 0; i < 4; i++)
            {
                int x = (int)cards.ElementAt(i).Rank;
                int y = (int)cards.ElementAt(i + 1).Rank;
                int z = (int)cards.ElementAt(i + 2).Rank;
                if (x == y && x == z)
                {
                    found = x;
                    break;
                }
            }

            if (found == -1)
            {
                score[0] = -1;
                return score;
            }

            score[1] = found;

            int found2 = -1;
            for (int i = 0; i < 5; i++)
            {
                int x = (int)cards.ElementAt(i).Rank;
                int y = (int)cards.ElementAt(i + 1).Rank;
                if (x == y && x != found)
                {
                    found2 = x;
                    break;
                }
            }

            if (found2 == -1)
            {
                score[0] = -1;
                return score;
            }

            score[2] = found2;
            return score;
        }

        private int[] Flush()
        {
            int[] score = new int[6];
            score[0] = 5;

            int found = -1;
            for (int i = 0; i < 4; i ++)
            {
                int count = 0;
                for (int j = 0; j < cards.Count - 1; j++)
                {
                    int x = (int)cards.ElementAt(j).Suit;
                    if (x == i)
                        count++;
                }

                if (count >= 5)
                {
                    found = i;
                    break;
                }
            }

            if (found == -1)
            {
                score[0] = -1;
                return score;
            }

            int have = 0;
            int need = 5;
            int index = 0;
            while (have < need)
            {
                Card c = cards.ElementAt(index);
                if ((int)c.Suit == found)
                {
                    score[1 + have] = (int)c.Rank;
                    have++;
                }
                index++;
            }

            return score;
        }

        private int[] Straight()
        {
            int[] score = new int[2];
            score[0] = 4;

            int found = -1;
            for (int i = 0; i < 3; i++)
            {
                int a = (int)cards.ElementAt(i).Rank;
                int have = 0;
                int need = 4;
                for (int j = i+1; j < cards.Count-1; j++)
                {
                    int b = (int)cards.ElementAt(j).Rank;
                    if (b == a-1-have)
                    {
                        have++;
                    }
                }
                if (have >= need)
                {
                    found = a;
                    break;
                }
            }

            if (found == -1)
            {
                score[0] = found;
                return score;
            }

            score[1] = found;
            return score;
        }

        private int[] ThreeOfAKind()
        {
            int[] score = new int[4];
            score[0] = 3;

            int found = -1;
            for (int i = 0; i < 4; i++)
            {
                int x = (int)cards.ElementAt(i).Rank;
                int y = (int)cards.ElementAt(i + 1).Rank;
                int z = (int)cards.ElementAt(i + 2).Rank;
                if (x == y && x == z)
                {
                    found = x;
                    break;
                }
            }

            if (found == -1)
            {
                score[0] = -1;
                return score;
            }

            score[1] = found;
            int have = 0;
            int needed = 2;
            int index = 0;
            while (have < needed && index < cards.Count - 1)
            {
                int value = (int)cards.ElementAt(index).Rank;

                index++;
                if (value == found) continue;
                else
                {
                    score[2 + have] = value;
                    have++;
                }
            }

            return score;
        }

        private int[] TwoPair()
        {
            int[] score = new int[4];
            score[0] = 2;

            int found = -1;
            for (int i = 0; i < 5; i++)
            {
                int x = (int)cards.ElementAt(i).Rank;
                int y = (int)cards.ElementAt(i + 1).Rank;
                if (x == y)
                {
                    found = x;
                    break;
                }
            }

            if (found == -1)
            {
                score[0] = -1;
                return score;
            }

            int found2 = -1;
            for (int i = 0; i < 5; i++)
            {
                int x = (int)cards.ElementAt(i).Rank;
                int y = (int)cards.ElementAt(i + 1).Rank;
                if (x == y)
                {
                    if (x != found)
                    {
                        found2 = x;
                        break;
                    }
                }
            }

            if (found2 == -1)
            {
                score[0] = -1;
                return score;
            }

            score[1] = found;
            score[2] = found2;

            int index = 0;
            while (index < cards.Count - 1)
            {
                int value = (int)cards.ElementAt(index).Rank;

                index++;
                if (value == found) continue;
                else if (value == found2) continue;
                else
                {
                    score[3] = value;
                    break;
                }
            }

            return score;
        }

        private int[] OnePair()
        {
            int[] score = new int[5];
            score[0] = 1;

            int found = -1;
            for (int i = 0; i < 5; i++)
            {
                int x = (int)cards.ElementAt(i).Rank;
                int y = (int)cards.ElementAt(i + 1).Rank;
                if (x == y)
                {
                    found = x;
                    break;
                }
            }

            if (found == -1)
            {
                score[0] = -1;
                return score;
            } 
            else
            {
                score[1] = found;
                int have = 0;
                int needed = 3;
                int index = 0;
                while (have < needed && index < cards.Count-1)
                {
                    int value = (int)cards.ElementAt(index).Rank;

                    index++;
                    if (value == found) continue;
                    else
                    {
                        score[2 + have] = value;
                        have++;
                    }
                }
            }

            return score;
        }

        private int[] HighCard()
        {
            int[] score = new int[6];
            score[0] = 0;
            score[1] = (int)cards.ElementAt(0).Rank;
            score[2] = (int)cards.ElementAt(1).Rank;
            score[3] = (int)cards.ElementAt(2).Rank;
            score[4] = (int)cards.ElementAt(3).Rank;
            score[5] = (int)cards.ElementAt(4).Rank;
            return score;
        }

        public bool Beats(Hand other)
        {
            int[] yourScore = this.GetScore();
            int[] otherScore = other.GetScore();

            if (yourScore[0] > otherScore[0])
                return true;
            else if (yourScore[0] < otherScore[0])
                return false;
            else
            {
                for (int i = 1; i < yourScore.Length; i++)
                {
                    if (yourScore[i] > otherScore[i])
                        return true;
                    else if (yourScore[i] < otherScore[i])
                        return false;
                }
            }
            //reaching this means a perfect tie (very rare) (returning false does not represent a tie well, but works for now)
            return false;
        }

        private void Sort()
        {
            List<Card> newCards = cards.OrderByDescending(o => o.Rank).ToList();
            cards = newCards;
        }
    }
}
