namespace LNUCSharp.Task1
{
    class Program
    {
        public static void Main(string[] args)
        {
            Menu<int, Contract> menu = new Menu<int, Contract>();
            menu.MainLoop();
        }
    }
}
