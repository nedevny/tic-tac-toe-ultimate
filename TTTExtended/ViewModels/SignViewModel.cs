using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTExtended.Common;
using Windows.UI;

namespace TTTExtended.ViewModels
{
    public class SignViewModel : BindableBase
    {
        private string sign;
        private Color color;


        public string Sign
        {
            get
            {
                return this.sign;
            }
            set
            {
                if (this.sign != value)
                {
                    this.sign = value;
                    this.OnPropertyChanged("Sign");
                }
            }
        }

        public Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.OnPropertyChanged("Color");
                }
            }
        }

        public SignViewModel()
            : this(string.Empty, new Color())
        {
        }

        public SignViewModel(string sign, Color color)
        {
            this.Sign = sign;
            this.Color = color;
        }
    }
}
