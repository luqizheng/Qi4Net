using Qi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace Qi.Test
{


    /// <summary>
    ///This is a test class for QCacheTest and is intended
    ///to contain all QCacheTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QCacheTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for QCache Constructor
        ///</summary>
        [TestMethod()]
        public void QCacheConstructorTest()
        {
            for (int i = 0; i < 3; i++)
            {
                QCache target = new QCache();
                var key = target.Add("15", 1000);
                Assert.AreEqual("15", target.GetObj(key), "index:" + i);
            }
        }

        /// <summary>
        ///A test for QCache Constructor
        ///</summary>
        [TestMethod()]
        public void QCahce_test_expiry()
        {

            QCache target = new QCache();
            var key1 = target.Add("1", 1000);
            var key2 = target.Add("2", 3000);
            var key3 = target.Add("3", 6000);
            Assert.AreEqual("1", target.GetObj(key1));
            Assert.AreEqual("2", target.GetObj(key2));
            Assert.AreEqual("3", target.GetObj(key3));

            Thread.Sleep(1200);
            Assert.IsNull(target.GetObj(key1));
            Assert.AreEqual("2", target.GetObj(key2));
            Assert.AreEqual("3", target.GetObj(key3));

            Thread.Sleep(2000);
            Assert.IsNull(target.GetObj(key1));
            Assert.IsNull(target.GetObj(key2));
            Assert.IsNotNull(target.GetObj(key3));

            Thread.Sleep(3000);
            Assert.IsNull(target.GetObj(key1));
            Assert.IsNull(target.GetObj(key2));
            Assert.IsNull(target.GetObj(key3));

            Thread.Sleep(10000); //wait for disposing all obj.
            target.Dispose();

        }




    

    }
}
