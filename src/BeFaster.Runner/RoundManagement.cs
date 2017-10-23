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

        public static string DisplayAndSaveDescription(string label, string description)
        {
            Console.WriteLine($"Starting round: {label}");
            Console.WriteLine(description);

            // Save description.
            var descriptionPath = Path.Combine(ChallengesPath, $"{label}.txt");

            File.WriteAllText(descriptionPath, description.Replace("\n", Environment.NewLine));
            Console.WriteLine($"Challenge description saved to file: {descriptionPath}.");

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
