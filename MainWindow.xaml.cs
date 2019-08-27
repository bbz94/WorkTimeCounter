using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Diagnostics;
using System.ComponentModel;
using System.Timers;
using System.Windows.Threading;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Reporting.WinForms;
using MahApps.Metro.Controls;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Windows.Interop;

namespace AGSWorkTimeCounter
{
    public static class User
    {
        //Get username
        public static WindowsIdentity user = System.Security.Principal.WindowsIdentity.GetCurrent();
        public static string userName = user.Name.Substring(user.Name.LastIndexOf("\\") + 1);
    }

    public static class Tables
    {
        //datatable
        public static DataTable Activitiestable = new DataTable("WindowsTitleCounter");
        public static DataTable Attendancetable = new DataTable("LogInTimeCounter");
    }

    public static class Globals
    {
        //DB connection string
        public static String connectionString = "Data Source=DESKTOP-O5L00A2\\SQLEXPRESS;Initial Catalog=Development;Persist Security Info=True;User ID=Development;Password=Daugava1";

        //Session Switch state variable
        public static string SessionSwitchState;

        //Notification icon
        public static System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //Timer
        private readonly System.Diagnostics.Stopwatch Activities5min = new System.Diagnostics.Stopwatch();
        private readonly System.Diagnostics.Stopwatch ActivitiesSwDt = new System.Diagnostics.Stopwatch();
        private readonly System.Diagnostics.Stopwatch Attendance5min = new System.Diagnostics.Stopwatch();
        private readonly System.Diagnostics.Stopwatch AttendanceSwDt = new System.Diagnostics.Stopwatch();

        public object Time { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            //Sustem events
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            //Tray icon
            Globals.ni.Icon = new System.Drawing.Icon("Timer.ico");
            Globals.ni.Visible = true;
            Globals.ni.MouseClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_Click);

            //ContextMenu
            System.Windows.Forms.ContextMenu cm = new System.Windows.Forms.ContextMenu();
            cm.MenuItems.Add("Refresh now", new EventHandler(Refresh_now));
            Globals.ni.ContextMenu = cm;
        }

        private void RefreshActivties()
        {
            string UserName = $"\'{User.userName}\'";

            //Activities
            try
            {
                for (int i = Tables.Activitiestable.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = Tables.Activitiestable.Rows[i];
                    string Date = $"\'{dr["Date"].ToString()}\'";
                    string WindowsTitle = $"\'{dr["WindowsTitle"].ToString()}\'";
                    string Time = $"\'{dr["Time"].ToString()}\'";
                    string TimeForSum = dr["Time"].ToString();

                    string Query = $"Select CONVERT(nvarchar(14), Time, 114) as [Time] from Activities WHERE WindowsTitle = {WindowsTitle} and Username = {UserName} and Date = {Date}";
                    string TimeString = SQLExecuteValue(Query);

                    if (TimeString != null)
                    {
                        //MessageBox.Show(TimeString);
                        TimeSpan TimeStringSql = TimeSpan.Parse(TimeString);
                        TimeSpan TimeStringDT = TimeSpan.Parse(TimeForSum);
                        string TimeSumStr = $"\'{TimeStringSql.Add(TimeStringDT).ToString()}\'";

                        string QueryUpd = $"UPDATE Activities SET Time = {TimeSumStr} WHERE Username = {UserName} and WindowsTitle = {WindowsTitle} and Date = {Date}";
                        SQLExecuteBasic(QueryUpd);
                    }
                    else
                    {
                        string query = $"INSERT INTO Activities(Username, Date, Time, WindowsTitle) VALUES({UserName}, {Date}, {Time}, {WindowsTitle})";
                        SQLExecuteBasic(query);
                    }
                    dr.Delete();
                    TimeString = null;
                }
                Tables.Activitiestable.AcceptChanges();
            }
            catch (Exception e)
            {
                ShowBallonTip("Failed to connect to SQL server", e.Message, 1000);
            }
        }

