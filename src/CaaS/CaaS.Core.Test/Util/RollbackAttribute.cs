using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CaaS.Core.Test.Util
{
    internal class RollbackAttribute : Attribute, ITestAction
    {
        private TransactionScope? transaction;

        public void BeforeTest(ITest test)
        {
            transaction = new TransactionScope();
        }

        public void AfterTest(ITest test)
        {
            transaction.Dispose();  
        }

        public ActionTargets Targets
        {
            get { return ActionTargets.Test; }
        }
    }
}
