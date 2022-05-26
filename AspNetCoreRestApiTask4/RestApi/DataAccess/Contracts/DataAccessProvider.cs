namespace RestApi.DataAccess.Contracts;

using RestApi.Models.Contracts;

public class ContractDataAccessProvider : IContractDataAccessProvider
{
    private readonly ContractPostgreSqlContext _context;

    public ContractDataAccessProvider(ContractPostgreSqlContext context)
    {
        _context = context;
    }

    public void AddContractRecord(Contract contract)
    {
        _context.Contracts.Add(contract);
        _context.SaveChanges();
    }

    public void AddRange(List<Contract> contracts)
    {
        _context.Contracts.AddRange(contracts);
        _context.SaveChanges();
    }

    public void UpdateContractRecord(Contract contract)
    {
        _context.Contracts.Update(contract);
        _context.SaveChanges();
    }

    public void DeleteContractRecord(string id)
    {
        var entity = _context.Contracts.FirstOrDefault(t => t.Id == id);
        _context.Contracts.Remove(entity!);
        _context.SaveChanges();
    }

    public Contract GetContractSingleRecord(string id)
    {
        return _context.Contracts.FirstOrDefault(t => t.Id == id);
    }

    public List<Contract> GetContractRecords()
    {
        return _context.Contracts.ToList();
    }
}