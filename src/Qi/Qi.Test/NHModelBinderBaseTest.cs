using Qi.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Qi.Test
{
    
    
    /// <summary>
    ///This is a test class for NHModelBinderBaseTest and is intended
    ///to contain all NHModelBinderBaseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NHModelBinderBaseTest
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


        internal virtual NHModelBinder CreateNHModelBinderBase()
        {
            // TODO: Instantiate an appropriate concrete class.
            NHModelBinder target = null;
            return target;
        }

        /// <summary>
        ///A test for IsPersistentType
        ///</summary>
        [TestMethod()]
        public void IsPersistentTypeTest()
        {
            NHModelBinder target = CreateNHModelBinderBase(); // TODO: Initialize to an appropriate value
            Type modelType = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsPersistentType(modelType);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
