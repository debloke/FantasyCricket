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
    public class Gangs : IGangs
    {
        public Gang[] GetGangs(Guid magicKey)
        {
            throw new NotImplementedException();
        }
    }
}
