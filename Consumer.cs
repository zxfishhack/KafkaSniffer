using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KafkaSniffer
{
    class Consumer : INotifyPropertyChanged
    {
        public string Topic { get; set; } = "Test Topic";
        public string GroupId { get; set; } = "Test Group";
        private string _messageLog = "";

        public string MessageLog
        {
            get { return _messageLog; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
