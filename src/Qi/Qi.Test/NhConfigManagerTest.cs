using Qi.Nhibernates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Qi.Test
{


    /// <summary>
    ///This is a test class for NhConfigManagerTest and is intended
    ///to contain all NhConfigManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NhConfigManagerTest
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
        ///A test for Contains
        ///</summary>
        [TestMethod(),ExpectedException(typeof(ArgumentNullException))]
        public void ContainsTest_Empty()
        {
            string sessionFactoryName = string.Empty;
            bool expected = false;
            bool actual;
            actual = NhConfigManager.Contains(sessionFactoryName);
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod(), ExpectedException(typeof(ArgumentNullException))]
        public void ContainsTest_null()
        {
            string sessionFactoryName = string.Empty;
            bool expected = false;
            bool actual;
            actual = NhConfigManager.Contains(sessionFactoryName);
            Assert.AreEqual(expected, actual);

        }
    }
}
