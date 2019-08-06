using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Collections.ObjectModel;
using System.Windows;

namespace KafkaSniffer
{
    internal class Consumer : BrokerInfo
    {
        private string _messageLog = "";
        private readonly List<string> _messageLogs = new List<string>();
        private string _topic = "", _groupId = "";
        private bool _notSubscribe = true;
        private int _count = 0;
        private int _consumerCnt = 0;
        private StreamWriter _logFile = null;
        private bool _firstAssigned = true;
        private Task _pollTask = null;
        private CancellationTokenSource _cancelConsumer = null;

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

        public async void Close()
        {
            _cancelConsumer.Cancel();
            if (!NotSubscribe && _pollTask != null)
            {
                await _pollTask;
            }
        }

        public void SubScribe()
        {
            _cancelConsumer = new CancellationTokenSource();
            _consumerCnt = 0;
            var brokerList = Endpoint;
            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                GroupId = _groupId,
                ApiVersionRequest = true,
                EnableAutoCommit = true,
                ApiVersionRequestTimeoutMs = 0,
            };
            var consumer = new ConsumerBuilder<string, string>(config)
                .SetPartitionsAssignedHandler((c, partitions) =>
            {
                var parFiltered = partitions.Where(_ => _.Topic == Topic).ToList();
                if (!parFiltered.Any())
                {
                    return;
                }
                if (!_firstAssigned)
                {
                    c.Assign(parFiltered);
                    return;
                }
                if (CurOffsetType == "Beginning")
                {
                    var par = parFiltered.Select(p => new TopicPartitionOffset(p.Topic, p.Partition, Confluent.Kafka.Offset.Beginning)).ToList();
                    c.Assign(par);
                }
                else if (CurOffsetType == "End")
                {
                    var par = parFiltered.Select(p => new TopicPartitionOffset(p.Topic, p.Partition, Confluent.Kafka.Offset.End)).ToList();
                    c.Assign(par);
                }
                else
                {
                    c.Assign(parFiltered);
                }
                _messageLogs.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} partitions assigned.\n\n");
                OnPropertyChanged("MessageLog");
                _firstAssigned = false;
            }).Build();
            consumer.Subscribe(Topic);
            _messageLogs.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} subscribe done.\n\n");
            OnPropertyChanged("MessageLog");
            _pollTask = Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            var result = consumer.Consume(_cancelConsumer.Token);
                            if (result.IsPartitionEOF)
                            {
                                continue;
                            }
                            OnMessage(result);
                        }
                        catch (ConsumeException e)
                        {
                            MessageBox.Show($"Consume error: {e.Error.Reason}");
                        }

                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
                consumer.Dispose();
            });
            NotSubscribe = false;
        }

        private void OnMessage(ConsumeResult<string, string> e)
        {
            if (_count > 0)
            {
                if (_consumerCnt == _count)
                {
                    _cancelConsumer.Cancel();
                    Task.Run(() =>
                    {
                        NotSubscribe = true;
                    });
                }
                _consumerCnt++;
            }
            var now = DateTime.Now;
            var msg = $"{now:yyyy-MM-dd HH:mm:ss} Partition:[{e.Partition}] Offset:[{e.Offset}] Length:[{e.Value.Length}]\n{e.Key}\n{e.Value}\n\n";
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
