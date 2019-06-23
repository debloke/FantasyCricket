using FantasyCricket.Common.Net.Exceptions;
using FantasyCricket.Database;
using FantasyCricket.Models;
using FantasyCricket.Namak;
using Newtonsoft.Json;
using Sqlite.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading;

namespace FantasyCricket.Service
{
    public class User : IUser
    {

        private readonly string ADDORUPDATEUSER = "INSERT INTO [User] (  username, password, displayname ,magickey) VALUES (  @username, @password, @displayname,@magickey)";

        private readonly string SELECTUSER = "SELECT magickey,lastlogin,username,displayname,password FROM [User] where username = @username";

        private readonly string SELECTALLUSER = "SELECT magickey,lastlogin,username,displayname FROM [User]";

        private readonly string SELECTUSERBYGUID = "SELECT magickey,lastlogin,username,displayname FROM [User] where magickey=@magickey";

        private readonly string SELECTUSERBYUSERNAME = "SELECT magickey,lastlogin,username,displayname FROM [User] where username=@username";

        private readonly string UPDATEUSERLOGINTIME = "UPDATE [User] SET lastlogin = @lastlogin WHERE username = @username";

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
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(ADDORUPDATEUSER, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", NamakGenerator.Get(password));
                        command.Parameters.AddWithValue("@displayname", displayName);
                        command.Parameters.AddWithValue("@magickey", Guid.NewGuid().ToString());
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException exception)
            {
                if (exception.ResultCode == SQLiteErrorCode.Constraint)
                {
                    throw new UserAlreadyExistsException($"username:{username} is already taken");
                }
                else
                {
                    throw exception;
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
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            MagicKey key = reader.Read<MagicKey>();
                            if (key != null)
                            {
                                // Verify password
                                if (NamakGenerator.Verify(key.Password, password))
                                {
                                    if (key.Magic == null || key.Magic == Guid.Empty)
                                    {
                                        key.Magic = Guid.NewGuid();
                                        using (SQLiteCommand command1 = new SQLiteCommand(UPDATEUSERGUID, connection))
                                        {
                                            command1.CommandType = System.Data.CommandType.Text;
                                            command1.Parameters.AddWithValue("@username", username);
                                            command1.Parameters.AddWithValue("@newmagickey", key.Magic.ToString());
                                            command1.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        using (SQLiteCommand command1 = new SQLiteCommand(UPDATEUSERLOGINTIME, connection))
                                        {
                                            command1.CommandType = System.Data.CommandType.Text;
                                            command1.Parameters.AddWithValue("@username", username);
                                            command1.Parameters.AddWithValue("@lastlogin", DateTime.UtcNow);
                                            command1.ExecuteNonQuery();
                                        }
                                    }
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            return key;
                        }
                    }
                }

            }
        }


        public MagicKey LoginUser(Guid magicKey)
        {
            lock (login)
            {
                using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(SELECTUSERBYGUID, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.AddWithValue("@magickey", magicKey.ToString());
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            return reader.Read<MagicKey>();
                        }
                    }

                }
            }
        }

        public void LogoutUser(string username)
        {
            lock (login)
            {
                using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
                {
                    connection.Open();
                    using (SQLiteCommand command1 = new SQLiteCommand(UPDATEUSERGUID, connection))
                    {
                        command1.CommandType = System.Data.CommandType.Text;

                        command1.Parameters.AddWithValue("@username", username);
                        command1.Parameters.AddWithValue("@newmagickey", null);
                        command1.ExecuteNonQuery();
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
            try
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

                                            command1.Parameters.AddWithValue("@username", key.UserName);
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
            catch (Exception exception)
            {
                //TODO Add logging
            }
        }

        public void SaveTeam(UserTeam userTeam, string username)
        {

            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();

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

        public UserTeam GetTeam(string username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();

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


        public UserTeamHistory[] GetHistoryTeam(string username)
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
                            userList.Add(key.UserName);
                        }

                    }
                }
            }
            return userList.ToArray();
        }

        public MagicKey LoginUser(string username)
        {
            lock (login)
            {
                using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(SELECTUSERBYUSERNAME, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.AddWithValue("@username", username);
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            return reader.Read<MagicKey>();
                        }
                    }

                }
            }
        }
    }
}