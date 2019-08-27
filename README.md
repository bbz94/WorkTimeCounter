# WorkTimeCounter
Software for tracking employee work time on Windows devices.
* Goal was to collect necessary information without any action from user side. User can open program to check progress, in some way it could help employee to be more motivated. Also this software can be used as control for employeer or boss, to see how effective are employees.
* Best works for persons who are working all day using PC;

## Description
C# WPF applicaiton. Works only on Windows Devices. Representing reports in this program using Microsoft Report Builder addon;
Tool collect information like:
* Activity;
 * Writes in MS SQL DB information about programms which where used by user on PC;
 * Writes in DB [UserName] ,[Date] ,[Time] ,[WindowsTitle];
* Attendance;
 * Writes in MS SQL DB information about how long was user sesion on PC;
 * Writes in DB [UserName] ,[Date] ,[OnTime] ,[OfTime];
* Projects;
 * Used to show reports in software based on applicaiton Windows Titles which are added in SQL DB in specifc format;
 * In SQL DB administrator should add Project name and part of Windows Title (Keywords) by which time will be calculated;
 * Writes in DB [ProjectName] ,[WindowsTitleContains]
 

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
