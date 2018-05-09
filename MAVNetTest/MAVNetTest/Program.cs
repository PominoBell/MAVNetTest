using System;
using System.Collections.Generic;
using System.Threading;

namespace MAVNetTest
{
    class Program
    {
        static void Main()
        {
            InformationLoader informationLoader = new InformationLoader();
            List<Entity> entities = informationLoader.LoadEntities(@"C:\Users\pomino\Source\Repos\MAVNetTest\MAVNetTest\MAVNetTest\EntityInformation.xml");

            Thread[] threads = new Thread[entities.Count];
            CompletedFlags.Initialization(entities.Count);
            Map.Initialization(entities);
            AddressBook.Initialization(entities.ToArray());

            for (var i = 0; i < entities.Count; i++)
            {
                threads[i] = new Thread(entities[i].SimStart);
            }

            foreach (var thread in threads)
            {
                thread.Start(480000);
            }

            while (!CompletedFlags.IsAllCompleted())
            {
                Thread.Sleep(1000);
            }

            //计算延迟和显示跳数
            foreach (var entity in entities)
            {
                if (entity.TypeReader == (int)AddressBook.EntityType.Station)
                {
                    var entity1 = (Station)entity;
                    entity1.CalculateDelayAndShowTheHop();
                }
            }

            //计算抵达的比率
            int sumOfSend = 0;
            int sumOfReceive = 0;

            foreach (var entity in entities)
            {
                if (entity.TypeReader == (int)AddressBook.EntityType.Station)
                {
                    Station entity1 = (Station) entity;
                    sumOfReceive = entity1.AmountOfReceive;
                }
                else if (entity.TypeReader == (int)AddressBook.EntityType.Ferrying || entity.TypeReader == (int)AddressBook.EntityType.Searching)
                {
                    SearchingMav entity1 = (SearchingMav) entity;
                    sumOfSend += entity1.AmountOfSent;
                }
            }

            Console.WriteLine("The Deliver Ratio is {0}", (double)sumOfReceive / sumOfSend);
        }
    }
}
