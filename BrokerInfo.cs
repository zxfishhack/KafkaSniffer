using System;
using System.ComponentModel;
using Confluent.Kafka;
using NLog;

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

        public bool Debug { get; set; } = Instance?.Debug ?? false;

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

        public static LogLevel MapLogLevel(SyslogLevel lvl)
        {
            switch (lvl)
            {
                case SyslogLevel.Emergency:
                    return LogLevel.Fatal;
                case SyslogLevel.Alert:
                    return LogLevel.Fatal;
                case SyslogLevel.Critical:
                    return LogLevel.Fatal;
                case SyslogLevel.Error:
                    return LogLevel.Error;
                case SyslogLevel.Warning:
                    return LogLevel.Warn;
                case SyslogLevel.Notice:
                    return LogLevel.Info;
                case SyslogLevel.Info:
                    return LogLevel.Info;
                case SyslogLevel.Debug:
                    return LogLevel.Debug;
            }

            return LogLevel.Info;
        }
    }
}
