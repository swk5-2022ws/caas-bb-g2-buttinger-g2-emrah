using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Test.Util.MemoryRepositories;
using CaaS.Core.Test.Util.RepositoryStubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Logic
{
    public class CouponLogicTest
    {
        ICouponLogic sut;
        ICouponRepository couponRepository;
        ICartRepository cartRepository;
        IProductCartRepository productCartRepository;
        IProductRepository productRepository;
        IShopRepository shopRepository;

        [SetUp]
        public void InitializeSut()
        {
            //cartRepository = new CartRepositoryStub(new Dictionary<int, Cart>()
            //{

            //});
            couponRepository = new CouponRepositoryStub(new Dictionary<int, Coupon>()
            {
                {1, new Coupon(1, 1, 10) },
                {2, new Coupon(2, 1, 20){ CartId = 1 } }
            });

            shopRepository = new ShopRepositoryStub(new Dictionary<int, Shop>()
            {
                {1, new Shop(1, 1, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906"), "shop") }
            });


            sut = new CouponLogic(couponRepository, cartRepository, productCartRepository, productRepository, shopRepository);
        }
    }
}
