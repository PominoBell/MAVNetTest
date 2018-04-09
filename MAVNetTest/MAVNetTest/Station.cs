namespace MAVNetTest
{
    internal class Station : Entity
    {
        public Station()
        {
        }

        public Station(string id, int type, double longitude, double latitude, double altitude, double range) : base(id, type, longitude, latitude, altitude, range)
        {
        }
    }
}
