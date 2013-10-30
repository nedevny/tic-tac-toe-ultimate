using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace TTTExtended.Helpers
{
    public class MakeMoveCommandParamModel
    {
        public Grid sender { get; set; }
        public TappedRoutedEventArgs e { get; set; }
    }
}
