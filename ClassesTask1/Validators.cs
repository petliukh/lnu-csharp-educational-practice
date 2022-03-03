using System.Text.RegularExpressions;

namespace LNUCSharp.Task1
{
    class ContractValidators
    {
        public static bool ValidateName(string name)
        {
            Regex rgx = new Regex(@"^[A-Z][a-z]{1,16}$");
            return rgx.IsMatch(name);
        }

        public static bool ValidateEmail(string email)
        {
            Regex rgx = new Regex(@"^[a-z0-9.]+@[a-z0-9.]+.[a-z0-9.]+");
            return rgx.IsMatch(email);
        }

        public static bool ValidatePhoneNumberUA(string phoneNumberUA)
        {
            Regex rgx = new Regex(@"\+380[0-9]{9}");
            return rgx.IsMatch(phoneNumberUA);
        }

        public static bool ValidateIBAN(string IBAN)
        {
            Regex rgx = new Regex(@"^[a-zA-Z]{2}[0-9]{2}\s?[a-zA-Z0-9]{4}\s?[0-9]{4}\s?[0-9]{3}([a-zA-Z0-9]\s?[a-zA-Z0-9]{0,4}\s?[a-zA-Z0-9]{0,4}\s?[a-zA-Z0-9]{0,4}\s?[a-zA-Z0-9]{0,3})?$");
            return rgx.IsMatch(IBAN);            
        }
    }
}