using FantasyCricket.Database;
using System.Data.SQLite;
using System.Net;
using System.Net.Http;

namespace FantasyCricket.Service
{
    public class User : IUser
    {

        private readonly HttpClient httpClient = new HttpClient();

        private static readonly string ADDORUPDATEUSER = "INSERT INTO [User] (  username, password, displayname ) VALUES (  @username, @password, @displayname)";

        private static readonly string SELECTUSER = "SELECT username FROM [User] where username = @username and password = @password";


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
        public void LoginUser(string username, string password)
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
                        if (!reader.Read())
                        {
                           throw new HttpResponseException(HttpStatusCode.Unauthorized, "Yoy are trying to look at something you should not, RUN AWAY boy","",null);
                        }
                    }
                }
            }


        }
    }

}
