using System.Threading.Tasks;
using Entities.Domain.Transactions;

namespace Integration.OutboundTransactions
{
    public interface IOutboundTransactionProcessor
    {
        Task Process(Transaction outboundTransaction);
    }
}