using System;
using Ornament.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Qi.Test
{
    
    
    /// <summary>
    ///This is a test class for NamedFormatterHelperTest and is intended
    ///to contain all NamedFormatterHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NamedFormatterHelperTest
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
        ///A test for Replace
        ///</summary>
        [TestMethod]
        public void ReplaceTest()
        {
            string formatString = "Hello,[Person], you are a good [job]";
            IDictionary<string, string> replacePattern = new Dictionary<string, string>
                                                             {
                                                                 {"Person", "John"},
                                                                 {"job", "student"}
                                                             };
            string expected = "Hello,John, you are a good student";
            string actual;
            actual = NamedFormatterHelper.Replace(formatString, replacePattern);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CollectVariable
        ///</summary>
        [TestMethod]
        public void CollectVariableTest()
        {
            string content = @"string [a] ok
[b.a]kdjfkdjfkdj [a][b]
[c]isAGood[a]";
            var expected = new[] { "a", "b.a", "c", "b" };
            string[] actual;
            actual = NamedFormatterHelper.CollectVariable(content);
            Assert.AreEqual(expected.Length, actual.Length);

            for (int idx = 0; idx < actual.Length; idx++)
            {
                int aryIndex = Array.IndexOf(expected, actual[idx]);
                Assert.AreNotEqual(-1, aryIndex, expected[idx] + " isn't in expected array");
            }
        }
    }
}
