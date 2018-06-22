using System.Collections.Generic;
using System.Text;
using System.Windows;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace KafkaSniffer
{
    class Producer : BrokerInfo
    {
        private string _topic = "", _key = "";
        private bool _notInit = true;
        private Confluent.Kafka.Producer<string, string> _producer;

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
            _producer = new Confluent.Kafka.Producer<string, string>(config
                , new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8));
            NotInit = false;
        }

        public void Close()
        {
            if (!NotInit)
            {
                _producer.Dispose();
                _producer = null;
            }
            NotInit = true;
        }

        public async void ProduceMessage()
        {
            Init();

            var result = await _producer.ProduceAsync(_topic
                , _key
                , Message
            );
            MessageBox.Show(result.Error
                ? $"Send message to [{_topic}] fail. Error:[{result.Error.Reason}]"
                : $"Send message to [{_topic}] succ.");
        }
    }
}
