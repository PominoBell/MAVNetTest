using System;
using System.Collections.Generic;

namespace MAVNetTest
{
    class PortableMap
    {
        /// <summary>
        /// 存储顶点信息
        /// Used for saving the information of Vertex
        /// </summary>
        private List<Node> _nodes = new List<Node>();

        /// <summary>
        /// 存储边信息
        /// Used for saving the information of Edge
        /// </summary>
        private List<Edge> _edges = new List<Edge>();

        /// <summary>
        /// 用于读取设置顶点
        /// Used for reading or setting the vertex
        /// </summary>
        public List<Node> NodesSetter
        {
            get => _nodes;
            set => _nodes = value;
        }

        /// <summary>
        /// 用于读取和设置边
        /// Used for reading or setting the edge
        /// </summary>
        public List<Edge> EdgesSetter
        {
            get => _edges;
            set => _edges = value;
        }

        /// <summary>
        /// 用于选择算法
        /// Used for selecting Algorithm
        /// </summary>
        /// <param name="algorithmNo">算法序号  The number of algorithm</param>
        /// <param name="source">源点  Source</param>
        /// <returns>下一个点  The next node</returns>
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

        /// <summary>
        /// 用于计算两个经纬度之间的距离
        /// Used for calculating the distance between two coordinates
        /// </summary>
        /// <param name="longitude1">第一个点的经度    The longitude of the first node</param>
        /// <param name="longitude2">第二个点的经度    The longitude of the second node</param>
        /// <param name="latitude1">第一个点的纬度     The latitude of the first node</param>
        /// <param name="latitude2">第二个点的纬度     The latitude of the second node</param>
        /// <returns>两个经纬度之间的距离     The distance between two coordinates</returns>
        public double GetDistance(double longitude1, double longitude2, double latitude1, double latitude2)
        {
            double EarthRadius = 6371.393;

            double radLat1 = latitude1 * Math.PI / 180.0;
            double radLat2 = latitude2 * Math.PI / 180.0;

            double radLon1 = longitude1 * Math.PI / 180.0;
            double radLon2 = longitude2 * Math.PI / 180.0;


            double a = radLat1 - radLat2;
            double b = radLon1 - radLon2;

            double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2)
                                                    + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));

            result = result * EarthRadius;
            result = Math.Round(result * 10000d) / 10000d;
            return result * 1000;
        }

        /// <summary>
        /// 使用迪杰斯特拉算法求得最短路径的下一个点
        /// Used for calculate the next node on the shortest route calculated by Dijkstra
        /// </summary>
        /// <param name="source">源点 Source</param>
        /// <returns>下一个点  The next node</returns>
        public Node ShortestRoute(ref Node source)
        {
            List<Node> nodes = new List<Node>();
            List<List<Node>> route = new List<List<Node>>();

            //给每一条路径添加源点
            //Add the source to every route in the List route
            for (var i = 0; i < _nodes.Count - 1; i++)
            {
                List<Node> tmp = new List<Node> { source };
                route.Add(tmp);
            }

            //定义一个数组，将源点到每个点的权重存储起来
            //Define an array to save the weights between source and another node
            double[] weight = new double[_nodes.Count];

            for (int i = 0; i < _nodes.Count; i++)
                weight[i] = double.MaxValue;

            foreach (var edge in _edges)
            {
                if ((source.IsSame(edge.Node1) || source.IsSame(edge.Node2)) && edge.EdgeType != 0)
                {
                    if (source.IsSame(edge.Node1))
                    {
                        weight[_nodes.IndexOf(edge.Node2)] = edge.Weight;
                    }
                    else if (source.IsSame(edge.Node2))
                    {
                        weight[_nodes.IndexOf(edge.Node1)] = edge.Weight;
                    }
                }
            }

            weight[_nodes.IndexOf(source)] = 0;

            nodes.Add(source);

            //每次将距离最近的点加入到集合中，并检查加入的点的边，计算到加入的点的权重和加入的点到相邻点的权重之和，若小于加入之前的权重
            //则更新对应的权重值
            //
            //Add the closest node to the List nodes.
            //Check the edge contains this node.
            //If the sum of weight of this node and weight of adjacent node less than weight of adjacent node
            //Update the weight of the adjacent node.
            while (nodes.Count != _nodes.Count)
            {
                double min = double.MaxValue;
                int posMin = -1;

                for (var i = 0; i < weight.Length; i++)
                {
                    if (weight[i] < min && !nodes.Contains(_nodes[i]))
                    {
                        min = weight[i];
                        posMin = i;
                    }
                }

                if (posMin == -1)
                    break;

                nodes.Add(_nodes[posMin]);

                int routePos;

                if (posMin > _nodes.IndexOf(source))
                    routePos = posMin - 1;
                else
                    routePos = posMin;

                route[routePos].Add(_nodes[posMin]);

                foreach (var edge in _edges)
                {
                    if ((_nodes[posMin].IsSame(edge.Node1) || _nodes[posMin].IsSame(edge.Node2)) && edge.EdgeType != 0)
                    {
                        if (_nodes[posMin].IsSame(edge.Node1) && !nodes.Contains(edge.Node2))
                        {
                            if (weight[_nodes.IndexOf(edge.Node2)] > weight[posMin] + edge.Weight)
                            {
                                weight[_nodes.IndexOf(edge.Node2)] = weight[posMin] + edge.Weight;

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
                            if (weight[_nodes.IndexOf(edge.Node1)] > weight[posMin] + edge.Weight)
                            {
                                weight[_nodes.IndexOf(edge.Node1)] = weight[posMin] + edge.Weight;
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

            //若抵达的该点为基站，则返回下一个点
            //If the end of route is station, return the second node.

            if (route[0].Count == 1) return source;
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
