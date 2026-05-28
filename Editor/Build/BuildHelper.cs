using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Scream.UniEJ.Editor
{
    public enum BuildOrder
    {
        First = int.MaxValue,
        AfterFirst = 100,
        Higher = 50,
        Default = default,
        Lower = -50,
        BeforeLast = -100,
        Last = int.MinValue,
    }

    public interface IProcessBuild<T> where T : Arguments
    {
        int Order { get; }
        bool ShouldExecute(T args);
        void Process(T args, IBuilder<T> builder);
    }
    public interface IProcessPreBuild<T> : IProcessBuild<T> where T : Arguments { }

    public interface IProcessPostBuild<T> : IProcessBuild<T> where T : Arguments { }

    public abstract class BuildHelper<T> where T : Arguments
    {
        public List<IProcessPreBuild<T>> PreProcessor { get; private set; }
        public List<IProcessPostBuild<T>> PostProcessor { get; private set; }
        public BuildHelper()
        {
            PreProcessor = InitPreProcessor();
            PostProcessor = InitPostProcessor();

        }

        protected abstract List<IProcessPreBuild<T>> GetPreProcessor();
        protected abstract List<IProcessPostBuild<T>> GetPostProcessor();

        protected List<IProcessPreBuild<T>> InitPreProcessor()
        {
            var result = new List<IProcessPreBuild<T>>();
            result.AddRange(GetPreProcessor());
            result.Sort((x, y) =>
            {
                return x.Order.CompareTo(y.Order);
            });
            result.Reverse();
            return result;
        }

        protected List<IProcessPostBuild<T>> InitPostProcessor()
        {
            var result = new List<IProcessPostBuild<T>>();
            result.AddRange(GetPostProcessor());
            result.Sort((x, y) =>
            {
                return x.Order.CompareTo(y.Order);
            });
            result.Reverse();
            return result;
        }

        public void PreBuild(T args, IBuilder<T> builder)
        {
            Stopwatch sw = new Stopwatch();
            if (PreProcessor == null)
            {
                return;
            }
            foreach (var processor in PreProcessor)
            {
                if (processor.ShouldExecute(args))
                {
                    Debug.Log($"Start process : {processor.GetType().Name}");
                    sw.Restart();
                    processor.Process(args, builder);
                    sw.Stop();
                    Debug.Log($"Stop process : {processor.GetType().Name} with {sw.ElapsedMilliseconds} ms");
                }
                else
                {
                    Debug.Log($"{processor.GetType().Name} been skipped");
                }
            }
        }

        public void PostBuild(T args, IBuilder<T> builder)
        {
            Stopwatch sw = new Stopwatch();
            if (PostProcessor == null)
            {
                return;
            }
            foreach (var processor in PostProcessor)
            {
                if (processor.ShouldExecute(args))
                {
                    Debug.Log($"Start process : {processor.GetType().Name}");
                    sw.Restart();
                    processor.Process(args, builder);
                    sw.Stop();
                    Debug.Log($"Stop process : {processor.GetType().Name} with {sw.ElapsedMilliseconds} ms");
                }
                else
                {
                    Debug.Log($"{processor.GetType().Name} been skipped");
                }
            }
        }
    }
}





