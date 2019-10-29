using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Milamation
{
    public class WinAppUpdater : IAppUpdater
    {
        private readonly ILogger<WinAppUpdater> logger;
        private const string UpdateUrl = "https://github.com/jmelosegui/milamation/releases/latest/download/";
        private readonly string updateExe;
        public WinAppUpdater(ILogger<WinAppUpdater> logger)
        {
            this.logger = logger;
            string currentDirectory = Path.GetDirectoryName(typeof(WinAppUpdater).Assembly.Location);
            updateExe = Path.Combine(currentDirectory, "..", "Update.exe");
            this.logger.LogInformation($"Update.exe should be at at {updateExe}");
        }
        public Task<WinCheckForUpdateResult?> GetLatestVersionAsync(CancellationToken ct)
        {
            if (File.Exists(updateExe))
            {
                return Task.Run(() =>
                {
                    string textResult = null;
                    var pi = new ProcessStartInfo(updateExe, $"--checkForUpdate={UpdateUrl}");
                    pi.RedirectStandardOutput = true;
                    var p = new Process();
                    pi.UseShellExecute = false;
                    p.StartInfo = pi;
                    p.OutputDataReceived += (s, e) =>
                    {
                        Debug.WriteLine($"Checking: {e.Data}");
                        if (e.Data?.StartsWith("{") ?? false)
                        {
                            textResult = e.Data;
                        }
                    };
                    p.Start();
                    p.BeginOutputReadLine();
                    p.WaitForExit();
                    if (textResult != null)
                    {
                        logger.LogInformation($"Updater response is: {textResult}");
                    }
                    else
                    {
                        logger.LogWarning("Got no meaningful response from updater");
                    }
                    if (textResult != null)
                    {
                        return JsonConvert.DeserializeObject<WinCheckForUpdateResult>(textResult);
                    }
                    else
                    {
                        return (WinCheckForUpdateResult?)null;
                    }
                });
            }
            else
            {
                logger.LogWarning($"Couldn't find Update.exe");
            }
            return Task.FromResult((WinCheckForUpdateResult?)null);
        }
        public Task UpdateAsync(CancellationToken ct)
        {
            if (File.Exists(updateExe))
            {
                return Task.Run(() =>
                {
                    var pi = new ProcessStartInfo(updateExe, $"--update={UpdateUrl}");
                    pi.RedirectStandardOutput = true;
                    var p = new Process();
                    pi.UseShellExecute = false;
                    p.StartInfo = pi;
                    p.OutputDataReceived += (s, e) =>
                    {
                        Debug.WriteLine($"Updating: {e.Data}");
                    };
                    p.Start();
                    p.BeginOutputReadLine();
                    p.WaitForExit();
                });
            }
            return null;
        }
    }
}
