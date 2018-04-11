using System;
using System.Threading;

namespace MAVNetTest
{
    class Program
    {
        enum EntityType
        {
            Station = 1,
            Ferrying,
            Searching
        }

        static void Main()
        {
            InformationLoader informationLoader = new InformationLoader();
            Entity[] entities = informationLoader.LoadEntities("123.xml");

            Thread[] threads = new Thread[entities.Length];
            CompletedFlags.Initialization(entities.Length);


            for (var i = 0; i < entities.Length; i++)
            {
                threads[i] = new Thread(entities[i].SimStart);
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            while (CompletedFlags.IsAllCompleted())
            {
                Thread.Sleep(1000);
            }

            foreach (var entity in entities)
            {
                var entity1 = (Station) entity;
                if (entity.TypeReader == (int)EntityType.Station)
                {
                    entity1.CalculateDelayAndShowTheHop();
                }
            }

            int sumOfSend = 0;
            int sumOfReceive = 0;

            foreach (var entity in entities)
            {
                if (entity.TypeReader == (int)EntityType.Station)
                {
                    Station entity1 = (Station) entity;
                    sumOfReceive = entity1.AmountOfReceive;
                }
                else if (entity.TypeReader == (int)EntityType.Ferrying || entity.TypeReader == (int)EntityType.Searching)
                {
                    SearchingMav entity1 = (SearchingMav) entity;
                    sumOfSend += entity1.AmountOfCreate;
                }
            }

            Console.WriteLine(sumOfReceive / sumOfSend);
        }
    }
}
