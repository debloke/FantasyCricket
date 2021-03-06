﻿using FantasyCricket.Common.Net.Exceptions;
using FantasyCricket.Database;
using FantasyCricket.Models;
using Sqlite.SqlClient;
using System;
using System.Data.SQLite;

namespace FantasyCricket.Service
{
    public class Gangs : IGangs
    {

        private readonly string SELECTGANGS = "Select gangs.seriesid, gangs.gangid, gangs.name, gangs.owner, GangUserMap.username, GangUserMap.approved from Gangs INNER JOIN GangUserMap on GangUserMap.gangid = Gangs.gangid where seriesid = @seriesid and GangUserMap.gangid in (select gangid from GangUserMap where username =  @username) and (GangUserMap.Approved=1 OR (GangUserMap.Approved = 0 and GangUserMap.username = @username) OR Gangs.owner= @username)";

        private readonly string CREATEGANG = "INSERT INTO [Gangs] (  name, owner, seriesid) VALUES (  @name, @owner, @seriesid)";

        private readonly string REMOVEGANG = "DELETE FROM [Gangs]  WHERE gangid = @gangid AND owner = @owner";

        private readonly string GETGANGOWNER = "SELECT owner From [Gangs] WHERE gangid = @gangid";

        private readonly string CREATEGANGUSERMAP = "INSERT OR REPLACE INTO [GangUserMap] (  username, gangid, approved) VALUES (  @username, @gangid, @approved)";

        private readonly string REMOVEGANGUSERMAP = "DELETE FROM [GangUserMap]  WHERE gangid = @gangid AND username = @username";

        private readonly string CREATEGANGUSERMAPDEFAULT = "INSERT OR REPLACE INTO [GangUserMap] (username, gangid) VALUES (  @owner, @gangid)";

        private readonly string GETGANGID = "SELECT gangid FROM Gangs where name = @name and owner=@owner";

        public void AcceptGangMembership(int gangid, string username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(CREATEGANGUSERMAP, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@gangid", gangid);
                    command.Parameters.AddWithValue("@approved", Status.Approved);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddToGang(int gangid, string[] usernames, string owner)
        {
            string failedUsernames = String.Empty;
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(GETGANGOWNER, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@gangid", gangid);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        bool hasRows = reader.Read();
                        if (!hasRows)
                        {
                            throw new GangNotFoundException($"No gang found with gangid : {gangid}");
                        }
                        int ownerOrdinal = reader.GetOrdinal("owner");

                        string gangOwner = reader.GetString(ownerOrdinal);

                        if (!gangOwner.Equals(owner))
                        {
                            throw new NotAGangOwnerException($"You are not the gang owner");
                        }
                    }
                }



                foreach (string username in usernames)
                {
                    try
                    {
                        using (SQLiteCommand command = new SQLiteCommand(CREATEGANGUSERMAP, connection))
                        {
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.AddWithValue("@username", username);
                            command.Parameters.AddWithValue("@gangid", gangid);
                            command.Parameters.AddWithValue("@approved", Status.Pending);
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception exception)
                    {
                        failedUsernames += (username + ",");
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


                using (SQLiteCommand command = new SQLiteCommand(GETGANGID, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@name", gang.GangName);
                    command.Parameters.AddWithValue("@owner", gang.GangOwner);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        bool hasRows = reader.Read();

                        int gangIdOrdinal = reader.GetOrdinal("gangid");

                        int gangId = reader.GetInt32(gangIdOrdinal);
                        using (SQLiteCommand createUserMapcommand = new SQLiteCommand(CREATEGANGUSERMAPDEFAULT, connection))
                        {
                            createUserMapcommand.CommandType = System.Data.CommandType.Text;
                            createUserMapcommand.Parameters.AddWithValue("@owner", gang.GangOwner);
                            createUserMapcommand.Parameters.AddWithValue("@gangid", gangId);
                            createUserMapcommand.ExecuteNonQuery();
                        }
                    }
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

        public void RemoveFromGang(int gangid, string[] usernames, string owner)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(GETGANGOWNER, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@gangid", gangid);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        bool hasRows = reader.Read();
                        if (!hasRows)
                        {
                            throw new GangNotFoundException($"No gang found with gangid : {gangid}");
                        }
                        int ownerOrdinal = reader.GetOrdinal("owner");

                        string gangOwner = reader.GetString(ownerOrdinal);

                        if (!gangOwner.Equals(owner))
                        {
                            throw new NotAGangOwnerException($"You are not the gang owner");
                        }
                    }
                }



                foreach (string username in usernames)
                {
                    using (SQLiteCommand command = new SQLiteCommand(REMOVEGANGUSERMAP, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@gangid", gangid);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void RemoveGang(int gangid, string owner)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DatabaseSetup.GetConnectString()))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(REMOVEGANG, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@gangid", gangid);
                    command.Parameters.AddWithValue("@owner", owner);

                    int deletedRows = command.ExecuteNonQuery();
                    if (deletedRows == 0)
                    {
                        throw new NotAGangOwnerException($"You are not the gang owner");
                    }
                }
            }
        }
    }
}
