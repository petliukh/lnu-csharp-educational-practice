
namespace LNUCSharp.Task2
{
	class User : IContainable
	{
		static private UserContainer _objects = new UserContainer();

		protected string? _firstName; 
		protected string? _lastName;
		protected string? _email;
		protected string? _salt;
		protected string? _password;
		protected Role? _role;
		protected Dictionary<string, string> _errors = new Dictionary<string, string>();

		public User()
		{
		}

		public User(
			string firstName,
			string lastName,
			string email,
			string password,
			Role role)
		{
			FirstName = firstName;
			LastName = lastName;
			Email = email;
			Password = password;
			UserRole = role;
		}

		public static UserContainer Objects
		{
			get => _objects;
		}

		public virtual Dictionary<string, FieldAttributes> KeyProperties
		{
			get
			{
				return new Dictionary<string, FieldAttributes>()
				{
					{ "Email", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input },
					{ "Salt", FieldAttributes.Read | FieldAttributes.Write },
					{ "Password", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input },
					{ "FirstName", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input },				
					{ "LastName", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input },				
					{ "UserRole", FieldAttributes.Read | FieldAttributes.Write },				
				};
			}
		}

		public Dictionary<string, string> Errors
		{
			get => _errors;
		}

		public object GetPrimaryKey()
		{
			return _email ?? "undefined";
		}

		public string? FirstName
		{
			get => _firstName;
			set
			{
				if (Validators.ValidateName(value))
					_firstName = value;
				else
					_errors.Add("FirstName", "Provided string either had an incorrect format or was a null value");
			}
		}

		public string? LastName
		{
			get => _lastName;
			set
			{
				if (Validators.ValidateName(value))
					_lastName = value;
				else
					_errors.Add("LastName", "Provided string either had an incorrect format or was a null value");
			}
		}
		
		public string? Email
		{
			get => _email;
			set
			{
				if (Validators.ValidateEmail(value))
					_email = value;
				else
					_errors.Add(
						"Email", 
						"Provided string either had an incorrect format or " +
						"was a null value");
			}
		}

		public string? Salt
		{
			get => _salt;
			set
			{
				if (value != null && value.GetType() == typeof(string))
					_salt = value;
				else
					_errors.Add(
						"Salt",
						"Salt was either not a string value or null.");
			}
		}

		public string? Password
		{
			get => _password;
			set
			{
				if (Validators.ValidatePassword(value))
				{
					if (_salt is null)
					{
						_salt = Helpers.GetSalt();
						_password = Helpers.GetHash(value + _salt);
					}
					else
					{
						_password = value;
					}
				}
				else
				{
					_errors.Add(
						"Password", 
						"Password is not strong enough. Make sure it is " +
						"least 8 characters long, container one capital " +
						"and one small letter.");
				}
			}
		}

		public Role? UserRole
		{
			get => _role;
			set
			{
				if (Validators.ValidateRole(value))
					_role = value;
				else 
					_errors.Add("UserRole", "No such role exists.");
			}
		}

		public bool Authenticate(string password)
		{
			if (Helpers.GetHash(password + _salt) == _password)
				return true;
			return false;
		}

		public string ToStringShort()
		{
			string shortRepr = String.Format("({0}: {1})", UserRole, Email);
			return shortRepr;
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

	class Admin : User
	{
		public Admin() : base()
		{
			UserRole = Role.Admin;
		}

		public Admin(
			string firstName,
			string lastName,
			string email,
			string password) 
				: base(firstName, lastName, email, password, Role.Admin)
		{
		}

		public bool ReviewEntry<TKey, TVal>(TKey key, List<Entry<TVal>> objects)
		{
			Entry<TVal>? entry = objects.Find(x => {
				return EqualityComparer<TKey>.Default.Equals(
					(TKey?)(x.model!.GetType()!.GetMethod("GetPrimaryKey")!.Invoke(x.model, new object[] {})),
					key);
			});

			if (entry != null)
			{
				Console.WriteLine("Set status:");
				foreach (var val in Enum.GetValues<Status>())
				{
					Console.WriteLine("{0} - {1}", (int)val, val);
				}

				Status status;
				while (true)
				{
					Console.WriteLine("Input status to set for this entry:");
					if (Status.TryParse(Console.ReadLine() ?? "", out status))
						break;
					else
						Console.WriteLine("No such status. Try again");
				}
				Console.WriteLine("Reason:");
				string? message = Console.ReadLine() ?? "";
				entry.status = status;
			}
			else
			{
				Console.WriteLine("No entry with such ID found.");
				return false;
			}

			return true;	
		}

		public void ShowEntriesForReview<ModelType>(List<Entry<ModelType>> objects)
		{
			List<Entry<ModelType>> myValues = objects.FindAll(
				x => (x.status == Status.Draft || x.status == Status.Rejected));

			foreach (var entry in myValues)
			{
				Console.WriteLine(entry);
			}
		}
	}

	class Staff : User
	{
		int? _salary;
		DateOnly? _firstDayInCompany;

		public Staff() : base()
		{
			UserRole = Role.Staff;
		}
		
		public Staff(
			string firstName,
			string lastName,
			string email,
			string password,
			int salary,
			DateOnly firstDayInCompany) 
				: base(firstName, lastName, email, password, Role.Staff)
		{
			Salary = salary;
			FirstDayInCompany = firstDayInCompany;
		}

		public override Dictionary<string, FieldAttributes> KeyProperties
		{
			get
			{
				var staffProperties = base.KeyProperties;
				staffProperties.Add("Salary", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input );
				staffProperties.Add("FirstDayInCompany", FieldAttributes.Read | FieldAttributes.Write | FieldAttributes.Input);

				return staffProperties;
			}
		}
			
		public int? Salary
		{
			get => _salary;
			set
			{
				if (Validators.ValidateInt(value))
					_salary = value;
				else
					_errors.Add("Salary", "Provided value was not numeric.");
			}
		}

		public DateOnly? FirstDayInCompany
		{
			get => _firstDayInCompany;
			set
			{
				if (Validators.ValidateDateOnly(value))
					_firstDayInCompany = value;
				else
					_errors.Add(
						"FirstDayInCompany", 
						"Provided value had incorrect date format.");
			}
		}

		public void ShowApprovedEntries<ModelType>(List<Entry<ModelType>> objects)
		{
			List<Entry<ModelType>> myValues = objects.FindAll(
				x => (x.status == Status.Approved));

			foreach (var entry in myValues)
			{
				Console.WriteLine(entry);
			}
		}

		public void ShowOwnEntries<ModelType>(List<Entry<ModelType>> objects)
		{
			Console.WriteLine("Filter by status:");
			foreach (var val in Enum.GetValues<Status>())
			{
				Console.WriteLine("Value: {0}, Status: {1}", (int)val, val);
			}

			Status status;
			while (true)
			{
				Console.WriteLine("Input value to filter by:");
				if (Status.TryParse(Console.ReadLine() ?? "", out status))
					break;
				else
					Console.WriteLine("No such status. Try again");
			}
			
			List<Entry<ModelType>> myValues = objects.FindAll(
				x => (x.status == status));

			foreach (var entry in myValues)
			{
				Console.WriteLine(entry);
			}
		}
	}
}
