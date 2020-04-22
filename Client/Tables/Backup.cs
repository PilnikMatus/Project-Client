using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon.Tables
{
    public class Backup
    {
        public int id { get; set; }
        public int id_admin { get; set; }
        public string name { get; set; }
        public string backup_type { get; set; }
        public string format_type { get; set; }
        public DateTime last_possible_backup_date { get; set; }
        public bool active { get; set; }
    }
}
