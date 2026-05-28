using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Scream.UniEJ.Editor
{
    [CreateAssetMenu(fileName = "PathContainer", menuName = "Scream/UniEJ/EditorJob/ContainerSetting/PathContainer", order = 0)]
    public class PathContainerSetting : ScriptableObject, IEditorJobContainerSetting
    {
        public string parentPath;
        public List<string> ignoreFilePath;
        public List<string> ignoreFolderPath;
        public string filter;

        public IList<string> GetPaths()
        {
            var result = new List<string>();
            var guids = AssetDatabase.FindAssets(string.IsNullOrEmpty(filter) ? string.Empty : filter, new string[] { parentPath });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (ignoreFilePath.Contains(path))
                {
                    continue;
                }

                bool isInIgnoreFolder = false;
                foreach (var folderPath in ignoreFolderPath)
                {
                    if (path.StartsWith(folderPath))
                    {
                        isInIgnoreFolder = true;
                        break;
                    }
                }

                if (isInIgnoreFolder)
                {
                    continue;
                }

                result.Add(path);
            }

            return result;
        }

        public IList<T> GetTypes<T>()
        {
            var result = new List<T>();
            var guids = AssetDatabase.FindAssets(string.IsNullOrEmpty(filter) ? string.Empty : filter, new string[] { parentPath });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (ignoreFilePath.Contains(path))
                {
                    continue;
                }

                bool isInIgnoreFolder = false;
                foreach (var folderPath in ignoreFolderPath)
                {
                    if (path.StartsWith(folderPath))
                    {
                        isInIgnoreFolder = true;
                        break;
                    }
                }

                if (isInIgnoreFolder)
                {
                    continue;
                }

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    continue;
                }

                var comp = prefab.GetComponent<T>();
                if (comp == null)
                {
                    continue;
                }

                result.Add(comp);
            }

            return result;
        }
    }
}
