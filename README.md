# WorkTimeCounter
Software for tracking employee work time on Windows devices.
* Goal was to collect necessary information without any action from user side. User can open program to check progress, in some way it could help employee to be more motivated. Also this software can be used as control for employeer or boss, to see how effective are employees.
* Best works for persons who are working all day using PC;

## Description
C# WPF applicaiton. Works only on Windows Devices. Representing reports in this program using Microsoft Report Builder addon.

Tool collects information like:
* Activity;
  * Writes in MS SQL DB information about programms which where used by user on PC;
  * Writes in DB [UserName] ,[Date] ,[Time] ,[WindowsTitle];
  * ![Activities](/Files/Screens/Activities.png)
  * ![ActivitiesDb](/Files/Screens/ActivitiesDb.png)
* Attendance;
  * Writes in MS SQL DB information about how long was user sesion on PC;
  * Writes in DB [UserName] ,[Date] ,[OnTime] ,[OfTime];
  * ![Attendance](/Files//Screens/Attendance.png)
  * ![AttendanceDb](/Files//Screens/AttendanceDb.png)
* Projects;
  * Used to show reports in software based on applicaiton Windows Titles which are added in SQL DB in specifc format;
  * In SQL DB administrator should add Project name and part of Windows Title (Keywords) by which time will be calculated;
  * Writes in DB [ProjectName] ,[WindowsTitleContains];
  * ![Projects](/Files//Screens/Projects.png)
  * ![ProjectsDb](/Files//Screens/ProjectsDb.png)
  
* When software started it cannot be turned off in normal ways, only from task manager;
* When software started you will see tray icon, if you click with left mouse, it will open app, if you click right mouse click you will see menu, where you can click on "Refresh Now" button which will send information to SQL DB. By default every 5 minutes data have been synced with SQL DB;
  * ![MouseLeftClick](/Files//Screens/MouseLeftClick.png)
  * ![MouseRightClick](/Files//Screens/MouseRightClick.png)
 
## Implementation guide
* Guide will be as detailed as possible. Goal is to make this software available for anyone who needs such informaiton colelcted.

###### Prerequsites
* [Microsoft® SQL Server® 2017 Express](https://www.microsoft.com/en-us/download/details.aspx?id=55994)
  * And database where to store collected information;
* [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-2017)
  * To run SQL quries against DB's which we will create ([Also can be done from Visual Studio side](https://docs.microsoft.com/en-us/azure/sql-data-warehouse/sql-data-warehouse-query-visual-studio));
* [Visual Studio](https://visualstudio.microsoft.com/)
  * To compile project and change few things like SQL connection string;
* [Micorosft Windows Device](https://lv.wikipedia.org/wiki/Microsoft_Windows)
  * Device on which install this software;
 
##### Impelementation
1. [Installing SQL Server 2017 Express](https://www.mssqltips.com/sqlservertip/5528/installing-sql-server-2017-express/);
1. Run SQL script againt MS SQL DB "\WorkTimeCounter-master\Files\SetupSql\PrepareSQL.sql";
   1. At result you will see that "Development" DB is create with 3 tables:
   1. ![SqlTables](/Files//Screens/SqlTables.png)
1. Open project from Visual Studio;
   1. From VS go to Tools -> Connect to database -> Connect to your database;
   1. ![ConnectToDb](/Files//Screens/ConnectToDb.png)
   1. Copy "Conection String";
   1. ![ConnectionString](/Files//Screens/ConnectionString.png)
   1. From VS open "MainWindow.xaml.cs";
   1. Replace connectionString in line 48;
   1. In case if you have "\\" in your data source you will need add one more slash "\\\\" otherwise you will not be able to connect to DB, also you need to add Password check example bellow;
   1. `public static String connectionString = "Data Source=DESKTOP-O5L00A2\\SQLEXPRESS;Initial Catalog=Development;Persist Security Info=True;User ID=Development;Password=YourPasswordHere";`
   1. Reason why I add connection string in code before compilation because users should not be able to find connection string. To see other users data is not allowed.
1. Compile code install on user computers and enjoy!
