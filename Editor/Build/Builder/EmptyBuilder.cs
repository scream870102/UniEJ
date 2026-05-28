namespace Scream.UniEJ.Editor
{
    public class EmptyBuilder<Arguments, BuildHelper> : IBuilder<Arguments, BuildHelper> where Arguments : Editor.Arguments where BuildHelper : BuildHelper<Arguments>
    {
        public void Build(Arguments args, BuildHelper buildHelper)
        {
            buildHelper.PreBuild(args,this);
            // Do nothing for empty builder
            buildHelper.PostBuild(args,this);
        }

        public string GetExtension(Arguments args)
        {
            return string.Empty;
        }

        public string GetOutputDir(Arguments args)
        {
            return string.Empty;
        }

        public string GetOutputName(Arguments args)
        {
            return string.Empty;
        }
    }
}
