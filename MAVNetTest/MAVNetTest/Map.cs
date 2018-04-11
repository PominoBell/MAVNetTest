namespace MAVNetTest
{
    internal class Map
    {
        public class Node
        {
            public string Id;

            public int MavType;

            public double Longitude;

            public double Latitude;

            public double Altitude;

            public double Range;
        }

        public class Edge
        {
            public Node Node1;

            public Node Node2;

            public double Weight;

            public int EdgeType;
        }
    }
}
