using System.Collections.ObjectModel;
using System.Reflection;

namespace LNUCSharp.Task1
{
    class Helpers
    {
        public static object? ParseFromString(Type type, string to_parse)
        {
            if (type == typeof(string))
            {
                return to_parse;
            }
            else if (type == typeof(int?))
            {
                int value;
                if (!int.TryParse(to_parse, out value))
                    return null;
                return value;
            }
            else if (type == typeof(DateOnly?))
            {
                DateOnly value;
                if (!DateOnly.TryParse(to_parse, out value))
                    return null;
                return value;
            }
            else
            {
                return null;
            }
        }

        public static Dictionary<string, string> InputTypeProperties(
            Type type,
            ReadOnlyCollection<string> keyProperties)
        {
            Dictionary<string, string> inputData = new Dictionary<string, string>();
            PropertyInfo[] properties = type.GetProperties();

            foreach (var prop in properties)
            {
                if (!keyProperties.Contains(prop.Name))
                    continue;
                inputData.Add(prop.Name, Console.ReadLine() ?? "");
            }
            return inputData;
        }

        public static Dictionary<string, string> InputDataForEdit()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            string key;
            string value;

            do
            {
                Console.WriteLine("Enter key you want to edit and its value, or exit to stop editing menu: ");
                key = Console.ReadLine() ?? "";
                if (key == "exit")
                    break;
                value = Console.ReadLine() ?? "";
                data.Add(key, value);
            }
            while (true);

            return data;
        }

        public static object GetPrimaryKey<TVal>(TVal? instance)
        {
            if (instance is null)
                throw new NullReferenceException("The instance to get PK from was a null reference");
            var prop = instance.GetType().GetProperty("ID");

            if (prop is null)
                throw new NullReferenceException("The object has not primary key property");
            var PK = prop.GetValue(instance);
            if (PK is null)
                throw new NullReferenceException("The primary key of the instance was a null reference");
            return PK;
        }

        public static ReadOnlyCollection<string> GetKeyProperties(Type type)
        {
            PropertyInfo? prop = type.GetProperty("KeyProperties");

            if (prop is null)
                throw new MissingFieldException("Value type does not have a 'KeyProperties' constant field");
            var keyProps = (ReadOnlyCollection<string>?)prop.GetValue(null);

            if (keyProps is null)
                throw new NullReferenceException("'KeyProperties' field was a null reference");
            return keyProps;
        }
    }
}