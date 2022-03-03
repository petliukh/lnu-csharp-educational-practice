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
            var keyProps = Helpers.GetKeyProperties(valType);

            foreach (var entry in data)
            {
                var instance = (TVal?)Activator.CreateInstance(valType, new object[] { entry });
                var key = (TKey?)Helpers.GetPrimaryKey(instance);
                this.Add(
                    key!,
                    instance!);
            }
        }

        public void UpdateFile(string filePath)
        {
            var dictList = new List<Dictionary<string, string>>();
            foreach (var entry in this.Values)
            {
                var toDictMtd = typeof(TVal).GetMethod("ToDictionary");

                if (toDictMtd is null)
                    continue;
                
                var dictRepr = (Dictionary<string, string>?)toDictMtd.Invoke(entry, new object[] {});

                if (dictRepr is null)
                    continue;

                dictList.Add(dictRepr);
            }

            string contents = JsonSerializer.Serialize(dictList);
            File.WriteAllText(filePath, contents);
        }

        public void InputData()
        {
            Type valType = typeof(TVal);
            var keyProps = Helpers.GetKeyProperties(valType);
            Dictionary<string, string> inputData = Helpers.InputTypeProperties(valType, keyProps);
            var instance = (TVal?)Activator.CreateInstance(typeof(TVal), new object[] { inputData });
            var key = (TKey?)Helpers.GetPrimaryKey(instance);

            this.Add(
                key!,
                instance!);
        }

        public void EditEntry(TKey ID, Dictionary<string, string> data)
        {
            TVal? entry;
            Type tVal = typeof(TVal);
            this.TryGetValue(ID, out entry);
            var parseMethod = tVal.GetMethod("ParseData");

            if (entry is null)
            {
                Console.WriteLine("Object with given key was not found");
                return;
            }
            if (parseMethod is null)
            {
                Console.WriteLine("Given object has not 'ParseData' method");
                return;
            }
            parseMethod.Invoke(entry, new object[] { data, true });
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