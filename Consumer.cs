using System;
using System.Collections.Generic;
using System.Text;
using kafka4net;
using kafka4net.ConsumerImpl;

namespace KafkaSniffer
{
    class Consumer : BrokerInfo
    {
        private string _messageLog = "";
        private readonly List<string> _messageLogs = new List<string>();
        private string _topic = "", _groupId = "";
        private bool _notSubscribe = true;
        private kafka4net.Consumer _consumer;

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

        public async void SubScribe()
        {
            string brokerList = Ip + ":" + Port;
            var config = new Dictionary<string, object>
            {
                {"group.id", _groupId },
                {"bootstrap.servers", brokerList }
            };
            _consumer = new kafka4net.Consumer(new ConsumerConfiguration(brokerList, _topic, new StartPositionTopicEnd()));
            _consumer.OnMessageArrived.Subscribe(msg =>
            {
                OnMessage(this, msg);
            });
            await _consumer.IsConnected;
            NotSubscribe = false;
        }

        private void OnMessage(object sender, ReceivedMessage e)
        {
            DateTime now = DateTime.Now;
            _messageLogs.Add($"{now:yyyy-MM-dd HH:mm:ss} [{e.Offset}]\n{Encoding.UTF8.GetString(e.Key)}\n{Encoding.UTF8.GetString(e.Value)}\n\n");
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
