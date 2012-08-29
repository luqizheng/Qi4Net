using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qi.SharePools.Stores;

namespace Qi.Test
{
    /// <summary>
    ///This is a test class for HttpStoreTest and is intended
    ///to contain all HttpStoreTest Unit Tests
    ///</summary>
    [TestClass]
    public class HttpStoreTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

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
        ///A test for Dictionary
        ///</summary>
        [TestMethod]
        public void SetDateTest()
        {
            var httpStore = new HttpStore();
            httpStore.SetData("a", "object");
            httpStore.SetData("a", "b");
        }

        /// <summary>
        ///A test for Dictionary
        ///</summary>
        [TestMethod]
        public void GetDateTest()
        {
            var httpStore = new HttpStore();
            httpStore.SetData("a", "object");
            Assert.AreEqual("object", httpStore.GetData("a"));
            httpStore.SetData("a", "b");
            Assert.AreEqual("b", httpStore.GetData("a"));
        }
    }
}