using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ChatRoom_Server
{
    class Server
    {
        /// <summary>
        /// 客户端容器
        /// </summary>
        public List<Client> ClientList { get; }

        /// <summary>
        /// 在线用户数
        /// </summary>
        public int OnlineClientCount { get { return ClientList.Count; } }

        /// <summary>
        /// 服务器最大连接数
        /// </summary>
        public int MaxConnectionNum { get; }

        /// <summary>
        /// 服务器IP地址
        /// </summary>
        public IPAddress AddressIP { get; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int ServerPort { get; }

        /// <summary>
        /// 服务器对象
        /// </summary>
        public Socket _Server;

        /// <summary>
        /// 当前连接的客户端实例对象
        /// </summary>
        Client CurrentConnectionClient;

        /// <summary>
        /// 客户端ID基数
        /// </summary>
        int ClientIDBase;

        /// <summary>
        /// Server 构造方法
        /// </summary>
        /// <param name="port">服务器端口</param>
        /// <param name="maxConnectionNum">最大连接数</param>
        public Server(int port, int maxConnectionNum)
        {
            // 获取IP
            AddressIP = GetAddressIP();
            // 获取端口
            ServerPort = port;

            // 初始化属性
            MaxConnectionNum = maxConnectionNum;
            ClientIDBase = 0;
        }

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <returns></returns>
        public bool ServerInit()
        {
            // 实例化服务器对象
            _Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // 绑定IP地址与端口 
                _Server.Bind(new IPEndPoint(AddressIP, ServerPort));

                // 指定连接队列数
                _Server.Listen(MaxConnectionNum);

                // 开辟线程启动连接侦听
                new Thread(ListenConnect).Start();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 启动连接侦听 - 异步方法
        /// </summary>
        void ListenConnect()
        {
            try
            {
                // 循环侦听连接
                while (true)
                {
                    // 创建连接对象 - 阻塞
                    Client client = (Client)_Server.Accept();

                    // 将客户端对象添加到容器
                    if (AddClienToClientList(client)) 
                    {
                        CurrentConnectionClient = client;

                        new Thread(ReceiveMessages).Start();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 添加客户端对象至客户端容器
        /// </summary>
        /// <param name="client">客户端实例对象</param>
        bool AddClienToClientList(Client client)
        {
            bool sign = true;

            client.Send(new Data(HeadInformation.CheckConnectState));

            Data result = client.Receive();

            if (OnlineClientCount != 0 && ClientList.Exists(i => i.ClientName == result.Data_Message.ClientName))
            {
                sign = false;

                client.Send(new Data(HeadInformation.CheckConnectState, new Message() { Sign = sign }));
            }
            else
            {
                int ClientID = ++ClientIDBase;
                string ClientName = result.Data_Message.ClientName;

                List<ClientList> onlineClientList = new List<ClientList>();

                foreach (var item in ClientList)
                {
                    onlineClientList.Add(new ClientList() { ClientID = item.ClientID, ClientName = item.ClientName });
                }

                Message msg = new Message() { ClientID = ClientID, ClientName = ClientName, Sign = sign, OnlineClientList = onlineClientList };

                client.Send(new Data(HeadInformation.CheckConnectState, msg));
            }

            return sign;
        }

        void ReceiveMessages()
        {
            try
            {
                Client client = CurrentConnectionClient;

                while (true)
                {
                    Data data = client.Receive();
                }
            }
            catch (Exception ex)
            {
                Thread.CurrentThread.Abort();
            }
        }

        public void SendMessageToClientByID(Data data)
        {

        }

        /// <summary>
        /// 向所有在线客户端广播消息
        /// </summary>
        /// <param name="data"></param>
        public void BroadcastMessage(Data data)
        {
            foreach (var item in ClientList)
            {
                item.Send(data);
            }
        }

        /// <summary>
        /// 获取内网IP地址
        /// </summary>
        /// <returns></returns>
        IPAddress GetAddressIP()
        {
            IPAddress AddressIP = null;

            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress;
                }
            }

            return AddressIP;
        }
    }
}