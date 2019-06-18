using FantasyCricket.Database;
using FantasyCricket.Models;
using Sqlite.SqlClient;
using System;
using System.Data.SQLite;

namespace FantasyCricket.Service
{
    public class Gangs : IGangs
    {

        private readonly string SELECTGANGS = "Select gangs.seriesid, gangs.gangid, gangs.name, gangs.owner, GangUserMap.username from Gangs INNER JOIN GangUserMap on GangUserMap.gangid = Gangs.gangid where seriesid = @seriesid and GangUserMap.gangid in (select gangid from GangUserMap where username = @username)";
        
        private readonly string CREATEGANG = "INSERT INTO [Gangs] (  name, owner, seriesid) VALUES (  @name, @owner, @seriesid)";

        private readonly string CREATEGANGUSERMAP = "INSERT INTO [GangUserMap] (  username, gangid) VALUES (  @username, @gangid)";

        private readonly string CREATEGANGUSERMAPDEFAULT = "INSERT INTO GangUserMap SELECT owner,gangid FROM Gangs where name = @name;";

        public void AddToGang(int gangid, string[] usernames)
        {
            string failedUsernames=String.Empty;
            foreach (string username in usernames)
            {
                using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
                {
                    connection.Open();
                    try
                    {
                        using (SQLiteCommand command = new SQLiteCommand(CREATEGANGUSERMAP, connection))
                        {
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.AddWithValue("@username", username);
                            command.Parameters.AddWithValue("@gangid", gangid);
                            command.ExecuteNonQuery();
                        }
                    }
                    catch(Exception exception)
                    {
                        failedUsernames += (username+",");
                    }
                }
            }

            if (!String.IsNullOrEmpty(failedUsernames))
            {
                throw new Exception($"Failed to add users:{failedUsernames} to gang");
            }

        }
        public void CreateGang(Gang gang)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(CREATEGANG, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@name", gang.GangName);
                    command.Parameters.AddWithValue("@owner", gang.GangOwner);
                    command.Parameters.AddWithValue("@seriesid", gang.SeriesId);

                    command.ExecuteNonQuery();
                }

                using (SQLiteCommand command = new SQLiteCommand(CREATEGANGUSERMAPDEFAULT, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@name", gang.GangName);

                    command.ExecuteNonQuery();
                }

            }
        }

        public Gang[] GetGangs(string username, int seriesid)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(SELECTGANGS, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@seriesid", seriesid);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        return reader.ReadAll<Gang>();
                    }
                }
            }
        }
    }
}
