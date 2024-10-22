USE [master]
GO
/****** Object:  Database [SocialMediaProject]    Script Date: 25-08-2024 12:59:29 ******/
CREATE DATABASE [SocialMediaProject]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SocialMediaProject', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.CYNOSUREDBS\MSSQL\DATA\SocialMediaProject.mdf' , SIZE = 40960KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SocialMediaProject_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.CYNOSUREDBS\MSSQL\DATA\SocialMediaProject_log.ldf' , SIZE = 40960KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [SocialMediaProject] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SocialMediaProject].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SocialMediaProject] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SocialMediaProject] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SocialMediaProject] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SocialMediaProject] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SocialMediaProject] SET ARITHABORT OFF 
GO
ALTER DATABASE [SocialMediaProject] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SocialMediaProject] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SocialMediaProject] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SocialMediaProject] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SocialMediaProject] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SocialMediaProject] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SocialMediaProject] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SocialMediaProject] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SocialMediaProject] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SocialMediaProject] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SocialMediaProject] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SocialMediaProject] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SocialMediaProject] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SocialMediaProject] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SocialMediaProject] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SocialMediaProject] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SocialMediaProject] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SocialMediaProject] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SocialMediaProject] SET  MULTI_USER 
GO
ALTER DATABASE [SocialMediaProject] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SocialMediaProject] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SocialMediaProject] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SocialMediaProject] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [SocialMediaProject] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [SocialMediaProject] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [SocialMediaProject] SET QUERY_STORE = OFF
GO
USE [SocialMediaProject]
GO
/****** Object:  Table [dbo].[CreateAccount]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CreateAccount](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [varchar](50) NULL,
	[Username] [varchar](40) NULL,
	[Email] [varchar](50) NULL,
	[Mobile] [varchar](13) NULL,
	[DateOfBirth] [date] NULL,
	[NewPassword] [varchar](20) NULL,
	[ConfirmPassword] [varchar](20) NULL,
	[ProfilePhotoPath] [varchar](30) NULL,
	[ProfileBio] [varchar](400) NULL,
	[IsLoggedIn] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserComment]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserComment](
	[UserCommentId] [int] IDENTITY(1,1) NOT NULL,
	[PostId] [int] NULL,
	[PostCommentUsername] [varchar](40) NULL,
	[PostCommentId] [int] NULL,
	[PostAddId] [int] NULL,
	[PostAddUsername] [varchar](40) NULL,
	[CommentInput] [varchar](400) NULL,
	[LogDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserCommentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserFollow]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserFollow](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[UserName] [varchar](40) NULL,
	[FollowId] [int] NULL,
	[FollowUserName] [varchar](40) NULL,
	[LogDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserLike]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLike](
	[UserLikeId] [int] IDENTITY(1,1) NOT NULL,
	[PostId] [varchar](5) NULL,
	[PostUserName] [varchar](30) NULL,
	[LikeId] [varchar](5) NULL,
	[LikeUserName] [varchar](30) NULL,
	[LogDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserLikeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserPost]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPost](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[PhotoPath] [varchar](200) NOT NULL,
	[PostCaption] [varchar](400) NULL,
	[PostLike] [int] NULL,
	[PostComment] [int] NULL,
	[PostTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CreateAccount] ADD  DEFAULT ((0)) FOR [IsLoggedIn]
GO
ALTER TABLE [dbo].[UserComment] ADD  DEFAULT (getdate()) FOR [LogDate]
GO
ALTER TABLE [dbo].[UserFollow] ADD  DEFAULT (getdate()) FOR [LogDate]
GO
ALTER TABLE [dbo].[UserLike] ADD  DEFAULT (getdate()) FOR [LogDate]
GO
ALTER TABLE [dbo].[UserPost] ADD  DEFAULT ('') FOR [PostTime]
GO
/****** Object:  StoredProcedure [dbo].[UpdateProfile]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[UpdateProfile]  
(  
@Id     Int,  
@ProfilePhotoPath   VARCHAR(200) = '',   
@Bio                VARCHAR(400) = '',
@DateOfBirth        Date = NULL ,  
@FullName           VARCHAR(50)  = '',  
@Mobile             VARCHAR(50) = '',
@Message            VARCHAR(50) Output  
)  
AS   
 BEGIN   
   SET NOCOUNT ON;  
   Update CreateAccount  
   Set ProfilePhotoPath = @ProfilePhotoPath,  
   ProfileBio   = @Bio,  
   FullName    = @FullName,  
   Mobile    = @Mobile,  
   DateOfBirth   = @DateOfBirth  
   Where Id = @Id  
   SET @Message = 'Account updated successfully.';  
 END   
  
GO
/****** Object:  StoredProcedure [dbo].[usp_CommentGet]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[usp_CommentGet]  
(  
@Mode					INT = 0,  
@PostId					INT = 0,  
@PostCommentUsername    VARCHAR(40) = '',  
@PostCommentId          INT = 0,  
@PostAddId				INT = 0,  
@PostAddUsername        VARCHAR(40) = '',  
@CommentInput           VARCHAR(400)= ''  
)  
as   
  BEGIN    
    If(@Mode = 1)  
		BEGIN   
			INSERT INTO UserComment (PostId, PostCommentUsername, PostCommentId,PostAddId,PostAddUsername,CommentInput )  
			VALUES (@PostId, @PostCommentUsername, @PostCommentId, @PostAddId, @PostAddUsername,@CommentInput)  
		END  
      If (@Mode=2)
		BEGIN
		Select * from UserComment where PostId = @PostId
		END
  END
GO
/****** Object:  StoredProcedure [dbo].[usp_FollowGet]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_FollowGet]       
(         
    @Mode        INT = 0,      
    @FollowId       INT = 0,       
    @FollowUserName      VARCHAR(40) = '',      
    @UserId        INT = 0,      
    @Username       VARCHAR(40) = '',      
 @FollowingId      INT = 0      
)      
AS       
BEGIN       
    SET NOCOUNT ON;      
    IF(@Mode = 1)      
    BEGIN      
        IF EXISTS (SELECT 1 FROM UserFollow WHERE UserId = @UserId AND FollowId = @FollowId)      
        BEGIN      
            SELECT 1 AS IsFollowing;      
        END      
        ELSE      
        BEGIN      
            SELECT 0 AS IsFollowing;      
        END      
    END      
      
    IF(@Mode = 2)      
    BEGIN      
        IF NOT EXISTS (SELECT 1 FROM UserFollow WHERE UserId = @UserId AND FollowId = @FollowId)      
        BEGIN      
            INSERT INTO UserFollow (UserId, FollowId, FollowUserName, Username)      
            VALUES (@UserId, @FollowId, @FollowUserName, @Username);      
      
            SELECT 1 AS IsFollowing;      
        END      
        ELSE      
        BEGIN      
            SELECT 1 AS IsFollowing;      
        END      
    END      
 --To count the Follower List here      
      IF (@Mode = 3)       
      BEGIN        
      SELECT         
        (SELECT COUNT(UserId) FROM UserFollow WHERE UserId = @FollowingId) AS FollowCount,                
        (SELECT COUNT(FollowId) FROM UserFollow WHERE FollowId = @FollowingId) AS FollowingCount,    
  (SELECT COUNT(UserId) from UserPost where UserId = @FollowingId) AS PostCount;    
   END    
      IF (@Mode = 4)  
  BEGIN  
        Delete From UserFollow where UserId = @UserId and FollowId = @FollowId  
  END  
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetUserDetails]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_GetUserDetails]      
(      
@Id             INT,       
@Mode           INT = 0      
)      
AS       
   BEGIN       
    IF @Mode=1      
    BEGIN       
     SELECT Id, FullName, Username, Email, DateOfBirth, ProfileBio, ProfilePhotoPath, Mobile FROM CreateAccount WHERE Id = @Id      
    
   END      
    ELSE IF       
   @Mode=2       
     BEGIN       
   SELECT * FROM UserPost WHERE UserId = @Id       
  END      
  
 END   
  
  
  
GO
/****** Object:  StoredProcedure [dbo].[usp_GoogleLogin]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_GoogleLogin]
(
    @Mode               INT,
    @Id                 INT = 0,
    @Name               VARCHAR(40) = '',
    @Email              VARCHAR(50) = '',
    @Mobile             VARCHAR(13) = '',
    @Username           VARCHAR(20) = '',
    @DateOfBirth        DATE = NULL,
    @ProfilePhotoPath   VARCHAR(200) = '',
    @ProfileBio         VARCHAR(400) = '',
    @UserId             INT OUTPUT,
    @Message            VARCHAR(255) OUTPUT,
    @UsernameLogin      VARCHAR(50) OUTPUT
)
AS
BEGIN
    SET @Message = '';
    SET @UserId = 0;
    SET @UsernameLogin = '';

    IF @Mode = 1
    BEGIN
        -- Check if the email already exists
        IF EXISTS (SELECT 1 FROM CreateAccount WHERE Email = @Email)
        BEGIN
            -- Retrieve the existing user details
            SELECT @UserId = Id, @UsernameLogin = Username
            FROM CreateAccount
            WHERE Email = @Email;

            SET @Message = 'Email already exists.';
            RETURN;
        END
        
        DECLARE @TempAccountId TABLE (Id INT);
        
        INSERT INTO CreateAccount (FullName, Email)
        OUTPUT inserted.Id INTO @TempAccountId(Id)
        VALUES (@Name, @Email);
        
        SELECT @UserId = Id FROM @TempAccountId;
        
        IF @UserId IS NOT NULL
        BEGIN
            SET @Message = 'Account Created and Login Successfully, Please update the Profile.';
        END
        ELSE
        BEGIN
            SET @Message = 'Account creation failed.';
        END
    END
    
    IF @Mode = 2
    BEGIN
        UPDATE CreateAccount
        SET
            Mobile = @Mobile,
            Username = @Username,
            DateOfBirth = @DateOfBirth,
            FullName = @Name,
            ProfileBio = @ProfileBio,
            ProfilePhotoPath = @ProfilePhotoPath
        WHERE Id = @Id;
        
        IF EXISTS (SELECT 1 FROM CreateAccount WHERE Username = @Username AND Id <> @Id)
        BEGIN
            UPDATE CreateAccount
            SET Username = ''
            WHERE Id = @Id;
            
            SET @Message = 'Username not available, Please input a unique username.';
        END
        ELSE
        BEGIN
            SET @Message = 'Profile updated successfully.';
        END
    END
    
    IF @Mode = 3
    BEGIN
        SELECT * FROM CreateAccount WHERE Id = @Id;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_loginVerify]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_loginVerify]
(
    @Username        VARCHAR(20),
    @Password        VARCHAR(20),
    @UserId          INT OUTPUT,
    @ErrorMessage    NVARCHAR(100) OUTPUT
)
AS
BEGIN
    SET @UserId = NULL;
    SET @ErrorMessage = NULL;

    SELECT @UserId = Id
    FROM CreateAccount
    WHERE Username = @Username AND ConfirmPassword = @Password;

    IF @UserId IS NULL
    BEGIN
        SET @ErrorMessage = 'Incorrect username or password';
    END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SignUp]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_SignUp]  
(     
    @Mode               INT = 0,  
    @Id                 INT = 0,  
    @Fullname           VARCHAR(40) = NULL,  
    @Username           VARCHAR(40) = NULL,  
    @Email              VARCHAR(50) = NULL,  
    @Mobile             VARCHAR(50) = NULL,  
    @NewPassword        VARCHAR(20) = NULL,  
    @ConfirmPassword    VARCHAR(20) = NULL,  
    @Message            NVARCHAR(200) OUTPUT,  
    @AccountId          INT OUTPUT,  
    @ProfilePhotoPath   VARCHAR(200) = NULL,   
    @Bio                VARCHAR(400) = NULL,  
    @DateOfBirth        Date = NULL,
	@FCMToken          VARCHAR(300) = ''
)  
AS  
BEGIN  
    SET NOCOUNT ON;  
      
    IF @Mode = 1  
    BEGIN  
        IF EXISTS (SELECT 1 FROM CreateAccount WHERE Email = @Email)  
        BEGIN  
            SET @Message = 'Email already exists.';  
            RETURN;  
        END  
  
        IF EXISTS (SELECT 1 FROM CreateAccount WHERE Username = @Username)  
        BEGIN  
            SET @Message = 'Username already exists.';  
            RETURN;  
        END  
  
        DECLARE @TempAccountId TABLE (Id INT);  
        INSERT INTO CreateAccount (FullName, Username, Email, Mobile, NewPassword, ConfirmPassword,  
                                   ProfilePhotoPath, ProfileBio, DateOfBirth, FCMToken)  
        OUTPUT inserted.Id INTO @TempAccountId(Id)   
        VALUES (@Fullname, @Username, @Email, @Mobile, @NewPassword, @ConfirmPassword,  
                @ProfilePhotoPath, @Bio, @DateOfBirth, @FCMToken);  
  
        SELECT @AccountId = Id FROM @TempAccountId;  
        SET @Message = 'Account created successfully.';  
    END  
  
    IF @Mode = 2  
    BEGIN  
        UPDATE CreateAccount  
        SET ProfilePhotoPath = @ProfilePhotoPath,  
            ProfileBio = @Bio,  
            DateOfBirth = @DateOfBirth  
        WHERE Id = @Id;  
  SET @AccountId = @Id;  
        SET @Message = 'Details added successfully.';  
    END  
END  
GO
/****** Object:  StoredProcedure [dbo].[usp_UserLikeGet]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[usp_UserLikeGet] 
(
 @Mode				   INT = 0,
 @PostId			   VARCHAR(5) = '',
 @PostUserName		   VARCHAR(30) = '',
 @LikeId			   VARCHAR(5) = '',
 @LikeUserName		   VARCHAR(30) = '' 
 )
 AS
 BEGIN 
 SET NOCOUNT ON;
 --To check Post is Alrealy liked or not
  IF(@Mode = 1)
   BEGIN
   IF EXISTS (SELECT 1 FROM UserLike WHERE PostId = @PostId and LikeId = @LikeId)
      BEGIN
				 SELECT 1 AS ISLIKED;
      END
   ELSE
	  BEGIN
				 SELECT  0 AS ISLIKED;
      END
  END
     
	 --If post is not liked before then Like the Post in  the Mode = 2 and Insert the data successfully 
	 IF(@Mode = 2)
	 BEGIN
			IF NOT EXISTS (SELECT 1 FROM UserLike WHERE PostId = @PostId and LikeId = @LikeId)
				BEGIN
				  INSERT INTO UserLike(PostId, PostUserName ,LikeId, LikeUserName) 
				  Values(@PostId, @PostUserName, @LikeId, @LikeUserName)

				  SELECT 1 AS ISLIKED;
				END
			ELSE
			   BEGIN 
			     SELECT 1 AS ISLIKED;
			   END
	 END
	 --Mode 3 to Unlike the Post here
	 If(@Mode=3)
		BEGIN
		Delete from UserLike Where LikeId = @LikeId and PostId = @PostId
		END
  END
GO
/****** Object:  StoredProcedure [dbo].[usp_UserPost]    Script Date: 25-08-2024 12:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_UserPost]        
(        
    @Id             INT = 0,        
    @Photopath      VARCHAR(200)= '',        
    @PostCaption    VARCHAR(400) = '',        
    @Mode           INT = 0,        
    @Search         VARCHAR(50) = ''        
)        
AS         
BEGIN         
    IF @Mode = 1         
    BEGIN        
        INSERT INTO UserPost (UserId, PhotoPath, PostCaption)        
        VALUES (@Id, @Photopath, @PostCaption);        
    END        
    ELSE IF @Mode = 2         
    BEGIN        
        SELECT *         
        FROM CreateAccount        
        WHERE (@Search = ''         
               OR Username LIKE '%' + @Search + '%'         
               OR FullName LIKE '%' + @Search + '%');        
    END        
 ELSE IF @Mode = 3        
 BEGIN        
 SELECT         
    ca.FullName,        
    ca.Username,         
    ca.ProfilePhotoPath,         
    up.PhotoPath,        
    up.PostCaption,        
    up.UserId,        
 up.id        
FROM         
    Userpost AS up        
INNER JOIN         
    CreateAccount AS ca        
ON         
    up.UserId = ca.Id       
 ORDER BY 1 DESC        
 END        
 IF(@Mode=4)    
 BEGIN    
 Select * from UserPost where Id = @Id    
 END    
  IF (@Mode=5)  
  BEGIN  
  UPDATE USERPOST  
  SET PostCaption = @PostCaption ,   
  PhotoPath = @Photopath  
  where Id = @Id  
  END  
  IF (@Mode=6)
   BEGIN
   DELETE FROM UserPost WHERE Id = @Id
   END
    
END
GO
USE [master]
GO
ALTER DATABASE [SocialMediaProject] SET  READ_WRITE 
GO
