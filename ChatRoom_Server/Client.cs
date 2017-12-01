using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ChatRoom_Server
{
    class Client : Socket
    {
        /// <summary>
        /// 客户端ID
        /// </summary>
        public int ClientID { get; set; }

        /// <summary>
        /// 客户端名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="headInfo">头部信息</param>
        /// <param name="data">消息</param>
        /// <returns>已发送Data字节</returns>
        public int Send(Data data)
        {

        }

        public Client(SocketInformation socketInformation) : base(socketInformation)
        {
        }

        public Client(SocketType socketType, ProtocolType protocolType) : base(socketType, protocolType)
        {
        }

        public Client(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : base(addressFamily, socketType, protocolType)
        {
        }
    }
}
