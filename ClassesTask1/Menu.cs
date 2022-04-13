using System.IO;

namespace LNUCSharp.Task1
{
    class Menu<TKey, TVal>
        where TKey : notnull
        where TVal : new()
    {
        public Menu()
        {
        }

        private Container<TKey, TVal> cont = new Container<TKey, TVal>();
        
        public delegate void MenuFuncDelegate();

        public void InputNewEntry()
        {
            this.cont.InputData();
        }

        public void ReadEntriesFromFile()
        {
            Console.WriteLine("Enter name of the file to read from: ");
            string? fileName = Console.ReadLine();
            try
            {
                this.cont.ReadFile(fileName ?? "");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Specified file does not exist");
            }
        }

        public void EditEntry()
        {
            var parseMtd = typeof(TKey).GetMethod("Parse", new[] { typeof(string) });
            if (parseMtd is null)
                return;
            Console.WriteLine("Enter ID of the entry you want to edit: ");
            var PK = (TKey?)parseMtd.Invoke(null, new object[] {Console.ReadLine() ?? ""});

            if (PK is null)
                return;
            var data = Helpers.InputDataForEdit();
            this.cont.EditEntry(PK, data);
        }

        public void UpdateEntriesInFile()
        {
            Console.WriteLine("Enter file path to write to: ");
            this.cont.UpdateFile(Console.ReadLine() ?? "");
        }

        public void SortEntries()
        {
            Console.WriteLine("Enter property name to sort entries by: ");
            string? pts = Console.ReadLine();
            var orderedEntries = this.cont.SortByField(pts ?? "");
            foreach (var entry in orderedEntries)
            {
                if (entry == null)
                    continue;
                Console.WriteLine(entry.ToString());
                Console.WriteLine("\n-\n-\n");
            }
        }

        public void SearchInfo()
        {
            string? searchQuery = Console.ReadLine();
            string sep = "-\n-\n-\n";
            var occurences = this.cont.Search(searchQuery ?? "");
            Console.WriteLine("Found occurences: ");

            foreach (var ocr in occurences)
            {
                if (ocr == null)
                    continue;
                Console.WriteLine(ocr.ToString());
                Console.WriteLine(sep);
            }
        }

        public void ReportErrors()
        {
            var errorsProp = typeof(TVal).GetProperty("Errors");
            if (errorsProp == null)
                return;
            foreach (var entry in this.cont.Values)
            {
                var errors = (Dictionary<string, string>?)errorsProp.GetValue(entry);
                if (errors == null)
                    continue;
                Console.WriteLine(string.Join(Environment.NewLine, errors));
            }
        }

        public void ShowEntries()
        {
            foreach (var entry in this.cont.Values)
            {
                Console.WriteLine(entry!.ToString());
                Console.WriteLine("\n\n");
            }
        }

        public void Manual()
        {
            var manual = this.GetManual();
            string mrepr = string.Join(Environment.NewLine, manual);
            Console.WriteLine(mrepr);
        }

        Dictionary<string, MenuFuncDelegate> GetMenuOptions()
        {
            Dictionary<string, MenuFuncDelegate> options = new Dictionary<string, MenuFuncDelegate>()
            {
                {"help", this.Manual},
                {"ipt", this.InputNewEntry},
                {"rd", this.ReadEntriesFromFile},
                {"edit", this.EditEntry},
                {"wrt", this.UpdateEntriesInFile},
                {"shw", this.ShowEntries},
                {"sch", this.SearchInfo},
                {"srt", this.SortEntries},
                {"rpt", this.ReportErrors}
            };
            return options;
        }

        Dictionary<string, string> GetManual()
        {
            Dictionary<string, string> manual = new Dictionary<string, string>()
            {
                {"ipt", "Add new contract entry to the container inputting data manually with keyboard"},
                {"rd", "Add new contract entries to the container by parsing JSON file"},
                {"edit", "Edit a contract entry specified by its ID"},
                {"wrt", "Write entries from the container to a file"},
                {"shw", "Output all entries from the container to the console"},
                {"sch", "Find search query occurences across all entries"},
                {"srt", "Display all entries sorted by specified property"},
                {"rpt", "Display all errors which occured while parsing input data"},
                {"exit", "Exit the programm"}
            };
            return manual;
        }

        public void MainLoop()
        {
            Console.WriteLine("Run 'help' command to list availiable options and read their manual");
            var options = this.GetMenuOptions();
            string? command;

            do
            {
                Console.WriteLine("Enter your command: ");

                command = Console.ReadLine();
                var menuFunc = options!.GetValueOrDefault(command);

                if (menuFunc != null)
                    menuFunc();
                else
                    Console.WriteLine("Invalid command! Try again");
            }
            while (command != "exit");
        }
    }
}
