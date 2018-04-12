using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace MAVNetTest
{
    internal class Station : Entity
    {
        private List<Packet> ReceiveList = new List<Packet>();

        public int AmountOfReceive => ReceiveList.Count;
        public Station()
        {
        }

        public Station(string id, int type, double longitude, double latitude, double altitude, double range) : base(id, type, longitude, latitude, altitude, range)
        {
        }

        public Station(XmlNode entity) : base(entity)
        {
        }

        public override void SimStart()
        {
            base.SimStart();

            UdpReceiver();
        }

        public void ReceiveCallBack(IAsyncResult ar)
        {
            UdpClient udpClient = (UdpClient) ar.AsyncState;
            IPEndPoint ipe = udpClient.Client.LocalEndPoint as IPEndPoint;

            byte[] resultBytes = udpClient.EndReceive(ar, ref ipe);
            string resultString = Encoding.ASCII.GetString(resultBytes);
            XmlDocument packetXmlDocument = new XmlDocument {InnerXml = resultString};

            Packet messagePacket = new Packet(packetXmlDocument);
            ReceiveList.Add(messagePacket);
        }

        public void UdpReceiver()
        {
            IPEndPoint ipe = AddressBook.SearchIpEndPoint(Type, Id);
            UdpClient udpClient = new UdpClient(ipe);

            while (true)
            {
                udpClient.BeginReceive(ReceiveCallBack, udpClient);
            }
        }

        public void CalculateDelayAndShowTheHop()
        {
            
        }
    }
}
