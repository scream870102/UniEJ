using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Scream.UniEJ.Editor
{
    [CreateAssetMenu(fileName = "TextureSizeSetProcess", menuName = "Scream/UniEJ/EditorJob/Process/TextureSizeSetProcess", order = 0)]
    public class TextureSizeSetProcess : BaseTextureRefreshProcess
    {
        private const string SizeLabelPrefix = "Size";
        private static int[] _validTextureSize = { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
        protected override bool ProcessInternal(TextureImporter importer, TextureImporterPlatformSettings originalSetting, TextureImporterPlatformSettings targetSetting)
        {
            var path = importer.assetPath;
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            var labels = AssetDatabase.GetLabels(asset);
            int index = System.Array.FindIndex(_validTextureSize, size => labels.Contains(SizeLabelPrefix + size));

            if (index != -1)
            {
                int targetSize = _validTextureSize[index];
                originalSetting.maxTextureSize = targetSize;
                Debug.Log($"Set max size to {targetSize}");
            }
            importer.SetPlatformTextureSettings(originalSetting);
            return index != -1;
        }
    }
}
