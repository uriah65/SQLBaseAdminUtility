using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLBaseAdmin
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Invalid number of arguments " + args.Length + ". Expected 4.");
                Console.WriteLine("SQLBaseAdmin.exe {abort | show} server { password | : } database ");
                return -1;
            }

            string command = "" + args[0];
            string server = "" + args[1];
            string password = "" + args[2];
            string database = "" + args[3];

            command = command.ToLowerInvariant();
            if (password == ":")
            {
                password = server;
            }

            int result = 0;
            using (SQLBase sqlbase = new SQLBase(server, password))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0}Executing '{1}' command for database '{2}' on the server '{3}'.{0}", Environment.NewLine, command, database, server);

                switch (command)
                {
                    case "abort":
                        result = AbortDatabaseConnections(sqlbase, database, true);
                        break;
                    case "show":
                        result = AbortDatabaseConnections(sqlbase, database, false);
                        break;
                    case "dbnames":
                        result = GetDatabaseNames(sqlbase);
                        break;
                }

            }


            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            return result;
        }

        private static int AbortDatabaseConnections(SQLBase sqlbase, string database, bool abort)
        {
            // get original processes
            List<T_Process> processes = CollectProcesses(sqlbase, database);
            DisplayProcesses(processes);

            if (abort == false)
            {
                return 0;
            }

            // abort
            List<int> ids = processes.Where(e => e.IsSelected).Select(e => e.Id).ToList();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0}Performing abort for {1} processes ...{0}", Environment.NewLine, ids.Count);
            sqlbase.AbortProcesses(ids);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Finished.{0}", Environment.NewLine);

            // refresh processes
            processes = sqlbase.GetProcessesWithCursors();
            DisplayProcesses(processes);

            return 0;
        }

        private static List<T_Process> CollectProcesses(SQLBase sqlbase, string database)
        {
            List<T_Process> processes = sqlbase.GetProcessesWithCursors();
            foreach (T_Process process in processes)
            {
                process.IsSelected = (process.Database.ToLowerInvariant() == database);
            }
            return processes;
        }

        private static void DisplayProcesses(List<T_Process> processes)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Current processes:");

            string format = "{0, 4}\t{1, -15}\t{2, -15}\t{3, 4}\t{4}";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(format, "Process", "Database", "User", "Cursors", "Activity");

            Console.ForegroundColor = ConsoleColor.White;
            foreach (T_Process process in processes)
            {
                Console.ForegroundColor = process.IsSelected ? ConsoleColor.Yellow : ConsoleColor.White;
                string marker = process.IsSelected ? "*" : "";
                Console.WriteLine(format, marker + process.Id, process.Database, process.User, process.Cursors.Count, process.Activity);
            }
        }

        private static int GetDatabaseNames(SQLBase sqlbase)
        {
            Console.ForegroundColor = ConsoleColor.White;
            List<string> databaseNames = sqlbase.GetDatabaseNames().OrderBy(e => e).ToList();
            if (databaseNames.Count == 0)
            {
                Console.WriteLine("No databases found.");
            }

            Console.WriteLine("Databases found:");
            for (int i = 0; i < databaseNames.Count; i++)
            {
                Console.WriteLine("{0} {1}", i + 1, databaseNames[i]);
            }

            return 0;
        }
    }
}
