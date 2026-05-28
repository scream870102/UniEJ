using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Scream.UniEJ.Editor
{
    public static class ExampleConfigHelper
    {
        public static void HandleEditorJob(string[] editorJobPath)
        {
            if (editorJobPath != null && editorJobPath.Length > 0)
            {
                var sw = new Stopwatch();
                for (int i = 0; i < editorJobPath.Length; i++)
                {
                    var path = editorJobPath[i];

                    Debug.Log($"Starting EditorJobHelper {i + 1}/{editorJobPath.Length} at path {path}");

                    var jobHelper = AssetDatabase.LoadAssetAtPath<EditorJobHelper>(path);
                    if (jobHelper == null)
                    {
                        Debug.LogError($"Can't load EditorJobHelper at path {path}");
                        continue;
                    }
                    sw.Restart();
                    jobHelper.Execute();
                    Debug.Log($"Executed EditorJobHelper at path {path} in {sw.ElapsedMilliseconds} ms");
                    sw.Stop();
                }
            }
        }
    }
}
