using System;
using System.Collections.Generic;
using System.Xml;

namespace MAVNetTest
{
    internal class SearchingMav : Entity
    {
        protected double Speed;

        protected double Orientation;

        private List<String> CreateList = new List<string>();

        public int AmountOfCreate => CreateList.Count;

        public SearchingMav()
        {
        }

        public SearchingMav(string id, int type, double longitude, double latitude, double altitude, double range) : base(id, type, longitude, latitude, altitude, range)
        {
        }

        public SearchingMav(XmlNode entity) : base(entity)
        {
            var attribute = entity.FirstChild;

            while (attribute != null && !attribute.LocalName.Equals("speed"))
            {
                attribute = attribute.NextSibling;
            }

            if (attribute != null && attribute.LocalName.Equals("speed"))
            {
                Speed = double.Parse(attribute.InnerText);
                attribute = attribute.NextSibling;
            }

            if (attribute != null) Orientation = double.Parse(attribute.InnerText);
        }
    }
}
