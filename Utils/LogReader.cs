using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace dvcsharp_core_api.Utils
{
    public class LogReader
    {
        private readonly string _logFilePath;
        
        public LogReader(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        /// <summary>
        /// Reads all log entries from the file
        /// </summary>
        /// <returns>List of log entries</returns>
        public async Task<List<string>> ReadAllLogsAsync()
        {
            try
            {
                if (!File.Exists(_logFilePath))
                {
                    throw new FileNotFoundException($"Log file not found at path: {_logFilePath}");
                }

                return new List<string>(await File.ReadAllLinesAsync(_logFilePath));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading log file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Reads the last n lines from the log file
        /// </summary>
        /// <param name="n">Number of lines to read from the end</param>
        /// <returns>List of the last n log entries</returns>
        public async Task<List<string>> ReadLastNLogsAsync(int n)
        {
            try
            {
                if (!File.Exists(_logFilePath))
                {
                    throw new FileNotFoundException($"Log file not found at path: {_logFilePath}");
                }

                var allLines = await File.ReadAllLinesAsync(_logFilePath);
                var result = new List<string>();
                
                int startIndex = Math.Max(0, allLines.Length - n);
                for (int i = startIndex; i < allLines.Length; i++)
                {
                    result.Add(allLines[i]);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading log file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Reads logs that match a specific search term
        /// </summary>
        /// <param name="searchTerm">Term to search for in logs</param>
        /// <returns>List of matching log entries</returns>
        public async Task<List<string>> SearchLogsAsync(string searchTerm)
        {
            try
            {
                if (!File.Exists(_logFilePath))
                {
                    throw new FileNotFoundException($"Log file not found at path: {_logFilePath}");
                }

                var matchingLines = new List<string>();
                using (var stream = new StreamReader(_logFilePath))
                {
                    string line;
                    while ((line = await stream.ReadLineAsync()) != null)
                    {
                        if (line.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        {
                            matchingLines.Add(line);
                        }
                    }
                }

                return matchingLines;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching log file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Monitors the log file for new entries in real-time
        /// </summary>
        /// <param name="onNewLogEntry">Action to execute when new log entry is detected</param>
        public void MonitorLogs(Action<string> onNewLogEntry)
        {
            try
            {
                if (!File.Exists(_logFilePath))
                {
                    throw new FileNotFoundException($"Log file not found at path: {_logFilePath}");
                }

                var watcher = new FileSystemWatcher(
                    Path.GetDirectoryName(_logFilePath),
                    Path.GetFileName(_logFilePath));

                watcher.Changed += (sender, e) =>
                {
                    if (e.ChangeType == WatcherChangeTypes.Changed)
                    {
                        try
                        {
                            // Read the last line of the file
                            var lastLine = File.ReadLines(_logFilePath).Last();
                            onNewLogEntry(lastLine);
                        }
                        catch (IOException)
                        {
                            // Handle file being locked
                        }
                    }
                };

                watcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error monitoring log file: {ex.Message}", ex);
            }
        }
    }
} 