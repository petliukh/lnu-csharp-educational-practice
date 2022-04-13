using System.Text.Json;

namespace LNUCSharp.Task2
{

    class ModelContainer<TKey, TVal> : Dictionary<TKey, TVal>
        where TKey : notnull
        where TVal : IContainable, new()
    {
        public virtual List<TVal> ReadFile(string fileName)
        {
            string fileContent = File.ReadAllText(fileName);
            var data = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(fileContent);
            var instances = new List<TVal>();

            if (data == null)
            {
                Console.WriteLine("File either has not content or it is in the wrong format");
                return instances;
            }

            foreach (var entry in data)
            {
                var instance = new TVal();
                Helpers.ParseData(ref instance, entry, instance.KeyProperties);
                var pkey = (TKey)instance.GetPrimaryKey();

                try
                {
                    this.Add(
                        pkey,
                        instance!);
                    instances.Add(instance);
                }
                catch (System.ArgumentException)
                {
                    Console.WriteLine("Object with such ID is already present in the collection");
                }
            }
            return instances;
        }

        public void WriteToFile(string filePath)
        {
            var dictList = new List<Dictionary<string, string>>();

            foreach (var instance in this.Values)
            {
                Dictionary<string, string> dictRepr = Helpers.ToDictionary(instance, instance.KeyProperties);

                if (dictRepr is null)
                    continue;
                dictList.Add(dictRepr);
            }

            string content = JsonSerializer.Serialize(dictList);
            File.WriteAllText(filePath, content);
        }

        public TVal InputData(bool dontAddIfErrors = false)
        {
            var instance = new TVal();
            Dictionary<string, string> inputData = Helpers.InputTypeProperties(instance.KeyProperties);
            Helpers.ParseData(ref instance, inputData, instance.KeyProperties);
            var key = (TKey)instance.GetPrimaryKey();

            if (dontAddIfErrors && instance.Errors.Count() > 0)
                return instance;

            this.Add(
                key,
                instance!);
            return instance;
        }

        public TVal? EditEntry(TKey ID, Dictionary<string, string> data)
        {
            TVal? instance;
            this.TryGetValue(ID, out instance);

            if (instance is null)
            {
                Console.WriteLine("Object with given key was not found");
                return instance;
            }
            Helpers.ParseData(ref instance, data, instance.KeyProperties, true);
            return instance;
        }

        public List<TVal> Search(string searchQuery)
        {
            List<TVal> occurences = new List<TVal>();

            foreach (var item in this.Values)
            {
                if (item == null)
                    continue;
                string repr = item.ToString() ?? "";
                int index = repr.ToString().ToLower().IndexOf(searchQuery.ToLower());
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
