using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Util;

namespace CaaS.Core.Logic.Util
{
    public static class Check
    {
        /// <summary>
        /// Checks wether a shop with a given id is in the repository and
        /// if the shops appkey matches with the passed one
        /// </summary>
        /// <param name="shopRepository">ShopRepository</param>
        /// <param name="shopId">Passed shopId</param>
        /// <param name="appKey">Passed appkey</param>
        /// <returns></returns>
        public static async Task ShopAuthorization(IShopRepository shopRepository, int shopId, Guid appKey)
        {
            var availableShop = await shopRepository.Get(shopId) ?? throw new ArgumentException($"The shop with id={shopId} is currently not available.");
            if (availableShop.AppKey != appKey) throw new ArgumentException($"You have not the right privileges.");
        }
    }
}
