using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System.Collections.ObjectModel;

namespace KafkaSniffer
{
    internal class Consumer : BrokerInfo
    {
        private string _messageLog = "";
        private readonly List<string> _messageLogs = new List<string>();
        private string _topic = "", _groupId = "";
        private bool _notSubscribe = true;
        private bool _end = false;
        private int _count = 0;
        private int _consumerCnt = 0;
        private readonly ManualResetEvent _endDone = new ManualResetEvent(false);
        private StreamWriter _logFile = null;

        public string CurOffsetType { get; set; } = "Stored Offset";
        public ObservableCollection<string> OffsetTypeList { get; } = new ObservableCollection<string> { "Beginning", "End", "Stored Offset" };

        ~Consumer()
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

        public string GroupId
        {
            get { return _groupId; }
            set
            {
                _groupId = value;
                OnPropertyChanged("GroupId");
            }
        }

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged("Count");
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
            if (_end)
            {
                return;
            }
            _end = true;
            if (!NotSubscribe)
            {
                _endDone.WaitOne();
            }
        }

        public void SubScribe()
        {
            _consumerCnt = 0;
            _end = false;
            _endDone.Reset();
            var brokerList = Endpoint;
            var config = new Dictionary<string, object>
            {
                {"group.id", _groupId },
                {"bootstrap.servers", brokerList }
            };

            var consumer = new Consumer<string, string>(
                config, new StringDeserializer(Encoding.UTF8), new StringDeserializer(Encoding.UTF8)
                );
            consumer.OnPartitionsAssigned += (obj, partitions) =>
            {
                if (CurOffsetType == "Beginning")
                {
                    var par = partitions.Select(p => new TopicPartitionOffset(p.Topic, p.Partition, Confluent.Kafka.Offset.Beginning)).ToList();
                    consumer.Assign(par);
                }
                else if (CurOffsetType == "End")
                {
                    var par = partitions.Select(p => new TopicPartitionOffset(p.Topic, p.Partition, Confluent.Kafka.Offset.End)).ToList();
                    consumer.Assign(par);
                }
                else
                {
                    consumer.Assign(partitions);
                }
                _messageLogs.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} partitions assigned.\n\n");
                OnPropertyChanged("MessageLog");
            };
            consumer.Subscribe(Topic);
            _messageLogs.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} subscribe done.\n\n");
            OnPropertyChanged("MessageLog");
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
            if (_count > 0)
            {
                if (_consumerCnt == _count)
                {
                    _end = true;
                    Task.Run(() =>
                    {
                        _endDone.WaitOne();
                        NotSubscribe = true;
                    });
                }
                _consumerCnt++;
            }
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
