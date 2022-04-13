using System.Reflection;

namespace LNUCSharp.Task2 
{
    class Contract : IContainable
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
            

        public Contract()
        {
        }

        public Contract(
            int? id,
            string? contractorFirstName,
            string? contractorLastName,
            string? contractorEmail,
            string? contractorPhoneNumber,
            string? contractorIBAN,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            ID = id;
            ContractorFirstName = contractorFirstName;
            ContractorLastName = contractorLastName;
            ContractorEmail = contractorEmail;
            ContractorPhoneNumber = contractorPhoneNumber;
            ContractorIBAN = contractorIBAN; 
            StartDate = startDate;
            EndDate = endDate;
        }

        public Dictionary<string, FieldAttributes> KeyProperties
        {
			get
			{
				return new Dictionary<string, FieldAttributes>()
				{
					{ "ID", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input },
					{ "ContractorFirstName", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input },
					{ "ContractorLastName", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input },				
					{ "ContractorEmail", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input },				
					{ "ContractorPhoneNumber", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input },				
					{ "ContractorIBAN", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input },				
					{ "StartDate", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input },				
					{ "EndDate", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input },				
				};
			}
        }

		public Dictionary<string, string> Errors
		{
			get => _errors;
		}

        public object GetPrimaryKey()
        {
            return _ID ?? -1;
        }

        public int? ID 
        {
            get => _ID;
            set
            {
                if (Validators.ValidateInt(value))
                    _ID = value;
                else
                    _errors.Add("ID", "Logic error: ID must be a positive value");
            }
        }

        public string? ContractorFirstName
        {
            get => _contractorFirstName;
            set 
            {
                if (Validators.ValidateName(value))
                    _contractorFirstName = value;
                else
                    _errors.Add(
                        "ContractorFirstName", 
                        "Formatting error: Name must start with capital letter and container only alphabetical values");
            }
        }

        public string? ContractorLastName
        {
            get => _contractorLastName;
            set 
            {
                if (Validators.ValidateName(value))
                    _contractorLastName = value;
                else
                    _errors.Add(
                        "ContractorLastName", 
                        "Formatting error: Surname must start with capital letter and container only alphabetical values");
            }
        }

        public string? ContractorEmail
        {
            get => _contractorEmail;
            set 
            {
                if (Validators.ValidateEmail(value))
                    _contractorEmail = value;
                else 
                    _errors.Add(
                        "ContractorEmail",
                        "Formatting error: Wrong email format");
            }
        }

        public string? ContractorPhoneNumber
        {
            get => _contractorPhoneNumber;
            set 
            {
                if (Validators.ValidatePhoneNumberUA(value))
                    _contractorPhoneNumber = value;
                else 
                    _errors.Add(
                        "ContractorPhoneNumber",
                        "Formatting error: Wrong ukrainian number format");
            }
        }

        public string? ContractorIBAN
        {
            get => _contractorIBAN;
            set 
            {
                if (Validators.ValidateIBAN(value))
                    _contractorIBAN = value;
                else 
                    _errors.Add(
                        "ContractorIBAN",
                        "Formatting error: Wrong IBAN format");
            }
        }

        public DateOnly? StartDate
        {
            get => _startDate;
            set
            {
                if (Validators.ValidateDateOnly(value))
                    _startDate = value;
                else
                    _errors.Add("StartDate", "Date was either given in the wrong format or was a null value");
            }
        }

        public DateOnly? EndDate
        {
            get => _endDate;
            set
            {
                if (Validators.ValidateDateOnly(value))
                {
                    _endDate = value;

                    if (_endDate < _startDate)
                    {
                        _endDate = null;
                        _errors.Add(
                            "EndDate",
                            "End date can not be earlier than start date"
                        );
                    }
                }
                else
                {
                    _errors.Add("EndDate", "Date was either given in the wrong format or was a null value");
                }
           }
        }

        public override string ToString()
        {
            string repr = "";
            foreach (var (propName, attr) in KeyProperties)
            {
				var prop = this.GetType().GetProperty(propName);
				if ((attr & FieldAttributes.Read) == FieldAttributes.Read &&
					prop != null)
				{
					repr += propName + ": " + Convert.ToString(prop.GetValue(this));
					repr += "\n";
				}
            }
            return repr;
        }
   }
}
