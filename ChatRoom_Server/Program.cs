using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom_Server
{
    class Program
    {
        /// <summary>
        /// 服务器对象
        /// </summary>
        static Server Server;

        /// <summary>
        /// 服务器端口
        /// </summary>
        static int ServerPort = 3360;

        /// <summary>
        /// 最大连接数
        /// </summary>
        static int MaxConnectionNum = 10;

        /// <summary>
        /// 控制台窗口标题
        /// </summary>
        static string WinTitle = "ChatRoom Server";

        static void Main(string[] args)
        {
            ServerInit();
        }

        /// <summary>
        /// 初始化服务器
        /// </summary>
        static void ServerInit()
        {
            // 设置控制台窗口标题
            Console.Title = WinTitle;

            SetServer();

            OutputLineMessage(ConsoleMessageType.Info, "ServerInitIng...");

            // 实例化服务器对象
            Server = new Server(ServerPort, MaxConnectionNum);

            // 绑定事件
            Server.AddClienToClientList_Event += new AddClienToClientList_EventHandler(AddClienToClientListEvent);
            Server.ReceiveMessages_Event += new ReceiveMessages_EventHandler(ReceiveMessagesEvent);
        }

        static void SetServer()
        {
            int _serverPort = ServerPort;
            int _maxConnectionNum = MaxConnectionNum;

            while (true)
            {
                string input;

                OutputMessage($"ServerPort ({_serverPort}):");

                input = GetInput();

                _serverPort = input.Trim() != "" ? int.Parse(input.Trim()) : _serverPort;

                OutputMessage($"MaxConnectionNum ({_maxConnectionNum}):");

                input = GetInput();

                _maxConnectionNum = input.Trim() != "" ? int.Parse(input.Trim()) : _maxConnectionNum;

                OutputMessage("Confirm Setting? (y/n):");

                input = GetInput();

                if (input.Trim().ToLower() == "y" || input.Trim().ToLower() == "yes")
                {
                    ServerPort = _serverPort;
                    MaxConnectionNum = _maxConnectionNum;

                    Clear();

                    return;
                }
                else
                {
                    _serverPort = ServerPort;
                    _maxConnectionNum = MaxConnectionNum;

                    Clear();
                }
            }
        }

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <returns></returns>
        static bool StartServer()
        {
            return true;
        }

        /// <summary>
        /// 添加客户端对象至客户端容器 事件
        /// </summary>
        static void AddClienToClientListEvent(Client client)
        {

        }

        /// <summary>
        /// 接收客户端消息 事件
        /// </summary>
        static void ReceiveMessagesEvent(Data data)
        {
            
        }

        static string GetInput()
        {
            return Console.ReadLine();
        }

        static void Clear()
        {
            Console.Clear();
        }

        /// <summary>
        /// 输出控制台消息
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="msg"></param>
        static void OutputMessage(string msg)
        {
            Console.Write(msg);
        }

        static void OutputLineMessage(Data data)
        {
            if (data.Data_Message.ToClientID != 0)
            {
                string toClientName = Server.ClientList.Find(i => i.ClientID == data.Data_Message.ToClientID).ClientName;
                int toClientID = data.Data_Message.ToClientID;

                Console.WriteLine($"[M][Message][{data.Data_Message.Time.ToString("T")}] [{data.Data_Message.ClientID}][{data.Data_Message.ClientName}] To [{toClientID}][{toClientName}] {data.Data_Message.Msg}");
            }
            else
            {
                Console.WriteLine($"[M][Message][{data.Data_Message.Time.ToString("T")}] [{data.Data_Message.ClientID}][{data.Data_Message.ClientName}] {data.Data_Message.Msg}");
            }
        }

        static void OutputLineMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        /// <summary>
        /// 输出一行控制台消息
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="msg"></param>
        static void OutputLineMessage(ConsoleMessageType msgType, string msg)
        {
            switch (msgType)
            {
                case ConsoleMessageType.Info:
                    Console.WriteLine($"[i][{msgType.ToString()}][{DateTime.Now.ToString("T")}] {msg}");
                    break;

                case ConsoleMessageType.Error:
                    Console.WriteLine($"[x][{msgType.ToString()}][{DateTime.Now.ToString("T")}] {msg}");
                    break;
            }
        }
    }
}