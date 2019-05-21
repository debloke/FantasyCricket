using FantasyCricket.Models;
using System;

using Common.Net.Extensions;
using System.Net.Http;
using Newtonsoft.Json;
using FantasyCricket.ScoreCalculator;
using System.Linq;

namespace FantasyCricket.Service
{
    public class CricApiLiveScore : ILiveScore
    {
        private readonly HttpClient httpClient = new HttpClient();
        public Points[] GetScore(int uniqueId)
        {
            Points[] points;
            string response = httpClient.InvokeGet(string.Format("https://cricapi.com/api/fantasySummary?apikey=ZlrRCAEEwjg9Vknh9hOgVlV17ls2&unique_id={0}", uniqueId));
            PlayerScoresResponse playerScoresResponse = JsonConvert.DeserializeObject<PlayerScoresResponse>(response, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            IScoreCalculator scoreCalulator;
            switch (playerScoresResponse.MatchType)
            {
                case MatchType.ODI:
                    scoreCalulator = new OdiScoreCalculator();
                    points = scoreCalulator.CalculateScore(playerScoresResponse).Values.ToArray();
                    break;
                default:
                    throw new Exception("Unknown match type");

            }
            return points;
        }
    }
}
