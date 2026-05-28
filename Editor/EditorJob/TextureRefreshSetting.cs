using UnityEditor;
using UnityEngine;

namespace Scream.UniEJ.Editor
{
    [CreateAssetMenu(fileName = "TextureRefreshSetting", menuName = "Scream/UniEJ/EditorJob/Setting/TextureRefreshSetting", order = 0)]
    public class TextureRefreshSetting : ScriptableObject, IEditorJobSetting
    {
        public TextureImporterPlatformSettings[] Settings => settings;
        [SerializeField] protected TextureImporterPlatformSettings[] settings;
    }
}
