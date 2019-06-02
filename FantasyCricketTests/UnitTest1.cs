using FantasyCricket.Database;
using FantasyCricket.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Sqlite.SqlClient;
using System;
using System.Collections.Concurrent;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace FantasyCricketTests
{

    [TestClass]
    public class UnitTest1
    {

        private readonly string SELECTALLUSER = "SELECT magickey,lastlogin,username FROM [User]";

        private readonly string UPDATEUSERGUID = "UPDATE [User] SET magickey = @newmagickey WHERE username = @username";

        [TestMethod]
        public void TestMagicExpiry()
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
                                    command1.CommandType = System.Data.CommandType.Text;

                                    command1.Parameters.AddWithValue("@username", key.username);
                                    command1.Parameters.AddWithValue("@newmagickey", Guid.NewGuid());
                                    command1.ExecuteNonQuery();
                                }
                            }
                        }

                    }


                }
            }
        }
        [TestMethod]
        public void TestMethodMatches()
        {
            string matchJson = File.ReadAllText(@"Match.json", Encoding.UTF8);

            Matches matches = JsonConvert.DeserializeObject<Matches>(matchJson);

            Console.WriteLine(JsonConvert.SerializeObject(matches.AllMatch[0]));
        }



        [TestMethod]
        public void TestMethod1()
        {
            ConcurrentDictionary<int, Points[]> liveScores = new ConcurrentDictionary<int, Points[]>();

            Points[] points = new Points[] { new Points() { BattingPoints = 1, BowlingPoints = 2 }, new Points() { FieldingPoints = 3 } };
            Points[] point1 = new Points[] { new Points() { BattingPoints = 2, FieldingPoints = 3 }, new Points() { FieldingPoints = 4, BowlingPoints = 5 } };

            liveScores.GetOrAdd(1, points);
            liveScores.GetOrAdd(2, point1);


            Console.WriteLine(JsonConvert.SerializeObject(liveScores));
        }
    }
}
