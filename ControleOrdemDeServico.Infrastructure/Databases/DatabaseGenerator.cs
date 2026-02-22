using Dapper;

namespace OsService.Infrastructure.Databases;

public sealed class DatabaseGenerator(
    IAdminSqlConnectionFactory factory,
    IDefaultSqlConnectionFactory defaultFactory)
{
    private const string CreateDbSql = @"
IF DB_ID(N'OsServiceDb') IS NULL
BEGIN
    CREATE DATABASE OsServiceDb;
END;";

    private const string CreateTablesSql = """
-- Customers
IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Customers (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL,
        Phone NVARCHAR(30) NULL,
        Email NVARCHAR(120) NULL,
        Document NVARCHAR(30) NULL,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2 NULL
    );

    CREATE INDEX IX_Customers_Phone ON dbo.Customers(Phone);
    CREATE INDEX IX_Customers_Email ON dbo.Customers(Email);
    CREATE INDEX IX_Customers_Document ON dbo.Customers(Document);
    CREATE INDEX IX_Customers_IsDeleted ON dbo.Customers(IsDeleted);
END;

-- ServiceOrders
IF OBJECT_ID(N'dbo.ServiceOrders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceOrders (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Number INT IDENTITY(1000, 1) NOT NULL,
        CustomerId UNIQUEIDENTIFIER NOT NULL,
        Description NVARCHAR(500) NOT NULL,
        Status INT NOT NULL,
        OpenedAt DATETIME2 NOT NULL,
        StartedAt DATETIME2 NULL,
        FinishedAt DATETIME2 NULL,
        Price DECIMAL(18, 2) NULL,
        Coin VARCHAR(3) NULL,
        UpdatedPriceAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        DeletedAt DATETIME2 NULL,
        CONSTRAINT FK_ServiceOrders_Customers
            FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id)
    );

    CREATE UNIQUE INDEX UX_ServiceOrders_Number ON dbo.ServiceOrders(Number);
    CREATE INDEX IX_ServiceOrders_CustomerId ON dbo.ServiceOrders(CustomerId);
    CREATE INDEX IX_ServiceOrders_Status ON dbo.ServiceOrders(Status);
    CREATE INDEX IX_ServiceOrders_IsDeleted ON dbo.ServiceOrders(IsDeleted);
END;

-- AuditLogs (novo)
IF OBJECT_ID(N'dbo.AuditLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditLogs (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Entity NVARCHAR(100) NOT NULL,
        EntityId UNIQUEIDENTIFIER NULL,
        Action NVARCHAR(100) NOT NULL,
        Data NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL
    );

    CREATE INDEX IX_AuditLogs_Entity ON dbo.AuditLogs(Entity);
    CREATE INDEX IX_AuditLogs_CreatedAt ON dbo.AuditLogs(CreatedAt);
END;
""";

    public async Task EnsureCreatedAsync(CancellationToken cancellationToken)
    {
        using (var adminConnection = factory.Create())
        {
            await adminConnection.ExecuteAsync(
                new CommandDefinition(CreateDbSql, cancellationToken: cancellationToken));
        }

        using var defaultConnection = defaultFactory.Create();
        await defaultConnection.ExecuteAsync(
            new CommandDefinition(CreateTablesSql, cancellationToken: cancellationToken));
    }
}