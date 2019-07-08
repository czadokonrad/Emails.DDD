using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Emails.DAL.IntegrationTests
{
    public class IsolatedAttribute : Attribute, ITestAction
    {
        private TransactionScope _scope;
        public ActionTargets Targets => ActionTargets.Test;

        public void AfterTest(ITest test)
        {
            if (_scope != null)
                _scope.Dispose();

            _scope = null;
        }

        public void BeforeTest(ITest test)
        {
            _scope = new TransactionScope(TransactionScopeOption.RequiresNew);

        }
    }
}
