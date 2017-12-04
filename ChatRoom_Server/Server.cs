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
                    AddClienToClientList(client);
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
        void AddClienToClientList(Client client)
        {
            // 设置客户端ID及获取客户端名称
            client.ClientID = ClientIDBase + 1;
            client.ClientName = GetClientName();

            // 将客户端实例对象添加到客户端容器
            ClientList.Add(client);

            /// 获取客户端名称
            string GetClientName()
            {
                client.Send(new Data(HeadInformation.GetClientName));

                Data data = client.Receive();

                return data.Data_Message;
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