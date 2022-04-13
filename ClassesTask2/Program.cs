
namespace LNUCSharp.Task2
{
	class Program 
	{
		public static void Main(string[] args)
		{
			Menu<int, Contract> menu = new Menu<int, Contract>(args);
            menu.MainLoop();
		}
	}
}
