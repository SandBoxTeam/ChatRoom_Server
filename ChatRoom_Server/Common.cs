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
        GetOnlineClientCount = 102, // 获取在线客户端数
        GetClientName        = 103, // 获取客户端名称
        ServerOffline        = 201, // 服务器离线
        ClientOnline         = 202, // 客户端上线
        ClientOffline        = 203, // 客户端离线
        Msg                  = 301  // 消息
    }
}