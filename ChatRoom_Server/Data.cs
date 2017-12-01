using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom_Server
{
    class Data
    {
        public HeadInformation HeadInfo { get;}

        public byte[] Byte_Data { get;}

        public string Str_Data { get; }

        public Data(HeadInformation headInfo)
        {
            HeadInfo = headInfo;
            Str_Data = null;
            Byte_Data = null;
        }

        public Data(HeadInformation headInfo, string data)
        {
            Str_Data = data;
        }

        public Data(HeadInformation headInfo, byte[] data)
        {
            Byte_Data = data;
        }
    }
}