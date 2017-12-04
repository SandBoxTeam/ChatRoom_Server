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
        /// <param name="data">消息</param>
        /// <returns>已发送Data字节</returns>
        public int Send(Data data)
        {
            return Send(data.Data_Byte);
        }

        public Data Receive()
        {
            // 总接收量
            int totalResultLength = 0;

            // 接收数据量
            int resultLength;

            // 数据容器
            byte[] totalResult = null;

            // 临时容器
            byte[] tempResult = null;

            // 接收容器
            byte[] result = null;

            do
            {
                result = new byte[1024];

                // 接收数据
                resultLength = Receive(result);

                if (totalResultLength == 0)
                {
                    totalResult = new byte[resultLength];

                    Array.Copy(result, totalResult, resultLength);

                    totalResultLength += resultLength;
                }
                else
                {
                    // 更新总接收量
                    totalResultLength += resultLength;

                    // 使用总接收量创建临时容器
                    tempResult = new byte[totalResultLength];

                    // 将数据容器中的数据放入临时容器
                    Array.ConstrainedCopy(totalResult, 0, tempResult, 0, totalResult.Length);
                    // 将接收容器的数据放入临时容器
                    Array.ConstrainedCopy(result, 0, tempResult, totalResult.Length, resultLength);

                    // 使用总接收量创建临时容器
                    totalResult = new byte[totalResultLength];

                    // 将临时容器的数据放入数据容器
                    totalResult = tempResult;
                }

            } while (resultLength == 1024);

            return new Data(totalResult);
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