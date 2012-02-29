using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qi.Test
{
    /// <summary>
    ///This is a test class for FounderTest and is intended
    ///to contain all FounderTest Unit Tests
    ///</summary>
    [TestClass]
    public class FounderTest
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
        ///A test for Closest
        ///</summary>
        [TestMethod]
        public void ClosestTest()
        {
            var org = new Org
                          {
                              Name = "Org_1_1_1",
                              Parent = new Org
                                           {
                                               Name = "Org_1_1",
                                               Parent = new Org
                                                            {
                                                                Name = "Org_1"
                                                            }
                                           }
                          };


            Org startObject = org;
            var result = startObject.Closest(o => o.Parent.Name, o => o != null);

            string reslt = String.Join("->", result);
            Assert.AreEqual("", reslt);
        }

        #region Nested type: Org

        public class Org
        {
            public string Name { get; set; }
            public Org Parent { get; set; }
        }

        #endregion
    }
}