USE [master]
GO
/****** Object:  Database [SocialMediaProject]    Script Date: 01-08-2024 20:39:34 ******/
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
/****** Object:  Table [dbo].[CreateAccount]    Script Date: 01-08-2024 20:39:35 ******/
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
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserPost]    Script Date: 01-08-2024 20:39:35 ******/
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
ALTER TABLE [dbo].[UserPost] ADD  DEFAULT ('') FOR [PostTime]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetUserDetails]    Script Date: 01-08-2024 20:39:35 ******/
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
	    SELECT * FROM CreateAccount WHERE Id = @Id
	  END
    ELSE IF 
	  @Mode=2 
	    BEGIN 
		 SELECT * FROM UserPost WHERE UserId = @Id 
		END
	END
GO
/****** Object:  StoredProcedure [dbo].[usp_loginVerify]    Script Date: 01-08-2024 20:39:35 ******/
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
/****** Object:  StoredProcedure [dbo].[usp_SignUp]    Script Date: 01-08-2024 20:39:35 ******/
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
    @DateOfBirth        Date = NULL
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
                                   ProfilePhotoPath, ProfileBio, DateOfBirth)
        OUTPUT inserted.Id INTO @TempAccountId(Id) 
        VALUES (@Fullname, @Username, @Email, @Mobile, @NewPassword, @ConfirmPassword,
                @ProfilePhotoPath, @Bio, @DateOfBirth);

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
/****** Object:  StoredProcedure [dbo].[usp_UserPost]    Script Date: 01-08-2024 20:39:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_UserPost]
(
    @Id             INT = Null,
    @Photopath      VARCHAR(200)= Null,
    @PostCaption    VARCHAR(400) = NULL,
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
END
GO
USE [master]
GO
ALTER DATABASE [SocialMediaProject] SET  READ_WRITE 
GO
