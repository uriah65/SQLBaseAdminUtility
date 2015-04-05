using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SQLBaseAdmin
{
    public sealed class SQLBase : IDisposable
    {
        private const int BUFFER_SIZE = 40000;

        private short _handle = 0;
        private string _server;


        #region Construct / Dispose

        public SQLBase(string server, string key = null)
        {
            _server = server;

            if (key == null)
            {
                key = _server;
            }

            short error = NativeMethods.sqlcsv(ref _handle, _server, key);
            if (error != 0)
            {
                string message = string.Format("Connection to server '{0}' failed. Error code '{1}'.", server, error);
                throw new ApplicationException(message);
            }

            GetDatabaseInformation();
        }

        public void Dispose()
        {
            if (_handle != 0)
            {
                short temp = _handle;
                _handle = 0;
                short error = NativeMethods.sqldsv(temp);
                
                //if (error != 0)
                //{
                //    string message = string.Format("Server '{0}' disconnect command failed.  Error code '{1}'.", _server, error);
                //    throw new ApplicationException(message);
                //}
            }
        }

        #endregion Construct / Dispose

        public List<string> GetDatabaseNames()
        {
            //short bufferLength = 50;
            byte[] buffer = new byte[BUFFER_SIZE];
            //string buffer = "                                                                                                                                        ";
            short error = NativeMethods.sqldbn(_server, buffer, BUFFER_SIZE);

            List<string> names = Utilities.ExtractStrings(buffer, 0, BUFFER_SIZE);

            return names;
        }

        public void GetDatabaseInformation()
        {
            //dbsdef
            short bufferLength = 0;
            byte[] buffer = new byte[BUFFER_SIZE];
            short error = NativeMethods.sqlgsi(_handle, NativeMethods.SQLGDBS, buffer, BUFFER_SIZE - 1, ref bufferLength);

            // db name start at 58 with step 64
        }

        public List<T_Process> GetProcesses()
        {
            List<T_Process> processes = new List<T_Process>();

            // Add zero process
            T_Process zeroProcess = new T_Process();
            zeroProcess.Init(0, "SQLBase");
            zeroProcess.Activity = "zero process";
            processes.Add(zeroProcess);

            short bufferLength = 0;
            byte[] buffer = new byte[BUFFER_SIZE];
            short error = NativeMethods.sqlgsi(_handle, 0x20 | 0x8000, buffer, BUFFER_SIZE - 1, ref bufferLength);
            if (error != 0)
            {
                string message = string.Format("Server '{0}' query processes operation failed. Error code '{1}'.", _server, error);
                throw new ApplicationException(message);
            }

            int i = 0;
            while (i < bufferLength - 160)
            {
                int id = (int)buffer[i + 22];
                string user = Utilities.ExtractString(buffer, i + 106);
                string activity = Utilities.ExtractString(buffer, i + 80);

                T_Process process = new T_Process();
                process.Init(id, user);
                process.Activity = activity;
                processes.Add(process);
                i = i + 160;
            }

            return processes;
        }

        public List<T_Cursor> GetCursors()
        {
            List<T_Cursor> cursors = new List<T_Cursor>();

            short bufferLength = 0;
            byte[] buffer = new byte[BUFFER_SIZE];

            short error = NativeMethods.sqlgsi(_handle, 2, buffer, BUFFER_SIZE - 1, ref bufferLength);
            if (error != 0)
            {
                string message = string.Format("Server '{0}' query cursors operation failed. Error code '{1}'.", _server, error);
                throw new ApplicationException(message);
            }

            int i = 0;
            while (i < bufferLength)
            {
                T_Cursor cursor = new T_Cursor()
                {
                    ProcessId = (int)buffer[i + 29],
                    IsolationLevel = Utilities.ExtractString(buffer, i + 30),
                    LoginName = Utilities.ExtractString(buffer, i + 33),
                    DatabaseName = Utilities.ExtractString(buffer, i + 52),
                };
                cursors.Add(cursor);
                i = i + 60;
            }

            return cursors;
        }

        public List<T_Process> GetProcessesWithCursors()
        {
            List<T_Process> processes = GetProcesses();
            List<T_Cursor> cursors = GetCursors();

            foreach (T_Cursor cursor in cursors)
            {
                T_Process process = processes.Single(e => e.Id == cursor.ProcessId);
                if (process == null)
                {
                    string message = string.Format("No process for the cursor process={0}, database={1}, user={2}, isolation={3}.", cursor.ProcessId, cursor.DatabaseName, cursor.LoginName, cursor.IsolationLevel);
                    throw new ApplicationException(message);
                }

                process.AddCursor(cursor);
            }

            return processes;
        }

        public void BackupDatabase(string database, string serverPath)
        {
            if (serverPath.Contains("ProgramData"))
            {
                string message = string.Format("Backup path includes protected folder 'ProgramData'.");
                throw new ApplicationException(message);
            }

            short error = NativeMethods.sqlbss(_handle, database, 0, serverPath, 0, 0, 1);
            if (error != 0)
            {
                string message = string.Format("Backup database '{0}' on server '{1}' to the folder '{3}' failed. Error code '{2}'.", database, _server, error, serverPath);
                throw new ApplicationException(message);
            }
        }

        public void AbortProcesses(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return;
            }

            foreach (int id in ids)
            {
                short error = NativeMethods.sqlsab(_handle, (short)id);
                if (error != 0)
                {
                    string message = string.Format("Abort process id='{0}' failed. Error code '{1}'.", id, error);
                    throw new ApplicationException(message);
                }
            }
        }
    }
}

