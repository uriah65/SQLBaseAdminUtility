# SQLBase Admin Utility

An utility for [Gupta SQLBase 11.7] (http://www.guptatechnologies.com/products/data_management/sqlbase/default.aspx) database service that allow to disconnect users from the given database by aborting database processes, as well as perform other actions listed below.

Implemented with C# Visual Studio 2015, compiled for 64 bit version, require sqlwntm.dll and sql.ini from your installation of SQLBase. [Download executable] (https://github.com/uriah65/SQLBaseAdminUtility/releases/tag/latest)

Detailed description [https://victorscode.wordpress.com/2015/03/29/sqlbase-11-and-c-admin-utility/](https://victorscode.wordpress.com/2015/03/29/sqlbase-11-and-c-admin-utility/).

---
<span style="color: green; font-weight: bold;">Usage:</span>

**To get help**
```
SqlBaseAdmin.exe --help
```
**To display database names on the server**
```
SqlBaseAdmin.exe -a names -s serverName -p serverPassword
```
**To display current processes and cursors on the server**
```
SqlBaseAdmin.exe -a show -s serverName -p serverPassword
```
**To abort processes (disconnect users) from a database**
```
SqlBaseAdmin.exe -a abort -s  serverName -p serverPassword -d databaseName
```
**To snapshot-backup databse to the directory on the server**
```
SqlBaseAdmin.exe -a snapshot -s  serverName -p serverPassword -d databaseName -r backuptopath
```
___
This is an example of the abort processes command. Processes highlighted in yellow match database name and are selected to abort. 
After abort command executed process list doesn't contain these processes any more.


![Screenshot](https://victorscode.files.wordpress.com/2015/03/abortsnapshot1.png "Screen shot")
