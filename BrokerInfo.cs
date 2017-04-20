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

        private string _ip = BrokerInfo.Instance == null ? "" : BrokerInfo.Instance.Ip;
        private int _port = BrokerInfo.Instance == null ? 9092 : BrokerInfo.Instance.Port;

        public string Ip
        {
            get { return _ip; }
            set
            {
                _ip = value;
                OnPropertyChanged("Ip");
            } 
        }

        public int Port
        {
            get
            {
                return _port;
                
            }
            set
            {
                _port = value;
                OnPropertyChanged("Port");
            }
        }

        public bool Setted { get; private set; } = false;

        public bool NotSetted => !Setted;

        public void SetDefault()
        {
            Setted = true;
            OnPropertyChanged("Setted");
            OnPropertyChanged("NotSetted");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
