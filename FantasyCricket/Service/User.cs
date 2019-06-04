using FantasyCricket.Database;
using FantasyCricket.Models;
using Newtonsoft.Json;
using Sqlite.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace FantasyCricket.Service
{
    public class User : IUser
    {

        private readonly string ADDORUPDATEUSER = "INSERT INTO [User] (  username, password, displayname ,magickey) VALUES (  @username, @password, @displayname,@magickey)";

        private readonly string SELECTUSER = "SELECT magickey,lastlogin,username FROM [User] where username = @username and password = @password";

        private readonly string SELECTALLUSER = "SELECT magickey,lastlogin,username FROM [User]";

        private readonly string SELECTUSERBYGUID = "SELECT magickey,lastlogin,username FROM [User] where magickey=@magickey";

        private readonly string UPDATEUSERLOGINTIME = "UPDATE [User] SET lastlogin = @lastlogin WHERE username = @username and password = @password";

        private readonly string UPDATEUSERGUID = "UPDATE [User] SET magickey = @newmagickey WHERE username = @username";

        private readonly string ADDUNEWSERTEAM = "INSERT OR REPLACE INTO [UserTeam] (  username, currentteam ) VALUES (  @username,  @currentteam)";

        private readonly string UPDATECURRENTSERTEAM = "UPDATE [UserTeam] SET currentteam = @currentteam where username=@username";

        private readonly string UPDATECURRENTSERTEAMWITHSUBS = "UPDATE [UserTeam] SET currentteam = @currentteam , remsub = @remsub where username=@username";

        private readonly string GETLASTTEAM = "SELECT * FROM [UserTeam] where username=@username";

        private readonly string GETUSERTEAMHISTORY = "SELECT * FROM [UserTeamPointsHistory] where username=@username";


        private readonly object login = new object();

        private Timer UserMaintenanceTimer { get; set; }

        private const int UserMaintenanceTimerPeriod = 60000;  // once an minute

        public User()
        {
            LoadUserMaintenanceTimer();
        }

        public void CreateUser(string username, string password, string displayName)
        {

            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(ADDORUPDATEUSER, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@displayname", displayName);
                    command.Parameters.AddWithValue("@magickey", Guid.NewGuid().ToString());
                    command.ExecuteNonQuery();
                }
            }
        }
        public MagicKey LoginUser(string username, string password)
        {
            lock (login)
            {
                using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(SELECTUSER, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            MagicKey key = reader.Read<MagicKey>();
                            if (key == null || key.Magic == null || key.Magic == Guid.Empty)
                            {
                                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Yoy are trying to look at something you should not, RUN AWAY boy", "", null);
                            }
                            else
                            {

                                using (SQLiteCommand command1 = new SQLiteCommand(UPDATEUSERLOGINTIME, connection))
                                {
                                    command1.CommandType = System.Data.CommandType.Text;
                                    command1.Parameters.AddWithValue("@username", username);
                                    command1.Parameters.AddWithValue("@password", password);
                                    command1.Parameters.AddWithValue("@lastlogin", DateTime.UtcNow);
                                    command1.ExecuteNonQuery();
                                }

                                return key;
                            }
                        }
                    }
                }

            }
        }


        public void LoadUserMaintenanceTimer()
        {
            try
            {
                UserMaintenanceTimer = null;   // Ensure null initialization

                UserMaintenanceTimer = new Timer(new TimerCallback(UserMaintenanceTimerEvent), this, UserMaintenanceTimerPeriod, UserMaintenanceTimerPeriod);
            }
            catch (Exception exception)
            {
                Exception rethrow_Exception = new Exception(
                    string.Format("Failed to initialize the User maintenance timer. {0}", exception.Message)
                    );

                // Throw exception to DhcpStartService
                throw rethrow_Exception;
            }
        }

        private void UserMaintenanceTimerEvent(object state)
        {
            lock (login)
            {
                using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(SELECTALLUSER, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            MagicKey[] keys = reader.ReadAll<MagicKey>();
                            foreach (MagicKey key in keys)
                            {
                                if (key.LastLogin != null && (DateTime.UtcNow - key.LastLogin).TotalHours >= 1)
                                {
                                    using (SQLiteCommand command1 = new SQLiteCommand(UPDATEUSERGUID, connection))
                                    {
                                        command1.CommandType = System.Data.CommandType.Text;

                                        command1.Parameters.AddWithValue("@username", key.username);
                                        command1.Parameters.AddWithValue("@newmagickey", Guid.NewGuid().ToString());
                                        command1.ExecuteNonQuery();
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        public void SaveTeam(UserTeam userTeam, Guid magicKey)
        {
            string username;

            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(SELECTUSERBYGUID, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@magickey", magicKey.ToString());
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        MagicKey[] keys = reader.ReadAll<MagicKey>();
                        if (keys.Length != 1)
                        {
                            throw new Exception("Unauthorised access");
                        }
                        username = keys[0].username;
                    }
                }

                // get next match info

                using (SQLiteCommand command = new SQLiteCommand(GETLASTTEAM, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            // Get Column ordinal
                            int ordinal = reader.GetOrdinal("lastteam");

                            string lastteamstr = (reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal));

                            int subsOrdinal = reader.GetOrdinal("lastremsub");
                            int lastRemSubs = reader.GetInt32(subsOrdinal);

                            if (String.IsNullOrEmpty(lastteamstr))
                            {
                                // Saving for first time just update currentteam
                                using (SQLiteCommand commandAddUser = new SQLiteCommand(UPDATECURRENTSERTEAM, connection))
                                {
                                    commandAddUser.CommandType = System.Data.CommandType.Text;
                                    commandAddUser.Parameters.AddWithValue("@username", username);
                                    commandAddUser.Parameters.AddWithValue("@currentteam", JsonConvert.SerializeObject(userTeam));
                                    commandAddUser.ExecuteNonQuery();
                                }

                            }
                            else
                            {
                                UserTeam lastTeam = JsonConvert.DeserializeObject<UserTeam>(reader.GetString(ordinal));

                                int subUsed = 0;
                                //user already played last match
                                List<int> lastPlayers = new List<int>(lastTeam.PlayerIds);
                                foreach (int id in userTeam.PlayerIds)
                                {
                                    if (!lastPlayers.Contains(id))
                                    {
                                        subUsed += 1;
                                    }
                                }


                                if (lastRemSubs - subUsed >= 0)
                                {
                                    using (SQLiteCommand commandAddUser = new SQLiteCommand(UPDATECURRENTSERTEAMWITHSUBS, connection))
                                    {
                                        commandAddUser.CommandType = System.Data.CommandType.Text;
                                        commandAddUser.Parameters.AddWithValue("@username", username);
                                        commandAddUser.Parameters.AddWithValue("@currentteam", JsonConvert.SerializeObject(userTeam));
                                        commandAddUser.Parameters.AddWithValue("@remsub", lastRemSubs - subUsed);
                                        commandAddUser.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    throw new Exception("Subs exceded");
                                }
                            }

                        }
                        else
                        {
                            // adding for first time
                            using (SQLiteCommand commandAddUser = new SQLiteCommand(ADDUNEWSERTEAM, connection))
                            {
                                commandAddUser.CommandType = System.Data.CommandType.Text;
                                commandAddUser.Parameters.AddWithValue("@username", username);
                                commandAddUser.Parameters.AddWithValue("@currentteam", JsonConvert.SerializeObject(userTeam));
                                commandAddUser.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        public UserTeam GetTeam(Guid magicKey)
        {



            string username;

            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(SELECTUSERBYGUID, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@magickey", magicKey.ToString());
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        MagicKey[] keys = reader.ReadAll<MagicKey>();
                        if (keys.Length != 1)
                        {
                            throw new Exception("Unauthorised access");
                        }
                        username = keys[0].username;
                    }
                }

                // get next match info

                using (SQLiteCommand command = new SQLiteCommand(GETLASTTEAM, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            // Get Column ordinal
                            int ordinal = reader.GetOrdinal("currentteam");

                            UserTeam currentTeam = JsonConvert.DeserializeObject<UserTeam>(reader.GetString(ordinal));

                            int subsOrdinal = reader.GetOrdinal("remsub");
                            int lastRemSubs = reader.GetInt32(subsOrdinal);

                            currentTeam.RemSubs = lastRemSubs;

                            return currentTeam;
                        }
                    }

                }

            }
            return new UserTeam();

        }


        public UserTeam GetLastTeam(string username)
        {

            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();

                // get next match info

                using (SQLiteCommand command = new SQLiteCommand(GETLASTTEAM, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            // Get Column ordinal
                            int ordinal = reader.GetOrdinal("lastteam");

                            UserTeam currentTeam = JsonConvert.DeserializeObject<UserTeam>(reader.GetString(ordinal));

                            int subsOrdinal = reader.GetOrdinal("lastremsub");
                            int lastRemSubs = reader.GetInt32(subsOrdinal);

                            currentTeam.RemSubs = lastRemSubs;

                            return currentTeam;
                        }
                    }

                }

            }
            return new UserTeam();

        }


        public UserTeamHistory[] GetUserTeamHistory(string username)
        {

            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();

                // get next match info

                using (SQLiteCommand command = new SQLiteCommand(GETUSERTEAMHISTORY, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        return reader.ReadAll<UserTeamHistory>();
                    }

                }

            }

        }




        public string[] GetUsers()
        {
            List<string> userList = new List<string>();
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(SELECTALLUSER, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        MagicKey[] keys = reader.ReadAll<MagicKey>();
                        foreach (MagicKey key in keys)
                        {
                            userList.Add(key.username);
                        }

                    }
                }
            }
            return userList.ToArray();
        }
    }
}