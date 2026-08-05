using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AssetChecker.Services
{
    /// <summary>
    /// Service responsible for managing application settings, custodian bookmarks, and department bookmarks persisted in an .ini file on disk.
    /// </summary>
    public class IniSettingsService
    {
        private readonly string _filePath;
        private readonly object _lock = new();

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
                    File.WriteAllText(_filePath, "[Bookmarks]\nCustodians=\nDepartments=\n");
                }
            }
        }

        public HashSet<string> GetBookmarkedCustodians()
        {
            return GetKeyValues("Custodians=");
        }

        public HashSet<string> GetBookmarkedDepartments()
        {
            return GetKeyValues("Departments=");
        }

        private HashSet<string> GetKeyValues(string keyPrefix)
        {
            lock (_lock)
            {
                if (!File.Exists(_filePath))
                {
                    return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }

                string[] lines = File.ReadAllLines(_filePath);
                string? line = lines
                    .FirstOrDefault(l => l.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrWhiteSpace(line))
                {
                    return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }

                string value = line.Substring(keyPrefix.Length).Trim();
                if (string.IsNullOrWhiteSpace(value))
                {
                    return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }

                return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
            }
        }

        public bool ToggleCustodianBookmark(string code)
        {
            return ToggleKeyValue("Custodians=", code);
        }

        public bool ToggleDepartmentBookmark(string code)
        {
            return ToggleKeyValue("Departments=", code);
        }

        private bool ToggleKeyValue(string keyPrefix, string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;
            code = code.Trim();

            lock (_lock)
            {
                var set = GetKeyValues(keyPrefix);
                bool isNowBookmarked;

                if (set.Contains(code))
                {
                    set.Remove(code);
                    isNowBookmarked = false;
                }
                else
                {
                    set.Add(code);
                    isNowBookmarked = true;
                }

                string newLineContent = keyPrefix + string.Join(",", set);

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

                    if (sectionFound && trimmed.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        newLines.Add(newLineContent);
                        keyFound = true;
                        continue;
                    }

                    newLines.Add(line);
                }

                if (!sectionFound)
                {
                    newLines.Add("[Bookmarks]");
                    newLines.Add(newLineContent);
                }
                else if (!keyFound)
                {
                    newLines.Add(newLineContent);
                }

                File.WriteAllLines(_filePath, newLines);
                return isNowBookmarked;
            }
        }
    }
}
