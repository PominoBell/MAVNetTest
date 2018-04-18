using System.Xml;

namespace MAVNetTest
{
    internal class Entity
    {
        protected readonly string Id;

        protected readonly int Type;

        protected double Longitude;

        protected double Latitude;

        protected double Altitude;

        protected double Range;

        protected PortableMap Map = new PortableMap();

        public string IdReader => Id;

        public int TypeReader => Type;

        public double RangeSetter
        {
            get => Range;
            set => Range = value;
        }

        public double LongitudeReader => Longitude;

        public double LatitudeReader => Latitude;

        public double AltitudeReader => Altitude;

        public Entity() { }

        public Entity(string id, int type, double longitude, double latitude, double altitude, double range)
        {
            Id = id;
            Type = type;
            Longitude = longitude;
            Latitude = latitude;
            Altitude = altitude;
            Range = range;
        }

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
