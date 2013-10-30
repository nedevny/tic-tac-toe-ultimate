using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace TTTExtended.Behavior
{
    public static class SelectionTappedCommandBehavior
    {
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(SelectionTappedCommandBehavior), new PropertyMetadata(null, OnCommandTapped));

        public static object GetCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(SelectionTappedCommandBehavior), new PropertyMetadata(null));

        private static void OnCommandTapped(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Grid c = d as Grid;
            if (c != null)
            {
                c.Tapped += OnTap;
            }
        }

        private static void OnTap(object sender, TappedRoutedEventArgs e)
        {
            Grid c = sender as Grid;
            ICommand cmd = c.GetValue(SelectionTappedCommandBehavior.CommandProperty) as ICommand;
            object param = c.GetValue(SelectionTappedCommandBehavior.CommandParameterProperty) ?? e.GetPosition(c);

            if (cmd != null && cmd.CanExecute(param))
            {
                cmd.Execute(param);
            }
        }
    }
}
