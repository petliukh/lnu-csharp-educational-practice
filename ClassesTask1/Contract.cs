using System.Reflection;
using System.Collections.ObjectModel;

namespace LNUCSharp.Task1 
{
    class Contract
    {
        private int? _ID;
        private string? _contractorFirstName;
        private string? _contractorLastName;
        private string? _contractorEmail;   
        private string? _contractorPhoneNumber;
        private string? _contractorIBAN;
        private DateOnly? _startDate;
        private DateOnly? _endDate;
        private Dictionary<string, string> _errors = new Dictionary<string, string>();
        private static readonly ReadOnlyCollection<string> _keyProperties = new ReadOnlyCollection<string>(
        new[] {
            "ID",
            "ContractorFirstName",
            "ContractorLastName",
            "ContractorEmail",
            "ContractorPhoneNumber",
            "ContractorIBAN",
            "StartDate",
            "EndDate"
        });    

        public Contract()
        {
        }

        public Contract(
            int? ID,
            string? contractorFirstName,
            string? contractorLastName,
            string? contractorEmail,
            string? contractorPhoneNumber,
            string? contractorIBAN,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            this.ID = ID;
            this.ContractorFirstName = contractorFirstName;
            this.ContractorLastName = contractorLastName;
            this.ContractorEmail = contractorEmail;
            this.ContractorPhoneNumber = contractorPhoneNumber;
            this.ContractorIBAN = contractorIBAN; 
            this.StartDate = startDate;
            this.EndDate = endDate;
        }

        public Contract(Dictionary<string, string> data)
        {
            this.ParseData(data);
        }

        public void ParseData(Dictionary<string, string> data, bool partialEdit = false)
        {
            foreach (string propName in KeyProperties)
            {
                PropertyInfo? p = this.GetType().GetProperty(propName);

                if (p == null)
                    continue;

                string? inputData;
                Type type = p.PropertyType; 
                data.TryGetValue(p.Name, out inputData);

                if (inputData == null)
                {
                    if (!partialEdit)
                        this._errors.Add(
                            p.Name,
                            "Read error. No data provided for the field");
                    continue;
                }

                object? parsedData = Helpers.ParseFromString(type, inputData);
                
                if (parsedData == null)
                {
                    this._errors.Add(
                        p.Name,
                        String.Format(
                            "Parsing error. Input string does not respresent a {0} type", 
                            p.PropertyType.ToString()));
                    continue;
                }
               
                p.SetValue(
                    this, 
                    parsedData);
            }
        }

        public int? ID 
        {
            get => _ID;
            set
            {
                if (value == null)
                    return;
                if (value > 0)
                {
                    _ID = value;
                }
                else
                {
                    this._errors.Add("ID", "Logic error: ID must be a positive value");
                }
            }
        }

        public string? ContractorFirstName
        {
            get => _contractorFirstName;
            set 
            {
                if (value == null)
                    return;
                if (ContractValidators.ValidateName(value))
                {
                    _contractorFirstName = value;
                }
                else
                {
                    this._errors.Add(
                        "ContractorFirstName", 
                        "Formatting error: Name must start with capital letter and container only alphabetical values");
                }
            }
        }

        public string? ContractorLastName
        {
            get => _contractorLastName;
            set 
            {
                if (value == null)
                    return;
                if (ContractValidators.ValidateName(value))
                {
                    _contractorLastName = value;
                }
                else
                {
                    this._errors.Add(
                        "ContractorLastName", 
                        "Formatting error: Surname must start with capital letter and container only alphabetical values");
                }
            }
        }

        public string? ContractorEmail
        {
            get => _contractorEmail;
            set 
            {
                if (value == null)
                    return;
                if (ContractValidators.ValidateEmail(value))
                {
                    _contractorEmail = value;
                }
                else 
                {
                    this._errors.Add(
                        "ContractorEmail",
                        "Formatting error: Wrong email format");
                }
            }
        }

        public string? ContractorPhoneNumber
        {
            get => _contractorPhoneNumber;
            set 
            {
                if (value == null)
                    return;
                if (ContractValidators.ValidatePhoneNumberUA(value))
                {
                    _contractorPhoneNumber = value;
                }
                else 
                {
                    this._errors.Add(
                        "ContractorPhoneNumber",
                        "Formatting error: Wrong ukrainian number format");
                }
            }
        }

        public string? ContractorIBAN
        {
            get => _contractorIBAN;
            set 
            {
                if (value == null)
                    return;
                if (ContractValidators.ValidateIBAN(value))
                {
                    _contractorIBAN = value;
                }
                else 
                {
                    this._errors.Add(
                        "ContractorIBAN",
                        "Formatting error: Wrong IBAN format");
                }
            }
        }

        public DateOnly? StartDate
        {
            get => this._startDate;
            set
            {
                this._startDate = value;
            }
        }

        public DateOnly? EndDate
        {
            get => this._endDate;
            set
            {
                this._endDate = value;
            }
        }

        public static ReadOnlyCollection<string> KeyProperties
        {
            get => _keyProperties;
        }

        private bool ValidateLogic()
        {
            if (this.StartDate == null && this.EndDate == null)
                return false;
            if (this.StartDate > this.EndDate)
            {
                this._errors.Add(
                    "StartDate",
                    "Start date can not be earlier than end date");
                return false;
            }
            return true;
        }

        public Dictionary<string, string> GetErrors()
        {
            this.ValidateLogic();
            return this._errors;
        }

        public override string ToString()
        {
            string repr = "";
            foreach (PropertyInfo prop in this.GetType().GetProperties())
            {
                repr += prop.Name + ": " + Convert.ToString(prop.GetValue(this, null));
                repr += "\n";
            }
            return repr;
        }

        public Dictionary<string, string> ToDictionary()
        {
            var serializedInstance = new Dictionary<string, string>();

            foreach (var pName in KeyProperties)
            {
                var p = this.GetType().GetProperty(pName);
                var value = p!.GetValue(this);
                if (value is null)
                    continue;
                string valueRepr = value.ToString() ?? "";
                serializedInstance.Add(pName, valueRepr);
            }
            return serializedInstance;
        }
    }
}