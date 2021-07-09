create DataBase ZIGITDB
go
use [ZIGITDB]
create table [ZIGITDB].[dbo].[Teams] (ID uniqueidentifier primary key, sName varchar(max))
create table [ZIGITDB].[dbo].[Users] (ID uniqueidentifier primary key, sEmail varchar(max), sPassword varchar(max), sName varchar(max), enrollDate datetime, sAvatar varchar(max), teamID uniqueidentifier,
Constraint FK_UserTeam FOREIGN KEY (teamID) REFERENCES Teams(ID))
create table [ZIGITDB].[dbo].[Projects] (ID uniqueidentifier primary key, sName varchar(max), grade int, numOfBugs int, startDate datetime, endDate datetime, userID uniqueidentifier,
Constraint FK_ProjectUser FOREIGN KEY (userID) REFERENCES Users(ID))
create table [ZIGITDB].[dbo].[Tokens] (ID uniqueidentifier primary key, userID uniqueidentifier,
Constraint FK_TokenUser FOREIGN KEY (userID) REFERENCES Users(ID))

  insert into [ZIGITDB].[dbo].[Teams] (ID, sName) values (NEWID(), 'DC')
  insert into [ZIGITDB].[dbo].[Teams] (ID, sName) values (NEWID(), 'Marvel')

  declare @dcId uniqueidentifier
  select @dcId = ID from Teams where sName='DC'
  declare @marvelId uniqueidentifier
  select @marvelId = ID from Teams where sName='Marvel'
  insert into [ZIGITDB].[dbo].[Users] (ID, sEmail, sPassword, sName, enrollDate, sAvatar, teamID) values
  (NEWID(), 'IronMan@gmail.com', 'IamIronMan1', 'Tony', '19630301 12:00:00 PM', 'https://www.pinterest.com/pin/590182726152538930/', @marvelId)
  insert into [ZIGITDB].[dbo].[Users] (ID, sEmail, sPassword, sName, enrollDate, sAvatar, teamID) values
  (NEWID(), 'BlackWidow@gmail.com', 'IAmSpy1', 'Natasha', '19640301 12:00:00 PM', 'https://iconarchive.com/show/superhero-avatar-icons-by-hopstarter/Avengers-Black-Widow-icon.html', @marvelId)
  insert into [ZIGITDB].[dbo].[Users] (ID, sEmail, sPassword, sName, enrollDate, sAvatar, teamID) values
  (NEWID(), 'Superman@gmail.com', 'IamStrong1', 'Clark', '19380701 12:00:00 PM', 'https://avatars.alphacoders.com/avatars/view/123472', @dcId)
  insert into [ZIGITDB].[dbo].[Users] (ID, sEmail, sPassword, sName, enrollDate, sAvatar, teamID) values
  (NEWID(), 'BatMan@gmail.com', 'IamBat1', 'Bruce', '19390501 12:00:00 PM', 'https://icon-icons.com/icon/batman-avatar/90804', @dcId)

  declare @IronManID uniqueidentifier
  select @IronManID = ID from Users where sName='Tony'
  declare @BlackWidowID uniqueidentifier
  select @BlackWidowID = ID from Users where sName='Natasha'
  declare @SuperManID uniqueidentifier
  select @SuperManID = ID from Users where sName='Clark'
  declare @BatManID uniqueidentifier
  select @BatManID = ID from Users where sName='Bruce'
  insert into Projects (ID ,sName , grade , numOfBugs , startDate , endDate , userID ) values
  (NEWID(), 'IronMan1Movie', '100', 0, '20080501 12:00:00 PM', '20190422 12:00:00 PM', @IronManID)
  insert into Projects (ID ,sName , grade , numOfBugs , startDate , endDate , userID ) values
  (NEWID(), 'IronMan2Movie', '50', 300, '20100429 12:00:00 PM', '20130422 12:00:00 PM', @IronManID)
  insert into Projects (ID ,sName , grade , numOfBugs , startDate , endDate , userID ) values
  (NEWID(), 'Superman1Movie', '90', 5, '19781210 12:00:00 PM', '20300101 12:00:00 PM', @SuperManID)
  insert into Projects (ID ,sName , grade , numOfBugs , startDate , endDate , userID ) values
  (NEWID(), 'Superman2Movie', '70', 30, '19801204 12:00:00 PM', '20250101 12:00:00 PM', @SuperManID)
    insert into Projects (ID ,sName , grade , numOfBugs , startDate , endDate , userID ) values
  (NEWID(), 'Superman3Movie', '45', 340, '19830612 12:00:00 PM', '20000101 12:00:00 PM', @SuperManID)
  insert into Projects (ID ,sName , grade , numOfBugs , startDate , endDate , userID ) values
  (NEWID(), 'BatMan1Movie', '99', 1, '19890723 12:00:00 PM', '19900101 12:00:00 PM', @BatManID)
  insert into Projects (ID ,sName , grade , numOfBugs , startDate , endDate , userID ) values
  (NEWID(), 'BlackWidowMovie', '100', 0, '20210708 12:00:00 PM', '20211231 12:00:00 PM', @BlackWidowID)
  -- create store procedures:
-- This block of comments will not be included in
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_CheckIfUserExistsAndReturnValue	-- Add the parameters for the stored procedure here
	@email varchar(max), @password varchar(max)
AS
BEGIN
declare  @userInfo table (TokenID uniqueidentifier, UserID uniqueidentifier, sName varchar(max), enrollDate datetime, sAvatar varchar(max), teamName varchar(max))
insert into @userInfo 
	SELECT null
		,t1.ID
      ,t1.[sName]
      ,t1.[enrollDate]
      ,t1.[sAvatar]
	  ,t2.sName as teamName
 from [ZIGITDB].[dbo].[users] as t1 join [ZIGITDB].[dbo].[Teams] as t2 on t1.[teamID] = t2.id where t1.sEmail=@email and t1.sPassword=@password

 if ((select count(*) from @userInfo)>0)
 begin
 declare @userID uniqueIdentifier
 select @userID = UserID from @userInfo 
 declare @existingToken uniqueIdentifier = null
 
 declare @tokenID uniqueIdentifier = null
 select @tokenID = ID from Tokens where userID=@userID
 if (@tokenID is null)
 begin
 print 'a'
 set @tokenID = NEWID();
 insert into [ZIGITDB].[dbo].[Tokens] (ID, userID) values (@tokenID, @userID)
 end
 update @userInfo set TokenID=@tokenID
 end
 select [TokenID], [sName], format(cast([enrollDate] as date),'yyyy-MM-dd') as [enrollDate], [sAvatar], [teamName] from @userInfo
END
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetUserProjectsByTokenID
	-- Add the parameters for the stored procedure here
	@TokenID uniqueidentifier
AS
BEGIN
declare @result table (id uniqueidentifier, sname varchar(max), score int, durationInDays int, bugsCount int,endDate datetime)
insert into @result (id, sname, score, durationInDays,bugsCount,endDate)
	SELECT t1.[ID]
      ,t1.[sName]
      ,t1.[grade]
      ,DATEDIFF(day,t1.[startDate],t1.[endDate])
      ,t1.[numOfBugs],
	  t1.endDate
	  from Projects as t1 join Tokens as t2 on t1.userID = t2.userID and t2.ID = @TokenID
	 
	  
	  select id, sname, score, durationInDays,bugsCount,endDate from @result
END
GO
