using Qi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Qi.Test
{
    
    
    /// <summary>
    ///This is a test class for ApplicationHelperTest and is intended
    ///to contain all ApplicationHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ApplicationHelperTest
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
        ///A test for PhysicalApplicationPath
        ///</summary>
        [TestMethod()]
        public void PhysicalApplicationPathTest()
        {
            string actual;
            actual = ApplicationHelper.PhysicalApplicationPath;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MapPath
        ///</summary>
        [TestMethod()]
        public void MapPathTest()
        {
            string currentPath = @"C:\aa\bb\cc\";
            string mapPath = "../../22/33/aa.html";
            string expected = @"C:\aa\22\33\aa.html";
            string actual;
            actual = ApplicationHelper.MapPath(new DirectoryInfo(currentPath), mapPath);
            Assert.AreEqual(expected, actual);

        }

        [TestMethod()]
        public void MapPathTest_mapPath_is_empty()
        {
            string currentPath = @"C:\aa\bb\cc";
            string mapPath = "";
            string expected = currentPath;
            string actual;
            actual = ApplicationHelper.MapPath(new DirectoryInfo(currentPath), mapPath);
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for ApplicationHelper Constructor
        ///</summary>
        [TestMethod()]
        public void ApplicationHelperConstructorTest()
        {
            ApplicationHelper target = new ApplicationHelper();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
