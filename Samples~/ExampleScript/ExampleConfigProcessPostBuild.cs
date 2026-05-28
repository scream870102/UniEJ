namespace Scream.UniEJ.Editor
{
    public class ExampleConfigProcessPostBuild : ConfigProcessPostBuild<ExampleConfig, Arguments>
    {
        protected override void ProcessInternal(Arguments args, ExampleConfig config, IBuilder<Arguments> builder)
        {
            //Do nothing
        }
    }
}
