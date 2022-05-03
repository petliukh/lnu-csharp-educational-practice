
namespace LNUCSharp.Task2
{
    public enum Role
    {
        Admin, 
        Staff
    }

    public enum Status
    {
        Draft,
        Approved,
        Rejected
    }

    [Flags]
    public enum FieldAttributes
    {
        Read = 0,
        Write = 1,
        Input = 2
    }
}