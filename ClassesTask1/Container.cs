using System.Text.Json;
using System.IO;

namespace LNUCSharp.Task1
{
    class Container<TKey, TVal> : Dictionary<TKey, TVal>
        where TKey : notnull
        where TVal : new()
    {
        public void ReadFile(string fileName)
        {
            string fileContent = File.ReadAllText(fileName);
            var data = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(fileContent);

            if (data == null)
                throw new FileLoadException("File either has not content or it is in the wrong format");

            Type valType = typeof(TVal);

            foreach (var entry in data)
            {
                var instance = (TVal?)Activator.CreateInstance(valType);
                var errors = Helpers.ParseData(ref instance, entry, Helpers.GetKeys(instance!.GetType()));
                var pkey = (TKey)Helpers.GetPrimaryKey(instance);

				var errorsProp = typeof(TVal).GetProperty("Errors");

				if (errorsProp != null)
				{
					foreach (var err in errors)
					{
						typeof(TVal)!.GetMethod("AddError")!.Invoke(instance, new object[] { err.Key, err.Value });
					}
				}

                try
                {
                    this.Add(
                        pkey!,
                        instance!);
                }
                catch (System.ArgumentException)
                {
					Console.WriteLine("Object with such ID is already present in the collection");
                }
            }
        }

        public void UpdateFile(string filePath)
        {
            var dictList = new List<Dictionary<string, string>>();
            foreach (var entry in this.Values)
            {
				Dictionary<string, string> dictRepr = Helpers.ToDictionary(entry, Helpers.GetKeys(entry!.GetType()));

                if (dictRepr is null)
                    continue;

                dictList.Add(dictRepr);
            }

            string contents = JsonSerializer.Serialize(dictList);
            File.WriteAllText(filePath, contents);
        }

        public void InputData()
        {
            Dictionary<string, string> inputData = Helpers.InputTypeProperties(
                typeof(TVal), Helpers.GetKeys(typeof(TVal)));
            var instance = (TVal?)Activator.CreateInstance(typeof(TVal));
			var errors = Helpers.ParseData(ref instance, inputData, Helpers.GetKeys(typeof(TVal)));
            var key = (TKey?)Helpers.GetPrimaryKey(instance);

			var errorsProp = typeof(TVal).GetProperty("Errors");

			if (errorsProp != null)
			{
				foreach (var err in errors)
				{
					typeof(TVal)!.GetMethod("AddError")!.Invoke(instance, new object[] { err.Key, err.Value });
				}
			}


            this.Add(
                key!,
                instance!);
        }

        public void EditEntry(TKey ID, Dictionary<string, string> data)
        {
            TVal? instance;
            Type tVal = typeof(TVal);
            this.TryGetValue(ID, out instance);

            if (instance is null)
            {
                Console.WriteLine("Object with given key was not found");
                return;
            }
            var errors = Helpers.ParseData(ref instance, data, Helpers.GetKeys(tVal), true);

			var errorsProp = typeof(TVal).GetProperty("Errors");

			if (errorsProp != null)
			{
				foreach (var err in errors)
				{
					typeof(TVal)!.GetMethod("AddError")!.Invoke(instance, new object[] { err.Key, err.Value });
				}
			}
        }

        public List<TVal> Search(string searchQuery)
        {
            List<TVal> occurences = new List<TVal>();

            foreach (var item in this.Values)
            {
                if (item == null)
                    continue;
                string repr = item.ToString() ?? "";
                int index = repr.ToString()!.IndexOf(searchQuery);
                if (index >= 0)
                    occurences.Add(item);
            }
            return occurences;
        }

        public IOrderedEnumerable<TVal> SortByField(string propertyName)
        {
            var values = this.Values;
            var orderedEnumerable = values.OrderBy(
                x =>
                {
                    var prop = x!.GetType().GetProperty(propertyName);
                    if (prop == null)
                        return null;
                    var val = prop.GetValue(x);
                    return val;
                });
            return orderedEnumerable;
        }
    }
}
