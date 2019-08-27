USE [Development]
GO

/****** Object:  Table [dbo].[Projects]    Script Date: 8/27/2019 11:27:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Projects](
	[ProjectName] [nvarchar](max) NULL,
	[WindowsTitleContains] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO