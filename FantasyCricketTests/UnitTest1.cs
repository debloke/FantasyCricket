using FantasyCricket.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace FantasyCricketTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void NoOFBallsTest()
        {
            float overs = 9.3f;
            int balls = 0;
            balls += (int)(overs / 1) * 6;
            balls+=(int)((overs % 1.0f)*10.0);
            Console.WriteLine(balls);


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

            Points[] points = new Points[] { new Points() {BattingPoints=1,BowlingPoints=2 }, new Points() {FieldingPoints=3 }};
            Points[] point1 = new Points[] { new Points() { BattingPoints = 2, FieldingPoints = 3 }, new Points() { FieldingPoints = 4 ,  BowlingPoints=5} };

            liveScores.GetOrAdd(1,points);
            liveScores.GetOrAdd(2, point1);


            Console.WriteLine(JsonConvert.SerializeObject(liveScores));
        }
    }
}
