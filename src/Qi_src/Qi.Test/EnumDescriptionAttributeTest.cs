using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qi.Test
{
    /// <summary>
    ///This is a test class for EnumDescriptionAttributeTest and is intended
    ///to contain all EnumDescriptionAttributeTest Unit Tests
    ///</summary>
    [TestClass]
    public class EnumDescriptionAttributeTest
    {
        #region Car enum

        public enum Car
        {
            [EnumDescription("巴士")] Bus,
            [EnumDescription("跑車")] RaceCard,
        }

        #endregion

        #region MultiDescription enum

        [Flags]
        public enum MultiDescription
        {
            [EnumDescription("1")] A = 1,
            [EnumDescription("2")] B = 2,
            [EnumDescription("3")] C = 4,
        }

        #endregion

        #region MultiDescription_HaveNotDesctiption enum

        [Flags]
        public enum MultiDescription_HaveNotDesctiption
        {
            [EnumDescription("1")] A = 1,
            B = 2,
            [EnumDescription("3")] C = 4,
        }

        #endregion

        #region Status enum

        [Flags]
        public enum Status
        {
            [EnumDescription("沒有權限")] None = 0,
            [EnumDescription("讀")] Read = 1,
            [EnumDescription("寫")] Write = 3,
            [EnumDescription("刪除")] Delete = 7
        }

        #endregion

        #region StatusNoflags enum

        public enum StatusNoflags
        {
            [EnumDescription("沒有權限")] None = 0,
            [EnumDescription("讀")] Read,
            [EnumDescription("寫")] Write,
            [EnumDescription("刪除")] Delete,
        }

        #endregion

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

        [TestMethod]
        public void TestGe()
        {
            SortedDictionary<string, TestResource> di = EnumHelper.GetDescriptionList<TestResource>();
            Assert.AreEqual(2, di.Count);
        }

        [TestMethod]
        public void TestToDescription()
        {
            Assert.AreEqual("巴士", Car.Bus.ToDescription());
            Assert.AreEqual("跑車", Car.RaceCard.ToDescription());
        }

        [TestMethod]
        public void Description_multi_enum()
        {
            Status target = Status.Delete;
            Assert.AreEqual("讀,寫,刪除", target.ToDescription());

            target = Status.Read;
            Assert.AreEqual("讀", target.ToDescription());

            target = Status.Write;
            Assert.AreEqual("讀,寫", target.ToDescription());
        }

        [TestMethod]
        public void Description_multi_enum_order_without_flagsattr()
        {
            StatusNoflags target = StatusNoflags.Delete;
            Assert.AreEqual("刪除", target.ToDescription());

            target = StatusNoflags.None;
            Assert.AreEqual("沒有權限", target.ToDescription());
        }

        [TestMethod]
        public void EnumDescriptionAttribute_ResourceKey_multiDistrict()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            string result = TestResource.Orange.ToDescription();
            Assert.AreEqual(Resource1.String2, result);

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("zh-CN");
            Resource1.Culture = Thread.CurrentThread.CurrentCulture;
            result = TestResource.Orange.ToDescription();
            Assert.AreEqual(Resource1.String2, result);

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("zh-TW");
            Resource1.Culture = Thread.CurrentThread.CurrentCulture;
            result = EnumHelper.ToDescription(TestResource.Apple, CultureInfo.GetCultureInfo("zh-TW"));
            Assert.AreEqual(Resource1.String1, result);
        }

        /// <summary>
        ///A test for Description
        ///</summary>
        [TestMethod]
        public void DescriptionTest_multi_descrption()
        {
            const MultiDescription input = MultiDescription.A | MultiDescription.B | MultiDescription.C;
            string actual = input.ToDescription();
            string expected = "1,2,3";
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Description
        ///</summary>
        [TestMethod]
        public void DescriptionTest_multi_haveNot_descrption()
        {
            const MultiDescription_HaveNotDesctiption input =
                MultiDescription_HaveNotDesctiption.A | MultiDescription_HaveNotDesctiption.B |
                MultiDescription_HaveNotDesctiption.C;
            string actual = input.ToDescription();
            string expected = "1,B,3";
            Assert.AreEqual(expected, actual);
        }

        #region Nested type: TestResource

        [Flags]
        private enum TestResource
        {
            [EnumDescription("String1", ResourceType = typeof (Resource1))] Apple,
            [EnumDescription("String2", ResourceType = typeof (Resource1))] Orange,
        }

        #endregion
    }
}