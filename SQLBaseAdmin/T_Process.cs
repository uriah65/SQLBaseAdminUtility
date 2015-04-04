using System.Collections.Generic;

namespace SQLBaseAdmin
{
    internal class T_Process
    {
        public int Id { get; set; }

        public bool IsKillable { get; private set; }

        public bool IsSelected { get; set; }

        public string Database { get; private set; }

        public string User { get; private set; }

        public string Activity { get; set; }

        public string LoginNames { get; set; }

        public int CursorCount { get { return Cursors.Count; } }

        public List<T_Cursor> Cursors { get; set; }

        public T_Process()
        {
            Database = "";
            Cursors = new List<T_Cursor>();
        }

        public void Init(int id, string user)
        {
            Id = id;
            User = user;
            IsKillable = true;
        }

        public void AddCursor(T_Cursor cursor)
        {
            if (Database == "")
            {
                Database = cursor.DatabaseName;
            }

            if (Database != cursor.DatabaseName)
            {
                IsKillable = false;
                string message = string.Format("Process database {0} not matches cursor database. Cursor process={1}, database={2}, user={3}, isolation={4}.", Database, cursor.ProcessId, cursor.DatabaseName, cursor.LoginName, cursor.IsolationLevel);
                LoginNames += message;
                //todo: clean up
                //throw new ApplicationException(message);
            }

            Cursors.Add(cursor);
        }
    }
}