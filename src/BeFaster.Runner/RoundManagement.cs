using System;
using System.IO;
using System.Linq;
using System.Text;
using BeFaster.Runner.Utils;

namespace BeFaster.Runner
{
    internal static class RoundManagement
    {
        private static readonly string ChallengesPath;
        private static readonly string LastFetchedRoundPath;

        static RoundManagement()
        {
            ChallengesPath = Path.Combine(PathHelper.RepositoryPath, "challenges");
            LastFetchedRoundPath = Path.Combine(ChallengesPath, "XR.txt");
        }

        public static void SaveDescription(string rawDescription, Action<string> callback)
        {
            // DEBT - the first line of the response is the ID for the round, the rest of the responseMessage is the description
            var newlineIndex = rawDescription.IndexOf('\n');
            if (newlineIndex <= 0) return;
            
            var roundId = rawDescription.Substring(0, newlineIndex);
            var lastFetchedRound = GetLastFetchedRound();
            if (!roundId.Equals(lastFetchedRound))
            {
                callback.Invoke(roundId);
            }
            SaveDescription(roundId, rawDescription);
        }

        public static string SaveDescription(string label, string description)
        {
            // Save description.
            var descriptionPath = Path.Combine(ChallengesPath, $"{label}.txt");     

            File.WriteAllText(descriptionPath, description.Replace("\n", Environment.NewLine));
            var relativePath = descriptionPath.Replace(PathHelper.RepositoryPath+Path.DirectorySeparatorChar, "");
            Console.WriteLine($"Challenge description saved to file: {relativePath}.");

            // Save round label.
            File.WriteAllText(LastFetchedRoundPath, label);

            return "OK";
        }

        public static string GetLastFetchedRound() =>
            File.Exists(LastFetchedRoundPath)
                ? File.ReadLines(LastFetchedRoundPath, Encoding.Default).FirstOrDefault()
                : "noRound";

    }
}
