using CaaS.Core.Domainmodels;
using CaaS.Core.Test.Util;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CaaS.Core.Test.Integration.Template
{
    public class InsertTemplateTests
    {
        [Test]
        [TestCase(1, 1, 0.3)]
        [TestCase(2, null, 0.01)]
        [TestCase(3, 5, 0.534)]
        public async Task InsertSingleCouponToShop(int shopId, int? cartId, double value)
        {

            var insertedId = await Setup.GetTemplateEngine().InsertAsync<Coupon>(new
            {
                ShopId = shopId,
                CartId = cartId,
                Value = value,
                Id = 0
            });

            Assert.IsNotNull(insertedId);
            Assert.That(insertedId.Count, Is.EqualTo(1));
            Assert.That(insertedId[0], Is.Not.EqualTo(0));

            var selectInsertedCoupon = await Setup.GetTemplateEngine().QueryFirstOrDefaultAsync(ReadToCoupon, whereExpression: new
            {
                Id = insertedId
            });

            CouponInsertionChecks(selectInsertedCoupon, insertedId[0], shopId, cartId, value);
        }

        [Test, Rollback]
        [TestCase(1, 1, 0.3)]
        [TestCase(2, null, 0.01)]
        [TestCase(3, 5, 0.534)]
        public async Task InsertMultipleCouponToShop(int shopId, int? cartId, double value, int numberOfInserts = 2)
        {
            var objectToInsert = new
            {
                ShopId = shopId,
                CartId = cartId,
                Value = value,
                Id = 0
            };

            var listOfInsertableObjects = new List<object>();
            for (int i = 0; i < numberOfInserts; i++)
            {
                listOfInsertableObjects.Add(objectToInsert);
            }

            var newIds = await Setup.GetTemplateEngine().InsertAsync<Coupon>(listOfInsertableObjects);

            Assert.IsNotNull(newIds);
            Assert.That(newIds.Count, Is.EqualTo(numberOfInserts));


            var selectInsertedCoupons = (await Setup.GetTemplateEngine().QueryAsync(ReadToCoupon, whereExpression: new
            {
                Id = newIds
            })).ToList();


            Assert.IsNotNull(selectInsertedCoupons);
            Assert.That(newIds.Count, Is.EqualTo(selectInsertedCoupons.Count));

            for (int i = 0; i < newIds.Count; i++)
            {
                CouponInsertionChecks(selectInsertedCoupons[i], newIds[i], shopId, cartId, value);

            }

            foreach (var insertedId in newIds)
            {
            }
        }

        #region Asserts
        private void CouponInsertionChecks(Coupon? coupon, int insertedId, int shopId, int? cartId, double value)
        {
            Assert.IsNotNull(coupon);
            Assert.That(coupon.Id, Is.EqualTo(insertedId));
            Assert.That(coupon.ShopId, Is.EqualTo(shopId));
            Assert.That(coupon.CartId, Is.EqualTo(cartId));
            Assert.That(coupon.Value, Is.EqualTo(value));
            Assert.IsNull(coupon.Deleted);
        }
        #endregion

        #region DataReader
        private Coupon ReadToCoupon(IDataRecord reader) => new Coupon((int)reader["Id"], (int)reader["ShopId"], (double)reader["Value"])
        {
            CartId = reader["CartId"] == DBNull.Value ? null : (int)reader["CartId"],
            Deleted = reader["Deleted"] == DBNull.Value ? null : Convert.ToDateTime(reader["Deleted"])
        };
        #endregion DataReader
    }
}
