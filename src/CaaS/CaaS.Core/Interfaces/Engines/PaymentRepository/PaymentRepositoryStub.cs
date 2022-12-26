using CaaS.Core.Engines.PaymentModels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Engines.PaymentRepository
{
    //this is not a real repository
    //it is a stub for a real payment
    public class PaymentRepositoryStub : IPaymentRepository
    {
        //the data in the stub is actually encrypted
        //the decryption takes place in the payment engine
        private static IList<PaymentInformation> _informations = new List<PaymentInformation>()
        {
            //this is the default credit card that can be used to test an available amount
            new PaymentInformation()
            {
                CreditCardNumber = "OrkRWqplepEtBD8m++7Cgnoc66VFVaAky7xs7NnbjeZOMzttjLSsOQf+7rYXr0va1Ydhlv7BAWAJ1lZyM3WGpQ==", //1234567812345678
                CVV = "+mY3fecYSQ0ivYVMkovf1UCOtQvD+2u9FLrBEXoNJuJOzgmYsH6WK6jZMbvs9pmm", //123
                Expiration = "RwvMfB/obNiIgHDM8kru60zdjRuGf162LDGjUCNDdeQiLEJIMVCy//RV3gTztC4W", // 12/24
                IsInUse = true,
                AvailableAmount = double.MaxValue
            },
            //this is the default credit card to check if the card is still in use
            new PaymentInformation()
            {
                CreditCardNumber = "3msNf6bHnfZhuYLPrvGSWAwNI7Tiux8eJbrsWNhxyUOTjZdbcsLmQRLHAEhKLoetY3iHOkH6Gz+8aQFOfVKzVg==", //1234567812345679
                CVV = "+mY3fecYSQ0ivYVMkovf1UCOtQvD+2u9FLrBEXoNJuJOzgmYsH6WK6jZMbvs9pmm", //123
                Expiration = "RwvMfB/obNiIgHDM8kru60zdjRuGf162LDGjUCNDdeQiLEJIMVCy//RV3gTztC4W", // 12/24
                IsInUse = false,
                AvailableAmount = double.MaxValue
            },
            //this is the default credit card has enough credit
            new PaymentInformation()
            {
                CreditCardNumber = "IWz3FFgdJzFgJN/bvQOKjBOVwOr76mU7/j7H9X9dWkmHWHv1yN2lR5gN4+FQ1c/0TD0uJ7G4iAJTD8r1XabAcQ==", //1234567812345677
                CVV = "+mY3fecYSQ0ivYVMkovf1UCOtQvD+2u9FLrBEXoNJuJOzgmYsH6WK6jZMbvs9pmm", //123
                Expiration = "RwvMfB/obNiIgHDM8kru60zdjRuGf162LDGjUCNDdeQiLEJIMVCy//RV3gTztC4W", // 12/24
                IsInUse = true,
                AvailableAmount = 0
            },
            //this is the default credit card that can be used to check if the card has barely not enough credit
            new PaymentInformation()
            {
                CreditCardNumber = "cscXdT7Cc79fNAeDeJ3f1FuiuOWR6cbh/RT3QWDYn/6GdRBnEHyEVaJBca9bNN1Dy6CaW1PBOcWAR2y1Sb48UA==", //1234567812345676
                CVV = "+mY3fecYSQ0ivYVMkovf1UCOtQvD+2u9FLrBEXoNJuJOzgmYsH6WK6jZMbvs9pmm", //123
                Expiration = "RwvMfB/obNiIgHDM8kru60zdjRuGf162LDGjUCNDdeQiLEJIMVCy//RV3gTztC4W", // 12/24
                IsInUse = true,
                AvailableAmount = 10
            },
            //this is the default credit card that has already expired
            new PaymentInformation()
            {
                CreditCardNumber = "Ju0/k2n6OKBPg7koikIWUF7/qSeDe/Wj+Nf5j2+WH0ob8MOEW8NJ9U3fGVG45ybHqkW4hQQO/ENzPzb2M9U+Hg==", //1234567812345675
                CVV = "+mY3fecYSQ0ivYVMkovf1UCOtQvD+2u9FLrBEXoNJuJOzgmYsH6WK6jZMbvs9pmm", //123
                Expiration = "9Vk4SHvcgjcYXze8HXGINfMiMusBsCXC+n2Y5PvIUEuehINrtjKIi9IlLwm32grj", // 12/21
                IsInUse = true,
                AvailableAmount = 10
            },

        };

        public IList<PaymentInformation> GetAll() => _informations;
        public double? Get(string creditCartnumber, string cvv, string expiration) => (GetInformation(creditCartnumber, cvv, expiration))?.AvailableAmount ?? null;

        public bool Update(string creditCartnumber, string cvv, string expiration, double amount)
        {
            var info = GetInformation(creditCartnumber, cvv, expiration);

            if (info is null) return false;
            if (amount > info.AvailableAmount) throw new InvalidOperationException("A higher amount is not possible");

            info.AvailableAmount = amount;
            return true;
        }

        /// <summary>
        /// Retrieves the credit information if available.
        /// Passed data is encrypted.
        /// </summary>
        /// <param name="creditCartnumber">the credit card number</param>
        /// <param name="cvv">the cvv</param>
        /// <param name="expiration">the expiration</param>
        /// <returns></returns>
        /// <exception cref="AccessViolationException">If the credit information are no longer in use.</exception>
        private PaymentInformation? GetInformation(string creditCartnumber, string cvv, string expiration)
        {
            var _dcCn = CryptographyUtil.Decrypt(creditCartnumber, Constants.PASS);
            var _dcvv = CryptographyUtil.Decrypt(cvv, Constants.PASS);
            var _dcExp = CryptographyUtil.Decrypt(expiration, Constants.PASS);

            foreach (var info in _informations)
            {
                var _diCcn = CryptographyUtil.Decrypt(info.CreditCardNumber, Constants.PASS);
                var _diCvv = CryptographyUtil.Decrypt(info.CVV, Constants.PASS);
                var _diExp = CryptographyUtil.Decrypt(info.Expiration, Constants.PASS);

                if (_dcCn == _diCcn && _dcvv == _diCvv && _dcExp == _diExp)
                {
                    if (info.IsInUse)
                        return info;

                    throw new AccessViolationException("The credit card is currently not in use anymore");
                }
            }

            return null;
        }
    }
}
