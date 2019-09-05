using System.Threading.Tasks;
using Domain.Transaction;

namespace Integration.OutboundTransactions
{
    public interface IOutboundTransactionProcessor
    {
        Task Process(Transaction outboundTransaction);
    }
}