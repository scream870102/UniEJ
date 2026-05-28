using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Scream.UniEJ.Editor
{
    public abstract class BaseTextureRefreshProcess : ScriptableObject, IEditorJobProcess
    {
        public virtual void Execute(IEditorJobContainer container, IEditorJobContainerSetting containerSetting, IEditorJobSetting setting, in StringBuilder log, in StringBuilder sourceControl)
        {
            if (containerSetting is not PathContainerSetting || setting is not TextureRefreshSetting)
            {
                var tmp = $"Parameter wrong Type \n" +
                            $"Container Setting : {containerSetting.GetType()} Need : {typeof(PathContainerSetting)} \n" +
                            $"Setting : {setting.GetType()} Need : {typeof(TextureRefreshSetting)}";
                Debug.LogError(tmp);
                log.AppendLine(tmp);
                return;
            }

            var conSet = containerSetting as PathContainerSetting;
            var set = setting as TextureRefreshSetting;

            var sb = new StringBuilder();
            var importers = GetImporters(conSet);
            var isBatchMode = Application.isBatchMode;

            foreach (var importer in importers)
            {
                bool anythingChange = false;
                foreach (var platformSetting in set.Settings)
                {
                    var originalSetting = importer.GetPlatformTextureSettings(platformSetting.name);
                    var isChange = ProcessInternal(importer, originalSetting, platformSetting);
                    if (isChange)
                    {
                        anythingChange = true;
                    }
                }
                if (anythingChange)
                {
                    var fullPath = GetFullPath(importer);
                    fullPath = $"{fullPath}.meta";
                    sb.AppendLine(fullPath);

                    if (isBatchMode)
                    {
                        // In CI batchmode: only write the .meta file.
                        // Avoid triggering texture recompression (e.g. ASTC encoder) which
                        // crashes in Windows containers that lack the required CPU extensions.
                        AssetDatabase.WriteImportSettingsIfDirty(importer.assetPath);
                    }
                    else
                    {
                        importer.SaveAndReimport();
                    }
                }
                Debug.Log($"Refresh {importer.assetPath} setting");
            }

            if (!isBatchMode)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
            log.AppendLine(sb.ToString());
            sourceControl.AppendLine(sb.ToString());
        }


        public static string GetFullPath(TextureImporter importer)
        {
            var fullPath = $"{Path.Combine(Directory.GetCurrentDirectory(), importer.assetPath)}";
#if UNITY_EDITOR_WIN
            fullPath = fullPath.Replace("/", "\\");
#endif
            return fullPath;
        }

        protected abstract bool ProcessInternal(TextureImporter importer, TextureImporterPlatformSettings originalSetting, TextureImporterPlatformSettings targetSetting);

        public static TextureImporter[] GetImporters(PathContainerSetting setting)
        {
            var result = new List<TextureImporter>();
            var guids = AssetDatabase.FindAssets(setting.filter, new string[] { setting.parentPath });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (setting.ignoreFilePath.Contains(path))
                {
                    continue;
                }

                bool isInIgnoreFolder = false;
                foreach (var folderPath in setting.ignoreFolderPath)
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

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != default)
                {
                    result.Add(importer);
                }
            }

            return result.ToArray();
        }
    }
}
