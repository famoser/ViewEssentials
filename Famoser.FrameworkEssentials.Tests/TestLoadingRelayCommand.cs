using System;
using System.Linq;
using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.View.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.FrameworkEssentials.Tests
{
    [TestClass]
    public class TestLoadingRelayCommand
    {
        [TestMethod]
        public void TestDisposable()
        {
            var command = new LoadingRelayCommand(() => { });
            var pgs = new ProgressService();
            var key = Guid.NewGuid();

            Assert.IsTrue(command.CanExecute(null));
            using (var g = command.GetProgressDisposable(pgs, key))
            {
                Assert.IsFalse(command.CanExecute(null));
                Assert.IsTrue(pgs.AnyProgressActive);
                Assert.IsTrue(pgs.IndeterminateProgressActive);
                Assert.IsTrue(pgs.GetActiveIndeterminateProgresses().Count == 1);
                Assert.IsTrue((Guid)pgs.GetActiveIndeterminateProgresses().First() == key);
            }
            Assert.IsTrue(command.CanExecute(null));
        }
    }
}
