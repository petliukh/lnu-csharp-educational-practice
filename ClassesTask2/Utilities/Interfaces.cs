
namespace LNUCSharp.Task2
{
    public interface IContainable
    {
        Dictionary<string, FieldAttributes> KeyProperties { get; }
        Dictionary<string, string> Errors { get; }
        object GetPrimaryKey();
    }
}