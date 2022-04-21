
namespace LNUCSharp.Task2
{
    class Entry<ModelType>
    {
        public User user;
        public ModelType model;
        public Status status;
        public string? message;

        public Entry(User user, ModelType model, Status status, string? message = null)
        {
            this.user = user;
            this.model = model;
            this.status = status;
            this.message = message;
        }

        public override string ToString()
        {
            string? repr = model!.ToString();
            repr = String.Format(
                "--- Status: {0} ---\n--- Message: {1} ---\n", 
                status, message) + repr;
            return repr;
        }
    }

    class Menu<TKey, TVal>
        where TKey : notnull
        where TVal : IContainable, new()
    {
        public Menu(string[] args)
        {
            try
            {
                User.Objects.ReadFile("Data/Users.json");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("No users have registered yet.");
            }

            if (args.Contains("createsuperuser"))
            {
                RegisterSuperUser();
            }
        }

        private ModelContainer<TKey, TVal> models = new ModelContainer<TKey, TVal>();
        List<Entry<TVal>> _entries = new List<Entry<TVal>>();
        private User? currentlyLoggedInUser;

        public delegate void MenuFuncDelegate();

        public void ShowApprovedEntries()
        {
            if (currentlyLoggedInUser!.UserRole == Role.Staff)
            {
                ((Staff)currentlyLoggedInUser).ShowApprovedEntries(_entries);
            }
            else
            {
                Console.WriteLine("Only staff can do this action.");
            }
        }
        
        public void ShowDraftEntries()
        {
            if (currentlyLoggedInUser!.UserRole == Role.Admin)
            {
                ((Admin)currentlyLoggedInUser).ShowEntriesForReview(_entries);
            }
            else
            {
                Console.WriteLine("Only admins can do this action.");
            }
        }

        public void ShowOwnEntries()
        {
            if (currentlyLoggedInUser!.UserRole == Role.Staff)
            {
                ((Staff)currentlyLoggedInUser).ShowOwnEntries(_entries);
            }
            else
            {
                Console.WriteLine("Only staff can do this action.");
            }
        }

        public void RegisterSuperUser()
        {
            bool registered = false;

            while (!registered)
            {
                var user = User.Objects.InputAdminData(dontAddIfErrors: true);

                if (user.Errors.Count() > 0)
                {
                    Console.WriteLine(
                        "Invalid data was given. " +
                        "Could not register the user. Please, try again.");
                    Console.WriteLine(
                        String.Join("\n", user.Errors));
                }
                else
                {
                    Console.WriteLine("User {0} was registered successfully", user.ToStringShort());
                    User.Objects.WriteToFile("Data/Users.json");
                    registered = true;
                }
            }
        }

        public void RegisterUser()
        {
            var user = User.Objects.InputStaffData(dontAddIfErrors: true);

            if (user.Errors.Count() > 0)
            {
                Console.WriteLine(
                    "Invalid data was given. Could not register the user. Please, try again.");
                Console.WriteLine(
                    String.Join("\n", user.Errors));
            }
            else
            {
                Console.WriteLine(
                    "User {0} was registered successfully",
                    user.ToStringShort());
                User.Objects.WriteToFile("Data/Users.json");
            }
        }

        public void LoginUser()
        {
            Console.WriteLine("Email:");
            string email = Console.ReadLine() ?? "";
            Console.WriteLine("Password:");
            string password = Console.ReadLine() ?? "";

            User? user;
            user = User.Objects.GetValueOrDefault(email);

            if (user is null)
            {
                Console.WriteLine("User with such email does not exist.");
                return;
            }
            if (user.Authenticate(password))
            {
                currentlyLoggedInUser = user;
                Console.WriteLine("User {0} logged in successfully!", user.ToStringShort());
            }
            else
            {
                Console.WriteLine("Password did not match. Try again");
            }
        }

        public void ShowCurrentlyLoggedInUser()
        {
            if (currentlyLoggedInUser is null)
                Console.WriteLine(
                    "No user is currently logged in. " + 
                    "Please, login or register a new account.");
            else
                Console.WriteLine(currentlyLoggedInUser);
        }

        public void InputNewEntry()
        {
            var model = models.InputData();
            Entry<TVal> entry = new Entry<TVal>(
                currentlyLoggedInUser!, model, Status.Draft);

            _entries.Add(entry);
        }

        public void ReadEntriesFromFile()
        {
            Console.WriteLine("Enter name of the file to read from: ");
            string? fileName = Console.ReadLine();
            try
            {
                var instances = models.ReadFile(fileName ?? "");
                foreach (var instance in instances)
                {
                    _entries.Add(new Entry<TVal>(
                        currentlyLoggedInUser!,
                        instance,
                        Status.Draft));
                }
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
            var edited_instace = models.EditEntry(PK, data);
            if (edited_instace != null)
            {
                _entries.Add(new Entry<TVal>(
                    currentlyLoggedInUser!,
                    edited_instace,
                    Status.Draft));
            }
        }

        public void ReviewEntries()
        {
            if (currentlyLoggedInUser!.UserRole == Role.Admin)
            {
                var parseMtd = typeof(TKey).GetMethod("Parse", new[] { typeof(string) });
                if (parseMtd is null)
                    return;
                Console.WriteLine("Enter ID of the entry you want to approve: ");
                var PK = (TKey?)parseMtd.Invoke(null, new object[] {Console.ReadLine() ?? ""});

                if (PK is null)
                    return;

                ((Admin)currentlyLoggedInUser).ReviewEntry(PK, _entries);
            }
            else
            {
                Console.WriteLine("Only admins can do this action.");
            }
        }


        public void RemoveEntry()
        {
            var key = (TKey?)Helpers.ParseFromString(typeof(TKey), Console.ReadLine() ?? "");

            if (key != null)
                models.Remove(key);
            else
                Console.WriteLine("No entry with such PK found.");
        }

        public void UpdateEntriesInFile()
        {
            Console.WriteLine("Enter file path to write to: ");
            var approvedEntries = new ModelContainer<TKey, TVal>();
            foreach (var entry in _entries)
            {
                if (entry.status == Status.Approved)
                {
                    TKey key = (TKey)entry.model.GetPrimaryKey();
                    approvedEntries.Add(key, entry.model);
                }
            }
            models.WriteToFile(Console.ReadLine() ?? "");
        }

        public void SortEntries()
        {
            Console.WriteLine("Enter property name to sort entries by: ");
            string? pts = Console.ReadLine();
            var orderedEntries = models.SortByField(pts ?? "");
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
            var occurences = models.Search(searchQuery ?? "");
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
            foreach (var entry in models.Values)
            {
                var errors = (Dictionary<string, string>?)errorsProp.GetValue(entry);
                if (errors == null)
                    continue;
                Console.WriteLine(string.Join(Environment.NewLine, errors));
            }
        }

        public void ShowEntries()
        {
            foreach (var entry in models.Values)
            {
                Console.WriteLine(entry!.ToString());
                Console.WriteLine("\n\n");
            }
        }

        public void Manual()
        {
            var manual = GetManual();
            string mrepr = string.Join(Environment.NewLine, manual);
            Console.WriteLine(mrepr);
        }

        Dictionary<string, MenuFuncDelegate> GetMenuOptions()
        {
            Dictionary<string, MenuFuncDelegate> options = new Dictionary<string, MenuFuncDelegate>()
            {
                {"help", Manual},
                {"register", RegisterUser},
                {"login", LoginUser},
                {"shwusr", ShowCurrentlyLoggedInUser},
                {"shwpvd", ShowApprovedEntries},
                {"shwown", ShowOwnEntries},
                {"shwdft", ShowDraftEntries},
                {"review", ReviewEntries},
                {"ipt", InputNewEntry},
                {"rmv", RemoveEntry},
                {"rd", ReadEntriesFromFile},
                {"edit", EditEntry},
                {"wrt", UpdateEntriesInFile},
                {"shw", ShowEntries},
                {"sch", SearchInfo},
                {"srt", SortEntries},
                {"rpt", ReportErrors}
            };
            return options;
        }

        Dictionary<string, string> GetManual()
        {
            Dictionary<string, string> manual = new Dictionary<string, string>()
            {
                {"register", "Register a new account"},
                {"login", "Login into your account"},
                {"shwusr", "Show currently logged in user"},
                {"shwdft", "Show draft entries"},
                {"review", "Review entries"},
                {"shwmy", "Show entries added by currently logged in user."},
                {"ipt", "Add new contract entry to the container inputting data manually with keyboard"},
                {"rmv", "Remove the instance specified by its PK from the container"},
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
            var options = GetMenuOptions();
            string? command;

            do
            {
                Console.WriteLine("Enter your command: ");

                command = Console.ReadLine();
                var menuFunc = options!.GetValueOrDefault(command);

                if (command == "login")
                {
                    LoginUser();
                }
                if (command == "register")
                {
                    RegisterUser();
                }
                if (command == "help")
                {
                    Manual();
                }
                else if (menuFunc != null)
                {
                    if (currentlyLoggedInUser == null)
                    {
                        Console.WriteLine(
                            "You are not authnticated. Please, log in " +
                            "or register a new account");
                        continue;
                    }
                    menuFunc();
                }
                else if (command == "exit")
                {
                    break;
                }
                else 
                {
                    Console.WriteLine("Invalid command! Try again");
                }
            }
            while (true);
        }
    }
}
