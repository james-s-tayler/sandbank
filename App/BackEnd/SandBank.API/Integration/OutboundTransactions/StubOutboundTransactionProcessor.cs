using System.Threading.Tasks;
using Entities.Domain.Transactions;

namespace Integration.OutboundTransactions
{
    public class StubOutboundTransactionProcessor : IOutboundTransactionProcessor
    {
        public Task Process(Transaction outboundTransaction)
        {
            return Task.CompletedTask;
        }
    }
}