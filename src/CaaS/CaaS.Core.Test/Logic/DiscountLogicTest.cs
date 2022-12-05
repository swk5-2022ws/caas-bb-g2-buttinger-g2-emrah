using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util.MemoryRepositories;
using CaaS.Core.Test.Util.RepositoryStubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Logic
{
    [Category("Unit")]
    [TestFixture]
    public class DiscountLogicTest
    {
        IDiscountLogic sut;
        ICartRepository cartRepository;
        IShopRepository shopRepository;
        IDiscountRepository discountRepository;

        [SetUp]
        public void InitializeSut()
        {
            discountRepository = new DiscountRepositoryStub(new Dictionary<int, Discount>()
            {
                //{1, new Discount(1, new DiscountRule(1, 1, "super", new DiscountRul)) }
            });

            cartRepository = new CartRepositoryStub(new Dictionary<int, Cart>()
            {
                {1, new Cart(1, "a82724ba-ced5-32e8-9ada-17b06d427906") }
            });

            shopRepository = new ShopRepositoryStub(new Dictionary<int, Shop>()
            {
                {1, new Shop(1, 1, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906"), "shop") }
            });

            sut = new DiscountLogic(discountRepository, shopRepository, cartRepository);
        }
    }
}
