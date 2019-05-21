using System;
using System.Data.SQLite;

namespace FantasyCricket.Database
{
    public class DatabaseSetup
    {


        public DatabaseSetup()
        {
            Console.WriteLine(string.Format("Sqlite Connection String={0}", GetConnectString()));
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(GetConnectString()))
                {
                    connection.Open();
                    CreateTables(connection);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        public static string GetConnectString()
        {
            SQLiteConnectionStringBuilder connectionStringBuilder = new SQLiteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "FantasyCricket.sqlite";
            connectionStringBuilder.Version = 3;
            connectionStringBuilder.DateTimeFormat = SQLiteDateFormats.ISO8601;
            connectionStringBuilder.DateTimeKind = DateTimeKind.Utc;
            connectionStringBuilder.JournalMode = SQLiteJournalModeEnum.Delete;
            connectionStringBuilder.FailIfMissing = false;

            return connectionStringBuilder.ToString();
        }
        public static void CreateTables(SQLiteConnection connection)
        {
            SQLiteCommand sqlite_cmd = connection.CreateCommand();

            // Create IPL Player Table
            sqlite_cmd.CommandText = @"CREATE TABLE IF NOT EXISTS
                                     [IplPlayer] (
                                     [Pid]     INTEGER NOT NULL PRIMARY KEY,
                                     [Pname]     VARCHAR(255) NOT NULL,
                                     [Cost]   INTEGER NULL,
                                     [Role]     VARCHAR(255) NULL,
                                     [Team]   VARCHAR(255) NOT NULL)";
            sqlite_cmd.ExecuteNonQuery();

            // Create Country Player Table
            sqlite_cmd.CommandText = @"CREATE TABLE IF NOT EXISTS
                                     [OdiPlayer] (
                                     [Pid]     INTEGER NOT NULL PRIMARY KEY,
                                     [Pname]     VARCHAR(255) NOT NULL,
                                     [Cost]   INTEGER NULL,
                                     [Role]     VARCHAR(255) NULL,
                                     [Team]   VARCHAR(255) NOT NULL)";
            sqlite_cmd.ExecuteNonQuery();

            // Create Series Table
            sqlite_cmd.CommandText = @"CREATE TABLE IF NOT EXISTS
                                     [Series] (
                                     [Seriesid]     INTEGER PRIMARY KEY AUTOINCREMENT,
                                     [Seriesname]     VARCHAR(255) NOT NULL UNIQUE)";
            sqlite_cmd.ExecuteNonQuery();

            // Create Matches Table
            sqlite_cmd.CommandText = @"CREATE TABLE IF NOT EXISTS
                                     [Match] (
                                     [unique_id]     INTEGER NOT NULL PRIMARY KEY,
                                     [MatchTime]     DATETIME NOT NULL,
                                     [Seriesid]     INTEGER,
                                     [Type] VARCHAR(255) NOT NULL,
                                      [team1]     VARCHAR(255) NOT NULL,  
                                      [team2]     VARCHAR(255) NOT NULL ,
                                      FOREIGN KEY(Seriesid) REFERENCES Series(Seriesid))";
            sqlite_cmd.ExecuteNonQuery();

            // Create User Table
            sqlite_cmd.CommandText = @"CREATE TABLE IF NOT EXISTS
                                     [User] (
                                     [user_id]     INTEGER PRIMARY KEY AUTOINCREMENT ,
                                     [username]     VARCHAR(255) NOT NULL UNIQUE,
                                      [password]     VARCHAR(255) NOT NULL,  
 [displayname]     VARCHAR(255) NOT NULL )";
            sqlite_cmd.ExecuteNonQuery();


        }
    }
}
