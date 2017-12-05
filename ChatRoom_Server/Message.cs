using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom_Server
{
    class Message
    {
        public int ClientID { get; set; }

        public string ClientName { get; set; }

        public int ToClientID { get; set; }

        public bool Sign { get; set; }

        public List<ClientList> OnlineClientList{ get; set; }

        public string Msg { get; set; }
    }
}