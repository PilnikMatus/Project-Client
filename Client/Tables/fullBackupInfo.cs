using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon.Tables
{
    public class fullBackupInfo
    {
        public int id { get; set; }
        public int id_admin { get; set; }
        public string name { get; set; }
        public string backup_type { get; set; }
        public string format_type { get; set; }
        public DateTime last_possible_backup_date { get; set; }
        public bool active { get; set; }
        public List<backup_source> backup_source { get; set; } = new List<backup_source>();
        public List<backup_target> backup_target { get; set; } = new List<backup_target>();
        public List<backup_time> backup_time { get; set; } = new List<backup_time>();
    }
}
