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
            connectionStringBuilder.ForeignKeys = true;

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
                                     [Status]	VARCHAR(255) DEFAULT 0,
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
                                     [lastlogin]     DATETIME  ,
                                     [magickey]     GUID,
                                     [displayname]     VARCHAR(255) NOT NULL )";
            sqlite_cmd.ExecuteNonQuery();      

            sqlite_cmd.CommandText = @"CREATE TABLE IF NOT EXISTS UserTeam (
                                        username      VARCHAR(255) NOT NULL,
                                        lastteam VARCHAR(255) ,
                                        currentteam VARCHAR(255),
                                        lastremsub INTEGER DEFAULT 100,
                                        remsub INTEGER DEFAULT 100,
                                        FOREIGN KEY (username) REFERENCES User (username) 
                                        ON DELETE CASCADE ON UPDATE NO ACTION)";
            sqlite_cmd.ExecuteNonQuery();

            // Create UserTeam
            sqlite_cmd.CommandText = @"CREATE TABLE IF NOT EXISTS UserTeamPointsHistory (
                                        username      VARCHAR(255) NOT NULL,
                                        unique_id integer NOT NULL,
                                        selectedteam VARCHAR(255) ,
                                        points integer DEFAULT 0,
                                        PRIMARY KEY (username, unique_id),
                                        FOREIGN KEY (username) REFERENCES User (username) 
                                        ON DELETE CASCADE ON UPDATE NO ACTION,
                                        FOREIGN KEY (unique_id) REFERENCES Match (unique_id) 
                                        ON DELETE CASCADE ON UPDATE NO ACTION)";
            sqlite_cmd.ExecuteNonQuery();


            // Create Gang
            sqlite_cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Gangs (
                                        gangid     INTEGER PRIMARY KEY AUTOINCREMENT,
                                        name      VARCHAR(255) NOT NULL,
                                        owner      VARCHAR(255) NOT NULL,
                                        seriesid      integer NOT NULL,
                                        UNIQUE (name, owner)
                                        FOREIGN KEY (owner) REFERENCES User (username) 
                                        ON DELETE CASCADE ON UPDATE NO ACTION,
                                        FOREIGN KEY (seriesid) REFERENCES Series (Seriesid) 
                                        ON DELETE CASCADE ON UPDATE NO ACTION)";
            sqlite_cmd.ExecuteNonQuery();



            // Create GangUserMap
            sqlite_cmd.CommandText = @"CREATE TABLE IF NOT EXISTS GangUserMap (
                                        username      VARCHAR(255) NOT NULL,
                                        gangid      INTEGER NOT NULL,
                                        PRIMARY KEY (username, gangid),
                                        FOREIGN KEY (username) REFERENCES User (username) 
                                        ON DELETE CASCADE ON UPDATE NO ACTION,
                                        FOREIGN KEY (gangid) REFERENCES Gangs (gangid) 
                                        ON DELETE CASCADE ON UPDATE NO ACTION)";
            sqlite_cmd.ExecuteNonQuery();

            // Create GangUserMap
            sqlite_cmd.CommandText = @"CREATE TABLE IF NOT EXISTS MatchPointsHistory (
                                        unique_id integer NOT NULL,
                                        points      VARCHAR(4000) NOT NULL,
                                        PRIMARY KEY (unique_id),
                                        FOREIGN KEY (unique_id) REFERENCES Match (unique_id) 
                                        ON DELETE CASCADE ON UPDATE NO ACTION)";
            sqlite_cmd.ExecuteNonQuery();



        }
    }
}
