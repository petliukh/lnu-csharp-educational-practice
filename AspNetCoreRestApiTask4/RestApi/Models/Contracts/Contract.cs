namespace RestApi.Models.Contracts;

using System.ComponentModel.DataAnnotations;

public class Contract
{
    public string Id { get; set; }

    [RegularExpression(@"^[A-Z][a-z]{1,16}$")]
    public string ContractorFirstName { get; set; } 

    [RegularExpression(@"^[A-Z][a-z]{1,16}$")]
    public string ContractorLastName { get; set; }

    [EmailAddress]
    public string ContractorEmail { get; set; }

    [Phone]
    public string ContractorPhoneNumber { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}