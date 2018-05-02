using System;
using System.Text;
using System.Xml;

namespace MAVNetTest
{
    internal class Packet
    {
        /// <summary>
        /// 序列编号
        /// No. of the message in sequence
        /// </summary>
        private readonly string _sequenceNumber;

        /// <summary>
        /// 算法编号
        /// No. of the algorithmNo
        /// </summary>
        private int _algorithmNo;

        /// <summary>
        /// 跳数
        /// hop
        /// </summary>
        private int _livedTime;

        /// <summary>
        /// 创建时间
        /// Time of Creation
        /// </summary>
        private readonly DateTime _creationTime;

        /// <summary>
        /// 到达时间
        /// Time of Arrival
        /// </summary>
        private DateTime _arrivalTime;

        /// <summary>
        /// 消息转发的路径
        /// The route for message transmission
        /// </summary>
        private string _messageRoute;

        /// <summary>
        /// 包中存储的数据
        /// data in the packet
        /// </summary>
        private byte[] _data;

        /// <summary>
        /// 最大跳数
        /// Time to live
        /// </summary>
        public const int TimeToLive = 20;

        /// <summary>
        /// 跳数，用于获取和更改的属性
        /// The property of _livedTime
        /// </summary>
        public int LivedTime
        {
            get => _livedTime;
            set => _livedTime = value;
        }

        /// <summary>
        /// 到达时间，用于获取和更改的属性
        /// The property of _arrivalTime
        /// </summary>
        public DateTime ArrivalTime
        {
            get => _arrivalTime;
            set => _arrivalTime = value;
        }

        /// <summary>
        /// 转发路径，用于获取和更改的属性
        /// The property of _messageRoute
        /// </summary>
        public string MessageRoute
        {
            get => _messageRoute;
            set => _messageRoute += value;
        }

        /// <summary>
        /// 算法序号，用于获取和更改的属性
        /// The property of _algorithmNo
        /// </summary>
        public int AlgorithmNo
        {
            get => _algorithmNo;
            set => _algorithmNo = value;
        }

        /// <summary>
        /// 创建时间，用于获取的属性
        /// The property of _creationTime
        /// </summary>
        public DateTime CreationTime => _creationTime;

        /// <summary>
        /// 消息序列号，用于获取的属性
        /// The property of _sequenceNumber
        /// </summary>
        public string SequenceNumber => _sequenceNumber;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Packet()
        {
            _sequenceNumber = null;
            _algorithmNo = 0;
            _livedTime = 0;
            _creationTime = DateTime.Now;
            _arrivalTime = _creationTime;
            _messageRoute = null;
        }

