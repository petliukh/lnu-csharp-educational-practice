namespace RestApi.Controllers.Users.Contracts;

using Microsoft.AspNetCore.Mvc;  

using RestApi.DataAccess.Contracts;  
using RestApi.Models.Contracts;  
using RestApi.Models.Users;  
using RestApi.Authorization;

[Authorize]
[ApiController]
[Route("api/[controller]")]  
public class ContractsController : ControllerBase  
{  
    private readonly IContractDataAccessProvider _dataAccessProvider;  

    private static readonly Dictionary<string, Func<Contract, object>> _sortParamsMapping = new Dictionary<string, Func<Contract, object>>()
    {
        { "firstName", c => c.ContractorFirstName },
        { "lastName", c => c.ContractorLastName },
        { "email", c => c.ContractorEmail },
        { "phone", c => c.ContractorPhoneNumber },
        { "startDate", c => c.StartDate },
        { "endDate", c => c.EndDate }
    };

    public ContractsController(IContractDataAccessProvider dataAccessProvider)  
    {  
        _dataAccessProvider = dataAccessProvider;  
    }  

    [HttpGet]  
    public IEnumerable<Contract> Get(string? sortBy, string? direction, string? search)  
    {  
        var contracts = _dataAccessProvider.GetContractRecords();

        if (search != null)
        {
            var searchStrings = search.ToLower().Split();
            contracts = contracts.Where(
                c => searchStrings.Any(s => c.ContractorFirstName.ToLower().Contains(s.ToLower())) ||
                searchStrings.Any(s => c.ContractorLastName.ToLower().Contains(s.ToLower())) ||
                searchStrings.Any(s => c.ContractorEmail.ToLower().Contains(s.ToLower())) ||
                searchStrings.Any(s => c.ContractorPhoneNumber.ToLower().Contains(s.ToLower())) ||
                searchStrings.Any(s => c.StartDate.ToString().ToLower().Contains(s.ToLower())) ||
                searchStrings.Any(s => c.EndDate.ToString().ToLower().Contains(s.ToLower()))
            ).ToList();
        }
        if (sortBy != null)
        {
            var getPropertyLambda = _sortParamsMapping.GetValueOrDefault(sortBy);

            if (getPropertyLambda != null)
            {
                switch (direction)
                {
                    case "asc":
                        return contracts.OrderBy(getPropertyLambda);
                    case "desc":
                        return contracts.OrderByDescending(getPropertyLambda);
                    default:
                        return contracts.OrderBy(getPropertyLambda);
                }
            }
        }
       return contracts.OrderBy(c => c.Id);
    }  

    [Authorize(Role.Admin)]
    [HttpPost]  
    public IActionResult Create([FromBody]Contract contract)  
    {  
        Guid obj = Guid.NewGuid();  
        contract.Id = obj.ToString();  
        _dataAccessProvider.AddContractRecord(contract);  
        return Created(new Uri(Request.Path), contract);
    }  

    [Authorize(Role.Admin)]
    [HttpPost("bulk-create")]  
    public IActionResult BulkCreate([FromBody]List<Contract> contracts)  
    {  
        foreach (var contract in contracts)
        {
            Guid obj = Guid.NewGuid();  
            contract.Id = obj.ToString();
        }
        _dataAccessProvider.AddRange(contracts);  
        return Created(new Uri(Request.Path), contracts);
    }

    [HttpGet("{id}")]  
    public Contract Details(string id)  
    {  
        return _dataAccessProvider.GetContractSingleRecord(id);  
    }  

    [Authorize(Role.Admin)]
    [HttpPut]  
    public IActionResult Edit([FromBody]Contract contract)  
    {  
        _dataAccessProvider.UpdateContractRecord(contract);  
        return Ok();  
    }  

    [Authorize(Role.Admin)]
    [HttpDelete("{id}")]  
    public IActionResult DeleteConfirmed(string id)  
    {  
        var data = _dataAccessProvider.GetContractSingleRecord(id);  
        if (data == null)  
        {  
            return NotFound();  
        }  
        _dataAccessProvider.DeleteContractRecord(id);  
        return Ok();  
    }  
}  