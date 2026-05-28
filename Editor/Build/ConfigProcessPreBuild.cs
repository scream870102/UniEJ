using UnityEngine;

namespace Scream.UniEJ.Editor
{
    public abstract class ConfigProcessPreBuild<Config, Args> : IProcessPreBuild<Args> where Args : Arguments
    {
        public virtual int Order => (int)BuildOrder.Higher;
        protected Config config;

        public void Process(Args args, IBuilder<Args> builder)
        {
            string fullPath = GetConfigFullPath(args.config);
            if (!System.IO.File.Exists(fullPath))
            {
                Debug.LogError($"File {fullPath} doesn't exist!!!");
                return;
            }
            string jsonText = System.IO.File.ReadAllText(fullPath);
            config = JsonUtility.FromJson<Config>(jsonText);
            ProcessInternal(args, config, builder);
        }

        protected abstract void ProcessInternal(Args args, Config config, IBuilder<Args> builder);

        public virtual bool ShouldExecute(Args args)
        {
            var fullPath = GetConfigFullPath(args.config);
            var hasConfig = !string.IsNullOrWhiteSpace(args.config);
            var exists = hasConfig && System.IO.File.Exists(fullPath);
            if (!exists)
            {
                Debug.Log($"Skip config process: raw config='{args.config}', resolved='{fullPath}', exists={exists}");
            }

            return exists;
        }

        private static string GetConfigFullPath(string rawConfigPath)
        {
            var normalized = (rawConfigPath ?? string.Empty).Trim().Trim('"', '\'');
            if (string.IsNullOrEmpty(normalized))
            {
                return string.Empty;
            }

            var projectRoot = System.IO.Path.GetDirectoryName(Application.dataPath);
            string fullPath;

            if (System.IO.Path.IsPathRooted(normalized))
            {
                fullPath = normalized;
            }
            else if (!string.IsNullOrEmpty(projectRoot))
            {
                fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(projectRoot, normalized));
            }
            else
            {
                fullPath = System.IO.Path.GetFullPath(normalized);
            }

            if (System.IO.File.Exists(fullPath))
            {
                return fullPath;
            }

            // Some CI invocations may pass ".../editorJobConfig" without extension.
            // If no extension is provided, fall back to ".../editorJobConfig.json".
            if (string.IsNullOrEmpty(System.IO.Path.GetExtension(fullPath)))
            {
                var jsonPath = $"{fullPath}.json";
                if (System.IO.File.Exists(jsonPath))
                {
                    return jsonPath;
                }
            }

            return fullPath;
        }
    }
}

