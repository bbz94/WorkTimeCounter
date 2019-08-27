USE [Development]
GO

/****** Object:  Table [dbo].[Activities]    Script Date: 8/27/2019 11:25:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Activities](
	[UserName] [nchar](10) NULL,
	[Date] [date] NULL,
	[Time] [time](7) NULL,
	[WindowsTitle] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
