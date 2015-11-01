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
    public class Win32_AdapterConfiguration_Help_TestClass
    {
        /// <summary>
        /// at an invalid NIC name provision 
        /// </summary>
        [TestCase]
        public void setDHCPTest() {
            string nic = "WrongTestNIC";
            int rtn = Win32_NetAdapConfig_Help.SetDHCP(nic);
            Assert.AreEqual(-1, rtn);
        }

        /// <summary>
        /// at an invalid NIC name provision 
        /// </summary>
        [TestCase]
        public void setIPTest() {
            string nic = "WrongTestNIC";
            int[] rtn = Win32_NetAdapConfig_Help.SetIP(nic,"","","","");
            Assert.AreEqual(-1, rtn[0]);
            Assert.AreEqual(-1, rtn[1]);
            Assert.AreEqual(-1, rtn[2]);      
        }

        /// <summary>
        /// at an invalid NIC name provision 
        /// </summary>
        [TestCase]
        public void getIPTest() {
            string[] ipAddress = null;
            string[] subnets = null;
            string[] gateways = null;
            string[] dnses = null;

            string nicName = "WrongTestNIC";
            Win32_NetAdapConfig_Help.GetIP(nicName, out ipAddress, out subnets, out gateways, out dnses);

            Assert.AreEqual(null, ipAddress);
            Assert.AreEqual(null, subnets);
            Assert.AreEqual(null, gateways);
            Assert.AreEqual(null, dnses);

        }

        /// <summary>
        /// test whether 'Win32_NetAdapConfig_Help.getNetworkCards()' method returns at least one NIC name
        /// </summary>
        [TestCase]
        public void getNICsTest() {
            ArrayList nicList = Win32_NetAdapConfig_Help.getNetworkCards();
            Assert.Greater(nicList.Count, 0);
        }



    }
}
