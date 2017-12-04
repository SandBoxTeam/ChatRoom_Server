using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom_Server
{
    class Data
    {
        /// <summary>
        /// 头部信息
        /// </summary>
        public HeadInformation HeadInfo { get;}

        /// <summary>
        /// 数据长度
        /// </summary>
        public int DataSize { get; }

        /// <summary>
        /// Byte_Data
        /// </summary>
        public byte[] Byte_Data { get;}

        /// <summary>
        /// Str_Data
        /// </summary>
        public string Str_Data { get; }

        public Data(HeadInformation headInfo)
        {
            HeadInfo = headInfo;
            DataSize = 0;
            Str_Data = null;
            Byte_Data = null;
        }

        public Data(string data)
        {
            Str_Data = data;
        }

        public Data(byte[] data)
        {
            Byte_Data = data;
        }
    }
}