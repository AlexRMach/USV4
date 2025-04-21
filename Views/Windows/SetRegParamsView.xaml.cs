using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ush4.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для SetRegParamsView.xaml
    /// </summary>
    public partial class SetRegParamsView : UserControl
    {
        public SetRegParamsView()
        {
            InitializeComponent();
        }

        private void PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if ((e.Key.Equals(Key.Enter)) || (e.Key.Equals(Key.Return)))
            {
                e.Handled = true;

                /*
                RoutedEventArgs r = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, Key.Tab);
                r.RoutedEvent = Keyboard.KeyDownEvent;

                ((TextBox)sender).RaiseEvent(r);// (sender, new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, Key.Tab));
                */
                //BindingExpression be = FreqInputTextBox.GetBindingExpression(TextBox.TextProperty);
                BindingExpression be = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
                be.UpdateSource();
            }
        }
    }
}
