# SQLBaseAdminUtility

An utility for [Gupta SQLBase 11.7] (http://www.guptatechnologies.com/products/data_management/sqlbase/default.aspx) database service that allow to disconnect users from the given database by aborting database processes.

Implemented with C# Visual Studio 2015. Compiled for 64 bit version, require _sqlwntm.dll_  and _sql.ini_ from your installation of SQLBase. [Download executable] (https://github.com/uriah65/SQLBaseAdminUtility/tree/master/SQLBaseAdmin/bin/x64/Release)

**[Detailed description is here ...](https://victorscode.wordpress.com/2015/03/29/sqlbase-11-and-c-admin-utility/)**. Usage:


`
To display current processes and cursors on the server

 **SqlBaseAdmin.exe  show serverName  serverPassword**

To abort processes (disconnect users) from a database

 **SqlBaseAdmin.exe  abort serverName  serverPassword  databaseName** 

To display database names on the server

 **SqlBaseAdmin.exe  dbnames serverName  password**
`