using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

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

        int ClientIdBase;

        /// <summary>
        /// Server 构造方法
        /// </summary>
        /// <param name="port">服务器端口</param>
        /// <param name="maxConnectionNum">最大连接数</param>
        public Server(int port, int maxConnectionNum)
        {
            AddressIP = GetAddressIP();
            ServerPort = port;
            MaxConnectionNum = maxConnectionNum;
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

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 启动连接侦听
        /// </summary>
        void ListenConnect()
        {
            try
            {
                while (true)
                {
                    Client client = (Client)_Server.Accept();

                    AddClienToClientList(client);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        void AddClienToClientList(Client client)
        {


            string GetClientName()
            {

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
