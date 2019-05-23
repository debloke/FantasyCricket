using Common.Net.Extensions;
using FantasyCricket.Database;
using FantasyCricket.Models;
using Newtonsoft.Json;
using Sqlite.SqlClient;
using System;
using System.Data.SQLite;
using System.Net.Http;

namespace FantasyCricket.Service
{
    public enum IplTeamName
    {
        KingsXIPunjab,
        ChennaiSuperKings,
        MumbaiIndians,
        KolkataKnightRiders,
        DelhiCapitals,
        RoyalChallengersBangalore,
        SunrisersHyderabad,
        RajasthanRoyals

    }

    public enum CountryTeamName
    {
        India,
        England,
        Pakistan,
        Australia,
        WestIndies,
        Srilanka,
        Bangladesh,
        Afghanistan,
        SouthAfrica,
        NewZealand,
        Unknown


    }

    public enum TeamType
    {
        IPL,
        ODI
    }
    public class PlayerInfo : IPlayerInfo
    {

        private readonly HttpClient httpClient = new HttpClient();

        private static readonly string ADDORUPDATEPLAYERIPL = "INSERT OR REPLACE INTO [IplPlayer] (  Pid, Pname, Team, Role ) VALUES (  @Pid, @Pname, @Team, @Role)";

        private static readonly string ADDORUPDATEPLAYERCOUNTRY = "INSERT INTO [OdiPlayer] (  Pid, Pname, Team, Role ) VALUES (  @Pid, @Pname, @Team, @Role)";

        private static readonly string SQLSELECTIPL = "SELECT * FROM IplPlayer";

        private static readonly string SQLSELECTODI = "SELECT * FROM OdiPlayer";

        private static readonly string SQLUPDATEIPLCOST = "UPDATE [IplPlayer] SET Cost = @Cost, Role = @Role WHERE Pid = @Pid";

        private static readonly string SQLUPDATEODICOST = "UPDATE [OdiPlayer] SET Cost = @Cost, Role = @Role WHERE Pid = @Pid";

        public Player[] GetPlayers(TeamType teamType)
        {
            string query;
            switch (teamType)
            {

                case TeamType.IPL:
                    query = SQLSELECTIPL;
                    break;
                case TeamType.ODI:
                    query = SQLSELECTODI;
                    break;
                default:
                    throw new Exception("Unsupported tournament");
            }

            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        return reader.ReadAll<Player>();
                    }
                }
            }
        }
        public void UpdateCountryPlayers(int uniqueId)
        {
            SquadResponse squadResponse = JsonConvert.DeserializeObject<SquadResponse>(httpClient.InvokeGet(string.Format("https://cricapi.com/api/fantasySquad?apikey=ZlrRCAEEwjg9Vknh9hOgVlV17ls2&unique_id={0}", uniqueId)));
            foreach (Team team in squadResponse.Teams)
            {
                string teamName = team.TeamName.Replace(" ", "");
                // Check  If country

                if (Enum.TryParse(typeof(CountryTeamName), teamName, true, out object countryTeamName))
                {
                    foreach (Player player in team.Players)
                    {
                        try
                        {
                            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
                            {
                                connection.Open();
                                using (SQLiteCommand insertCommand = new SQLiteCommand(ADDORUPDATEPLAYERCOUNTRY, connection))
                                {
                                    insertCommand.CommandType = System.Data.CommandType.Text;
                                    insertCommand.Parameters.AddWithValue("@Pid", player.PlayerId);
                                    insertCommand.Parameters.AddWithValue("@Pname", player.PlayerName);
                                    insertCommand.Parameters.AddWithValue("@Team", teamName);
                                    insertCommand.Parameters.AddWithValue("@Role", Role.UNKNOWN);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                        catch
                        {
                            // TODO
                        }
                    }
                }
                else
                {
                    throw new Exception("Provided Match Id is not for an ODI match");
                }

            }
        }
        void IPlayerInfo.UpdateIplPlayers(int uniqueId)
        {
            SquadResponse squadResponse = JsonConvert.DeserializeObject<SquadResponse>(httpClient.InvokeGet(string.Format("https://cricapi.com/api/fantasySquad?apikey=ZlrRCAEEwjg9Vknh9hOgVlV17ls2&unique_id={0}", uniqueId)));
            foreach (Team team in squadResponse.Teams)
            {
                string teamName = team.TeamName.Replace(" ", "");
                // Check  If Ipl

                if (Enum.TryParse(typeof(IplTeamName), teamName, true, out object iplTeamName))
                {
                    foreach (Player player in team.Players)
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
                        {
                            connection.Open();
                            using (SQLiteCommand insertCommand = new SQLiteCommand(ADDORUPDATEPLAYERIPL, connection))
                            {
                                insertCommand.CommandType = System.Data.CommandType.Text;
                                insertCommand.Parameters.AddWithValue("@Pid", player.PlayerId);
                                insertCommand.Parameters.AddWithValue("@Pname", player.PlayerName);
                                insertCommand.Parameters.AddWithValue("@Team", teamName);
                                insertCommand.Parameters.AddWithValue("@Role", Role.UNKNOWN);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Provided Match Id is not for an IPL match");
                }


            }
        }

        public void UpdatePlayerCost(int id, int cost, Role role, TeamType teamType)
        {
            string query;
            switch (teamType)
            {

                case TeamType.IPL:
                    query = SQLUPDATEIPLCOST;
                    break;
                case TeamType.ODI:
                    query = SQLUPDATEODICOST;
                    break;
                default:
                    throw new Exception("Unsupported tournament");
            }

            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@Pid", id);
                    command.Parameters.AddWithValue("@Cost", cost);
                    command.Parameters.AddWithValue("@Role", role);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
