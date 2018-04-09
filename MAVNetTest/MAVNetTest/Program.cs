using System.Threading;

namespace MAVNetTest
{
    class Program
    {
        static void Main()
        {
            InformationLoader informationLoader = new InformationLoader();
            Entity[] entities = informationLoader.LoadEntities("123.xml");

            Thread[] threads = new Thread[entities.Length];

            for (var i = 0; i < entities.Length; i++)
            {
                threads[i] = new Thread(entities[i].SimStart);
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }
        }
    }
}
