using Qi.Sms.Protocol.SendCommands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test.Qi.Sms.Core
{
    
    
    /// <summary>
    ///This is a test class for CmtiCommandTest and is intended
    ///to contain all CmtiCommandTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CmtiCommandTest
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
        ///A test for GetSmsIndex
        ///</summary>
        [TestMethod()]
        public void GetSmsIndexTest()
        {
            string content = "CMTI: \"SM\",1\r\n\r\nCMTI: \"SM\",2\r\n\r\nCMTI: \"SM\",3\r\n\r\nCMTI: \"SM\",4\r\n\r\nCMTI: \"SM\",5\r\n\r\n";
            int[] expected = new int[]{1,2,3,4,5};
            int[] actual;
            actual = CmtiCommand.GetSmsIndex(content);
            Assert.AreEqual(expected, actual);
            
        }
    }
}
