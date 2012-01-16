using Qi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Qi.Test
{


    /// <summary>
    ///This is a test class for ArrayHelperTest and is intended
    ///to contain all ArrayHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ArrayHelperTest
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
        ///A test for Split
        ///</summary>
        [TestMethod]
        public void SplitTestHelper()
        {
            int[] ary = new int[33];
            for (int i = 0; i < ary.Length; i++)
            {
                ary[i] = i;
            }
            int eachAryLength = 10;

            int remainder = 0;
            int remainderExpected = 3;

            var actual = ArrayHelper.Split<int>(ary, eachAryLength, out remainder);
            Assert.AreEqual(4, actual.Count);
            Assert.AreEqual(3, remainder);
            Assert.AreEqual(remainderExpected, remainder);

        }



        [TestMethod()]
        public void DivEqualTest_7_div_3()
        {
            int[] ary = new int[7];
            for (int i = 0; i < ary.Length; i++)
            {
                ary[i] = i;
            }
            int dividEquallyNumber = 3;
            var target = ary.DivEqual(dividEquallyNumber);
            Assert.AreEqual(target.Length, 3);
            Assert.AreEqual(3, target[0].Length);
            var startChecked = 0;
            for (int i = startChecked; i < target[0].Length; i++)
            {
                Assert.AreEqual(i, target[0][i]);
                startChecked++;
            }

            Assert.AreEqual(2, target[1].Length);
            for (int i = startChecked; i < target[1].Length; i++)
            {
                Assert.AreEqual(i, target[1][i]);
            }
        }

        [TestMethod()]
        public void DivEqualTest_8_for_4()
        {
            int[] ary = new int[8];
            for (int i = 0; i < ary.Length; i++)
            {
                ary[i] = i;
            }
            int dividEquallyNumber = 4;
            var target = ary.DivEqual(dividEquallyNumber);
            Assert.AreEqual(target.Length, 4);
            foreach (var aryItem in target)
            {
                Assert.AreEqual(2, aryItem.Length);
            }
        }

        [TestMethod()]
        public void DivEqualTest_4_for_8()
        {
            int[] ary = new int[4];
            for (int i = 0; i < ary.Length; i++)
            {
                ary[i] = i;
            }
            int dividEquallyNumber = 8;
            var target = ary.DivEqual(dividEquallyNumber);
            Assert.AreEqual(target.Length, 4);
            foreach (var aryItem in target)
            {
                Assert.AreEqual(1, aryItem.Length);
            }
        }

        [TestMethod()]
        public void SubArrayTest()
        {
            int[] array = new int[]
            {
                0,1,2,3,4,5
            };

            var target = array.SubArray(1,2);
            Assert.AreEqual(1, target[0]);
            Assert.AreEqual(2, target[1]);
            
        }


        [TestMethod()]
        public void SubArray_Test()
        {
            int[] array = new int[]
            {
                0,1,2,3,4,5
            };

            var target = array.SubArray(1);
            Assert.AreEqual(1, target[0]);
            Assert.AreEqual(2, target[1]);
            Assert.AreEqual(3, target[2]);
            Assert.AreEqual(4, target[3]);
            Assert.AreEqual(5, target[4]);
        }


    }
}
