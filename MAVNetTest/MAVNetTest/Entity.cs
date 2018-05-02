using System.Net.Sockets;
using System.Xml;

namespace MAVNetTest
{
    internal class Entity
    {
        /// <summary>
        /// 实体序列号
        /// The Id of Entity
        /// </summary>
        protected readonly string Id;

        /// <summary>
        /// 实体类型
        /// The Type of Entity
        /// </summary>
        protected readonly int Type;

        /// <summary>
        /// 经度
        /// The longitude of Entity
        /// </summary>
        protected double Longitude;

        /// <summary>
        /// 纬度
        /// The latitude of Entity
        /// </summary>
        protected double Latitude;

        /// <summary>
        /// 海拔高度
        /// The altitude of Entity
        /// </summary>
        protected double Altitude;

        /// <summary>
        /// 发送范围
        /// The max distance of sending data
        /// </summary>
        protected double Range;

        /// <summary>
        /// 地图
        /// The Portable Map of Entity
        /// </summary>
        protected PortableMap Map = new PortableMap();

        /// <summary>
        /// UDP客户端
        /// The UdpClient of Entity
        /// </summary>
        protected UdpClient UdpClient;

        /// <summary>
        /// 实体序列号读取
        /// The reader of Id
        /// </summary>
        public string IdReader => Id;

        /// <summary>
        /// 实体类型读取
        /// The reader of Type
        /// </summary>
        public int TypeReader => Type;

        /// <summary>
        /// 范围设置
        /// The setting of Range
        /// </summary>
        public double RangeSetter
        {
            get => Range;
            set => Range = value;
        }

        /// <summary>
        /// 经度读取
        /// The reader of longitude
        /// </summary>
        public double LongitudeReader => Longitude;

        /// <summary>
        /// 纬度读取
        /// The reader of latitude
        /// </summary>
        public double LatitudeReader => Latitude;

        /// <summary>
        /// 高度读取
        /// The reader of altitude
        /// </summary>
        public double AltitudeReader => Altitude;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Entity() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">实体序列号</param>
        /// <param name="type">实体类型</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="altitude">高度</param>
        /// <param name="range">范围</param>
        public Entity(string id, int type, double longitude, double latitude, double altitude, double range)
        {
            Id = id;
            Type = type;
            Longitude = longitude;
            Latitude = latitude;
            Altitude = altitude;
            Range = range;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity">Xml节点对象  XmlNode Object</param>
        public Entity(XmlNode entity)
        {
            var attribute = entity.FirstChild;

            Id = attribute.InnerText;
            attribute = attribute.NextSibling;

            if (attribute != null)
            {
                Type = int.Parse(attribute.InnerText);
                attribute = attribute.NextSibling;
            }

            if (attribute != null)
            {
                Longitude = double.Parse(attribute.InnerText);
                attribute = attribute.NextSibling;
            }

            if (attribute != null)
            {
                Latitude = double.Parse(attribute.InnerText);
                attribute = attribute.NextSibling;
            }

            if (attribute != null)
            {
                Altitude = double.Parse(attribute.InnerText);
                attribute = attribute.NextSibling;
            }

            if (attribute != null)
            {
                Range = double.Parse(attribute.InnerText);
            }
        }

        public virtual void SimStart() { }
    }
}
