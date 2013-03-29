using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Qi.NHibernateExtender.Types;

namespace Qi.Test
{
    /// <summary>
    ///     This is a test class for KeyValueCollectionTypeTest and is intended
    ///     to contain all KeyValueCollectionTypeTest Unit Tests
    /// </summary>
    [TestClass]
    public class KeyValueCollectionTypeTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
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
        ///     A test for Compare
        /// </summary>
        [TestMethod]
        public void CompareTest()
        {
            var target = new KeyValueCollectionType(); // TODO: Initialize to an appropriate value
            object x = null; // TODO: Initialize to an appropriate value
            object y = null; // TODO: Initialize to an appropriate value
            var entityMode = new EntityMode?(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.Compare(x, y, entityMode);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}