using Qi.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Qi.Test
{


    /// <summary>
    ///This is a test class for JsonHelperTest and is intended
    ///to contain all JsonHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class JsonHelperTest
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
        ///A test for ToJson
        ///</summary>
        [TestMethod()]
        public void ToJsonTest()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("Name", "11");
            data.Add("Teacher", new { Name = "theacher", Couse = "English" });
            data.Add("Family", new Dictionary<string, object>()
                                    {
                                        {"facther",new {Name="john",Birthday=new DateTime(1979,1,1)}},
                                        {"mother",new {Name="Hellen",Birthday=new DateTime(1979,1,1)}},
                                    });
            var expect = "{\"Name\":\"11\",\"Teacher\":{\"Name\":\"theacher\",\"Couse\":\"English\"},\"Family\":{\"facther\":{\"Name\":\"john\",\"Birthday\":\"\\/Date(283968000000)\\/\"},\"mother\":{\"Name\":\"Hellen\",\"Birthday\":\"\\/Date(283968000000)\\/\"}}}";

            var target = data.ToJson(false);
            Assert.AreEqual(expect, target);

        }
    }
}
