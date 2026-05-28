namespace Scream.UniEJ.Editor
{
    public class ExampleConfigProcessPreBuild : ConfigProcessPreBuild<ExampleConfig, Arguments>
    {
        protected override void ProcessInternal(Arguments args, ExampleConfig config, IBuilder<Arguments> builder)
        {
            ExampleConfigHelper.HandleEditorJob(config.editorJobPath);
        }
    }
}
