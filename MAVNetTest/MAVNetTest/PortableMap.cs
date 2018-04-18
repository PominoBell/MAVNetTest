using System;
using System.Collections.Generic;

namespace MAVNetTest
{
    class PortableMap
    {
        private List<Node> _nodes = new List<Node>();
        private List<Edge> _edges = new List<Edge>();

        public List<Node> NodesSetter
        {
            get => _nodes;
            set => _nodes = value;
        }

        public List<Edge> EdgesSetter
        {
            get => _edges;
            set => _edges = value;
        }

        public string AlgorithmSelect(int algorithmNo, string source)
        {
            string result;
            switch (algorithmNo)
            {
                case 1:
                    result = ShortestRoute(source);
                    break;
                default:
                    result = source;
                    break;
            }

            return result;
        }

        public string ShortestRoute(string source)
        {
            return null;
        }
    }
}
