using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Interfaces.Services;
using CaaS.Util;
using System.Transactions;

namespace CaaS.Core.Services
{
    public class CleanupService : ICleanupService
    {
        private readonly ICartRepository cartRepository;
        private readonly TimeSpan deleteWhenDiffersBy = TimeSpan.FromHours(1);
        public CleanupService(ICartRepository cartRepository, IProductCartRepository productCartRepository)
        {
            this.cartRepository = cartRepository ?? throw ExceptionUtil.ParameterNullException(nameof(cartRepository));
        }

        public async Task RemoveNotUsedCartsAsync()
        {
            var currentTime = DateTime.UtcNow;
            var carts = await this.cartRepository.GetAll();

            var modifiedCartIdsToDelete = carts.Where(x => x.ModifiedDate.HasValue && x.ModifiedDate.Value - currentTime >= deleteWhenDiffersBy).Select(x => x.Id);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var id in modifiedCartIdsToDelete)
                {
                    await cartRepository.Delete(id);
                }

                transaction.Complete();
            }
        }
    }
}
