using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.FrameworkEssentials.Tests.LanguageTests.TestClasses
{
    public class DelegateHelper
    {
        public static SomeModel GetSomeModelStatic()
        {
            return new SomeModel();
        }

        public SomeModel GetSomeModel()
        {
            return new SomeModel();
        }

        public async Task<SomeModel> GetSomeModelAsync()
        {
            return new SomeModel();
        }

        public async Task DoStuffAsync()
        {
            return;
        }

        public void DoStuffStatic()
        {
            return;
        }

        public void DoStuff()
        {
            return;
        }
    }
}
