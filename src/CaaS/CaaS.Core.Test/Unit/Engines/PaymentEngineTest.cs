using CaaS.Core.Engines;
using CaaS.Core.Interfaces.Engines;
using CaaS.Core.Test.Util;
using CaaS.Util;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Unit.Engines
{
    [Category("Unit")]
    [TestFixture]
    public class PaymentEngineTest
    {
        //the delay is Deactivated due to faster tests
        private IPaymentEngine _engine;
        [SetUp]
        public void InitializeSut()
        {
            _engine = new PaymentEngine(false);
        }

        private string Encrypt(string toEncrypt) => CryptographyUtil.Encrypt(toEncrypt, Constants.PASS);

        [Test, Rollback]
        [TestCase("1")]
        [TestCase("123456781235467")]
        [TestCase("12345678123546789")]
        public void PayWithWrongLengthOfCardNumberThrowsException(string cardNumber) =>
            Assert.CatchAsync(async () => await _engine.Payment(10, Encrypt(cardNumber), Encrypt("123"), Encrypt("12/24")));

        [Test, Rollback]
        [TestCase("12345678123546x8")]
        [TestCase("1234567812354/89")]
        public void PayWithInvalidCharactersInCardNumberThrowsException(string cardNumber) =>
            Assert.CatchAsync(async () => await _engine.Payment(10, Encrypt(cardNumber), Encrypt("123"), Encrypt("12/24")));
        
        [Test, Rollback]
        [TestCase("-123456781235468")]
        [TestCase("-123456781235489")]
        public void PayWithNegativeCardNumberThrowsException(string cardNumber) =>
            Assert.CatchAsync(async () => await _engine.Payment(10, Encrypt(cardNumber), Encrypt("123"), Encrypt("12/24")));

        [Test, Rollback]
        [TestCase("1")]
        [TestCase("12")]
        [TestCase("1234")]
        public void PayWithWrongLengthOfCvvThrowsException(string cvv) =>
            Assert.CatchAsync(async () => await _engine.Payment(10, Encrypt("1234567812345678"), Encrypt(cvv), Encrypt("12/24")));


        [Test, Rollback]
        [TestCase("1A-")]
        [TestCase("12/")]
        [TestCase("12a")]
        public void PayWithInvalidCharactersInCvvThrowsException(string cvv) =>
            Assert.CatchAsync(async () => await _engine.Payment(10, Encrypt("1234567812345678"), Encrypt(cvv), Encrypt("12/24")));

        [Test, Rollback]
        [TestCase("-12")]
        [TestCase("-00")]
        public void PayWithNegativeCvvThrowsException(string cvv) =>
            Assert.CatchAsync(async () => await _engine.Payment(10, Encrypt("1234567812345678"), Encrypt(cvv), Encrypt("12/24")));

        [Test, Rollback]
        [TestCase("1")]
        [TestCase("123456")]
        [TestCase("1234")]
        public void PayWithWrongLengthOfExpirationThrowsException(string expiration) =>
            Assert.CatchAsync(async () => await _engine.Payment(10, Encrypt("1234567812345678"), Encrypt("123"), Encrypt(expiration)));

        [Test, Rollback]
        [TestCase("12345")]
        [TestCase("1/345")]
        [TestCase("123/5")]
        public void PayWithNoSlashOnThirdExpirationThrowsException(string expiration) =>
            Assert.CatchAsync(async () => await _engine.Payment(10, Encrypt("1234567812345678"), Encrypt("123"), Encrypt(expiration)));

        [Test, Rollback]
        [TestCase("1x/A5")]
        [TestCase("1//45")]
        [TestCase("12/-5")]
        public void PayWithInvalidCharactersInExpirationThrowsException(string expiration) =>
            Assert.CatchAsync(async () => await _engine.Payment(10, Encrypt("1234567812345678"), Encrypt("123"), Encrypt(expiration)));
        
        [Test, Rollback]
        [TestCase("00/24")]
        [TestCase("13/24")]
        [TestCase("99/24")]
        [TestCase("-9/24")]
        public void PayWithWrongMonthInExpirationThrowsException(string expiration) =>
            Assert.CatchAsync(async () => await _engine.Payment(10, Encrypt("1234567812345678"), Encrypt("123"), Encrypt(expiration)));
        
        [Test, Rollback]
        [TestCase("01/-1")]
        [TestCase("12/-9")]
        public void PayWithNegativeYearInExpirationThrowsException(string expiration) =>
            Assert.CatchAsync(async () => await _engine.Payment(10, Encrypt("1234567812345678"), Encrypt("123"), Encrypt(expiration)));

        [Test, Rollback]
        [TestCase("01/22")]
        [TestCase("11/22")]
        [TestCase("01/20")]
        public void PayWithExpiredExpirationThrowsException(string expiration) =>
            Assert.CatchAsync(async () => await _engine.Payment(10, Encrypt("1234567812345678"), Encrypt("123"), Encrypt(expiration)));

        [Test, Rollback]
        public void PayWithWrongCardDataThrowsException() =>
            Assert.CatchAsync(async () => await _engine.Payment(10, Encrypt("1234567812345678"), Encrypt("124"), Encrypt("12/24")));

        [Test, Rollback]
        [TestCase(10.1d)]
        [TestCase(100d)]
        public void PayWithTooHighAmountThrowsException(double amount) =>
            Assert.CatchAsync(async () => await _engine.Payment(amount, Encrypt("1234567812345676"), Encrypt("123"), Encrypt("12/24")));
        
        [Test, Rollback]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(-0.1d)]
        public void PayWithNegativeAmountThrowsException(double amount) =>
            Assert.CatchAsync(async () => await _engine.Payment(amount, Encrypt("1234567812345676"), Encrypt("123"), Encrypt("12/24")));

        [Test, Rollback]
        public async Task PayReturnsTrue() =>
            Assert.That(await _engine.Payment(10, Encrypt("1234567812345678"), Encrypt("123"), Encrypt("12/24")), Is.True);
    }
}
