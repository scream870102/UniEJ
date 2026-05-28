namespace Scream.UniEJ.Editor
{
    public abstract class ConfigProcessPostBuild<Config, Args> : ConfigProcessPreBuild<Config, Args>, IProcessPostBuild<Args> where Args : Arguments
    {
    }
}
