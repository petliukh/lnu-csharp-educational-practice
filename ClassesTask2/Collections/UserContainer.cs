using System.Text.Json;

namespace LNUCSharp.Task2
{
    class UserContainer : ModelContainer<string, User>
    {
        public override List<User> ReadFile(string fileName)
        {
            string fileContent = File.ReadAllText(fileName);
            var data = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(fileContent);
            var users = new List<User>();

            if (data == null)
            {
                Console.WriteLine("File either has not content or it is in the wrong format");
                return users;
            }

            foreach (var entry in data)
            {
                string? role = entry.GetValueOrDefault("UserRole");

                if (role == "Staff")
                {
                    var instance = new Staff();
                    Helpers.ParseData(ref instance, entry, instance.KeyProperties);
                    var pkey = (string)instance.GetPrimaryKey();

                    try
                    {
                        this.Add(
                            pkey,
                            instance!);
                        users.Add(instance);
                    }
                    catch (System.ArgumentException)
                    {
                        Console.WriteLine("Object with such ID is already present in the collection");
                    }
                }
                if (role == "Admin")
                {
                    var instance = new Admin();
                    Helpers.ParseData(ref instance, entry, instance.KeyProperties);
                    var pkey = (string)instance.GetPrimaryKey();

                    try
                    {
                        this.Add(
                            pkey,
                            instance!);
                        users.Add(instance);
                    }
                    catch (System.ArgumentException)
                    {
                        Console.WriteLine("Object with such ID is already present in the collection");
                    }
                }
            }
            return users;
        }

        public Staff InputStaffData(bool dontAddIfErrors = false)
        {
            var instance = new Staff();
            Dictionary<string, string> inputData = Helpers.InputTypeProperties(instance.KeyProperties);
            Helpers.ParseData(ref instance, inputData, instance.KeyProperties, false, true);
            var key = (string)instance.GetPrimaryKey();

            if (dontAddIfErrors && instance.Errors.Count() > 0)
                return instance;

            try
            {
                this.Add(
                    key,
                    instance!);
            }
            catch (System.ArgumentException)
            {
                instance.Errors.Add("Unique constraint", "User with such email already exists.");
            }
            return instance;
        }

        public Admin InputAdminData(bool dontAddIfErrors = false)
        {
            var instance = new Admin();
            Dictionary<string, string> inputData = Helpers.InputTypeProperties(instance.KeyProperties);
            Helpers.ParseData(ref instance, inputData, instance.KeyProperties, false, true);
            var key = (string)instance.GetPrimaryKey();

            if (dontAddIfErrors && instance.Errors.Count() > 0)
                return instance;

            try
            {
                this.Add(
                    key,
                    instance!);
            }
            catch (System.ArgumentException)
            {
                instance.Errors.Add("Unique constraint", "User with such email already exists.");
            }
            return instance;
        }
    }
}