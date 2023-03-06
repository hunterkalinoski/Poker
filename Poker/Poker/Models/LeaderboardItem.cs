using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Models
{
    public class LeaderboardItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public override string ToString()
        {
            String str = "Id " + Id + " Name " + Name + " Score " + Score;
            return str;
        }
    }
}
