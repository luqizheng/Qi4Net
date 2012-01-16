using Qi.Secret;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace Qi.Test
{


    /// <summary>
    ///This is a test class for EncryptHelperTest and is intended
    ///to contain all EncryptHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EncryptHelperTest
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
        ///A test for DecryptByDes
        ///</summary>
        [TestMethod()]
        public void DecryptByDesTest()
        {
            string content = "123456789";
            Encoding getByteFunc = Encoding.UTF8;
            string rgbKey = "abcdefgj";
            string rgbIv = "~!@#$%^&";

            var actual = DesHelper.EncryptByDes(content, getByteFunc, rgbKey, rgbIv);

            var target = DesHelper.DecryptByDes(actual, getByteFunc, rgbKey, rgbIv);
            var tar = getByteFunc.GetString(target);
            Assert.AreEqual(content, tar);

        }
    }
}
