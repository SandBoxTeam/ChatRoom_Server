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
        /// 添加客户端对象至客户端容器 事件
        /// </summary>
        public event AddClienToClientList_EventHandler AddClienToClientList_Event;

        /// <summary>
        /// 接收客户端消息 事件
        /// </summary>
        public event ReceiveMessages_EventHandler ReceiveMessages_Event;

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
        /// 关闭服务器
        /// </summary>
        public void ServerStop()
        {
            try
            {
                BroadcastMessage(new Data(HeadInformation.ServerOffline));

                _Server.Close();
                _Server.Dispose();

                ClientIDBase = 0;
                ClientList.Clear();
            }
            catch (Exception ex)
            {

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
                        new Thread(new ParameterizedThreadStart(ReceiveMessages)).Start(client);
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
            try
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

                    client.ClientID = ClientID;
                    client.ClientName = ClientName;

                    ClientList.Add(client);

                    List<ClientList> onlineClientList = new List<ClientList>();

                    foreach (var item in ClientList)
                    {
                        onlineClientList.Add(new ClientList() { ClientID = item.ClientID, ClientName = item.ClientName });
                    }

                    Message msg = new Message() { ClientID = ClientID, ClientName = ClientName, Sign = sign, OnlineClientList = onlineClientList };

                    client.Send(new Data(HeadInformation.CheckConnectState, msg));

                    AddClienToClientList_Event?.Invoke(client);

                    BroadcastMessage(new Data(HeadInformation.ClientOnline, new Message() { ClientID = ClientID, ClientName = ClientName }));
                }

                return sign;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 接收客户端消息
        /// </summary>
        /// <param name="client">客户端实例</param>
        void ReceiveMessages(object client)
        {
            try
            {
                Client _client = (Client)client;

                while (true)
                {
                    // 接收客户端消息
                    Data data = _client.Receive();

                    // 触发事件
                    ReceiveMessages_Event?.Invoke(data, _client);

                    // 判断头部信息类型
                    if (data.HeadInfo == HeadInformation.Message) // 消息
                    {
                        // 判断是否为私聊消息
                        if (data.Data_Message.ToClientID != 0) // 私聊
                        {
                            // 发送私聊消息
                            SendMessageToClientByID(data.Data_Message.ToClientID, data);
                        }
                        else // 群聊
                        {
                            // 广播群聊消息
                            BroadcastMessage(data);
                        }
                    }
                    else if (data.HeadInfo == HeadInformation.ClientOffline) // 客户端离线
                    {
                        ClientList.Remove(ClientList.Find(i => i.ClientID == data.Data_Message.ClientID));

                        // 广播客户端离线消息
                        BroadcastMessage(new Data(HeadInformation.ClientOffline, new Message() { ClientID = data.Data_Message.ClientID, ClientName = data.Data_Message.ClientName }));
                    }
                }
            }
            catch (Exception ex)
            {
                // 终止当前线程
                Thread.CurrentThread.Abort();
            }
        }

        /// <summary>
        /// 向指定客户端发送消息
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="data">消息data</param>
        void SendMessageToClientByID(int ClientID, Data data)
        {
            ClientList.Find(i => i.ClientID == ClientID).Send(data);
        }

        /// <summary>
        /// 向指定客户端发送消息
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="msg">消息字符串</param>
        public void SendMessageToClientByID(int ClientID, string msg)
        {
            ClientList.Find(i => i.ClientID == ClientID).Send(new Data(HeadInformation.Message, new Message() { ClientID = -1, ToClientID = ClientID, Msg = msg }));
        }

        /// <summary>
        /// 向所有在线客户端广播消息
        /// </summary>
        /// <param name="data"></param>
        int BroadcastMessage(Data data)
        {
            int num = 0;

            if (OnlineClientCount == 0)
            {
                return num;
            }

            foreach (var item in ClientList)
            {
                item.Send(data);
                num++;
            }

            return num;
        }

        /// <summary>
        /// 向所有在线客户端广播消息
        /// </summary>
        /// <param name="msg">消息字符串</param>
        /// <returns></returns>
        public int BroadcastMessage(string msg)
        {
            int num = 0;

            if (OnlineClientCount == 0)
            {
                return num;
            }

            foreach (var item in ClientList)
            {
                item.Send(new Data(HeadInformation.Message, new Message() { ClientID = -1, Msg = msg }));
                num++;
            }

            return num;
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