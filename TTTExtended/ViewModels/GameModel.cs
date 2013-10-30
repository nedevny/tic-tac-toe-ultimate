using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TTTExtended.ViewModels
{
    [DataContract]
    public class GameModel
    {
        [DataMember(Name = "player")]
        public string PlayerName { get; set; }

        [DataMember(Name = "opponent")]
        public string Opponent { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }
    }
}