        /// <summary>
        /// 复制构造函数
        /// </summary>
        /// <param name="packet"></param>
        public Packet(Packet packet)
        {
            _sequenceNumber = packet._sequenceNumber;
            _algorithmNo = packet._algorithmNo;
            _livedTime = packet._livedTime;
            _creationTime = packet._creationTime;
            _arrivalTime = packet._arrivalTime;
            _messageRoute = packet._messageRoute;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">实体的类型</param>
        /// <param name="id">实体的序号 The ID of MAV</param>
        /// <param name="number">消息序号 The No. of message in sequence</param>
        /// <param name="algorithmNo">算法序号 The No. of algorithm</param>
        public Packet(int type, string id, int number, int algorithmNo)
        {
            _sequenceNumber = type + id + number;
            _algorithmNo = algorithmNo;
            _livedTime = 0;
            _creationTime = DateTime.Now;
            _arrivalTime = _creationTime;
            _messageRoute = id;
            CreateBytes();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="packetXmlDocument">XML对象   XML Object</param>
        public Packet(XmlDocument packetXmlDocument)
        {
            XmlElement root = packetXmlDocument.DocumentElement;

            if (root != null)
            {
                XmlNode tmp = root.FirstChild.FirstChild;
                XmlNode data = root.LastChild;

                _data = Encoding.ASCII.GetBytes(data.InnerText);

                _sequenceNumber = tmp.InnerText;
                tmp = tmp.NextSibling;

                if (tmp != null)
                {
                    _algorithmNo = Int32.Parse(tmp.InnerText);
                    tmp = tmp.NextSibling;
                }

                if (tmp != null)
                {
                    _livedTime = Int32.Parse(tmp.InnerText);
                    tmp = tmp.NextSibling;
                }

                if (tmp != null)
                {
                    _creationTime = DateTime.Parse(tmp.InnerText);
                    tmp = tmp.NextSibling;
                }

                if (tmp != null)
                {
                    _arrivalTime = DateTime.Parse(tmp.InnerText);
                    tmp = tmp.NextSibling;
                }

                if (tmp != null) _messageRoute = tmp.InnerText;
            }
        }

        /// <summary>
        /// 将Packet实例的信息转化为XML格式对象
        /// Convert the instance of Class Packet to a XML Object
        /// </summary>
        /// <returns>XmlDocument对象  XmlDocument Object</returns>
        public XmlDocument CreateXmlDocument()
        {
            XmlDocument result = new XmlDocument();

            XmlElement packetElement = result.CreateElement("Packet");
            XmlElement headerElement = result.CreateElement("Header");
            XmlElement sequenceNumber = result.CreateElement("SequenceNumber");
            XmlElement algorithmNo = result.CreateElement("AlgorithmNo");
            XmlElement livedTime = result.CreateElement("LivedTime");
            XmlElement creationTime = result.CreateElement("CreationTime");
            XmlElement arrivalTime = result.CreateElement("ArrivalTime");
            XmlElement messageRoute = result.CreateElement("MessageRoute");
            XmlElement data = result.CreateElement("Date");

            XmlText sequenceNumberText = result.CreateTextNode(_sequenceNumber);
            XmlText algorithmNoText = result.CreateTextNode(_algorithmNo.ToString());
            XmlText livedTimeText = result.CreateTextNode(_livedTime.ToString());
            XmlText creationTimeText = result.CreateTextNode(_creationTime.ToString("yyyy/MM/dd HH:mm:ss.fff"));
            XmlText arrivalTimeText = result.CreateTextNode(_arrivalTime.ToString("yyyy/MM/dd HH:mm:ss.fff"));
            XmlText messageRouteText = result.CreateTextNode(_messageRoute);

            CreateBytes();
            XmlText dataText = result.CreateTextNode(Encoding.ASCII.GetString(_data));

            sequenceNumber.AppendChild(sequenceNumberText);
            algorithmNo.AppendChild(algorithmNoText);
            livedTime.AppendChild(livedTimeText);
            creationTime.AppendChild(creationTimeText);
            arrivalTime.AppendChild(arrivalTimeText);
            messageRoute.AppendChild(messageRouteText);

            headerElement.AppendChild(sequenceNumber);
            headerElement.AppendChild(algorithmNo);
            headerElement.AppendChild(livedTime);
            headerElement.AppendChild(creationTime);
            headerElement.AppendChild(arrivalTime);
            headerElement.AppendChild(messageRoute);

            data.AppendChild(dataText);

            packetElement.AppendChild(headerElement);
            packetElement.AppendChild(data);

            result.AppendChild(packetElement);

            return result;
        }

        /// <summary>
        /// 用于填充data
        /// Use for Create data
        /// </summary>
        public void CreateBytes()
        {
            int size = 1400 - (_sequenceNumber.Length + _algorithmNo.ToString().Length + _livedTime.ToString().Length
                       + _creationTime.ToString("yyyy/MM/dd HH:mm:ss.fff").Length + _arrivalTime.ToString("yyyy/MM/dd HH:mm:ss.fff").Length +
                       _messageRoute.Length);
            _data = new byte[size];
        }
    }
}
