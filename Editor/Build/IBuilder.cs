namespace Scream.UniEJ.Editor
{
    public interface IBuilder<Arguments> where Arguments : Editor.Arguments
    {
        string GetOutputDir(Arguments args);
        string GetOutputName(Arguments args);
        string GetExtension(Arguments args);
    }
    public interface IBuilder<Arguments, BuildHelper> : IBuilder<Arguments> where Arguments : Editor.Arguments where BuildHelper : BuildHelper<Arguments>
    {
        void Build(Arguments args, BuildHelper buildHelper);
    }
}
