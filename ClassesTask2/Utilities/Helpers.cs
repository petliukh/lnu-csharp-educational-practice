using System.Security.Cryptography;
using System.Text;
using System.Reflection;

namespace LNUCSharp.Task2
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

		public static void ParseData<T>(
            ref T instance,
            Dictionary<string, string> data, 
			Dictionary<string, FieldAttributes> keyProperties,
            bool partialEdit = false,
            bool inputMode = false)
        {
            foreach (var (key, attr) in keyProperties)
            {
                if ((attr & FieldAttributes.Write) == FieldAttributes.Write && !inputMode ||
                    (attr & FieldAttributes.Input) == FieldAttributes.Input)
                {
                    PropertyInfo? p = instance!.GetType()!.GetProperty(key);

                    if (p == null)
                        continue;

                    string? inputValue;
                    data.TryGetValue(p.Name, out inputValue);
                    object? parsedData = Helpers.ParseFromString(p.PropertyType, inputValue ?? "");

                    p.SetValue(
                        instance, 
                        parsedData);
                }
            }
        }
		
        public static Dictionary<string, string> ToDictionary<T>(
            T instance, 
            Dictionary<string, FieldAttributes> keyProperties)
        {
            var serializedInstance = new Dictionary<string, string>();

            foreach (var (propName, attr) in keyProperties)
            {
                if ((attr & FieldAttributes.Write) == FieldAttributes.Write ||
                    (attr & FieldAttributes.Input) == FieldAttributes.Input) 
                {
                    var p = instance!.GetType()!.GetProperty(propName);
                    var value = p!.GetValue(instance);
                    if (value is null)
                        continue;
                    string valueRepr = value.ToString() ?? "";
                    serializedInstance.Add(propName, valueRepr);
                }
            }
            return serializedInstance;
        }
 
        public static Dictionary<string, string> InputTypeProperties(
            Dictionary<string, FieldAttributes> keyProperties)
        {
            Dictionary<string, string> inputData = new Dictionary<string, string>();

            foreach (var (propName, attr) in keyProperties)
            {
                if ((attr & FieldAttributes.Input) == FieldAttributes.Input) 
                {
                    Console.WriteLine(propName + ": ");
                    inputData.Add(propName, Console.ReadLine() ?? "");
                }
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

		public static string GetHash(string val)
		{
			using (var sha256 = SHA256.Create())  
			{  
				var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(val));  
				return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();  
			}  
		}

		public static string GetSalt()  
		{  
			byte[] bytes = new byte[128 / 8];  
			using(var keyGenerator = RandomNumberGenerator.Create())  
			{  
				keyGenerator.GetBytes(bytes);  
				return BitConverter.ToString(bytes).Replace("-", "").ToLower();  
			}  
		}
	}
}
