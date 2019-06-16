using FantasyCricket.Database;
using FantasyCricket.Models;
using System;
using System.Data.SQLite;

namespace FantasyCricket.Service
{
    public class Gangs : IGangs
    {

        private readonly string SELECTGANGS = "SELECT * FROM [Gangs] where name in @owner OR ";

        private readonly string CREATEGANG = "INSERT INTO [Gangs] (  name, owner, seriesname) VALUES (  @name, @owner, @seriesname)";

        private readonly string CREATEGANGUSERMAP = "INSERT INTO [GangUserMap] (  username, name) VALUES (  @username, @name)";


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
                    command.Parameters.AddWithValue("@seriesname", gang.SeriesName);

                    command.ExecuteNonQuery();
                }
                using (SQLiteCommand command = new SQLiteCommand(CREATEGANGUSERMAP, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@username", gang.GangOwner);
                    command.Parameters.AddWithValue("@name", gang.GangName);

                    command.ExecuteNonQuery();
                }
            }
        }

        public Gang[] GetGangs(string username)
        {
            throw new NotImplementedException();
        }
    }
}
