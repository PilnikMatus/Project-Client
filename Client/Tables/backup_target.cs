using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon.Tables
{
    public class backup_target
    {
        public int id { get; set; }
        public int id_backup { get; set; }
        public string target_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string server { get; set; }
        public string port { get; set; }
        public string path { get; set; }
    }
}
