using System;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Hand hand = new Hand();
            hand.Add(new Card(0, 12));
            hand.Add(new Card(0, 11));
            hand.Add(new Card(0, 10));
            hand.Add(new Card(0, 9));
            hand.Add(new Card(0, 8));
            hand.Add(new Card(0, 0));
            hand.Add(new Card(0, 0));

            Hand hand2 = new Hand();
            hand2.Add(new Card(0, 11));
            hand2.Add(new Card(0, 10));
            hand2.Add(new Card(0, 9));
            hand2.Add(new Card(0, 8));
            hand2.Add(new Card(0, 7));
            hand2.Add(new Card(0, 0));
            hand2.Add(new Card(0, 0));

            if (hand.Beats(hand2))
                Console.WriteLine("hand 1 wins.");
            else
                Console.WriteLine("hand 2 wins.");
        }
    }
}
