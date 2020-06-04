using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon.Tables
{
    public class job_history
    {
        public int id { get; set; }
        public int id_job { get; set; }
        public DateTime date { get; set; }
        public bool success { get; set; }
        public string error_message { get; set; }
        public string info { get; set; }
    }
}
