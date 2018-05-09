using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace MAVNetTest
{
    internal class SearchingMav : Entity
    {
        /// <summary>
        /// 无人机速度
        /// The speed of MAV
        /// </summary>
        protected double Speed;

        /// <summary>
        /// 飞行角度
        /// The orientation of MAV
        /// </summary>
        protected double Orientation;

        /// <summary>
        /// 每秒生成包数量
        /// The number of Creating Packets every second
        /// </summary>
        protected const int PacketsPerSecond = 25;

        /// <summary>
        /// 完成标志
        /// The flags of Completed
        /// </summary>
        protected bool[] Completed = new bool[3];

        /// <summary>
        /// 子线程序列号
        /// The Id of subThread
        /// </summary>
        protected int[] ThreadInts = new int[3];

        /// <summary>
        /// 速度设置
        /// The setting of Speed
        /// </summary>
        public double SpeedSetter
        {
            get => Speed;
            set => Speed = value;
        }

        /// <summary>
        /// 读写锁
        /// ReaderWriterLock
        /// </summary>
        protected ReaderWriterLockSlim RWlockSlim = new ReaderWriterLockSlim();

        /// <summary>
        /// 发送列表
        /// The list of sent message
        /// </summary>
        protected List<String> SendList = new List<string>();

        /// <summary>
        /// 创建队列
        /// The queue of created message
        /// </summary>
        protected ConcurrentQueue<Packet> CreateQueue = new ConcurrentQueue<Packet>();

        /// <summary>
        /// 接收队列
        /// The queue of received message
        /// </summary>
        protected ConcurrentQueue<Packet> ReceiveQueue = new ConcurrentQueue<Packet>();

        /// <summary>
        /// 发送数量
        /// The amount of sent message
        /// </summary>
        public int AmountOfSent => SendList.Count;

        /// <inheritdoc />
        /// <summary>
        /// 构造函数
        /// </summary>
        public SearchingMav()
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
        public SearchingMav(string id, int type, double longitude, double latitude, double altitude, double range) : base(id, type, longitude, latitude, altitude, range)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity">Xml节点对象  XmlNode Object</param>
        public SearchingMav(XmlNode entity) : base(entity)
        {
            var attribute = entity.FirstChild;

            while (attribute != null && !attribute.LocalName.Equals("speed"))
            {
                attribute = attribute.NextSibling;
            }

            if (attribute != null && attribute.LocalName.Equals("speed"))
            {
                Speed = double.Parse(attribute.InnerText);
                attribute = attribute.NextSibling;
            }

            if (attribute != null) Orientation = double.Parse(attribute.InnerText);
        }

        /// <summary>
        /// 用于开始模拟
        /// Use for starting simulation
        /// </summary>
        public override void SimStart(Object testTime)
        {
            base.SimStart(testTime);
            
            IPEndPoint ipe = AddressBook.SearchIpEndPoint(Type, Id);
            UdpClient = new UdpClient(ipe);

            Thread[] threads = new Thread[3];

            threads[0] = new Thread(UdpReceiver);
            threads[1] = new Thread(UdpSender);
            threads[2] = new Thread(CreatePackets);

            for (var i = 0; i < threads.Length; i++)
            {
                ThreadInts[i] = threads[i].ManagedThreadId;
                threads[i].Start();
            }

            while (AllCompleted())
            {
                Thread.Sleep(1000);
            }

            CompletedFlags.Completed();
        }

        /// <summary>
        /// 用于完成接收的回调函数
        /// Used for finishing receiving 
        /// </summary>
        /// <param name="ar"></param>
        public virtual void ReceiveCallBack(IAsyncResult ar)
        {
            AutoResetEvent autoResetEvent = (AutoResetEvent) ar.AsyncState;
            IPEndPoint ipe = UdpClient.Client.LocalEndPoint as IPEndPoint;

            try
            {
                byte[] resultBytes = UdpClient.EndReceive(ar, ref ipe);
                string resultString = Encoding.ASCII.GetString(resultBytes);
                XmlDocument packetXmlDocument = new XmlDocument {InnerXml = resultString};
                Packet messagePacket = new Packet(packetXmlDocument);

                messagePacket.MessageRoute = Type + Id;
                messagePacket.LivedTime = messagePacket.LivedTime + 1;

                ReceiveQueue.Enqueue(messagePacket);
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
        public virtual void LeftTime(object obj)
        {
            LeftTimeCount = LeftTimeCount + 1000;
        }

        /// <summary>
        /// 用于进行消息接收
        /// CallBack Function Used for receiving the message
        /// </summary>
        public virtual void UdpReceiver()
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            var timer = new Timer(SetFlag, Thread.CurrentThread.ManagedThreadId, TotalTestTime, TotalTestTime);
            var leftTimer = new Timer(LeftTime, null, 1000, 1000);

            while (!AllCompleted())
            {
                UdpClient.BeginReceive(ReceiveCallBack, autoResetEvent);
                autoResetEvent.WaitOne(TotalTestTime - LeftTimeCount);
            }

            Console.WriteLine("Thread UdpReceiver Completed. Entity Type {0}, Entity Id {1}", Type, Id);
            leftTimer.Dispose();
            timer.Dispose();
        }

        /// <summary>
        /// 用于完成发送的回调函数
        /// CallBack Function Used for finishing sending message
        /// </summary>
        /// <param name="ar"></param>
        public virtual void SendCallBack(IAsyncResult ar)
        {
            UdpClient.EndSend(ar);
        }

        /// <summary>
        /// 用于发送数据
        /// Used for sending message
        /// </summary>
        public virtual void UdpSender()
        {
            var numberCount = 0;
            var timer = new Timer(SetFlag, Thread.CurrentThread.ManagedThreadId, TotalTestTime, TotalTestTime);
            var sendTrigger = false;

            while (!AllCompleted())
            {
                MAVNetTest.Map.FetchMap(Map);
                Node source = new Node
                {
                    MavType = Type,
                    Altitude = Altitude,
                    Id = Id,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    Range = Range
                };

                Node nextNode = Map.AlgorithmSelect(1, source);

                if (nextNode.IsSame(source))
                    continue;

                IPEndPoint ipeTo = AddressBook.SearchIpEndPoint(nextNode.MavType, nextNode.Id);

                string message = "No message";

                if (sendTrigger)
                {
                    sendTrigger = false;
                    if (CreateQueue.TryDequeue(out var result))
                    {
                        SendList.Add(Type + Id + numberCount);
                        message = result.CreateXmlDocument().InnerXml;
                    }
                    else if (ReceiveQueue.TryDequeue(out result))
                        message = result.CreateXmlDocument().InnerXml;
                }
                else
                {
                    sendTrigger = true;
                    if (ReceiveQueue.TryDequeue(out var result))
                        message = result.CreateXmlDocument().InnerXml;
                    else if (CreateQueue.TryDequeue(out result))
                    {
                        SendList.Add(Type + Id + numberCount);
                        message = result.CreateXmlDocument().InnerXml;
                    }
                }
                
                byte[] sendBytes = Encoding.ASCII.GetBytes(message);

                if(!message.Equals("No message"))
                {
                    Thread.Sleep(CalculateTheoryDelay(nextNode));
                    UdpClient.BeginSend(sendBytes, sendBytes.Length, ipeTo, SendCallBack, numberCount);
                }
            }

            Console.WriteLine("Thread UdpSender Completed. Entity Type {0}, Entity Id {1}", Type, Id);
            timer.Dispose();
        }

        /// <summary>
        /// 用于重制计数器状态的函数
        /// Used for reset the state of timer
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Reset(Object obj)
        {
            AutoResetEvent autoEvent = (AutoResetEvent) obj;
            autoEvent.Set();
        }

        /// <summary>
        /// 用于创建数据包
        /// Used for Creating message Packets
        /// </summary>
        public virtual void CreatePackets()
        {
            int count = 0;
            int numbercount = 0;
            var autoEvent = new AutoResetEvent(false);
            var timer = new Timer(SetFlag, Thread.CurrentThread.ManagedThreadId, TotalTestTime, TotalTestTime);

            while (!AllCompleted())
            {
                var oneSecond = new Timer(Reset, autoEvent, 1000, 1000);

                while (count != PacketsPerSecond)
                {
                    count++;
                    numbercount++;
                    CreateQueue.Enqueue(new Packet(Type, Id, numbercount, 1));
                }

                autoEvent.WaitOne();
                count = 0;
                oneSecond.Dispose();
            }

            Console.WriteLine("Thread CreatePackets Completed. Entity Type {0}, Entity Id {1}", Type, Id);
            timer.Dispose();
        }

        /// <summary>
        /// 用于计算理论上的因为距离导致的传输延迟
        /// Used for calculating the delay which caused by distance
        /// </summary>
        /// <param name="node">下一个传输节点  Next node</param>
        /// <returns></returns>
        public int CalculateTheoryDelay(Node node)
        {
            var distance = Map.GetDistance(Longitude, node.Longitude, Latitude, node.Latitude);

            var second = 1400 * 8 / (Math.Pow(10, 6) * (-9.09 * Math.Log(2, distance) + 72.58));

            var millisecond = (int)(second * 1000);

            return millisecond;
        }

        /// <summary>
        /// 设置完成标志
        /// Used for setting Completed Flags
        /// </summary>
        /// <param name="id"></param>
        public void SetFlag(Object id)
        {
            RWlockSlim.EnterWriteLock();
            for (var i = 0; i < ThreadInts.Length; i++)
            {
                if (ThreadInts[i] == (int)id)
                {
                    Completed[i] = true;
                    break;
                }
            }
            RWlockSlim.ExitWriteLock();
        }

        /// <summary>
        /// 检查是否所有线程已完成任务
        /// Used for checking all subThread finished
        /// </summary>
        /// <returns></returns>
        public bool AllCompleted()
        {
            RWlockSlim.EnterReadLock();
            foreach (var flag in Completed)
            {
                if (!flag)
                {
                    RWlockSlim.ExitReadLock();
                    return false;
                }
            }

            RWlockSlim.ExitReadLock();
            return true;
        }
    }
}
