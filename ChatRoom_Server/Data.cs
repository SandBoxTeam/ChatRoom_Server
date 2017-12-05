using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatRoom_Server
{
    class Data
    {
        /// <summary>
        /// 头部信息
        /// </summary>
        public HeadInformation HeadInfo { get; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public int DataSize { get; }

        /// <summary>
        /// Str_Data
        /// </summary>
        public string Data_Str { get; }

        /// <summary>
        /// Byte_Data
        /// </summary>
        public byte[] Data_Byte { get; }

        /// <summary>
        /// 消息
        /// </summary>
        public Message Data_Message { get; }

        public Data(HeadInformation headInfo)
        {
            HeadInfo = headInfo;
            DataSize = 0;

            Data_Str = ((int)headInfo).ToString() + DataSize.ToString();
            Data_Byte = GetBytes(Data_Str).Length == 1024 ? GetBytes(Data_Str + " ") : GetBytes(Data_Str);
            Data_Message = null;
        }

        public Data(HeadInformation headInfo, Message msg)
        {
            HeadInfo = headInfo;
            DataSize = GetBytes(JsonConvert.SerializeObject(msg)).Length;

            Data_Str = ((int)headInfo).ToString() + DataSize.ToString() + JsonConvert.SerializeObject(msg);
            Data_Byte = GetBytes(Data_Str).Length == 1024 ? GetBytes(Data_Str + " ") : GetBytes(Data_Str);
            Data_Message = msg;
        }

        public Data(string data)
        {
            Data_Str = data;
            Data_Byte = GetBytes(data);

            HeadInfo = (HeadInformation)int.Parse(GetString(Data_Byte, 0, 4));
            DataSize = int.Parse(GetString(Data_Byte, 4, 8));

            Data_Message = JsonConvert.DeserializeObject<Message>(GetString(Data_Byte, 8, DataSize));
        }

        public Data(byte[] data)
        {
            Data_Str = GetString(data);
            Data_Byte = data;

            HeadInfo = (HeadInformation)int.Parse(GetString(Data_Byte, 0, 4));
            DataSize = int.Parse(GetString(Data_Byte, 4, 8));

            Data_Message = JsonConvert.DeserializeObject<Message>(GetString(Data_Byte, 8, DataSize));
        }

        Byte[] GetBytes(string str)
        {
            return Encoding.Unicode.GetBytes(str);
        }

        string GetString(Byte[] bytes, int? index = null, int? count = null)
        {
            if (index != null && count != null)
            {
                return Encoding.Unicode.GetString(bytes, (int)index, (int)count);
            }
            else
            {
                return Encoding.Unicode.GetString(bytes);
            }
        }
    }
}