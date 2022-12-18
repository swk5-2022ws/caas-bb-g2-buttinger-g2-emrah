using CaaS.Core.Interfaces.Engines;
using CaaS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

        public bool Payment(double amount, string creditCardNumber, string cvv, string expiration)
        {
            //decrypt the data passed to the payment engine
            //amount is the only thing that is not encrypted here!
            string _eCreditCardNumber = CryptographyUtil.Decrypt(creditCardNumber, Constants.PASS);
            string _eCVV = CryptographyUtil.Decrypt(cvv, Constants.PASS);
            string _eExpration = CryptographyUtil.Decrypt(expiration, Constants.PASS);

            Check(amount, _eCreditCardNumber, _eCVV, _eExpration);
            var bookable = GetBookableAmount(_eCreditCardNumber, _eCVV, _eExpration);

            if(bookable < amount)
            return false;

            SetAmount(bookable - amount, _eCreditCardNumber, _eCVV, _eExpration);
            return true;
        }

       

        private void CheckCreditCardNumber(string creditCardNumber)
        {
            if (creditCardNumber.Length != 16) throw new ArgumentOutOfRangeException("A credit card number has to have exactly 16 numbers");
            if (!Int64.TryParse(creditCardNumber, out _)) throw new InvalidDataException("Only numbers can be passed as a credit card");
        }

        private void CheckCvv(string cvv)
        {
            if (cvv.Length != 3) throw new ArgumentOutOfRangeException("A cvv has to have exactly 3 numbers");
            if (!int.TryParse(cvv, out _)) throw new InvalidDataException("Only numbers can be passed as cvv");
        }

        private void CheckExpiration(string expiration)
        {
            if (expiration.Length != 5) throw new ArgumentOutOfRangeException("A cvv has to have exactly 5 numbers");

            var month = new string(expiration.Take(2).ToArray());
            var year = new string(expiration.Take(2).ToArray());

            if (!int.TryParse(month, out _) || 
                !int.TryParse(year, out _)) throw new InvalidDataException("Only numbers can be passed as for the expiration date");

            if (expiration[2] != '/') throw new InvalidDataException("There must be a / on third place");

            if (int.Parse(month) < 1 && int.Parse(month) > 12) throw new ArgumentOutOfRangeException("The month must be between 01 and 12");

            if (int.Parse(year) < 0) throw new ArgumentOutOfRangeException("The year must be positiv");
        }


        private void CheckCorrectnessOfInformation(string creditCardNumber, string cvv, string expiration)
        {
            throw new NotImplementedException();
        }

        private void Check(double amount, string creditCardNumber, string cvv, string expiration)
        {
            CheckCreditCardNumber(creditCardNumber);
            CheckCvv(cvv);
            CheckExpiration(expiration);

            CheckCorrectnessOfInformation(creditCardNumber, cvv, expiration);

        }

        private double GetBookableAmount(string creditCardNumber, string cvv, string expiration)
        {
            throw new NotImplementedException();
        }
        private void SetAmount(double v, string eCreditCardNumber, string eCVV, string eExpration)
        {
            throw new NotImplementedException();
        }
    }
}
