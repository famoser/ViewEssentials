using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Tests.LanguageTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FrameworkEssentials.Tests.LanguageTests
{
    [TestClass]
    public class DelegateTests
    {
        [TestMethod]
        public void TestFuncInheritance()
        {
            var dh = new DelegateHelper();
            Delegate delegateAction = new Action(dh.DoStuff);
            Delegate delegateStaticAction = new Action(dh.DoStuffStatic);
            Delegate delegateFuncAsync = new Func<Task>(dh.DoStuffAsync);
            Delegate delegateFuncReturnAsync = new Func<Task<SomeModel>>(dh.GetSomeModelAsync);
            Delegate delegateFunc = new Func<SomeModel>(dh.GetSomeModel);
        }
    }
}
