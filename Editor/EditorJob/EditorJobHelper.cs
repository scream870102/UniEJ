using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Scream.UniEJ.Common;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Scream.UniEJ.Editor
{
    public interface IEditorJobContainerSetting { }

    public interface IEditorJobProcess
    {
        void Execute(IEditorJobContainer container, IEditorJobContainerSetting containerSetting, IEditorJobSetting setting, in StringBuilder log, in StringBuilder svn);
    }

    public interface IEditorJobSetting { }

    public interface IEditorJobContainer
    {
        InterfaceReference<IEditorJobContainerSetting> Setting { get; }
        List<EditorJobSettingProcessPair> Jobs { get; }
    }

    [CreateAssetMenu(fileName = "JobHelper", menuName = "Scream/UniEJ/EditorJob/JobHelper")]
    public class EditorJobHelper : ScriptableObject
    {
        public const string DefaultLogName = "EditorJobLog{0}";
        public const string DefaultExtension = ".txt";
        public const string DefaultSourceControlName = "EditorJobSourceControl{0}";
        [SerializeField] private InterfaceReference<IEditorJobContainer>[] _containers;
        [SerializeField] private bool _sourceControlEnable = false;
        public string logPath;
        public string sourceControlPath;
        [FilterType("GetFilteredTypeList")] public FilterTypeReference<ISourceControlHelper> sourceControlHelper;


        [Button()]
        public void Execute()
        {
            var sw = new Stopwatch();
            var log = new StringBuilder();
            var sourceControl = new StringBuilder();
            foreach (var container in _containers)
            {
                sw.Restart();
                foreach (var pair in container.Value.Jobs)
                {
                    foreach (var process in pair.Processes)
                    {
                        process.Value.Execute(container.Value, container.Value.Setting.Value, pair.Setting.Value, in log, in sourceControl);
                    }
                }

                log.AppendLine($"Execute finish in :{sw.ElapsedMilliseconds} ms");
                Debug.Log($"Execute finish in :{sw.ElapsedMilliseconds} ms");
                if (!string.IsNullOrEmpty(logPath))
                {
                    Directory.CreateDirectory(logPath);
                    var fullLogPath = Path.Combine(logPath, string.Format(DefaultLogName, DateTime.Now.ToString("yyyyMMdd_HHmmss")) + DefaultExtension);
                    File.AppendAllText(fullLogPath, log.ToString());
                    log.Clear();
                }

                if (!string.IsNullOrEmpty(sourceControlPath))
                {
                    Directory.CreateDirectory(sourceControlPath);
                    var fullSourceControlPath = Path.Combine(sourceControlPath, string.Format(DefaultSourceControlName, DateTime.Now.ToString("yyyyMMdd_HHmmss")) + DefaultExtension);
                    File.AppendAllText(fullSourceControlPath, sourceControl.ToString());
                    if (_sourceControlEnable)
                    {
                        CommitToSourceControl(sourceControl);
                    }
                    sourceControl.Clear();
                }
            }
        }

        private void CommitToSourceControl(StringBuilder sourceControlSb)
        {
            var helper = sourceControlHelper?.CreateInstance();
            if (helper == null)
            {
                Debug.LogError("Source control helper type is not selected.");
                return;
            }

            using (StringReader reader = new StringReader(sourceControlSb.ToString()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    helper.Add(line.Trim());
                }
            }
            helper.Commit($"[EditorJob] {name} commit at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            helper.Push();
        }

        public IEnumerable<Type> GetFilteredTypeList()
        {
            var q = typeof(ISourceControlHelper).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsInterface)
                .Where(x => typeof(ISourceControlHelper).IsAssignableFrom(x));

            return q;
        }
    }
}
