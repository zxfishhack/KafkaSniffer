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
            Button obj = (Button)ConsumerList.Template.FindName("AddButton", ConsumerList);
            Vm.ConsumerList.Add(new Consumer());
            Vm.ConsumerList.Add(new Consumer());
            Vm.ConsumerList.Add(new Consumer());
            Vm.ConsumerList.Add(new Consumer());

            Vm.ProducerList.Add(new Producer());
            Vm.ProducerList.Add(new Producer());
            Vm.ProducerList.Add(new Producer());
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            Vm.ConsumerList.Add(new Consumer());
        }
    }
}
