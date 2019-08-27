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
  * ![Activities](/Screens/Activities.png)
  * ![ActivitiesDb](/Screens/ActivitiesDb.png)
* Attendance;
  * Writes in MS SQL DB information about how long was user sesion on PC;
  * Writes in DB [UserName] ,[Date] ,[OnTime] ,[OfTime];
  * ![Attendance](/Screens/Attendance.png)
  * ![AttendanceDb](/Screens/AttendanceDb.png)
* Projects;
  * Used to show reports in software based on applicaiton Windows Titles which are added in SQL DB in specifc format;
  * In SQL DB administrator should add Project name and part of Windows Title (Keywords) by which time will be calculated;
  * Writes in DB [ProjectName] ,[WindowsTitleContains];
  * ![Projects](/Screens/Projects.png)
  * ![ProjectsDb](/Screens/ProjectsDb.png)
  
* When software started it cannot be turned off in normal ways, only from task manager;
* When software started you will see tray icon, if you click with left mouse, it will open app, if you click right mouse click you will see menu, where you can click on "Refresh Now" button which will send information to SQL DB. By default every 5 minutes data have been synced with SQL DB;
  * ![MouseLeftClick](/Screens/MouseLeftClick.png)
  * ![MouseRightClick](/Screens/MouseRightClick.png)
 
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
 
##### SQL DB configuration
* `USE [master]
GO

/****** Object:  Database [Development]    Script Date: 8/27/2019 11:19:50 PM ******/
CREATE DATABASE [Development]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Development', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\Development.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Development_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\Development_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Development].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [Development] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [Development] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [Development] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [Development] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [Development] SET ARITHABORT OFF 
GO

ALTER DATABASE [Development] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [Development] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [Development] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [Development] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [Development] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [Development] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [Development] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [Development] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [Development] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [Development] SET  DISABLE_BROKER 
GO

ALTER DATABASE [Development] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [Development] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [Development] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [Development] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [Development] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [Development] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [Development] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [Development] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [Development] SET  MULTI_USER 
GO

ALTER DATABASE [Development] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [Development] SET DB_CHAINING OFF 
GO

ALTER DATABASE [Development] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [Development] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [Development] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [Development] SET QUERY_STORE = OFF
GO

ALTER DATABASE [Development] SET  READ_WRITE 
GO
`
