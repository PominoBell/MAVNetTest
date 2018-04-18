using System;
using System.Collections.Generic;
using System.Threading;

namespace MAVNetTest
{
    /// <summary>
    /// 图的节点
    /// The node of Map
    /// </summary>
    internal struct Node
    {
        public string Id;

        public int MavType;

        public double Longitude;

        public double Latitude;

        public double Altitude;

        public double Range;
    }

    /// <summary>
    /// 图的边
    /// The Edge of Map
    /// </summary>
    internal struct Edge
    {
        public Node Node1;

        public Node Node2;

        public double Weight;

        public int EdgeType;
    }

    internal static class Map
    {
        /// <summary>
        /// 读写锁
        /// ReadWriteLock
        /// </summary>
        private static readonly ReaderWriterLockSlim CacheLockSlim = new ReaderWriterLockSlim();

        /// <summary>
        /// 点的集合
        /// The list of nodes
        /// </summary>
        private static List<Node> _nodes = new List<Node>();

        /// <summary>
        /// 边的集合
        /// The list of edges
        /// </summary>
        private static List<Edge> _edges = new List<Edge>();

        /// <summary>
        /// 用于初始化地图
        /// Used for initialize the Map
        /// </summary>
        /// <param name="entities">实体集  Set of entities</param>
        public static void Initialization(List<Entity> entities)
        {
            CacheLockSlim.EnterWriteLock();
            try
            {
                foreach (var entity in entities)
                {
                    Node tmp = new Node
                    {
                        Id = entity.IdReader,
                        MavType = entity.TypeReader,
                        Longitude = entity.LongitudeReader,
                        Latitude = entity.LatitudeReader,
                        Altitude = entity.AltitudeReader,
                        Range = entity.RangeSetter
                    };

                    _nodes.Add(tmp);
                }

                for (var i = 0; i < _nodes.Count; i++)
                {
                    for (int j = i + 1; j < _nodes.Count; j++)
                    {
                        double longitude1 = _nodes[i].Longitude;
                        double latitude1 = _nodes[i].Latitude;
                        double longitude2 = _nodes[j].Longitude;
                        double latitude2 = _nodes[j].Latitude;

                        double distance = GetDistance(longitude1, longitude2, latitude1, latitude2);

                        if (distance < _nodes[i].Range)
                        {
                            Edge tmp = new Edge
                            {
                                EdgeType = 1,
                                Node1 = _nodes[i],
                                Node2 = _nodes[j],
                                Weight = 1400 / distance
                            };

                            _edges.Add(tmp);
                        }
                        else if (distance > _nodes[i].Range)
                        {
                            if (_nodes[i].MavType == (int)AddressBook.EntityType.Station && _nodes[j].MavType == (int)AddressBook.EntityType.Ferrying)
                            {
                                FerryingMav ferryingMav = (FerryingMav)entities[j];
                                Edge tmp = new Edge
                                {
                                    EdgeType = 0,
                                    Node1 = _nodes[i],
                                    Node2 = _nodes[j],
                                    Weight = 1400 / _nodes[i].Range +
                                             (distance - _nodes[i].Range) / ferryingMav.SpeedSetter
                                };

                                _edges.Add(tmp);
                            }
                            else if (_nodes[j].MavType == (int)AddressBook.EntityType.Station && _nodes[i].MavType == (int)AddressBook.EntityType.Ferrying)
                            {
                                FerryingMav ferryingMav = (FerryingMav)entities[i];
                                Edge tmp = new Edge
                                {
                                    EdgeType = 0,
                                    Node1 = _nodes[i],
                                    Node2 = _nodes[j],
                                    Weight = 1400 / _nodes[i].Range +
                                             (distance - _nodes[i].Range) / ferryingMav.SpeedSetter
                                };

                                _edges.Add(tmp);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                CacheLockSlim.ExitWriteLock();
            }
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
        public static double GetDistance(double longitude1, double longitude2, double latitude1, double latitude2)
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
        /// 获取地图    Fetch the Map
        /// </summary>
        /// <param name="portableMap">实体携带的地图副本</param>
        /// <returns></returns>
        public static PortableMap FetchMap(PortableMap portableMap)
        {
            CacheLockSlim.EnterReadLock();
            try
            {
                portableMap.NodesSetter = new List<Node>(_nodes);
                portableMap.EdgesSetter = new List<Edge>(_edges);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                CacheLockSlim.ExitReadLock();
            }

            return portableMap;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="node"></param>
        public static void WriteMap(double longitude, double latitude, string node)
        {
            CacheLockSlim.EnterWriteLock();
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                CacheLockSlim.ExitWriteLock();
            }
        }
    }
}
