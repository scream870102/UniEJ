using System;
using System.Diagnostics;
using System.IO;

namespace Scream.UniEJ.Editor
{
    [Serializable]
    public class GitHelper : ISourceControlHelper
    {
        private readonly string _workingDirectory;

        public GitHelper(string workingDirectory = null)
        {
            _workingDirectory = workingDirectory ?? Directory.GetCurrentDirectory();
        }

        public void Add(string path)
        {
            RunGitCommand($"add \"{path}\"");
        }

        public void Commit(string message)
        {
            RunGitCommand($"commit -m \"{message}\"");
        }

        public void Remove(string path)
        {
            string status = RunGitCommandWithOutput($"ls-files \"{path}\"").Trim();

            if (!string.IsNullOrEmpty(status))
            {
                RunGitCommand($"restore --source=HEAD --staged --worktree \"{path}\"");
            }
            else
            {
                RunGitCommand($"clean -f \"{path}\"");
            }
        }

        public void Push()
        {
            RunGitCommand("push");
        }

        public void Pull()
        {
            RunGitCommand("pull");
        }

        private void RunGitCommand(string arguments)
        {
            var startInfo = CreateStartInfo(arguments);
            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    process?.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Git Command Failed: {arguments}\n{ex.Message}");
            }
        }


        private string RunGitCommandWithOutput(string arguments)
        {
            var startInfo = CreateStartInfo(arguments);
            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    if (process == null) return string.Empty;
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return output;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private ProcessStartInfo CreateStartInfo(string arguments)
        {
            return new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = _workingDirectory,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
        }
    }
}
