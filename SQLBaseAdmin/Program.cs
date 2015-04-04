using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLBaseAdmin
{
    internal class Program
    {
        private static Options _options;
        private static ConsoleColor _originalColor;

        private static int Main(string[] args)
        {
            _originalColor = Console.ForegroundColor;
            _options = new Options();

            try
            {
                if (CommandLine.Parser.Default.ParseArguments(args, _options) == false)
                {
                    return Quit(-1);
                }

                _options.Action = _options.Action.ToLowerInvariant();
                if (_options.Pasword == ":")
                {
                    _options.Pasword = _options.Server;
                }

                //if (string.IsNullOrWhiteSpace(_options.OutputFile))
                //{
                //    _options.OutputFile = _options.InputFile;
                //}

                return Main_Inner();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(Utilities.ExceptionMessage(ex));
                return Quit(-1);
            }

        }

        private static int Main_Inner()
        {
            int result = 0;
            using (SQLBase sqlbase = new SQLBase(_options.Server, _options.Pasword))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0}Executing '{1}' command for database '{2}' on the server '{3}'.{0}", Environment.NewLine, _options.Action, _options.Database, _options.Server);

                switch (_options.Action)
                {
                    case "abort":
                        result = AbortDatabaseConnections(sqlbase, _options.Database, true);
                        break;

                    case "show":
                        result = AbortDatabaseConnections(sqlbase, _options.Database, false);
                        break;

                    case "dbnames":
                        result = GetDatabaseNames(sqlbase);
                        break;
                }
            }

            return Quit(0);
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

        private static int Quit(int exitCode)
        {
            //if (_options.Verbose)
            //{
            //    Console.WriteLine("Press any key to quit.");
            //    if (_options.NoUser == false)
            //    {
            //        Console.ReadKey();
            //    }
            //}

            Console.ForegroundColor = _originalColor;

            return exitCode;
        }
    }
}