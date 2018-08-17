using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace KafkaSniffer
{
    /// <summary>
    /// Interaction logic for ModifyOffset.xaml
    /// </summary>
    public partial class ModifyOffset : Window
    {
        private ModifyOffsetModel dataContext = new ModifyOffsetModel();

        public ModifyOffset(string endpoint)
        {
            dataContext.EndPoint = endpoint;
            DataContext = dataContext;
            InitializeComponent();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            var config = new Dictionary<string, object>
            {
                {"group.id", dataContext.GroupId },
                {"bootstrap.servers", dataContext.EndPoint },
                {"enable.auto.commit",  "false"}
            };

            var consumer = new Consumer<string, string>(
                config, new StringDeserializer(Encoding.UTF8), new StringDeserializer(Encoding.UTF8)
                );
            var topicPartition = new List<TopicPartition>();
            var meta = consumer.GetMetadata(false, TimeSpan.FromSeconds(10));
            var topicMeta = meta.Topics.Find(_ => _.Topic == dataContext.Topic);
            foreach(var partition in topicMeta.Partitions)
            {
                topicPartition.Add(new TopicPartition(dataContext.Topic, partition.PartitionId));
            }
            var topicPartitionOffset = consumer.Committed(topicPartition, TimeSpan.FromSeconds(10));
            dataContext.TopicPartionList.Clear();
            foreach (var p in topicPartitionOffset)
            {
                dataContext.TopicPartionList.Add(new PartitionOffset
                {
                    Partition = p.Partition,
                    Offset = p.Offset.Value,
                    Tooltip = p.Offset.ToString(),
                });
            }
        }

        private async void Apply_Click(object sender, RoutedEventArgs e)
        {
            var config = new Dictionary<string, object>
            {
                {"group.id", dataContext.GroupId },
                {"bootstrap.servers", dataContext.EndPoint },
                {"enable.auto.commit",  "false"}
            };

            var consumer = new Consumer<string, string>(
                config, new StringDeserializer(Encoding.UTF8), new StringDeserializer(Encoding.UTF8)
                );
            var topicPartitionOffset = new List<TopicPartitionOffset>();
            foreach(var po in dataContext.TopicPartionList)
            {
                var offset = new Offset(po.Offset);
                if (offset != Offset.Invalid)
                {
                    topicPartitionOffset.Add(new TopicPartitionOffset(dataContext.Topic, po.Partition, offset));
                }
            }
            var res = await consumer.CommitAsync(topicPartitionOffset);
            MessageBox.Show($"Modify Result: {res.Error.ToString()}");
        }
    }

    internal class PartitionOffset
    {
        public int Partition { get; set; } = -1;
        public long Offset { get; set; } = -1;
        public string Tooltip { get; set; } = "";
    }

    internal class ModifyOffsetModel : INotifyPropertyChanged
    {
        private string _endPoint = "";
        private string _topic = "";
        private string _groupId = "";

        public string EndPoint
        {
            get
            {
                return _endPoint;
            }

            set
            {
                _endPoint = value;
                OnPropertyChanged("EndPoint");
            }
        }

        public string Topic
        {
            get
            {
                return _topic;
            }

            set
            {
                _topic = value;
                OnPropertyChanged("Topic");
            }
        }

        public string GroupId
        {
            get
            {
                return _groupId;
            }

            set
            {
                _groupId = value;
                OnPropertyChanged("GroupId");
            }
        }

        public ObservableCollection<PartitionOffset> TopicPartionList { get; } = new ObservableCollection<PartitionOffset>();

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
