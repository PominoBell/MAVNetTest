using System.Collections.Generic;
using System.Net;

namespace MAVNetTest
{
    internal static class AddressBook
    {
        /// <summary>
        /// 定义实体类型的枚举
        /// A Enum defined for the type of entity
        /// </summary>
        public enum EntityType
        {
            Station = 1,
            Ferrying,
            Searching
        }

        /// <summary>
        /// 定义对应类型端口开始的地址
        /// A Enum defined for different type entity's port
        /// </summary>
        enum PortStart
        {
            Station = 3000,
            Ferrying = 4000,
            Searching = 5000
        }

        /// <summary>
        /// 用于存储所有实体通讯地址的字典
        /// A Dictionary for the IPEndPoint of entity
        /// </summary>
        private static Dictionary<string, IPEndPoint> _addressBook = new Dictionary<string, IPEndPoint>();

        /// <summary>
        /// 用于初始化字典
        /// Initialize the Dictionary
        /// </summary>
        /// <param name="entities">实体集  The set of entities</param>
        public static void Initialization(Entity[] entities)
        {
            foreach (var entity in entities)
            {
                Add((EntityType)entity.TypeReader, entity);
            }
        }

        /// <summary>
        /// 查找实体所对应的网络终结点
        /// Search for IPEndPoint of the entity
        /// </summary>
        /// <param name="type">实体类型 The type of entity</param>
        /// <param name="id">实体编号   The ID of entity</param>
        /// <returns></returns>
        public static IPEndPoint SearchIpEndPoint(int type, string id)
        {
            var key = type + id;
            _addressBook.TryGetValue(key, out var ipe);
            return ipe;
        }

        /// <summary>
        /// 用于向字典里添加值
        /// Add the pair of key and value into the Dictionary
        /// </summary>
        /// <param name="type">实体类型枚举 The EntityType Enum</param>
        /// <param name="entity">实体 Entity</param>
        public static void Add(EntityType type, Entity entity)
        {
            var key = entity.TypeReader + entity.IdReader;
            IPHostEntry iph = Dns.GetHostEntry("127.0.0.1");
            IPEndPoint ipe;

            switch (type)
            {
                case EntityType.Station:
                    ipe = new IPEndPoint(iph.AddressList[0], (int)PortStart.Station + int.Parse(entity.IdReader));
                    break;
                case EntityType.Ferrying:
                    ipe = new IPEndPoint(iph.AddressList[0], (int)PortStart.Ferrying + int.Parse(entity.IdReader));
                    break;
                case EntityType.Searching:
                    ipe = new IPEndPoint(iph.AddressList[0], (int)PortStart.Searching + int.Parse(entity.IdReader));
                    break;
                default:
                    ipe = new IPEndPoint(iph.AddressList[0], 2000);
                    break;
            }
            
            _addressBook.Add(key, ipe);
        }
    }
}
