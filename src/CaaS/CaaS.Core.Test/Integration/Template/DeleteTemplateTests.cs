using CaaS.Core.Domainmodels;
using CaaS.Core.Test.Util;
using System.Data;

namespace CaaS.Core.Test.Integration.Template
{
    [Category("Integration")]
    [TestFixture]
    public class DeleteTemplateTests
    {
        [Test, Rollback]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task DeleteSingleDisountActionById(int id)
        {
            var preDeleteSelect = await Setup.GetTemplateEngine().QueryAsync(ReadToDiscountAction);
            var isDeleted = await Setup.GetTemplateEngine().DeleteAsync<DiscountAction>(new
            {
                Id = id
            });
            var postDeleteSelect = await Setup.GetTemplateEngine().QueryAsync(ReadToDiscountAction);

            BaseDiscountActionAssertion(isDeleted, postDeleteSelect.Count(), preDeleteSelect.Count() - 1);

        }

        [Test, Rollback]
        public async Task DeleteAllDiscountAction()
        {
            var preDeleteSelect = await Setup.GetTemplateEngine().QueryAsync(ReadToDiscountAction);
            var isDeleted = await Setup.GetTemplateEngine().DeleteAsync<DiscountAction>();
            var postDeleteSelect = await Setup.GetTemplateEngine().QueryAsync(ReadToDiscountAction);

            BaseDiscountActionAssertion(isDeleted, postDeleteSelect.Count(), 0);
        }

        #region Assertions
        /// <summary>
        /// Basic assertions for the Discount Action used in this test class
        /// </summary>
        /// <param name="isDeleted">Deleted flag</param>
        /// <param name="afterDeletionCount">count after the deletion happened</param>
        /// <param name="matchDeletionCount">count which the deletion count afterwards should match</param>
        private void BaseDiscountActionAssertion(bool isDeleted, int afterDeletionCount, int matchDeletionCount)
        {
            Assert.That(isDeleted, Is.True);
            Assert.That(afterDeletionCount, Is.EqualTo(matchDeletionCount));
        }
        #endregion Assertions

        #region DataMapping
        private DiscountAction ReadToDiscountAction(IDataRecord reader) => 
            new DiscountAction((int)reader["Id"], (int)reader["ShopId"], (string)reader["ActionType"], (string)reader["Name"] /*(double)reader["Value"]*/);
        #endregion DataMapping
    }
}
