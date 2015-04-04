using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace SQLBaseAdmin
{
    internal class Options
    {
        [Option('a', "Action", Required = true, HelpText = "Action to be executed by the utility { dbnames | show | abort }.")]
        public string Action { get; set; }

        [Option('s', "Server", Required = true, HelpText = "SQLBase server name.")]
        public string Server { get; set; }

        [Option('p', "Password", Required = false, HelpText = "Password to SQLBase server. If : is used, password will be the same as SQLBase name.' ")]
        public string Pasword { get; set; }

        [Option('d', "Database", Required = false, HelpText = "Name of the database on SQLBase server.")]
        public string Database { get; set; }

        //[Option('v', "Verbose", DefaultValue = false, HelpText = "Prints all messages to standard output.")]
        //public bool Verbose { get; set; }

        //[Option('u', "NoUser", DefaultValue = false, HelpText = "Automatically accept all user prompts in the verbose mode.")]
        //public bool NoUser { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            HelpText help = HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            help.AddPreOptionsLine("ARGUMENTS:");
            help.AddPostOptionsLine("NOTES:");
            help.AddPostOptionsLine("");
            help.AddPostOptionsLine("SQLBaseAdmin utility for Gupta SQLBase 11.7.");
            help.AddPostOptionsLine("For more details visit https://github.com/uriah65/SQLBaseAdminUtility.");
            help.AddPostOptionsLine("");
            help.AddPostOptionsLine("EXAMPLES");
            help.AddPostOptionsLine("");
            help.AddPostOptionsLine("SQLBaseAdmin.exe -a dbnames -s MYSERVERNAME -p :");
            help.AddPostOptionsLine("SQLBaseAdmin.exe -a show -s MYSERVERNAME -p : -d: MYDATABASENAME");
            help.AddPostOptionsLine("SQLBaseAdmin.exe -a abort -s MYSERVERNAME -p : -d: MYDATABASENAME");
            help.AddPostOptionsLine("");

            return help;
        }
    }
}
