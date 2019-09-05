using System.Threading.Tasks;
using Domain.Transaction;

namespace Integration.OutboundTransactions
{
    public class StubOutboundTransactionProcessor : IOutboundTransactionProcessor
    {
        public async Task Process(Transaction outboundTransaction)
        {
            //swallow
        }
    }
}