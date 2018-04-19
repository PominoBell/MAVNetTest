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

        public Node AlgorithmSelect(int algorithmNo, Node source)
        {
            Node result;
            switch (algorithmNo)
            {
                case 1:
                    result = ShortestRoute(ref source);
                    break;
                default:
                    result = source;
                    break;
            }

            return result;
        }

        public Node ShortestRoute(ref Node source)
        {
            List<Node> nodes = new List<Node>();
            List<List<Node>> route = new List<List<Node>>();

            for (var i = 0; i < _nodes.Count - 1; i++)
            {
                List<Node> tmp = new List<Node> { source };
                route.Add(tmp);
            }

            double[] distance = new double[_nodes.Count];

            for (int i = 0; i < _nodes.Count; i++)
                distance[i] = double.MaxValue;

            foreach (var edge in _edges)
            {
                if (source.IsSame(edge.Node1) || source.IsSame(edge.Node2))
                {
                    if (source.IsSame(edge.Node1))
                    {
                        distance[_nodes.IndexOf(edge.Node2)] = edge.Weight;
                    }
                    else if (source.IsSame(edge.Node2))
                    {
                        distance[_nodes.IndexOf(edge.Node1)] = edge.Weight;
                    }
                }
            }

            distance[_nodes.IndexOf(source)] = 0;

            nodes.Add(source);

            while (nodes.Count != _nodes.Count)
            {
                double min = double.MaxValue;
                int posMin = 0;

                for (var i = 0; i < distance.Length; i++)
                {
                    if (distance[i] < min && !nodes.Contains(_nodes[i]))
                    {
                        min = distance[i];
                        posMin = i;
                    }
                }

                nodes.Add(_nodes[posMin]);

                int routePos;

                if (posMin > _nodes.IndexOf(source))
                    routePos = posMin - 1;
                else
                    routePos = posMin;

                route[routePos].Add(_nodes[posMin]);

                foreach (var edge in _edges)
                {
                    if (_nodes[posMin].IsSame(edge.Node1) || _nodes[posMin].IsSame(edge.Node2))
                    {
                        if (_nodes[posMin].IsSame(edge.Node1) && !nodes.Contains(edge.Node2))
                        {
                            if (distance[_nodes.IndexOf(edge.Node2)] > distance[posMin] + edge.Weight)
                            {
                                distance[_nodes.IndexOf(edge.Node2)] = distance[posMin] + edge.Weight;

                                int nodeRoutePos;

                                if (_nodes.IndexOf(edge.Node2) > _nodes.IndexOf(source))
                                    nodeRoutePos = _nodes.IndexOf(edge.Node2) - 1;
                                else
                                    nodeRoutePos = _nodes.IndexOf(edge.Node2);

                                route[nodeRoutePos] = new List<Node>(route[routePos]);
                            }
                        }
                        else if (_nodes[posMin].IsSame(edge.Node2) && !nodes.Contains(edge.Node1))
                        {
                            if (distance[_nodes.IndexOf(edge.Node1)] > distance[posMin] + edge.Weight)
                            {
                                distance[_nodes.IndexOf(edge.Node1)] = distance[posMin] + edge.Weight;
                                int nodeRoutePos;

                                if (_nodes.IndexOf(edge.Node1) > _nodes.IndexOf(source))
                                    nodeRoutePos = _nodes.IndexOf(edge.Node1) - 1;
                                else
                                    nodeRoutePos = _nodes.IndexOf(edge.Node1);

                                route[nodeRoutePos] = new List<Node>(route[routePos]);
                            }
                        }
                    }
                }
            }

            foreach (var node in _nodes)
            {
                if (node.MavType == (int)AddressBook.EntityType.Station)
                {
                    return route[_nodes.IndexOf(node)][1];
                }
            }

            return source;
        }
    }
}
