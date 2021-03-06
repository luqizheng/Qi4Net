﻿using Qi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Qi.Test
{
    
    
    /// <summary>
    ///This is a test class for QMathTest and is intended
    ///to contain all QMathTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QMathTest
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
        ///A test for Ceilling
        ///</summary>
        [TestMethod()]
        public void CeillingTest()
        {
            Decimal i = 16.151m;
            int midPointRouting = 2;
            Decimal expected = 16.16m;
            Decimal actual;
            actual = QMath.Ceilling(i, midPointRouting);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Ceilling
        ///</summary>
        [TestMethod()]
        public void CeillingTest_d()
        {
            Decimal i = 16.156m;
            int midPointRouting = 2;
            Decimal expected = 16.16m;
            Decimal actual;
            actual = QMath.Ceilling(i, midPointRouting);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Ceilling
        ///</summary>
        [TestMethod()]
        public void CeillingTest_d_over_point()
        {
            Decimal i = 16.156m;
            int midPointRouting = 4;
            Decimal expected = 16.156m;
            Decimal actual;
            actual = QMath.Ceilling(i, midPointRouting);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Ceilling
        ///</summary>
        [TestMethod()]
        public void CeillingTest_d_midPointRouting()
        {
            Decimal i = 16.156m;
            int midPointRouting = 0;
            Decimal expected = 17;
            Decimal actual;
            actual = QMath.Ceilling(i, midPointRouting);
            Assert.AreEqual(expected, actual);
        }
        /// <summary>
        ///A test for Truncate
        ///</summary>
        [TestMethod()]
        public void TruncateTest()
        {
            Decimal i = 16.151m;
            int midPointRouting = 2;
            Decimal expected = 16.15m;
            Decimal actual;
            actual = QMath.Truncate(i, midPointRouting);
            Assert.AreEqual(expected, actual);
        }
    }
}
