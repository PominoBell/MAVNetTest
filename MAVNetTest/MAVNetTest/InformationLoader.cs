using System.Collections.Generic;
using System.Xml;

namespace MAVNetTest
{
    internal class InformationLoader
    {
        public List<Entity> LoadEntities(string file)
        {
            List<Entity> entities = new List<Entity>();

            XmlDocument informationDocument = new XmlDocument();
            informationDocument.Load(file);

            XmlElement root = informationDocument.DocumentElement;

            if (root != null)
            {
                var entityType = root.FirstChild;
                var entity = entityType.FirstChild;

                while (entityType != null)
                {
                    if (entity != null)
                    {
                        var type = entity.FirstChild.NextSibling;
                        int typeReader = 0;

                        if (type != null)
                        {
                            typeReader = int.Parse(type.InnerText);
                        }

                        switch (typeReader)
                        {
                            case 1:
                                entities.Add(new Station(entity));
                                break;
                            case 2:
                                entities.Add(new FerryingMav(entity));
                                break;
                            case 3:
                                entities.Add(new SearchingMav(entity));
                                break;
                        }

                        entity = entity.NextSibling;

                        if (entity == null)
                        {
                            entityType = entityType.NextSibling;
                            if (entityType != null) entity = entityType.FirstChild;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return entities;
        }
    }
}
