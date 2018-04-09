using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MAVNetTest
{
    class Packet
    {
        private readonly String _sequenceNumber;

        private readonly int _algorithmNo;

        private int _livedTime;

        private readonly DateTime _creationTime;

        private DateTime _arrivalTime;

        private String _messageRoute;

        private const int TimeToLive = 20;

        public int LivedTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        public String MessageRoute { get; set; }

        public Packet()
        {
            _sequenceNumber = null;
            _algorithmNo = 0;
            _livedTime = 0;
            _creationTime = DateTime.Now;
            _arrivalTime = _creationTime;
            _messageRoute = null;
        }

        public Packet(ref Packet packet)
        {
            _sequenceNumber = packet._sequenceNumber;
            _algorithmNo = packet._algorithmNo;
            _livedTime = packet._livedTime;
            _creationTime = packet._creationTime;
            _arrivalTime = packet._arrivalTime;
            _messageRoute = packet._messageRoute;
        }

        public Packet(String id, int number, int algorithmNo)
        {
            _sequenceNumber = id + number.ToString();
            _algorithmNo = algorithmNo;
            _livedTime = 0;
            _creationTime = DateTime.Now;
            _arrivalTime = _creationTime;
            _messageRoute = id;
        }

        public Packet(XmlDocument packetXmlDocument)
        {
            
        }

        public XmlDocument CreateXmlDocument()
        {
            XmlDocument result = new XmlDocument();

            return result;
        }
    }
}
