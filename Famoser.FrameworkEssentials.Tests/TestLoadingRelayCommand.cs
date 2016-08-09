using System;
using System.Linq;
using System.Threading.Tasks;
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
            var dependent = new LoadingRelayCommand(() => { });
            command.AddDependentCommand(dependent);
            var pgs = new ProgressService();
            var key = Guid.NewGuid();

            Assert.IsTrue(command.CanExecute(null));
            Assert.IsTrue(dependent.CanExecute(null));
            using (var g = command.GetProgressDisposable(pgs, key))
            {
                Assert.IsFalse(command.CanExecute(null));
                Assert.IsFalse(dependent.CanExecute(null));
                Assert.IsTrue(pgs.AnyProgressActive);
                Assert.IsTrue(pgs.IndeterminateProgressActive);
                Assert.IsTrue(pgs.GetActiveIndeterminateProgresses().Count == 1);
                Assert.IsTrue((Guid)pgs.GetActiveIndeterminateProgresses().First() == key);
            }
            Assert.IsTrue(command.CanExecute(null));
            Assert.IsTrue(dependent.CanExecute(null));
        }

        [TestMethod]
        public async Task TestAsyncDisposable()
        {
            var command = new LoadingRelayCommand(async () => { await Task.Delay(1000); }, null, true);
            Assert.IsTrue(command.CanExecute(null));
            command.Execute(null);
            Assert.IsFalse(command.CanExecute(null));
            await Task.Delay(2000);
            Assert.IsTrue(command.CanExecute(null));
        }
    }
}
