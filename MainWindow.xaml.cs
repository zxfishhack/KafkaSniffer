using System.Windows;
using System.Windows.Controls;

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
            //addButton = (Button)ConsumerList.Template.FindName("AddButton", ConsumerList);
            //addButton.Click += AddConsumer;
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
    }
}
