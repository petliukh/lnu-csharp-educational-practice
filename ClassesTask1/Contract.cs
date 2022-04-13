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

        public static List<string> Keys
        {
            get
            {
                return new List<string>(new[] {
					"ID",
					"ContractorFirstName",
					"ContractorLastName",
					"ContractorEmail",
					"ContractorPhoneNumber",
					"ContractorIBAN",
					"StartDate",
					"EndDate"
				});
            }
        }
		
		public void AddError(string fieldName, string message)
		{
			this._errors.Add(fieldName, message);
		}

		public Dictionary<string, string> Errors
		{
			get => _errors;
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
                if (this._endDate < this._startDate)
                {
                    this._endDate = null;
                    this._errors.Add(
                        "EndDate",
                        "End date can not be earlier than start date"
                    );
                }
            }
        }

        public override string ToString()
        {
            string repr = "";
            foreach (PropertyInfo prop in this.GetType().GetProperties())
            {
				if (!Contract.Keys.Contains(prop.Name))
					continue;
                repr += prop.Name + ": " + Convert.ToString(prop.GetValue(this, null));
                repr += "\n";
            }
            return repr;
        }

   }
}