        private void RefreshAttendance()
        {
            string UserName = $"\'{User.userName}\'";

            //Attendance
            try
            {
                for (int i = Tables.Attendancetable.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = Tables.Attendancetable.Rows[i];
                    string Date = $"\'{dr["Date"].ToString()}\'";
                    string OnTime = $"\'{dr["OnTime"].ToString()}\'";
                    string OfTime = $"\'{dr["OfTime"].ToString()}\'";
                    string OnTimeForSum = dr["OnTime"].ToString();
                    string OfTimeForSum = dr["OfTime"].ToString();

                    string Query = $"Select CONVERT(nvarchar(14), OnTime, 114) as [OnTime] from Attendance WHERE Username = {UserName} and Date = {Date}";
                    string OnTimeString = SQLExecuteValue(Query);

                    string Query2 = $"Select CONVERT(nvarchar(14), OfTime, 114) as [OfTime] from Attendance WHERE Username = {UserName} and Date = {Date}";
                    string OfTimeString = SQLExecuteValue(Query2);

                    if (OnTimeString != null)
                    {
                        //From SQL string to TimeSpan
                        TimeSpan OnTimeStringSql = TimeSpan.Parse(OnTimeString);
                        TimeSpan OfTimeStringSql = TimeSpan.Parse(OfTimeString);

                        //To SQL string to TimeSpan
                        TimeSpan OnTimeStringDT = TimeSpan.Parse(OnTimeForSum);
                        TimeSpan OfTimeStringDT = TimeSpan.Parse(OfTimeForSum);
                        string OnTimeSumStr = $"\'{OnTimeStringSql.Add(OnTimeStringDT).ToString()}\'";
                        string OfTimeSumStr = $"\'{OfTimeStringSql.Add(OfTimeStringDT).ToString()}\'";

                        string QueryUpd = $"UPDATE Attendance SET OnTime = {OnTimeSumStr}, OfTime = {OfTimeSumStr} WHERE Username = {UserName} and Date = {Date}";
                        SQLExecuteBasic(QueryUpd);
                    }
                    else
                    {
                        string query = $"INSERT INTO Attendance(Username, Date, OnTime, OfTime) VALUES({UserName}, {Date}, {OnTime}, {OfTime})";
                        SQLExecuteBasic(query);
                    }
                    dr.Delete();

                    OnTimeString = null;
                    OfTimeString = null;
                }
                Tables.Attendancetable.AcceptChanges();
            }
            catch (Exception e)
            {
                ShowBallonTip("Failed to connect to SQL server", e.Message, 1000);
            }
        }

        private void Refresh_now(object sender, EventArgs e)
        {
            //Add in to DB information about Activities;
            RefreshActivties();

            //Add in to DB information about Attendance;
            RefreshAttendance();
        }

