USE [Development]
GO

/****** Object:  Table [dbo].[Attendance]    Script Date: 8/27/2019 11:26:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Attendance](
	[UserName] [nchar](10) NULL,
	[Date] [date] NULL,
	[OnTime] [time](7) NULL,
	[OfTime] [time](7) NULL
) ON [PRIMARY]
GO
