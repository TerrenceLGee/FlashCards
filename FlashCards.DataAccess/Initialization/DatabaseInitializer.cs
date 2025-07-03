using FlashCards.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;

namespace FlashCards.DataAccess.Initialization;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly string? _masterConn;
    private readonly string? _dbConn;
    private readonly string _dbName = "FlashcardsDb";
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
    {
        _masterConn = configuration.GetConnectionString("Master")
                      ?? throw new InvalidOperationException("Master connection string not found");
        _dbConn = configuration.GetConnectionString("FlashcardsDb")
                  ?? throw new InvalidOperationException("FlashcardsDb connection string not found");
        _logger = logger;
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            await using (var connection = new SqlConnection(_masterConn))
            {
                await connection.OpenAsync();
                var databaseCreationStatement =
                    $@"IF DB_ID(N'{_dbName}') IS NULL 
                        CREATE DATABASE [{_dbName}];";

                await connection.ExecuteAsync(databaseCreationStatement).ConfigureAwait(false);
            }
        }
        catch (SqlException ex)
        {
            _logger.LogCritical(ex, ex.Message);
        }

        try
        {
            await using (var connection = new SqlConnection(_dbConn))
            {
                await connection.OpenAsync();

                var createTableCommand =
                    @"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Stacks')
                    BEGIN 
                        CREATE TABLE dbo.Stacks (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Name NVARCHAR(200) NOT NULL UNIQUE);
                    END";

                await connection.ExecuteAsync(createTableCommand).ConfigureAwait(false);

                createTableCommand =
                    @"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Flashcards') 
                    BEGIN
                        CREATE TABLE dbo.Flashcards (
                            Id          INT IDENTITY(1,1) PRIMARY KEY,
                            StackId     INT NOT NULL,
                            Front       NVARCHAR(500) NOT NULL,
                            Back        NVARCHAR(500) NOT NULL,
                            Position    INT NOT NULL,
                            CONSTRAINT FK_Flashcards_Stacks FOREIGN KEY (StackId) 
                            REFERENCES dbo.Stacks(Id) ON DELETE CASCADE);
                    END";

                await connection.ExecuteAsync(createTableCommand).ConfigureAwait(false);

                createTableCommand =
                    @"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'StudySessions') 
                    BEGIN 
                        CREATE TABLE dbo.StudySessions (
                            Id          INT IDENTITY(1,1) PRIMARY KEY, 
                            StackId     INT NOT NULL, 
                            Date        DATETIME2 NOT NULL, 
                            Score       INT NOT NULL, 
                            CONSTRAINT FK_Sessions_Stacks FOREIGN KEY (StackId) 
                            REFERENCES dbo.Stacks(Id) ON DELETE CASCADE);
                    END";

                await connection.ExecuteAsync(createTableCommand).ConfigureAwait(false);
            }
        }
        catch (SqlException ex)
        {
            _logger.LogCritical(ex, ex.Message);
        }
    }
}