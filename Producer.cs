using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Confluent.Kafka;

namespace KafkaSniffer
{
    class Producer : BrokerInfo
    {
        private string _topic = "", _key = "";
        private bool _notInit = true;
        private Confluent.Kafka.Producer _producer;

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

        public Producer()
        {
            Ip = BrokerInfo.Instance.Ip;
            Port = BrokerInfo.Instance.Port;
        }

        private void Init()
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
            _producer = new Confluent.Kafka.Producer(config);
            NotInit = false;
        }

        public void ProduceMessage()
        {
            Init();

            Message result = _producer.ProduceAsync(_topic
                , Encoding.UTF8.GetBytes(_key)
                , Encoding.UTF8.GetBytes(Message)
            ).Result;
            if (result.Error)
            {
                MessageBox.Show($"Send message to [{_topic}] fail. Error:[{result.Error.Reason}]");
            }
            else
            {
                MessageBox.Show($"Send message to [{_topic}] succ.");
            }
        }
    }
}
