using System.Collections.ObjectModel;
using System.Reflection;

namespace LNUCSharp.Task1
{
    class Helpers
    {
        public static object? ParseFromString(Type type, string toParse)
        {
            if (type == typeof(string))
            {
                return toParse;
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                int value;
                if (!int.TryParse(toParse, out value))
                    return null;
                return value;
            }
            else if (type == typeof(DateOnly) || type == typeof(DateOnly?))
            {
                DateOnly value;
                if (!DateOnly.TryParse(toParse, out value))
                    return null;
                return value;
            }
            else
            {
                return null;
            }
        }

		public static Dictionary<string, string> ParseData<T>(
            ref T instance,
            Dictionary<string, string> data, 
			List<string> keys,
            bool partialEdit = false)
        {
			Dictionary<string, string> errors = new Dictionary<string, string>();

            foreach (string key in keys)
            {
                PropertyInfo? p = instance!.GetType()!.GetProperty(key);

                if (p == null)
                    continue;

                string? inputValue;
                data.TryGetValue(p.Name, out inputValue);

                if (inputValue == null)
                {
                    if (!partialEdit)
                        errors.Add(
                            p.Name,
                            "Read error. No data provided for the field");
                    continue;
                }

                object? parsedData = Helpers.ParseFromString(p.PropertyType, inputValue);
                
                if (parsedData == null)
                {
                    errors.Add(
                        p.Name,
                        String.Format(
                            "Parsing error. Input string does not respresent a {0} type", 
                            p.PropertyType.ToString()));
                    continue;
                }
                p.SetValue(
                    instance, 
                    parsedData);
            }
			return errors;
        }
		
        public static Dictionary<string, string> ToDictionary<T>(T instance, List<string> keys)
        {
            var serializedInstance = new Dictionary<string, string>();

            foreach (var key in keys)
            {
                var p = instance!.GetType()!.GetProperty(key);
                var value = p!.GetValue(instance);
                if (value is null)
                    continue;
                string valueRepr = value.ToString() ?? "";
                serializedInstance.Add(key, valueRepr);
            }
            return serializedInstance;
        }
 
        public static Dictionary<string, string> InputTypeProperties(
            Type type,
            List<string> keys)
        {
            Dictionary<string, string> inputData = new Dictionary<string, string>();
            PropertyInfo[] properties = type.GetProperties();

            foreach (var prop in properties)
            {
                if (!keys.Contains(prop.Name))
                    continue;
                Console.WriteLine(prop.Name + ": ");
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

				Console.WriteLine("Key: ");
                key = Console.ReadLine() ?? "";
                if (key == "exit")
                    break;

				Console.WriteLine("Value: ");
                value = Console.ReadLine() ?? "";

                data.Add(key, value);
            }
            while (true);

            return data;
        }

        public static List<string> GetKeys(Type type)
        {
            var keys = type.GetProperty("Keys");
            if (keys is null)
                return new List<string>();
            return (List<string>)keys.GetValue(null);
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
    }
}
