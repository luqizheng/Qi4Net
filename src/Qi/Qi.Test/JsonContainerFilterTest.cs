using Qi.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Qi.Test
{


    /// <summary>
    ///This is a test class for JsonContainerFilterTest and is intended
    ///to contain all JsonContainerFilterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class JsonContainerFilterTest
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




        ///// <summary>
        /////A test for IsArray
        /////</summary>
        //[TestMethod()]
        //public void IsArrayTest()
        //{
        //    JsonContainerFilter target = new JsonContainerFilter();
        //    string key = "data[]";

        //    bool actual;
        //    actual = target.IsArray(key);
        //    Assert.AreEqual(true, actual);

        //    key = "data.cc";
        //    actual = target.IsArray(key);
        //    Assert.AreEqual(false, actual);

        //    key = "data[cc]";
        //    actual = target.IsArray(key);
        //    Assert.AreEqual(false, actual);

        //    key = "data[0]";

        //    actual = target.IsArray(key);
        //    Assert.AreEqual(true, actual);

        //    key = "data[aa][0]";
        //    actual = target.IsArray(key);
        //    Assert.AreEqual(true, actual);

        //}



        /// <summary>
        ///A test for SetTheKeyPath
        ///</summary>
        [TestMethod()]
        public void ToJsonContainerExpressTest()
        {
            var actual = "j4[0][b1][]";
            string expected = "j4[0].b1[]";

            Assert.AreEqual(expected, JsonContainerFilter.ToJsonContainerExpress(actual));

            actual = "j4[0][b1]";
            expected = "j4[0].b1";
            Assert.AreEqual(expected, JsonContainerFilter.ToJsonContainerExpress(actual));

            actual = "Edit4Net[3][bTest]";
            expected = "Edit4Net[3].bTest";
            Assert.AreEqual(expected, JsonContainerFilter.ToJsonContainerExpress(actual));

        }


    }
}