///* message header */
//struct hdrdefx
//{
//    unsigned short hdrlen;      /* message length (including hdr)   */
//    unsigned short hdrrsv;		/* reserved			    */
//};
//typedef struct hdrdefx hdrdef;
//#define HDRSIZ SIZEOF(hdrdef)

///* message section header */
//struct mshdefx
//{
//    unsigned short mshflg;      /* section data type		    */
//    unsigned short mshten;      /* total # of entries available     */
//    unsigned short mshnen;      /* # of entries contained in msg    */
//    unsigned short mshlen;      /* # of data bytes in msg section   */
//                                /* (including this header)	    */
//};
//typedef struct mshdefx mshdef;
//#define MSHSIZ SIZEOF(mshdef)
///* cursor information */
//struct curdefx
//{
//	unsigned long  currow;		/* number of rows		    */
//	unsigned short curibl;		/* input buffer 		    */
//	unsigned short curobl;		/* output message buffer length     */
//	unsigned short curspr;		/* stat counter, physical reads     */
//	unsigned short curspw;		/* stat counter, physical writes    */
//	unsigned short cursvr;		/* stat counter, virtual reads	    */
//	unsigned short cursvw;		/* stat counter, virtual writes     */
//	unsigned char  curtyp;		/* command type 		    */
//	unsigned char  curpnm;		/* process number		    */
//	unsigned char  curiso[3];	/* locking isolation		    */
//	unsigned char  curunb[19];	/* user name buffer		    */
//	unsigned char  curdbn[17];	/* database name		    */
//	unsigned char  currsv[3];	/* reserved			    */
//};

//
///* process information */
//struct prcdefx
//{
//	unsigned short prccol;		/* current output message length    */
//	unsigned short prcibl;		/* input message buffer length	    */
//	unsigned short prcinl;		/* input length 		    */
//	unsigned short prcobl;		/* output message buffer length     */
//	unsigned short prcoul;		/* output length		    */
//	unsigned char  prcpnm;		/* process number		    */
//	unsigned char  prcact;		/* active flag			    */
//};
//typedef struct prcdefx prcdef;
//#define PRCSIZ SIZEOF(prcdef)
///* process information */
//struct prcdefxi
//{
//	ubyte4 prcsel;				/* number of selects */
//	ubyte4 prcins;				/* number of inserts */
//	ubyte4 prcupd;				/* number of updates */
//	ubyte4 prcdel;				/* number of deletes */
//	ubyte4 prctps;				/* number of transactions */
//	ubyte4 prcdlk;				/* number of deadlocks */
//	ubyte4 prcelp;				/* number of exclusive locks */
//	ubyte4 prcslp;				/* number of shared locks */
//	ubyte4 prculp;				/* number of update locks */
//	ubyte4 prcast;				/* accumulative system time */
//	ubyte4 prcptp;				/* time for prepare */
//	ubyte4 prcpte;				/* time for execute */
//	ubyte4 prcptf;				/* time for fetch */
//	ubyte4 prcmtt;				/* maximum transaction time */
//	ubyte1 prcpss[26];			/* status string */
//	ubyte1 prccln[13];			/* client name */
//	ubyte1 prcsta;				/* status flag */
//	ubyte1 prcpts[20];			/* process time stamp */
//	ubyte1 prciso;				/* isolation level flags */
//	ubyte4 prctmo;				/* number of timeouts */
//	ubyte1 prcrsv[27];			/* reserved */
//};
//typedef struct prcdefxi prcdefi;
//#define PRCXSIZ SIZEOF(prcdefi)
///* database information */
//struct dbsdefx
//{
//    unsigned long dbsbfs;       /* before image file size	    */
//    unsigned long dbsbwp;       /* before image write page	    */
//    unsigned long dbsdfs;       /* database file size		    */
//    unsigned long dbsfrp;       /* 1st reader page in circular log  */
//    unsigned long dbsfup;       /* 1st updater page in circular log */
//    unsigned long dbslpa;       /* last page allocated		    */
//    unsigned long dbslpm;       /* log page maximum		    */
//    unsigned long dbslpt;       /* log page threshold		    */
//    unsigned long dbslpw;       /* last page written		    */
//    unsigned long dbsltp;       /* last temporary page accessed     */
//    unsigned long dbsltw;       /* last temporary page written	    */
//    unsigned char dbsuse;       /* use count			    */
//    unsigned char dbsnat;       /* number of active transactions    */
//    unsigned char dbsntr;       /* number of transactions	    */
//    unsigned char dbsfnm[17];		/* database file name		    */
//};