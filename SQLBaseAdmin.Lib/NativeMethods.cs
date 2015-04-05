using System.Runtime.InteropServices;

namespace SQLBaseAdmin
{
    internal static class NativeMethods
    {
        /* server information flags */
        public const int SQLXGSI = 0x8000;     /* extended GSI information flag */
        public const int SQLGPWD = 0x01;       /* send password		         */
        public const int SQLGCUR = 0x02;       /* cursor information		     */
        public const int SQLGDBS = 0x04;       /* database information 	     */
        public const int SQLGCFG = 0x08;       /* configuration information	 */
        public const int SQLGSTT = 0x10;       /* statistics			         */
        public const int SQLGPRC = 0x20;		/* process information		     */

        // connect to SQLBase server
        [DllImport("sqlwntm.dll")]
        public static extern short sqlcsv(ref short handle, string serverName, string serverKey);

        // disconnect from to SQLBase server
        [DllImport("sqlwntm.dll")]
        public static extern short sqldsv(short handle);

        // getting server information
        [DllImport("sqlwntm.dll")]
        public static extern short sqlgsi(short handle, int flags, byte[] buffer, int sizeofbuffer, ref short bufferLength);

        // abort database process
        [DllImport("sqlwntm.dll")]
        public static extern short sqlsab(short handle, short processId);

        // perform 'snapshot' backup
        [DllImport("sqlwntm.dll")]
        public static extern short sqlbss(short handle, string dbname, int dbnamel, string bkpdir, int bkpdirl, short local, short over);

        // get database names
        [DllImport("sqlwntm.dll")]
        public static extern short sqldbn(string server, byte[] buffer, int sizeofbuffer);
    }
}