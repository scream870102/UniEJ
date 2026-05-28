using System.Collections.Generic;
using Scream.UniEJ.Common;
using UnityEngine;

namespace Scream.UniEJ.Editor
{
    [System.Serializable]
    public struct EditorJobSettingProcessPair
    {
        public InterfaceReference<IEditorJobSetting> Setting;
        public List<InterfaceReference<IEditorJobProcess>> Processes;
    }

    [CreateAssetMenu(fileName = "EditorJobContainer", menuName = "Scream/UniEJ/EditorJob/JobContainer", order = 0)]
    public class EditorJobContainer : ScriptableObject, IEditorJobContainer
    {
        public InterfaceReference<IEditorJobContainerSetting> setting;
        public List<EditorJobSettingProcessPair> jobs;

        public InterfaceReference<IEditorJobContainerSetting> Setting => setting;

        public List<EditorJobSettingProcessPair> Jobs => jobs;
    }
}
