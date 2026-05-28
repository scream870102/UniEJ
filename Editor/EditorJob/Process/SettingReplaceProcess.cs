using UnityEditor;
using UnityEngine;

namespace Scream.UniEJ.Editor
{
    [CreateAssetMenu(fileName = "SettingReplaceProcess", menuName = "Scream/UniEJ/EditorJob/Process/SettingReplaceProcess", order = 0)]
    public class SettingReplaceProcess : BaseTextureRefreshProcess
    {
        protected override bool ProcessInternal(TextureImporter importer, TextureImporterPlatformSettings originalSetting, TextureImporterPlatformSettings targetSetting)
        {
            importer.SetPlatformTextureSettings(targetSetting);
            Debug.Log($"Replace texture setting with {targetSetting}");
            return !IsSame(originalSetting, targetSetting);
        }

        private static bool IsSame(TextureImporterPlatformSettings a, TextureImporterPlatformSettings b)
        {
            if (a == null || b == null) return a == b;

            return a.name == b.name &&
                    a.overridden == b.overridden &&
                    a.ignorePlatformSupport == b.ignorePlatformSupport &&
                    a.maxTextureSize == b.maxTextureSize &&
                    a.resizeAlgorithm == b.resizeAlgorithm &&
                    a.format == b.format &&
                    a.textureCompression == b.textureCompression &&
                    a.compressionQuality == b.compressionQuality &&
                    a.crunchedCompression == b.crunchedCompression &&
                    a.allowsAlphaSplitting == b.allowsAlphaSplitting &&
                    a.androidETC2FallbackOverride == b.androidETC2FallbackOverride;
        }


    }
}
