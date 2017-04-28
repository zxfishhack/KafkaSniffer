using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace KafkaSniffer
{
    internal class Consumer : BrokerInfo
    {
        private string _messageLog = "";
        private readonly List<string> _messageLogs = new List<string>();
        private string _topic = "", _groupId = "";
        private bool _notSubscribe = true;
        private Consumer<string, string> _consumer;

        public string Topic
        {
            get { return _topic; }
            set
            {
                _topic = value;
                OnPropertyChanged("Topic");
            }
        }

        public string GroupId
        {
            get { return _groupId; }
            set
            {
                _groupId = value;
                OnPropertyChanged("GroupId");
            }
        }

        public string MessageLog
        {
            get { return _messageLog; }
            private set
            {
                _messageLog = value;
                OnPropertyChanged("MessageLog");
            }
        }

        public bool NotSubscribe
        {
            get
            {
                return _notSubscribe;
            }
            set
            {
                _notSubscribe = value;
                OnPropertyChanged("NotSubscribe");
            }
        }

        public void ClearMessageLog()
        {
            _messageLogs.Clear();
            MessageLog = "";
        }

        public void SubScribe()
        {
            Task.Run(() =>
            {
                var brokerList = Ip + ":" + Port;
                var config = new Dictionary<string, object>
                {
                    {"group.id", _groupId },
                    {"bootstrap.servers", brokerList }
                };
                _consumer = new Consumer<string, string>(
                    config, new StringDeserializer(Encoding.UTF8), new StringDeserializer(Encoding.UTF8)
                    );
                _consumer.Subscribe(Topic);
                _consumer.OnMessage += OnMessage;
                while (true)
                {
                    _consumer.Poll();
                }
            });
            NotSubscribe = false;
        }

        private void OnMessage(object sender, Message<string, string> e)
        {
            DateTime now = DateTime.Now;
            _messageLogs.Add($"{now:yyyy-MM-dd HH:mm:ss} Offset:[{e.Offset}] Length:[{e.Value.Length}]\n{e.Key}\n{e.Value}\n\n");
            if (_messageLogs.Count > 20)
            {
                _messageLogs.RemoveAt(0);
            }
            _messageLog = "";
            _messageLogs.ForEach(log =>
            {
                _messageLog += log;
            });
            OnPropertyChanged("MessageLog");
        }
    }
}
