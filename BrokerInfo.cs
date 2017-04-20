using System;
using System.ComponentModel;

namespace KafkaSniffer
{
    internal class BrokerInfo : INotifyPropertyChanged
    {
        public static BrokerInfo Instance = new Lazy<BrokerInfo>(() => new BrokerInfo()).Value;

        private string _ip = Instance?.Ip ?? "";
        private int _port = Instance?.Port ?? 9092;

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

        public bool Setted { get; private set; }

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
