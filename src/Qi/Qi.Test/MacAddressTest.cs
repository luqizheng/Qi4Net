using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qi.Net;

namespace Qi.Test
{


    /// <summary>
    ///This is a test class for MacAddressTest and is intended
    ///to contain all MacAddressTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MacAddressTest
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
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            byte[] bytes = new byte[] { 1, 2, 3, 4, 5, 6 };
            MacAddress target = new MacAddress(bytes);
            string expected = "01-02-03-04-05-06";
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for Parase
        ///</summary>
        [TestMethod()]
        public void ParaseTest()
        {
            MacAddress expected = new MacAddress(0x00, 0x23, 0x33, 0x84, 0x1f, 0x40);
            MacAddress actual;
            actual = MacAddress.Parse("002333841f40");
            Assert.AreEqual(expected.ToString(), actual.ToString());

        }

        /// <summary>
        ///A test for MacAddress Constructor
        ///</summary>
        [TestMethod()]
        public void MacAddressConstructorTest1()
        {
            byte a = 1;
            byte b = 2;
            byte c = 3;
            byte d = 4;
            byte e = 5;
            byte f = 6;
            MacAddress target = new MacAddress(a, b, c, d, e, f);

        }



        /// <summary>
        ///A test for Parse
        ///</summary>
        [TestMethod()]
        public void ParseTest()
        {
            string mac = "F0-01-0E-EF-37-37";

            byte[] bytes = new byte[] { 240, 1, 14, 239, 55, 55 };
            MacAddress expected = new MacAddress(bytes);

            MacAddress actual = MacAddress.Parse(mac);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EditVoid()
        {
            byte[] bytes = new byte[] { 240, 1, 14, 239, 55, 55 };
            MacAddress expected = new MacAddress(bytes);
            var result = new System.Collections.Generic.Dictionary<MacAddress, string> {{expected, "ok"}};

            MacAddress target = new MacAddress(bytes);
            Assert.IsTrue(result.ContainsKey(target));
        }

      
    }
}
