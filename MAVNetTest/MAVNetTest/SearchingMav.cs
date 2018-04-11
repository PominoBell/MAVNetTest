using System;
using System.Collections.Generic;

namespace MAVNetTest
{
    internal class SearchingMav : Entity
    {
        private List<String> CreateList = new List<string>();

        public int AmountOfCreate => CreateList.Count;

        public SearchingMav()
        {
        }

        public SearchingMav(string id, int type, double longitude, double latitude, double altitude, double range) : base(id, type, longitude, latitude, altitude, range)
        {
        }
    }
}
