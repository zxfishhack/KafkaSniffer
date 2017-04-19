using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaSniffer
{
    class Producer
    {
        public string Topic { get; set; } = "Test Topic";
        public string Key { get; set; } = "Test Key";
    }
}