        //Show hide system tray
        private void NotifyIcon_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //MessageBox.Show(e.Button.ToString());
            if (e.Button.ToString() == "Left")
            {
                //Add dates
                //AddDatesToDatapicker();

                this.Show();
                this.WindowState = WindowState.Normal;

                //move to right corner
                var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
                this.Left = desktopWorkingArea.Right - this.Width;
                this.Top = desktopWorkingArea.Bottom - this.Height;
            }
        }

        private void ShowBallonTip(string Title, string Text, int Duration)
        {
            Globals.ni.BalloonTipTitle = Title;
            Globals.ni.BalloonTipText = Text;
            Globals.ni.ShowBalloonTip(Duration);
        }

        //When Windows load add dates to date picker
        private void Windows_Loaded(object sender, RoutedEventArgs e)
        {
            //Start WindowsTitle Time counter function to write in DataTable
            WindowsTitleTimeCounterDataTable();

            //Start Log in Log Of time counter to write in DataTable
            LoginInLogOfTimeCounterDataTable();

            //Add dates to datepicker
            AddDatesToDatapicker();
        }

        //Cancel Alt+F4
        private void Windows_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        //Add dates to datapicker
        private void AddDatesToDatapicker()
        {
            //Get date
            DateTime dateTime = DateTime.UtcNow.Date;
            string ToDay = dateTime.ToString("yyyy-MM-dd");

            //Activities datepicker
            From_Time_Days.Text = ToDay;
            To_Time_Days.Text = ToDay;

            From_Time_Total.Text = ToDay;
            To_Time_Total.Text = ToDay;

            //Attendance datepicker
            Attendance_From_Time_Days.Text = ToDay;
            Attendance_To_Time_Days.Text = ToDay;

            Attendance_From_Time_Total.Text = ToDay;
            Attendance_To_Time_Total.Text = ToDay;

            //Projects datepicker
            Projects_From_Time_Days.Text = ToDay;
            Projects_To_Time_Days.Text = ToDay;

            Projects_From_Time_Total.Text = ToDay;
            Projects_To_Time_Total.Text = ToDay;
        }

        //Remove task bar icon
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        //Get Windows title
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }


        //Get Session state
        void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            Globals.SessionSwitchState = e.Reason.ToString();
        }

        //Create Datatable of SQL query output
        private DataTable SQLExecuteDataTable(string Query)
        {
            String connectionString = Globals.connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(Query, con);
            DataTable dt = new DataTable();
            try
            {
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                //Activities.ItemsSource = dt.DefaultView;
            }
            catch (Exception e)
            {
                ShowBallonTip("Failed to connect to SQL server", e.Message, 1000);
            }
            cmd.Dispose();
            con.Close();
            return dt;
        }

        public void SQLExecuteBasic(string Query)
        {
            String connectionString = Globals.connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(Query, con);

            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            cmd.Dispose();
            con.Close();
        }

        public string SQLExecuteValue(string Query)
        {
            String connectionString = Globals.connectionString;
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand QueryValue = con.CreateCommand();
            QueryValue.CommandText = Query;
            string Value = null;
            con.Open();
            Value = (string)QueryValue.ExecuteScalar();
            con.Close();

            return Value;
        }

        public void RunReportViewer(string SqlQuery, string DataSet, string ReportPath)
        {
            ReportViewer.Reset();
            string query = SqlQuery;
            DataTable dt = SQLExecuteDataTable(query);
            ReportDataSource ds = new ReportDataSource(DataSet, dt);
            ReportViewer.LocalReport.DataSources.Add(ds);

            ReportViewer.LocalReport.ReportPath = ReportPath;
            ReportViewer.RefreshReport();
        }

        public void WindowsTitleTimeCounterDataTable()
        {
            var bgw = new BackgroundWorker();
            string UserName = User.userName;
            string WindowsTitleA = "";

            //Add collumnds to data table
            Tables.Activitiestable.Columns.Add("Date", typeof(string));
            Tables.Activitiestable.Columns.Add("WindowsTitle", typeof(string));
            Tables.Activitiestable.Columns.Add("Time", typeof(string));

            bgw.DoWork += (_, __) =>
            {
                string WindowsTitleB = null;
                while (true)
                {
                    //Start 5 min timer
                    Activities5min.Start();

                    //Write information every 5 minutes in to DB
                    if (Activities5min.Elapsed.Minutes == 5)
                    {
                        RefreshActivties();
                        Activities5min.Reset();
                    }

                    Thread.Sleep(1000);
                    if (Globals.SessionSwitchState == "SessionUnlock" || Globals.SessionSwitchState == null)
                    {
                        ActivitiesSwDt.Start();
                        WindowsTitleA = GetActiveWindowTitle();
                        if (WindowsTitleB != WindowsTitleA)
                        {
                            if (WindowsTitleB != null)
                            {
                                DateTime dateTime = DateTime.UtcNow.Date;
                                string Date = dateTime.ToString("yyyy-MM-dd");

                                string elapsed = ActivitiesSwDt.Elapsed.ToString();
                                
                                string WindowsTitle = Regex.Replace(WindowsTitleB, "'", "\"");

                                //Get value from data table
                                string TimeString = null;
                                foreach (DataRow dr in Tables.Activitiestable.Rows) // search whole table
                                {
                                    if (dr["WindowsTitle"].ToString() == WindowsTitle && dr["Date"].ToString() == Date) // if id==2
                                    {
                                        TimeString = dr["Time"].ToString();
                                    }
                                }

                                if (TimeString != null)
                                {
                                    TimeSpan TimeStringDT = TimeSpan.Parse(TimeString);
                                    string TimeSumStr = (TimeStringDT.Add(ActivitiesSwDt.Elapsed)).ToString();

                                    foreach (DataRow dr in Tables.Activitiestable.Rows) // search whole table
                                    {
                                        if (dr["WindowsTitle"].ToString() == WindowsTitle && dr["Date"].ToString() == Date) // if id==2
                                        {
                                            dr["Time"] = TimeSumStr;
                                        }
                                    }
                                }
                                else
                                {
                                    Tables.Activitiestable.Rows.Add(Date, WindowsTitle, elapsed);
                                }
                                TimeString = null;
                            }
                            ActivitiesSwDt.Reset();
                            WindowsTitleB = GetActiveWindowTitle();
                        }
                    }
                    else
                    {
                        DateTime dateTime = DateTime.UtcNow.Date;
                        string Date = dateTime.ToString("yyyy-MM-dd");

                        string elapsed = ActivitiesSwDt.Elapsed.ToString();

                        string WindowsTitle = Regex.Replace(WindowsTitleA, "'", "\"");
                        string TimeString = null;
                        foreach (DataRow dr in Tables.Activitiestable.Rows) // search whole table
                        {
                            if (dr["WindowsTitle"].ToString() == WindowsTitle && dr["Date"].ToString() == Date) // if id==2
                            {
                                TimeString = dr["Time"].ToString();
                            }
                        }

                        if (TimeString != null)
                        {
                            TimeSpan TimeStringDT = TimeSpan.Parse(TimeString);
                            string TimeSumStr = (TimeStringDT.Add(ActivitiesSwDt.Elapsed)).ToString();

                            foreach (DataRow dr in Tables.Activitiestable.Rows) // search whole table
                            {
                                if (dr["WindowsTitle"].ToString() == WindowsTitle && dr["Date"].ToString() == Date) // if id==2
                                {
                                    dr["Time"] = TimeSumStr;
                                }
                            }
                        }
                        else
                        {
                            Tables.Activitiestable.Rows.Add(Date, WindowsTitle, elapsed);
                        }
                        TimeString = null;
                        ActivitiesSwDt.Reset();
                        ActivitiesSwDt.Stop();
                    }
                }
            };
            bgw.RunWorkerAsync();
        }

        public void LoginInLogOfTimeCounterDataTable()
        {
            var bgw = new BackgroundWorker();
            bgw.DoWork += (_, __) =>
            {
                string UserName = User.userName;
                //string UserNameSQL = $"\'{UserName}\'";
                bool OfVar = false;
                bool OnVar = false;

                //Add columnds to datatable
                Tables.Attendancetable.Columns.Add("Date", typeof(string));
                Tables.Attendancetable.Columns.Add("OnTime", typeof(string));
                Tables.Attendancetable.Columns.Add("OfTime", typeof(string));
                while (true)
                {
                    //Start 5 min timer
                    Attendance5min.Start();

                    //Write information every 5 minutes in to DB
                    if (Attendance5min.Elapsed.Minutes == 5)
                    {
                        RefreshAttendance();
                        Attendance5min.Reset();
                    }

                    Thread.Sleep(1000);
                    DateTime dateTime = DateTime.UtcNow.Date;
                    string Date = dateTime.ToString("yyyy-MM-dd");

                    AttendanceSwDt.Start();
                    if (Globals.SessionSwitchState == "SessionUnlock" || Globals.SessionSwitchState == null)
                    {
                        OnVar = true;
                        if (OfVar)
                        {
                            string elapsed = AttendanceSwDt.Elapsed.ToString();

                            string TimeString = null;
                            foreach (DataRow dr in Tables.Attendancetable.Rows) // search whole table
                            {
                                if (dr["Date"].ToString() == Date) // if id==2
                                {
                                    TimeString = dr["OfTime"].ToString();
                                }
                            }

                            if (TimeString != null)
                            {
                                TimeSpan TimeStringDT = TimeSpan.Parse(TimeString);
                                string TimeSumStr = (TimeStringDT.Add(AttendanceSwDt.Elapsed)).ToString();

                                foreach (DataRow dr in Tables.Attendancetable.Rows) // search whole table
                                {
                                    if (dr["Date"].ToString() == Date) // if id==2
                                    {
                                        dr["OfTime"] = TimeSumStr;
                                    }
                                }
                            }
                            else
                            {
                                Tables.Attendancetable.Rows.Add(Date, "00:00:00.0000000", elapsed);
                            }
                            TimeString = null;
                            AttendanceSwDt.Reset();
                        }
                        OfVar = false;
                    }
                    else
                    {
                        OfVar = true;
                        if (OnVar)
                        {
                            string elapsed = AttendanceSwDt.Elapsed.ToString();
                            string TimeString = null;
                            foreach (DataRow dr in Tables.Attendancetable.Rows) // search whole table
                            {
                                if (dr["Date"].ToString() == Date) // if id==2
                                {
                                    TimeString = dr["OnTime"].ToString();
                                }
                            }
                            if (TimeString != null)
                            {
                                TimeSpan TimeStringDT = TimeSpan.Parse(TimeString);
                                string TimeSumStr = (TimeStringDT.Add(AttendanceSwDt.Elapsed)).ToString();

                                foreach (DataRow dr in Tables.Attendancetable.Rows) // search whole table
                                {
                                    if (dr["Date"].ToString() == Date) // if id==2
                                    {
                                        dr["OnTime"] = TimeSumStr;
                                    }
                                }
                            }
                            else
                            {
                                Tables.Attendancetable.Rows.Add(Date, elapsed, "00:00:00.0000000");
                            }
                            TimeString = null;
                            AttendanceSwDt.Reset();
                        }
                        OnVar = false;
                    }
                }

            };
            bgw.RunWorkerAsync();
        }

        //Mouse down
        private void Activities_Days_MouseDown(object sender, RoutedEventArgs e)
        {
            string SqlQuery = $"Select UserName, Date, CONVERT(nvarchar(14), Time, 114) as [Time], WindowsTitle from dbo.Activities";
            RunReportViewer(SqlQuery, "DataSet1", "Activities_Date.rdlc");
        }

        private void Activities_Total_MouseDown(object sender, RoutedEventArgs e)
        {
            string SqlQuery = $"select Distinct a.UserName, A.WindowsTitle, TimeCalc.Time as [Time] from Activities as a INNER JOIN (SELECT WindowsTitle,CONVERT(TIME, DATEADD(s, SUM(( DATEPART(hh, Time) * 3600 ) + ( DATEPART(mi, Time) * 60 ) + DATEPART(ss, Time)), 0)) AS Time FROM   Activities group by WindowsTitle) as TimeCalc on a.WindowsTitle=TimeCalc.WindowsTitle Order by Time DESC";
            RunReportViewer(SqlQuery, "DataSet1", "Activities_Total.rdlc");
        }

        private void Attendance_Days_MouseDown(object sender, RoutedEventArgs e)
        {
            string SqlQuery = $"Select UserName, Date, CONVERT(nvarchar(14), OnTime, 114) as [OnTime], CONVERT(nvarchar(14), OfTime, 114) as [OfTime] from dbo.Attendance";
            RunReportViewer(SqlQuery, "DataSet1", "Attendance_Date.rdlc");
        }

        private void Attendance_Total_MouseDown(object sender, RoutedEventArgs e)
        {
            string SqlQuery = $"select Distinct a.UserName, TimeCalc.OnTime as [OnTime], TimeCalc.OfTime as [OfTime] from Attendance as a INNER JOIN (SELECT UserName,CONVERT(Time, DATEADD(s, SUM(( DATEPART(hh, OnTime) * 3600 ) + ( DATEPART(mi, OnTime) * 60 ) + DATEPART(ss, OnTime)), 0)) AS [OnTime],CONVERT(Time, DATEADD(s, SUM(( DATEPART(hh, OfTime) * 3600 ) + ( DATEPART(mi, OfTime) * 60 ) + DATEPART(ss, OfTime)), 0)) AS [OfTime] FROM Attendance group by UserName) as TimeCalc on a.UserName=TimeCalc.UserName";
            RunReportViewer(SqlQuery, "DataSet1", "Attendance_Total.rdlc");
        }

        private void Projects_Days_MouseDown(object sender, RoutedEventArgs e)
        {
            string SqlQuery = $"SELECT ac.UserName, ac.Date, pr.ProjectName, CONVERT(Time, DATEADD(s, SUM(( DATEPART(hh, Time) * 3600 ) + ( DATEPART(mi, Time) * 60 ) + DATEPART(ss, Time)), 0)) AS [Time] FROM Activities as ac INNER JOIN dbo.projects as pr on ac.WindowsTitle like '%'+ pr.WindowsTitleContains +'%' group by ac.UserName,ac.date,pr.ProjectName";
            RunReportViewer(SqlQuery, "DataSet1", "Projects_Date.rdlc");
        }

        private void Projects_Total_MouseDown(object sender, RoutedEventArgs e)
        {
            string SqlQuery = $"SELECT ac.UserName,pr.ProjectName,CONVERT(Time, DATEADD(s, SUM(( DATEPART(hh, Time) * 3600 ) + ( DATEPART(mi, Time) * 60 ) + DATEPART(ss, Time)), 0)) AS [Time] FROM Activities as ac INNER JOIN dbo.projects as pr on ac.WindowsTitle like '%'+ pr.WindowsTitleContains +'%' group by ac.UserName,pr.ProjectName";
            RunReportViewer(SqlQuery, "DataSet1", "Projects_Total.rdlc");
        }

        //Show mouse click
        private void Show_Acitivities_Days_Click(object sender, RoutedEventArgs e)
        {
            string From = $"\'{From_Time_Days.SelectedDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}\'";
            string To = $"\'{To_Time_Days.SelectedDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}\'";

            string SqlQuery = $"Select UserName, Date, CONVERT(nvarchar(14), Time, 114) as [Time], WindowsTitle from Activities where Date between {From} and {To} order by Time DESC";
            RunReportViewer(SqlQuery, "DataSet1", "Activities_Date.rdlc");
        }

        private void Show_Acitivities_Total_Click(object sender, RoutedEventArgs e)
        {
            string From = $"\'{From_Time_Total.SelectedDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}\'";
            string To = $"\'{To_Time_Total.SelectedDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}\'";

            string SqlQuery = $"select Distinct a.UserName, A.WindowsTitle, TimeCalc.Time as [Time] from Activities as a INNER JOIN (SELECT WindowsTitle,Time FROM Activities where Date between {From} and {To} group by WindowsTitle,Time) as TimeCalc on a.WindowsTitle=TimeCalc.WindowsTitle where a.Date between {From} and {To} Order by Time DESC";
            RunReportViewer(SqlQuery, "DataSet1", "Activities_Total.rdlc");
        }

        private void Show_Attendance_Days_Click(object sender, RoutedEventArgs e)
        {
            string From = $"\'{Attendance_From_Time_Days.SelectedDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}\'";
            string To = $"\'{Attendance_To_Time_Days.SelectedDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}\'";

            string SqlQuery = $"Select UserName, Date, CONVERT(nvarchar(14), OnTime, 114) as [OnTime], CONVERT(nvarchar(14), OfTime, 114) as [OfTime] from Attendance where Date between {From} and {To}";
            RunReportViewer(SqlQuery, "DataSet1", "Attendance_Date.rdlc");
        }

        private void Show_Attendance_Total_Click(object sender, RoutedEventArgs e)
        {
            string From = $"\'{Attendance_From_Time_Total.SelectedDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}\'";
            string To = $"\'{Attendance_To_Time_Total.SelectedDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}\'";

            string SqlQuery = $"select Distinct a.UserName, TimeCalc.OnTime as [OnTime], TimeCalc.OfTime as [OfTime] from Attendance as a INNER JOIN (SELECT UserName,OnTime,OfTime FROM Attendance where Date between {From} and {To} group by UserName,OnTime,OfTime) as TimeCalc on a.UserName=TimeCalc.UserName where Date between {From} and {To}";
            RunReportViewer(SqlQuery, "DataSet1", "Attendance_Total.rdlc");
        }

        private void Show_Projects_Days_Click(object sender, RoutedEventArgs e)
        {
            string From = $"\'{Projects_From_Time_Days.SelectedDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}\'";
            string To = $"\'{Projects_To_Time_Days.SelectedDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}\'";

            string SqlQuery = $"SELECT ac.UserName, ac.Date,pr.ProjectName,Time FROM Activities as ac INNER JOIN dbo.projects as pr on ac.WindowsTitle like '%'+ pr.WindowsTitleContains +'%' where Date between {From} and {To} group by ac.UserName,ac.date,pr.ProjectName,Time";
            RunReportViewer(SqlQuery, "DataSet1", "Projects_Date.rdlc");
        }

        private void Show_Projects_Total_Click(object sender, RoutedEventArgs e)
        {
            string From = $"\'{Projects_From_Time_Total.SelectedDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}\'";
            string To = $"\'{Projects_To_Time_Total.SelectedDate.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}\'";

            string SqlQuery = $"SELECT ac.UserName,pr.ProjectName,Time FROM Activities as ac INNER JOIN dbo.projects as pr on ac.WindowsTitle like '%'+ pr.WindowsTitleContains +'%' where Date between {From} and {To} group by ac.UserName,pr.ProjectName,Time";
            RunReportViewer(SqlQuery, "DataSet1", "Projects_Total.rdlc");
        }
    }
}

