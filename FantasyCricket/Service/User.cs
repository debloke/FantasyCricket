using FantasyCricket.Database;
using FantasyCricket.Models;
using Sqlite.SqlClient;
using System;
using System.Data.SQLite;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace FantasyCricket.Service
{
    public class User : IUser
    {

        private readonly HttpClient httpClient = new HttpClient();

        private static readonly string ADDORUPDATEUSER = "INSERT INTO [User] (  username, password, displayname ) VALUES (  @username, @password, @displayname)";

        private static readonly string SELECTUSER = "SELECT magickey,lastlogin FROM [User] where username = @username and password = @password";

        private static readonly string SELECTALLUSER = "SELECT magickey,lastlogin FROM [User]";

        private static readonly string UPDATEUSERLOGINTIME = "UPDATE [User] SET lastlogin = @lastlogin WHERE username = @username and password = @password";

        private static readonly string UPDATEUSERGUID = "UPDATE [User] SET magickey = @newmagickey WHERE magickey = @oldmagickey";

        private readonly object login = new object();

        private Timer UserMaintenanceTimer { get; set; }

        private const int UserMaintenanceTimerPeriod = 3600000;  // once an hour

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
                            if (key == null)
                            {
                                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Yoy are trying to look at something you should not, RUN AWAY boy", "", null);
                            }
                            else
                            {

                                using (SQLiteCommand command1 = new SQLiteCommand(UPDATEUSERLOGINTIME, connection))
                                {
                                    command.CommandType = System.Data.CommandType.Text;
                                    command.Parameters.AddWithValue("@username", username);
                                    command.Parameters.AddWithValue("@password", password);
                                    command.Parameters.AddWithValue("@logintime", DateTime.UtcNow);
                                    command.ExecuteNonQuery();
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
                    string.Format("Failed to initialize the SqLite maintenance timer. {0}", exception.Message)
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
                                if (key.LastLogin != null && (((DateTime.UtcNow - key.LastLogin).TotalHours >= 3)))
                                {
                                    using (SQLiteCommand command1 = new SQLiteCommand(UPDATEUSERGUID, connection))
                                    {
                                        command.CommandType = System.Data.CommandType.Text;

                                        command.Parameters.AddWithValue("@oldmagickey", key.Magic);
                                        command.Parameters.AddWithValue("@newmagickey", Guid.NewGuid());
                                        command.ExecuteNonQuery();
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }
    }




}

