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
            var patchNotes = GeneratePatchNotes(changeLines);

            File.WriteAllLines($"./patchnotes/{nextVersion}.md", patchNotes);
        }

        private static string NextVersion(string lastVersion)
        {
            var majorMinor = lastVersion.Substring(0, lastVersion.LastIndexOf(".") + 1);
            var patchVersion = int.Parse(lastVersion.Substring(lastVersion.LastIndexOf(".") + 1));
            return $"{majorMinor}{patchVersion + 1}";
        }

        private static List<string> GeneratePatchNotes(IEnumerable<string> commitLog)
        {
            var filteredLines = commitLog.Where(c => c.Contains("Close") || c.Contains("(#"));
            var cleansedLines = commitLog.Select(c => {
                var delimiterIndex = Math.Max(c.IndexOf("Close"), c.IndexOf("(#"));
                var cleaned = c.Substring(0, Math.Max(0, delimiterIndex)).Trim();
                return cleaned.Length > 0 ? "- " + cleaned : "";
            }).Where(c => c.Length > 0);

            foreach(var l in cleansedLines)
                Console.WriteLine(l);
            return cleansedLines.ToList();
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
