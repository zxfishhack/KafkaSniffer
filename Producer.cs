using System.Collections.Generic;
using System.Text;
using System.Windows;
using kafka4net;

namespace KafkaSniffer
{
    class Producer : BrokerInfo
    {
        private string _topic = "", _key = "";
        private bool _notInit = true;
        private kafka4net.Producer _producer;

        public string Topic
        {
            get { return _topic; }
            set
            {
                _topic = value;
                OnPropertyChanged("Topic");
            }
        }

        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                OnPropertyChanged("Key");
            }
        }

        public string Message { get; set; } = "";

        public bool NotInit
        {
            get { return _notInit; }
            set
            {
                _notInit = value;
                OnPropertyChanged("NotInit");
            }
        }

        private async void Init()
        {
            if (!NotInit)
            {
                return;
            }
            var brokerList = Ip + ":" + Port;
            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", brokerList }
            };
            try
            {
                _producer = new kafka4net.Producer(brokerList, new ProducerConfiguration(_topic));
                await _producer.ConnectAsync();
                NotInit = false;
            }
            catch
            {
                // ignored
            }
        }

        public void ProduceMessage()
        {
            Init();
            _producer.Send(new Message
            {
                Key = Encoding.UTF8.GetBytes(_key),
                Value = Encoding.UTF8.GetBytes(Message)
            });
        }
    }
}
