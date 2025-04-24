using NLog.Fluent;
using SIOS_Interferometer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using ush4.Models.SIOS;
using ush4.ViewModels;
using ush4.Views.Windows;

namespace ush4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            /*
            SiosDataView siosDataView = new SiosDataView();            
            siosDataView.Show();            
            */
            InitializeComponent();

            //Common.WPF.ViewModels.DeviceViewModel.NewDeviceStatusEvent += log.vm.NewDeviceStatusHandler;            
            //DeviceViewModel.NewDeviceStatusEvent += log.vm.NewDeviceStatusHandler;
            ush4.ViewModels.Disp.DeviceViewModel.NewDeviceStatusEvent += log.vm.NewDeviceStatusHandler;
            ush4.ViewModels.Disp.DeviceViewModel.NewDeviceStatusEvent += log_er.vm.NewDeviceStatusHandler;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            vm.ConnectDevices();

            //SiosDataView siosDataView = new SiosDataView();
            //siosDataView.Show();

            PidParamsWindow pidParamsWindow = new PidParamsWindow();
            pidParamsWindow.Show();
        }

        void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);            
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

        bool IsGood(char c)
        {
            if ((c >= '0' && c <= '9') || (c == '.'))
                return true;
            
            return false;
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {            
            vm.Close();

            //MotionController.KillUserProgram();
            Application.Current.Shutdown();
        }

        private void RemoteControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        /*
        private void BtnStartOsc_Click(object sender, RoutedEventArgs e)
        {
            BtnStartOsc.IsEnabled = false;            
        }
        */
    }
}
