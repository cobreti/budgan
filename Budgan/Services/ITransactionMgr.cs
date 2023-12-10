using Budgan.Model;

namespace Budgan.Services;

public interface ITransactionMgr
{
    void Add(Transaction transaction);
}