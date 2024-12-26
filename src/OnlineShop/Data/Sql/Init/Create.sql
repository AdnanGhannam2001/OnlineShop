CREATE TABLE [AppUsers] (
    [Id]           VARCHAR(25) PRIMARY KEY,
    [Username]     VARCHAR(50) UNIQUE,
    [Role]         INT,
    [PasswordHash] VARBINARY(32) UNIQUE,
    [CreatedAt]    DATETIME DEFAULT GETDATE(),
);

CREATE TABLE [Categories] (
    [Id]        VARCHAR(25) PRIMARY KEY,
    [Label]     VARCHAR(50) UNIQUE,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
);

CREATE TABLE [Products] (
    [Id]        VARCHAR(25) PRIMARY KEY,
    [Name]      VARCHAR(50) UNIQUE,
    [Quantity]  INT,
    [Cost]      DECIMAL(10, 2),
    [Discount]  TINYINT CHECK (Discount >= 0 AND Discount <= 100),
    [Details]   NVARCHAR(MAX),
    [CreatedAt] DATETIME DEFAULT GETDATE(),
);

CREATE TABLE [Orders] (
    [Id]        VARCHAR(25) PRIMARY KEY,
    [UserId]    VARCHAR(25),
    [OrderedAt] DATETIME DEFAULT GETDATE(),
    FOREIGN KEY ([UserId]) REFERENCES [AppUsers]([Id]),
);

CREATE TABLE [OrdersProducts] (
    [OrderId]   VARCHAR(25),
    [ProductId] VARCHAR(25),
    PRIMARY KEY ([OrderId], [ProductId]),
    FOREIGN KEY ([OrderId])   REFERENCES [Orders]([Id]),
    FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id]),
);

-- Carts
CREATE TABLE [UsersProducts] (
    [UserId]    VARCHAR(25),
    [ProductId] VARCHAR(25),
    [AddedAt]   DATETIME DEFAULT GETDATE(),
    PRIMARY KEY ([UserId], [ProductId]),
    FOREIGN KEY ([UserId])    REFERENCES [AppUsers]([Id]),
    FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id]),
);
