using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon.Tables
{
    public class backup_time
    {
        public int id { get; set; }
        public int id_backup { get; set; }
        public DateTime start_time { get; set; }
        public string repetation_unit { get; set; }
        public int repetation_number { get; set; }
    }
}
