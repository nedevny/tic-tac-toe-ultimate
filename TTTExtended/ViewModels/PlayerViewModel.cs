using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace TTTExtended.ViewModels
{
    public class PlayerViewModel
    {
        public string Name { get; set; }

        public string Sign { get; set; }

        public Color Color { get; set; }

        public PlayerViewModel(string name, string sign, Color color)
        {
            this.Name = name;
            this.Sign = sign;
            this.Color = color;
        }
    }
}
