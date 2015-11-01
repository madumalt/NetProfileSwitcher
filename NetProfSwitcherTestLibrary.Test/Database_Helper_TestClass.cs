using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using NetProfileSwitcher;
using System.Collections;
using System.Data.SQLite;

namespace NetProfSwitcherTestLibrary.Test
{
    public class Database_Helper_TestClass
    {
        /// <summary>
        /// test inserting to network table
        /// </summary>
        [TestCase]
        public void Insert_to_networkTableTest() {
            NetworkPageDetails npd = new NetworkPageDetails();
            npd.profileName = "TestingProfile";
            Assert.AreEqual(DatabaseMessage.databaseError, Database_Helper.Insert_to_networkTable(npd));
        }

        /// <summary>
        /// inserting to proxy table
        /// </summary>
        [TestCase]
        public void Insert_to_proxyTableTest() {
            ProxyPageDetails ppd = new ProxyPageDetails();
            ppd.profileName = "TestingProfile";
            ppd.enableProxy = 0;
            Assert.AreEqual(DatabaseMessage.databaseError, Database_Helper.Insert_to_proxyTable(ppd));
        }

        /// <summary>
        /// test reading from network table
        /// </summary>
        [TestCase]
        public void Read_networkTableTest() {
            Assert.AreEqual("TestingProfile", Database_Helper.Read_networkTable("TestingProfile").profileName);
        }

        /// <summary>
        /// test reading from proxy table
        /// </summary>
        [TestCase]
        public void Readd_proxyTableTest() {
            Assert.AreEqual("TestingProfile", Database_Helper.Readd_proxyTable("TestingProfile").profileName);
        }
    }
}
