using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon.Tables
{
    public class Client
    {
        public string id { get; set; }
        public string name { get; set; }
        public string mac_address { get; set; }
        public string ip_address { get; set; }
        public int active { get; set; }
    }
}
