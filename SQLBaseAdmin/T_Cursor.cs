using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLBaseAdmin
{
    internal class T_Cursor
    {
        public int ProcessId { get; set; }
        public string DatabaseName { get; set; }
        public string IsolationLevel { get; set; }
        public string LoginName { get; set; }
        public string Message { get; set; }
    }
}
