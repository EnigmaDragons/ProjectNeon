using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace ChangeLogGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var lastTag = Bat("git describe --tags --abbrev^=0").Replace("\n", "");
            var nextVersion = NextVersion(lastTag);
            Console.WriteLine(nextVersion);
            var logCommand = "git log --oneline " + lastTag + "..@ --pretty=format:%s%b";

            var changeLines = Bat(logCommand).Split("\n");
            var patchNotes = GeneratePatchNotes(nextVersion, changeLines);

            patchNotes.ForEach(l => Console.WriteLine(l));
            File.WriteAllLines($"./patchnotes/{nextVersion}.md", patchNotes);
        }

        private static string NextVersion(string lastVersion)
        {
            var majorMinor = lastVersion.Substring(0, lastVersion.LastIndexOf(".") + 1);
            var patchVersion = int.Parse(lastVersion.Substring(lastVersion.LastIndexOf(".") + 1));
            return $"{majorMinor}{patchVersion + 1}";
        }

        private static List<string> GeneratePatchNotes(string version, IEnumerable<string> commitLog)
        {
            var filteredLines = commitLog.Where(c => c.Contains("Close") || c.Contains("(#"));
            var cleansedLines = commitLog.Select(c => {
                var delimiterIndex = Math.Max(c.IndexOf("Close"), c.IndexOf("(#"));
                var cleaned = c.Substring(0, Math.Max(0, delimiterIndex)).Trim();
                return cleaned.Length > 0 ? "- " + cleaned : "";
            }).Where(c => c.Length > 0);

            var categorized = cleansedLines.Select(l => GetCategorizedLine(l))
                .GroupBy(x => x.Item1)
                .ToDictionary(x => x.Key, x => x.Select(l => l.Item2).ToList());

            var patchNotes = new List<string>();
            patchNotes.Add($"## Patch Notes - {version}");
            patchNotes.Add("----");
            patchNotes.Add("");

            foreach(var c in categorized.OrderBy(x => _categoryOrder[x.Key]))
            {
                patchNotes.Add(c.Key + ":");
                c.Value
                    .OrderBy(l => l)
                    .ToList()
                    .ForEach(line => patchNotes.Add(CleanupRawLine(line)));
                patchNotes.Add("");
            }
            return patchNotes;
        }

        private static Dictionary<string, int> _categoryOrder = new Dictionary<string, int>
        {
            { "New Content", 1 },
            { "New Features", 10 },
            { "Balance Changes", 20 },
            { "Card Improvements", 40 },
            { "Art/UI Improvements", 50 },
            { "Bug Fixes", 85 },
            { "Miscellaneous", 99 },
        };

        private static Dictionary<string, string> _containsTextThenCategory = new Dictionary<string,string>
        {
            { "New Card:", "New Content" },
            { "Cards", "New Content" },
            { "cards", "New Content" },
            { "New Enemy:", "New Content" },
            { "New Boss:", "New Content" },
            { "Map:", "New Content" },
            { "Zone", "New Content" },
            { "New Hero:", "New Content" },
            { "Bug:", "Bug Fixes" },
            { "Fixed", "Bug Fixes" },
            { "UI:", "Art/UI Improvements" },
            { "UI", "Art/UI Improvements" },
            { "Tooltip", "Art/UI Improvements" },
            { "View", "Art/UI Improvements" },
            { "Art:", "Art/UI Improvements" },
            { "Art", "Art/UI Improvements" },
            { "Wording", "Card Improvements" },
            { "Interpolate", "Card Improvements" },
            { "Card Text", "Card Improvements" },
            { "Effect:", "New Features" },
            { "New Feature:", "New Features" },
            { "Rebalance:", "Balance Changes" },
            { "Rebalanced:", "Balance Changes" },
            { "Progression", "Balance Changes" },
            { "AI", "Balance Changes" },
            { "New Battle Role", "Balance Changes" },
        };

        private static Tuple<string, string> GetCategorizedLine(string rawLine)
        {
            var matches = _containsTextThenCategory.Where(x => rawLine.ToLowerInvariant().Contains(x.Key.ToLowerInvariant()));
            return matches.Any()
                ? new Tuple<string, string>(matches.First().Value, rawLine)
                : new Tuple<string, string>("Miscellaneous", rawLine);
        }

        private static string CleanupRawLine(string rawLine)
        {
            var line = rawLine.Trim();
            line = line.EndsWith('.') ? line.Substring(0, line.Length - 1) : line;
            return line;
        }

        public static string Bat(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            string result = Run("cmd.exe", $"/c \"{escapedArgs}\"");
            return result;
        }

        private static string Run(string filename, string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }
}
