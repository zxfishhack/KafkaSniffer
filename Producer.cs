using System.Collections.Generic;
using System.Text;
using System.Windows;
using Confluent.Kafka;
using System;

namespace KafkaSniffer
{
    class Producer : BrokerInfo
    {
        private string _topic = "", _key = "";
        private bool _notInit = true;
        private IProducer<string, string> _producer;

        ~Producer()
        {
            Close();
        }

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
            var brokerList = Endpoint;
            var config = new ProducerConfig
            {
                BootstrapServers = brokerList,
                ApiVersionRequest = true,
                ApiVersionRequestTimeoutMs = 0,
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
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

            try
            {
                var result = await _producer.ProduceAsync(_topic
                    , new Message<string, string>
                    {
                        Key = Key,
                        Value = Message,
                    });
                MessageBox.Show($"Send message to [{_topic}] success.");
            }
            catch (ProduceException<string, string> e)
            {
                MessageBox.Show($"Send message to [{_topic}] fail. Error:[{e.Error.Reason}]");
            }
        }
    }
}
