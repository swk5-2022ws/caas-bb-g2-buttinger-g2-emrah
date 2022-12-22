using CaaS.Core.Interfaces.Engines;
using CaaS.Core.Interfaces.Engines.PaymentRepository;
using CaaS.Core.Interfaces.Repository;
using CaaS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ZstdNet;

namespace CaaS.Core.Engines
{
    
    /// <summary>
    /// The payment engine is not actually a real engine for payments. 
    /// It is just a simulation of thrid party payment service.
    /// Before actually using the application in Public this should be 
    /// considered and a real thrid party PaymentService should be used.
    /// </summary>
    public class PaymentEngine : IPaymentEngine
    {
        private readonly IPaymentRepository repository;
        private readonly Random random = new();
        private readonly bool delay;

        public PaymentEngine(bool delay = true)
        {
            this.repository = new PaymentRepositoryStub();
            this.delay = delay;
        }

        public async Task<bool> Payment(double amount, string creditCardNumber, string cvv, string expiration)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (delay)
                {
                    await Task.Delay(random.Next(1000, 3000));
                }
                var availableAmount = await repository.Get(creditCardNumber, cvv, expiration);
                Check(amount, availableAmount, creditCardNumber, cvv, expiration);

                var updated = await repository.Update(creditCardNumber, cvv, expiration, availableAmount.Value - amount);
                transaction.Complete();
                return updated;
            }
        }

       
        /// <summary>
        /// Checks the correctness of the credit card number
        /// </summary>
        /// <param name="creditCardNumber"></param>
        /// <exception cref="ArgumentOutOfRangeException">The card is not in correct length</exception>
        /// <exception cref="InvalidDataException">The card contains invalid characters</exception>
        private void CheckCreditCardNumber(string creditCardNumber)
        {
            if (creditCardNumber.Length != 16) throw new ArgumentOutOfRangeException("A credit card number has to have exactly 16 numbers");
            if (!Int64.TryParse(creditCardNumber, out _)) throw new InvalidDataException("Only numbers can be passed as a credit card");
            if (Int64.Parse(creditCardNumber) < 0) throw new InvalidDataException("Not a valid credit card number");
        }

        /// <summary>
        /// Checks the correctness of the cvv
        /// </summary>
        /// <param name="cvv">the cvv</param>
        /// <exception cref="ArgumentOutOfRangeException">the cvv is not in correct length</exception>
        /// <exception cref="InvalidDataException">the cvv contains invalid characters</exception>
        private void CheckCvv(string cvv)
        {
            if (cvv.Length != 3) throw new ArgumentOutOfRangeException("A cvv has to have exactly 3 numbers");
            if (!int.TryParse(cvv, out _)) throw new InvalidDataException("Only numbers can be passed as cvv");
            if (Int64.Parse(cvv) < 0) throw new InvalidDataException("Not a valid cvv number");

        }

        /// <summary>
        /// Checks the correctness of the expiration
        /// </summary>
        /// <param name="expiration"></param>
        /// <exception cref="ArgumentOutOfRangeException">The expiration is not in correct length</exception>
        /// <exception cref="FormatException">month / year are not in correct format</exception>
        /// <exception cref="ArgumentException">Your card is expired</exception>
        /// <exception cref="InvalidDataException">the expiration contains invalid characters</exception>
        private void CheckExpiration(string expiration)
        {
            if (expiration.Length != 5) throw new ArgumentOutOfRangeException("A expration has to have exactly 5 numbers");

            var month = new string(expiration.Take(2).ToArray());
            var year = new string(expiration.Take(2).ToArray());

            if (!int.TryParse(month, out _) || 
                !int.TryParse(year, out _)) throw new InvalidDataException("Only numbers can be passed as for the expiration date");

            if (expiration[2] != '/') throw new InvalidDataException("There must be a / on third place");

            if (int.Parse(month) < 1 && int.Parse(month) > 12) throw new FormatException("The month must be between 01 and 12");

            if (int.Parse(year) < 0) throw new FormatException("The year must be positiv");

            int millenium = (Convert.ToInt32(DateTime.UtcNow.Year.ToString().Substring(0,2)) + 1) * 100;
            var currentDate = DateTime.UtcNow;
            if (currentDate.Year >= millenium + int.Parse(year))
            {
                if(currentDate.Month > int.Parse(month))
                {
                    throw new ArgumentException("Your credit card expired");
                }
            }
        }

        /// <summary>
        /// Contains all the checks done in the payment engine.
        /// </summary>
        /// <param name="amount">The amount given</param>
        /// <param name="availableAmount">The amount retrieved from the repository</param>
        /// <param name="creditCardNumber">the credit card number given</param>
        /// <param name="cvv">the cvv given</param>
        /// <param name="expiration">the expiration date given</param>
        private void Check(double amount, double? availableAmount, string creditCardNumber, string cvv, string expiration)
        {
            //decrypt the data passed to the payment engine
            //amount is the only thing that is not encrypted here!
            var _eCreditCardNumber = CryptographyUtil.Decrypt(creditCardNumber, Constants.PASS);
            var _eCVV = CryptographyUtil.Decrypt(cvv, Constants.PASS);
            var _eExpration = CryptographyUtil.Decrypt(expiration, Constants.PASS);

            CheckCreditCardNumber(_eCreditCardNumber);
            CheckCvv(_eCVV);
            CheckExpiration(_eExpration);
            CheckAmount(amount, availableAmount);
        }
        
        /// <summary>
        /// Checks the given and the retrieved amount from the repository
        /// </summary>
        /// <param name="amount">the given amount</param>
        /// <param name="availableAmount">the retrieved amount</param>
        /// <exception cref="ArgumentNullException">Passed if the retrieved amount is null. This is mostly due to wrong informations passed</exception>
        /// <exception cref="ArgumentOutOfRangeException">The amount given is greater than the amount available on the card.</exception>
        private void CheckAmount(double amount, double? availableAmount)
        {
            if (!availableAmount.HasValue)
            {
                throw new ArgumentNullException("No such credit card informations!");
            }
            else if(amount <= 0)
            {
                throw new ArgumentOutOfRangeException("An amount less or equal to zero can not be booked");
            }
            else if (availableAmount.Value < amount)
            {
                throw new ArgumentOutOfRangeException("Not enough money on your credit card!");
            }
        }
    }
}
