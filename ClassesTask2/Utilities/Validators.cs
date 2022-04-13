using System.Text.RegularExpressions;

namespace LNUCSharp.Task2
{
	class Validators
	{
        public static bool ValidatePhoneNumberUA(object? value)
        {
			if (value == null || value.GetType() != typeof(string))
				return false;
            Regex rgx = new Regex(@"\+380[0-9]{9}");
            return rgx.IsMatch(value as string ?? "");
        }

        public static bool ValidateIBAN(object? value)
        {
			if (value == null || value.GetType() != typeof(string))
				return false;
            Regex rgx = new Regex(@"^[a-zA-Z]{2}[0-9]{2}\s?[a-zA-Z0-9]{4}\s?[0-9]{4}\s?[0-9]{3}([a-zA-Z0-9]\s?[a-zA-Z0-9]{0,4}\s?[a-zA-Z0-9]{0,4}\s?[a-zA-Z0-9]{0,4}\s?[a-zA-Z0-9]{0,3})?$");
            return rgx.IsMatch(value as string ?? "");            
        }

		public static bool ValidateName(object? value)
		{
			if (value == null || value.GetType() != typeof(string))
				return false;
			Regex rgx = new Regex(@"^[A-Z][a-z]{1,16}$");
            return rgx.IsMatch(value as string ?? "");
		}

		public static bool ValidateEmail(object? value)
        {
			if (value == null || value.GetType() != typeof(string))
				return false;
            Regex rgx = new Regex(@"^[a-z0-9.]+@[a-z0-9.]+.[a-z0-9.]+");
            return rgx.IsMatch(value as string ?? "");
        }

		public static bool ValidatePassword(object? value)
		{
			if (value == null || value.GetType() != typeof(string))
				return false;
			Regex rgx = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
			return rgx.IsMatch(value as string ?? "");
		}

		public static bool ValidateRole(object? value)
		{
			if (value == null || value.GetType() != typeof(Role))
				return false;
			return true;
		}

		public static bool ValidateInt(object? value)
		{
			if (value == null || value.GetType() != typeof(int))
				return false;
			return true;
		}

		public static bool ValidateDateOnly(object? value)
		{
			if (value == null || value.GetType() != typeof(DateOnly))
				return false;
			return true;
		}
	}
}

