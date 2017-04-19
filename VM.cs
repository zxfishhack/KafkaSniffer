using System.Collections.Generic;

namespace KafkaSniffer
{
    internal class Vm
    {
        public BrokerInfo BrokerInfo { get; } = BrokerInfo.Instance;
        public List<Consumer> ConsumerList { get; } = new List<Consumer>();
        public List<Producer> ProducerList { get; } = new List<Producer>();
    }
}
