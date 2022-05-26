namespace RestApi.DataAccess.Contracts;

using RestApi.Models.Contracts;

public interface IContractDataAccessProvider
{
    void AddContractRecord(Contract contract);
    void AddRange(List<Contract> contracts);
    void UpdateContractRecord(Contract contract);
    void DeleteContractRecord(string id);
    Contract GetContractSingleRecord(string id);
    List<Contract> GetContractRecords();
}