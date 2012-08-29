using Qi.SharePools.Stores;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Qi.Test
{


    /// <summary>
    ///This is a test class for ThreadStaticStoreTest and is intended
    ///to contain all ThreadStaticStoreTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ThreadStaticStoreTest
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
        ///A test for ThreadStaticStore Constructor
        ///</summary>
        [TestMethod()]
        public void ThreadStaticStoreConstructorTest()
        {
            ThreadStaticStore target = new ThreadStaticStore();
            
        }

        /// <summary>
        ///A test for GetData
        ///</summary>
        [TestMethod()]
        public void GetDataTest()
        {
            ThreadStaticStore target = new ThreadStaticStore();

            string key = "object";
            object expected = "11";
            target.SetData(key, expected);
            object actual;
            actual = target.GetData(key);
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for SetData
        ///</summary>
        [TestMethod()]
        public void SetDataTest()
        {
            ThreadStaticStore target = new ThreadStaticStore();
            string key = "object";
            object data = 1;
            target.SetData(key, data);

            Assert.AreEqual(data, target.GetData(key));
            target.SetData(key, "2");
            Assert.AreEqual("2", target.GetData(key));

        }
    }
}
