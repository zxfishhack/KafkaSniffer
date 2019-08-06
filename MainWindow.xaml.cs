using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using NLog;

namespace KafkaSniffer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetDefault(object sender, RoutedEventArgs e)
        {
            Vm.BrokerInfo.SetDefault();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Button addButton = null;
            addButton = (Button)ConsumerList.Template.FindName("AddButton", ConsumerList);
            addButton.Click += AddConsumer;
            addButton = (Button)ProducerList.Template.FindName("AddButton", ProducerList);
            addButton.Click += AddProducer;
        }

        private void AddProducer(object sender, RoutedEventArgs e)
        {
            Vm.ProducerList.Add(new Producer());
        }

        private void AddConsumer(object sender, RoutedEventArgs e)
        {
            Vm.ConsumerList.Add(new Consumer());
        }

        private void ClearMessageLog(object sender, RoutedEventArgs e)
        {
            Vm.ConsumerList[ConsumerList.SelectedIndex].ClearMessageLog();
        }

        private void Subscribe(object sender, RoutedEventArgs e)
        {
            Vm.ConsumerList[ConsumerList.SelectedIndex].SubScribe();
        }

        private void ProduceMessage(object sender, RoutedEventArgs e)
        {
            Vm.ProducerList[ProducerList.SelectedIndex].ProduceMessage();
        }

        private void LogFile(object sender, RoutedEventArgs e)
        {
            var consumer = Vm.ConsumerList[ConsumerList.SelectedIndex];
            if (!consumer.IsLogToFile)
            {
                consumer.EndLogToFile();
            }
            else
            {
                var dlg = new SaveFileDialog
                {
                    Title = "Log to file ...",
                    Filter = "Log File(*.log)|*.log|All File(*.*)|*.*"
                };
                if (dlg.ShowDialog() == true)
                {
                    consumer.StartLogToFile(dlg.OpenFile());
                }
                else
                {
                    consumer.IsLogToFile = false;
                }
            }
            consumer.OnPropertyChanged("IsLogToFile");
        }

        private void CloseProducer(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Confirm Close?", "Notice", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Vm.ProducerList[ProducerList.SelectedIndex].Close();
                    Vm.ProducerList.RemoveAt(ProducerList.SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                LogManager.GetCurrentClassLogger().Error(ex);
            }

        }

        private void CloseConsumer(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Confirm Close?", "Notice", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Vm.ConsumerList[ConsumerList.SelectedIndex].Close();
                    Vm.ConsumerList.RemoveAt(ConsumerList.SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                LogManager.GetCurrentClassLogger().Error(ex);
            }

        }

        private void ModifyOffset(object sender, RoutedEventArgs e)
        {
            var wnd = new ModifyOffset(Vm.BrokerInfo.Endpoint);
            wnd.Owner = this;
            wnd.ShowDialog();
        }
    }
}
