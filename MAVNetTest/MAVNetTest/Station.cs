using System.Collections.Generic;

namespace MAVNetTest
{
    internal class Station : Entity
    {
        private List<Packet> ReceiveList;

        public int AmountOfReceive => ReceiveList.Count;
        public Station()
        {
        }

        public Station(string id, int type, double longitude, double latitude, double altitude, double range) : base(id, type, longitude, latitude, altitude, range)
        {
        }

        public void CalculateDelayAndShowTheHop()
        {
            
        }
    }
}
