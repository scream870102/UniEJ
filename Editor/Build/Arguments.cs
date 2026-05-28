using System;

namespace Scream.UniEJ.Editor
{
    public class Arguments
    {
        #region Config
        public string config = string.Empty;
        #endregion Config

        public Arguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (ContainsArg(arg, nameof(config)))
                {
                    string val = GetValue(arg);
                    config = val;
                }
            }
        }

        public string GetValue(string arg, char splitChar = '=')
        {
            string[] v = arg.Split(splitChar);
            if (v == null || v.Length == 0)
            {
                return string.Empty;
            }

            var value = v[v.Length - 1].Trim();
            return value.Trim('"', '\'');
        }

        public bool ContainsArg(string argToCompare, string argName)
        {
            return argToCompare.StartsWith($"-{argName}");
        }

        public bool TryGetLogFilePath(out string path)
        {
            path = string.Empty;
            string[] cmdArgs = Environment.GetCommandLineArgs();
            for (int i = 0; i < cmdArgs.Length; i++)
            {
                if (cmdArgs[i] == "-logFile")
                {
                    path = cmdArgs[i + 1];
                    break;
                }
            }

            return !string.IsNullOrEmpty(path);
        }
    }
}
