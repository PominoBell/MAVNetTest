using System;
using System.Text;
using System.Xml;

namespace MAVNetTest
{
    internal class Packet
    {
        /// <summary>
        /// 序列编号
        /// </summary>
        private readonly string _sequenceNumber;

        /// <summary>
        /// 算法编号
        /// </summary>
        private int _algorithmNo;

        /// <summary>
        /// 跳数
        /// </summary>
        private int _livedTime;

        /// <summary>
        /// 创建时间
        /// </summary>
        private readonly DateTime _creationTime;

        /// <summary>
        /// 到达时间
        /// </summary>
        private DateTime _arrivalTime;

        /// <summary>
        /// 消息转发的路径
        /// </summary>
        private string _messageRoute;

        /// <summary>
        /// 包中存储的数据
        /// </summary>
        private byte[] _data;

        /// <summary>
        /// 最大跳数
        /// </summary>
        public const int TimeToLive = 20;

        /// <summary>
        /// 跳数，用于获取和更改的属性
        /// </summary>
        public int LivedTime
        {
            get => _livedTime;
            set => _livedTime = value;
        }

        /// <summary>
        /// 到达时间，用于获取和更改的属性
        /// </summary>
        public DateTime ArrivalTime
        {
            get => _arrivalTime;
            set => _arrivalTime = value;
        }

        /// <summary>
        /// 转发路径，用于获取和更改的属性
        /// </summary>
        public string MessageRoute
        {
            get => _messageRoute;
            set => _messageRoute += value;
        }

        /// <summary>
        /// 算法序号，用于获取和更改的属性
        /// </summary>
        public int AlgorithmNo
        {
            get => _algorithmNo;
            set => _algorithmNo = value;
        }

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
        /// <param name="id">无人机的序号</param>
        /// <param name="number">消息序号</param>
        /// <param name="algorithmNo">算法序号</param>
        public Packet(string id, int number, int algorithmNo)
        {
            _sequenceNumber = id + number;
            _algorithmNo = algorithmNo;
            _livedTime = 0;
            _creationTime = DateTime.Now;
            _arrivalTime = _creationTime;
            _messageRoute = id;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="packetXmlDocument">XML格式文件</param>
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
        /// </summary>
        /// <returns>XmlDocument对象</returns>
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
