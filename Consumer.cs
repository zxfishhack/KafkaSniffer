using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
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
        private bool _end = false;
        private readonly AutoResetEvent _endDone = new AutoResetEvent(false);
        private StreamWriter _logFile = null;

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

        public bool IsLogToFile { get; set; } = false;

        public void ClearMessageLog()
        {
            _messageLogs.Clear();
            MessageLog = "";
        }

        public void Close()
        {
            _end = true;
            if (!NotSubscribe)
            {
                _endDone.WaitOne();
            }
        }

        public void SubScribe()
        {
            var brokerList = Endpoint;
            var config = new Dictionary<string, object>
            {
                {"group.id", _groupId },
                {"bootstrap.servers", brokerList }
            };

            var consumer = new Consumer<string, string>(
                config, new StringDeserializer(Encoding.UTF8), new StringDeserializer(Encoding.UTF8)
                );
            consumer.Subscribe(Topic);
            consumer.OnMessage += OnMessage;
            Task.Run(() =>
            {
                while (!_end)
                {
                    consumer.Poll(100);
                }
                consumer.Dispose();
                _endDone.Set();
            });
            NotSubscribe = false;
        }

        private void OnMessage(object sender, Message<string, string> e)
        {
            var now = DateTime.Now;
            var msg = $"{now:yyyy-MM-dd HH:mm:ss} Offset:[{e.Offset}] Length:[{e.Value.Length}]\n{e.Key}\n{e.Value}\n\n";
            _messageLogs.Add(msg);
            _logFile?.Write(msg);
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

        public void EndLogToFile()
        {
            if (_logFile != null)
            {
                _logFile.Close();
                _logFile = null;
            }
        }

        public void StartLogToFile(Stream fs)
        {
            _logFile = new StreamWriter(fs);
        }
    }
}
