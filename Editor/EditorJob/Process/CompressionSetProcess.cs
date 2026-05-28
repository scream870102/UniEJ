using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Scream.UniEJ.Editor
{
    [CreateAssetMenu(fileName = "CompressionSetProcess", menuName = "Scream/UniEJ/EditorJob/Process/CompressionSetProcess", order = 0)]
    public class CompressionSetProcess : BaseTextureRefreshProcess
    {
        public const string CompressedHQ = nameof(CompressedHQ);
        public const string CompressedLQ = nameof(CompressedLQ);
        public const string Compressed = nameof(Compressed);
        public const string Uncompressed = nameof(Uncompressed);
        protected override bool ProcessInternal(TextureImporter importer, TextureImporterPlatformSettings originalSetting, TextureImporterPlatformSettings targetSetting)
        {
            var result = false;
            var path = importer.assetPath;
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            var labels = AssetDatabase.GetLabels(asset);
            if (labels.Contains(CompressedHQ))
            {
                originalSetting.textureCompression = TextureImporterCompression.CompressedHQ;
                result = true;
            }
            else if (labels.Contains(Compressed))
            {
                originalSetting.textureCompression = TextureImporterCompression.Compressed;
                result = true;
            }
            else if (labels.Contains(CompressedLQ))
            {
                originalSetting.textureCompression = TextureImporterCompression.CompressedLQ;
                result = true;
            }
            else if (labels.Contains(Uncompressed))
            {
                originalSetting.textureCompression = TextureImporterCompression.Uncompressed;
                result = true;
            }

            importer.SetPlatformTextureSettings(originalSetting);
            Debug.Log($"Set texture compression to {originalSetting.textureCompression}");
            return result;
        }
    }
}
