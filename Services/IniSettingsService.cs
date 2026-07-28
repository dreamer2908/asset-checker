using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LegacyWebBridge.Services
{
    /// <summary>
    /// Service responsible for managing application settings and custodian bookmarks persisted in an .ini file on disk.
    /// </summary>
    public class IniSettingsService
    {
        private readonly string _filePath;
        private readonly object _lock = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="IniSettingsService"/> class.
        /// </summary>
        /// <param name="environment">Host environment providing the content root path.</param>
        public IniSettingsService(IWebHostEnvironment environment)
        {
            _filePath = Path.Combine(environment.ContentRootPath, "bookmarks.ini");
            EnsureFileExists();
        }

        private void EnsureFileExists()
        {
            lock (_lock)
            {
                if (!File.Exists(_filePath))
                {
                    File.WriteAllText(_filePath, "[Bookmarks]\nCustodians=\n");
                }
            }
        }

        /// <summary>
        /// Retrieves the list of bookmarked custodian codes from the .ini file.
        /// </summary>
        /// <returns>A HashSet of trimmed custodian code strings.</returns>
        public HashSet<string> GetBookmarkedCustodians()
        {
            lock (_lock)
            {
                if (!File.Exists(_filePath))
                {
                    return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }

                string[] lines = File.ReadAllLines(_filePath);
                string? custodiansLine = lines
                    .FirstOrDefault(l => l.StartsWith("Custodians=", StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrWhiteSpace(custodiansLine))
                {
                    return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }

                string value = custodiansLine.Substring("Custodians=".Length).Trim();
                if (string.IsNullOrWhiteSpace(value))
                {
                    return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }

                return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Toggles a custodian code in the bookmarked list in the .ini file.
        /// </summary>
        /// <param name="code">Custodian code to toggle.</param>
        /// <returns>True if the custodian code is now bookmarked; false otherwise.</returns>
        public bool ToggleCustodianBookmark(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;
            code = code.Trim();

            lock (_lock)
            {
                var bookmarks = GetBookmarkedCustodians();
                bool isNowBookmarked;

                if (bookmarks.Contains(code))
                {
                    bookmarks.Remove(code);
                    isNowBookmarked = false;
                }
                else
                {
                    bookmarks.Add(code);
                    isNowBookmarked = true;
                }

                string newCustodiansLine = "Custodians=" + string.Join(",", bookmarks);

                string[] lines = File.Exists(_filePath) ? File.ReadAllLines(_filePath) : Array.Empty<string>();
                List<string> newLines = new();
                bool sectionFound = false;
                bool keyFound = false;

                foreach (var line in lines)
                {
                    string trimmed = line.Trim();
                    if (trimmed.Equals("[Bookmarks]", StringComparison.OrdinalIgnoreCase))
                    {
                        sectionFound = true;
                        newLines.Add(line);
                        continue;
                    }

                    if (sectionFound && trimmed.StartsWith("Custodians=", StringComparison.OrdinalIgnoreCase))
                    {
                        newLines.Add(newCustodiansLine);
                        keyFound = true;
                        continue;
                    }

                    newLines.Add(line);
                }

                if (!sectionFound)
                {
                    newLines.Add("[Bookmarks]");
                    newLines.Add(newCustodiansLine);
                }
                else if (!keyFound)
                {
                    newLines.Add(newCustodiansLine);
                }

                File.WriteAllLines(_filePath, newLines);
                return isNowBookmarked;
            }
        }
    }
}
