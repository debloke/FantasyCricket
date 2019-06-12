using FantasyCricket.Models;
using FantasyCricket.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FantasyCricketTests
{

    [TestClass]
    public class UnitTest1
    {

        private readonly string SELECTALLUSER = "SELECT * FROM [User]";

        private readonly string UPDATEUSERGUID = "UPDATE [User] SET password = @password WHERE username = @username";

        [TestMethod]
        public void UpdatePwd()
        {

        }
    }
}
