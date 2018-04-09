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

        public void MapFetch()
        {

        }

        public void SimStart()
        {
            
        }
    }
}
