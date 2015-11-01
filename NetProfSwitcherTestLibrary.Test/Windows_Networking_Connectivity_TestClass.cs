using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using NetProfileSwitcher;
using System.Collections;

namespace NetProfSwitcherTestLibrary.Test
{
    [TestFixture]
    public class Windows_Networking_Connectivity_TestClass
    {
        [TestCase]
        public void getConnectionProfilesTest() {
            string [] profiles = Windows_Networking_Connectivity_Helper.GetConnectionProfiles();
            Assert.Greater(profiles.Length, 0);
        }
    }
}
