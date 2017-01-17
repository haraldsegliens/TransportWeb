
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/17/2017 17:53:30
-- Generated from EDMX file: C:\Users\GAME\documents\visual studio 2015\Projects\TransportWeb\TransportWeb\Models\TransportWeb_DataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [TransportWeb_Database];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_RouteTransport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Routes] DROP CONSTRAINT [FK_RouteTransport];
GO
IF OBJECT_ID(N'[dbo].[FK_Route_SegmentsStop]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Route_Segments] DROP CONSTRAINT [FK_Route_SegmentsStop];
GO
IF OBJECT_ID(N'[dbo].[FK_StopTimetable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Timetables] DROP CONSTRAINT [FK_StopTimetable];
GO
IF OBJECT_ID(N'[dbo].[FK_RouteTimetable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Timetables] DROP CONSTRAINT [FK_RouteTimetable];
GO
IF OBJECT_ID(N'[dbo].[FK_Route_SegmentsRoute]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Route_Segments] DROP CONSTRAINT [FK_Route_SegmentsRoute];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Transport]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Transport];
GO
IF OBJECT_ID(N'[dbo].[Routes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Routes];
GO
IF OBJECT_ID(N'[dbo].[Route_Segments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Route_Segments];
GO
IF OBJECT_ID(N'[dbo].[Timetables]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Timetables];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[Stops]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Stops];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Transports'
CREATE TABLE [dbo].[Transports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Number] int  NOT NULL
);
GO

-- Creating table 'Routes'
CREATE TABLE [dbo].[Routes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [T_Id] int  NOT NULL
);
GO

-- Creating table 'Route_Segments'
CREATE TABLE [dbo].[Route_Segments] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Order] int  NOT NULL,
    [R_Id] int  NOT NULL,
    [S_Id] int  NOT NULL
);
GO

-- Creating table 'Timetables'
CREATE TABLE [dbo].[Timetables] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [S_Id] int  NOT NULL,
    [R_Id] int  NOT NULL,
    [Time] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Password_Cache] nvarchar(max)  NOT NULL,
    [Password_Salt] nvarchar(max)  NOT NULL,
    [Access] nvarchar(max)  NOT NULL,
    [Session] nvarchar(max)  NULL
);
GO

-- Creating table 'Stops'
CREATE TABLE [dbo].[Stops] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Transports'
ALTER TABLE [dbo].[Transports]
ADD CONSTRAINT [PK_Transports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Routes'
ALTER TABLE [dbo].[Routes]
ADD CONSTRAINT [PK_Routes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Route_Segments'
ALTER TABLE [dbo].[Route_Segments]
ADD CONSTRAINT [PK_Route_Segments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Timetables'
ALTER TABLE [dbo].[Timetables]
ADD CONSTRAINT [PK_Timetables]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Stops'
ALTER TABLE [dbo].[Stops]
ADD CONSTRAINT [PK_Stops]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [T_Id] in table 'Routes'
ALTER TABLE [dbo].[Routes]
ADD CONSTRAINT [FK_RouteTransport]
    FOREIGN KEY ([T_Id])
    REFERENCES [dbo].[Transports]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteTransport'
CREATE INDEX [IX_FK_RouteTransport]
ON [dbo].[Routes]
    ([T_Id]);
GO

-- Creating foreign key on [S_Id] in table 'Route_Segments'
ALTER TABLE [dbo].[Route_Segments]
ADD CONSTRAINT [FK_Route_SegmentsStop]
    FOREIGN KEY ([S_Id])
    REFERENCES [dbo].[Stops]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Route_SegmentsStop'
CREATE INDEX [IX_FK_Route_SegmentsStop]
ON [dbo].[Route_Segments]
    ([S_Id]);
GO

-- Creating foreign key on [S_Id] in table 'Timetables'
ALTER TABLE [dbo].[Timetables]
ADD CONSTRAINT [FK_StopTimetable]
    FOREIGN KEY ([S_Id])
    REFERENCES [dbo].[Stops]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StopTimetable'
CREATE INDEX [IX_FK_StopTimetable]
ON [dbo].[Timetables]
    ([S_Id]);
GO

-- Creating foreign key on [R_Id] in table 'Timetables'
ALTER TABLE [dbo].[Timetables]
ADD CONSTRAINT [FK_RouteTimetable]
    FOREIGN KEY ([R_Id])
    REFERENCES [dbo].[Routes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RouteTimetable'
CREATE INDEX [IX_FK_RouteTimetable]
ON [dbo].[Timetables]
    ([R_Id]);
GO

-- Creating foreign key on [R_Id] in table 'Route_Segments'
ALTER TABLE [dbo].[Route_Segments]
ADD CONSTRAINT [FK_Route_SegmentsRoute]
    FOREIGN KEY ([R_Id])
    REFERENCES [dbo].[Routes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Route_SegmentsRoute'
CREATE INDEX [IX_FK_Route_SegmentsRoute]
ON [dbo].[Route_Segments]
    ([R_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------