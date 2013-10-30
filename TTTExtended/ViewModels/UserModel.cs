using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTTExtended.ViewModels
{
    public class UserModel
    {
        public string PlayerName { get; set; }
        public int Losses { get; set; }
        public int Wins { get; set; }
        public int Ties { get; set; }
    }
}
