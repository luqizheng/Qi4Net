using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ornament;

namespace Qi.Test
{
    /// <summary>
    /// Summary description for EnumHelperTest
    /// </summary>
    [TestClass]
    public class EnumHelperTest
    {
        public enum TestEnum
        {
            [EnumDescription("apple", IsDefault = true), EnumDescription("苹果", Language = "zh-CN")]
            A,
            [EnumDescription("banana", IsDefault = true), EnumDescription("香蕉", Language = "zh-CN")]
            B,
            [EnumDescription("cat", IsDefault = true), EnumDescription("猫", Language = "zh-CN")]
            C,
            [EnumDescription("dog", IsDefault = true), EnumDescription("狗", Language = "zh-CN")]
            D,
        }

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
        ///A test for GetObjects
        ///</summary>
        [TestMethod]
        public void GetObjectsTestHelper()
        {
            var expected = new[] { TestEnum.A, TestEnum.B, TestEnum.C, TestEnum.D };
            TestEnum[] actual;
            actual = EnumHelper.GetObjects<TestEnum>();
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
            Assert.AreEqual(expected[2], actual[2]);
            Assert.AreEqual(expected[3], actual[3]);
        }

        /// <summary>
        ///A test for GetDescriptionList
        ///</summary>
        [TestMethod]
        public void GetDescriptionListTestHelper()
        {
            var expected = new Dictionary<string, TestEnum>
                               {
                                   {"apple", TestEnum.A},
                                   {"banana", TestEnum.B},
                                   {"cat", TestEnum.C},
                                   {"dog", TestEnum.D}
                               };
            Dictionary<string, TestEnum> actual;
            actual = EnumHelper.GetDescriptionList<TestEnum>();

            foreach (string key in expected.Keys)
            {
                Assert.AreEqual(expected[key], actual[key]);
            }
        }

        /// <summary>
        ///A test for GetDescriptionList
        ///</summary>
        [TestMethod]
        public void GetDescriptionList_MultiLanguage_TestHelper()
        {
            var expected = new Dictionary<string, TestEnum>
                               {
                                   {"苹果", TestEnum.A},
                                   {"香蕉", TestEnum.B},
                                   {"猫", TestEnum.C},
                                   {"狗", TestEnum.D}
                               };
            Dictionary<string, TestEnum> actual;
            actual = EnumHelper.GetDescriptionList<TestEnum>("zh-CN");

            foreach (string key in expected.Keys)
            {
                TestEnum s = expected[key];
                Assert.AreEqual(s, actual[key]);
            }
        }

        [TestMethod]
        public void ToEnum_Test()
        {
            TestEnum target = TestEnum.A;
            var except = EnumHelper.ToEnum<TestEnum>(target.ToString());
            Assert.AreEqual(target, except);
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            Enum enumValue = TestEnum.A;
            string expected = "苹果";
            string actual;
            actual = EnumHelper.ToString(enumValue);
            Assert.AreEqual(expected, actual);

        }
    }
}
