using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace MAVNetTest
{
    internal class Station : Entity
    {
        /// <summary>
        /// 接收消息列表
        /// The list of received messages
        /// </summary>
        private readonly List<Packet> _receiveList = new List<Packet>();

        /// <summary>
        /// 完成标志
        /// The Completed Flag
        /// </summary>
        private bool _completed;

        /// <summary>
        /// 接收总数
        /// The amount of received messages
        /// </summary>
        public int AmountOfReceive => _receiveList.Count;

        /// <inheritdoc />
        /// <summary>
        /// 构造函数
        /// </summary>
        public Station()
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">实体序列号</param>
        /// <param name="type">实体类型</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="altitude">高度</param>
        /// <param name="range">范围</param>
        public Station(string id, int type, double longitude, double latitude, double altitude, double range) : base(id, type, longitude, latitude, altitude, range)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity">Xml节点对象  XmlNode Object</param>
        public Station(XmlNode entity) : base(entity)
        {
        }

        /// <summary>
        /// 用于开始模拟
        /// Used for starting simulation
        /// </summary>
        public override void SimStart(object testTime)
        {
            base.SimStart(testTime);

            IPEndPoint ipe = AddressBook.SearchIpEndPoint(Type, Id);
            UdpClient = new UdpClient(ipe);

            UdpReceiver();

            while (!_completed)
            {
                Thread.Sleep(1000);
            }

            CompletedFlags.Completed();
        }

        /// <summary>
        /// 用于完成接收的回调函数
        /// CallBack Function used for finishing the receiving
        /// </summary>
        /// <param name="ar"></param>
        public void ReceiveCallBack(IAsyncResult ar)
        {
            AutoResetEvent autoResetEvent = (AutoResetEvent) ar.AsyncState;
            IPEndPoint ipe = UdpClient.Client.LocalEndPoint as IPEndPoint;

            try
            {
                byte[] resultBytes = UdpClient.EndReceive(ar, ref ipe);
                string resultString = Encoding.ASCII.GetString(resultBytes);
                XmlDocument packetXmlDocument = new XmlDocument {InnerXml = resultString};
                Packet messagePacket = new Packet(packetXmlDocument);

                messagePacket.ArrivalTime = DateTime.Now;
                messagePacket.MessageRoute = Type + Id;
                messagePacket.LivedTime = messagePacket.LivedTime + 1;

                _receiveList.Add(messagePacket);
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                autoResetEvent.Set();
            }
        }

        /// <summary>
        /// 用于对接收信息时的计时
        /// Used for timing the receiving time
        /// </summary>
        /// <param name="obj"></param>
        public virtual void LeftTime(Object obj)
        {
            LeftTimeCount = LeftTimeCount + 1000;
        }

        /// <summary>
        /// 用于接收信息
        /// Used for receiving messages
        /// </summary>
        public void UdpReceiver()
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            var timer = new Timer(SetFlag, null, TotalTestTime, TotalTestTime);
            var leftTimer = new Timer(LeftTime, null, 1000, 1000);

            while (!_completed)
            {
                UdpClient.BeginReceive(ReceiveCallBack, autoResetEvent);
                autoResetEvent.WaitOne(TotalTestTime - LeftTimeCount);
            }

            Console.WriteLine("Thread UdpReceiver Completed. Entity Type {0}, Entity Id {1}", Type, Id);
            leftTimer.Dispose();
            timer.Dispose();
        }

        /// <summary>
        /// 设置完成标志
        /// Set Completed Flag
        /// </summary>
        /// <param name="obj"></param>
        private void SetFlag(Object obj)
        {
            _completed = true;
        }

        /// <summary>
        /// 计算延时和显示跳数
        /// Used for calculating Delay and show hop
        /// </summary>
        public void CalculateDelayAndShowTheHop()
        {
            foreach (var packet in _receiveList)
            {
                int delayMillisecond = packet.ArrivalTime.Millisecond - packet.CreationTime.Millisecond;
                int delaySecond = packet.ArrivalTime.Second - packet.CreationTime.Second;

                Console.WriteLine("The message {0}'s delay is {1}, and the hop is {2}", packet.SequenceNumber
                    , delaySecond * 1000 + delayMillisecond, packet.LivedTime);
                Console.WriteLine(packet.MessageRoute);
            }
        }
    }
}
