namespace Scream.UniEJ.Editor
{
    public interface ISourceControlHelper
    {
        void Add(string path);
        void Remove(string path);
        void Commit(string message);
        void Push();
        void Pull();
    }
}
