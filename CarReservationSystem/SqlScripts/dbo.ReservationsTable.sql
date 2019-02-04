CREATE TABLE [dbo].[Reservations] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [UserId]   INT           NULL,
    [CarId]    INT           NULL,
    [Status]   VARCHAR (100) NOT NULL,
    [FromDate] DATE          NOT NULL,
    [ToDate]   DATE          NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]),
    FOREIGN KEY ([CarId]) REFERENCES [dbo].[Car] ([Id])
);