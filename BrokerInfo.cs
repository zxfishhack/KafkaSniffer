using System;
using System.ComponentModel;

namespace KafkaSniffer
{
    internal class BrokerInfo : INotifyPropertyChanged
    {
        public static BrokerInfo Instance = new Lazy<BrokerInfo>(() => new BrokerInfo()).Value;

        private string _endpoint = Instance?.Endpoint ?? "";

        public string Endpoint
        {
            get { return _endpoint; }
            set
            {
                _endpoint = value;
                OnPropertyChanged("Endpoint");
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
