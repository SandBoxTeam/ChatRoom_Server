using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom_Server
{
    /// <summary>
    /// 头部信息
    /// </summary>
    public enum HeadInformation
    {
        CheckConnectState    = 101, // 检查连接状态
        ServerOffline        = 201, // 服务器离线
        ClientOnline         = 202, // 客户端上线
        ClientOffline        = 203, // 客户端离线
        Message              = 301  // 消息
    }
}