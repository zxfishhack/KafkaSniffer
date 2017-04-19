using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KafkaSniffer
{
    internal class BrokerInfo : INotifyPropertyChanged
    {
        public static BrokerInfo Instance = new Lazy<BrokerInfo>(() => new BrokerInfo()).Value;

        public string Ip { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 9092;

        public bool Connected { get; set; } = false;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
