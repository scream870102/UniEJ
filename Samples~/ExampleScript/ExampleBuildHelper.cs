using System.Collections.Generic;

namespace Scream.UniEJ.Editor
{
    public class ExampleBuildHelper : BuildHelper<Arguments>
    {
        protected override List<IProcessPostBuild<Arguments>> GetPostProcessor()
        {
            return new List<IProcessPostBuild<Arguments>>() {
                new ExampleConfigProcessPostBuild()
                };
        }

        protected override List<IProcessPreBuild<Arguments>> GetPreProcessor()
        {
            return new List<IProcessPreBuild<Arguments>>() {
                new ExampleConfigProcessPreBuild()
                };
        }
    }
}
