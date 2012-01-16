using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qi.Test
{
    /// <summary>
    ///This is a test class for TimeTest and is intended
    ///to contain all TimeTest Unit Tests
    ///</summary>
    [TestClass]
    public class TimeTest
    {
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
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod]
        public void op_SubtractionTest()
        {
            var a = new Time(23, 59, 59);
            var b = new Time(23, 0, 0);
            var expected = new TimeSpan(0, 59, 59);
            TimeSpan actual;
            actual = (a - b);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for AddSecond
        ///</summary>
        [TestMethod]
        public void AddSecondTest()
        {
            int hour = 8;
            int mins = 0;
            int second = 1;
            var target = new Time(hour, mins, second);
            int second1 = 1;
            target = target.AddSeconds(second1);

            var expect = new TimeSpan(hour, mins, second1 + second);
            Assert.AreEqual(expect.Ticks, target.Ticks);
        }

        /// <summary>
        ///A test for AddMins
        ///</summary>
        [TestMethod]
        public void AddMinsTest()
        {
            int hour = 8;
            int mins = 9;
            int second = 1;
            var target = new Time(hour, mins, second);
            int min = 55;
            target = target.AddMinutes(min);
            var expect = new TimeSpan(hour, (min + mins), second);
            Assert.AreEqual(expect.Ticks, target.Ticks);
        }

        /// <summary>
        ///A test for AddMillsecond
        ///</summary>
        [TestMethod]
        public void AddMillsecondTest()
        {
            int hour = 1;
            int mins = 2;
            int second = 3;
            var target = new Time(hour, mins, second);
            int millsecond = 3;
            target = target.AddMillseconds(millsecond);
            var expcet = new TimeSpan(0, 1, 2, 3, 3);
            Assert.AreEqual(expcet.Ticks, expcet.Ticks);
        }

        /// <summary>
        ///A test for AddHour
        ///</summary>
        [TestMethod]
        public void AddHourTest()
        {
            int hour = 1;
            int mins = 2;
            int second = 3;
            var target = new Time(hour, mins, second);
            int hour1 = 3;
            target.AddHours(hour1);
            var expect = new DateTime(1, 1, 1, hour, mins, second);
            expect.AddHours(hour1);

            Assert.AreEqual(expect.Ticks, target.Ticks);
        }
        [TestMethod]
        public void AddHourMoreThan_23_hour()
        {
            int hour = 1;
            int mins = 0;
            int second = 0;
            var target = new Time(hour, mins, second);
            int hour1 = 29;
            target = target.AddHours(hour1);

            Assert.AreEqual("6:0:0", target.ToString());
        }


        /// <summary>
        ///A test for Time Constructor
        ///</summary>
        [TestMethod]
        public void TimeConstructorTest1()
        {
            int hour = 1;
            int mins = 2;
            int second = 3;
            var target = new Time(hour, mins, second);
            var except = new DateTime(1, 1, 1, hour, mins, second);
            Assert.AreEqual(except.Ticks, target.Ticks);
        }

        /// <summary>
        ///A test for Time Constructor
        ///</summary>
        [TestMethod]
        public void TimeConstructorTest()
        {
            int hour = 1;
            int mins = 2;
            int second = 3;
            int millsecond = 1;
            var target = new Time(hour, mins, second, millsecond);
            var except = new DateTime(1, 1, 1, hour, mins, second, millsecond);
            Assert.AreEqual(except.Ticks, target.Ticks);
        }

        /// <summary>
        ///A test for Ticks
        ///</summary>
        [TestMethod]
        public void TicksTest()
        {
            var target = new Time(1, 0, 0);
            long actual;
            actual = target.Ticks;
            var expect = new TimeSpan(1, 0, 0);

            Assert.AreEqual(expect.Ticks, target.Ticks);
        }

        /// <summary>
        ///A test for Seconds
        ///</summary>
        [TestMethod]
        public void SecondsTest()
        {
            var target = new Time(23, 1, 0);
            int actual;
            actual = target.Seconds;
            Assert.AreEqual(actual, 0);

        }

        /// <summary>
        ///A test for Minutes
        ///</summary>
        [TestMethod]
        public void MinutesTest()
        {
            var target = new Time(0, 23, 0);
            int actual;
            actual = target.Minutes;
            Assert.AreEqual(23, actual);
        }

        /// <summary>
        ///A test for Millseconds
        ///</summary>
        [TestMethod]
        public void MillsecondsTest()
        {
            var target = new Time(1, 2, 3, 4);
            int actual;
            actual = target.Millseconds;
            Assert.AreEqual(4, actual);
        }

        /// <summary>
        ///A test for Hours
        ///</summary>
        [TestMethod]
        public void HoursTest()
        {
            var target = new Time(1, 0, 0);
            int actual;
            actual = target.Hours;
            Assert.AreEqual(actual, 1);
        }

        /// <summary>
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod]
        public void op_SubtractionTest1()
        {
            var a = new Time(2, 2, 2);
            var b = new Time(1, 1, 1);
            var expected = new TimeSpan(1, 1, 1);
            TimeSpan actual;
            actual = (a - b);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void op_AdditionTest()
        {
            var a = new Time(23, 0, 0);
            var b = new Time(2, 0, 0);
            var expected = new TimeSpan(25, 0, 0);
            TimeSpan actual;
            actual = (a + b);
            Assert.AreEqual(expected, actual);

        }




        /// <summary>
        ///A test for Time Constructor
        ///</summary>
        [TestMethod]
        public void TimeConstructorTest2()
        {
            var expect = new TimeSpan(23, 22, 22);
            long ticks = expect.Ticks;
            var target = new Time(ticks);
            Assert.AreEqual(expect.Ticks, target.Ticks);
        }


        /// <summary>
        ///A test for CheckTimeSpanBound
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Ornament.Core.dll")]
        public void CheckTimeSpanBoundTest()
        {
            TimeSpan time = new TimeSpan(10, 23, 2, 3);
            TimeSpan expected = new TimeSpan(23, 2, 3);
            TimeSpan actual;
            actual = Time_Accessor.CheckTimeSpanBound(time);
            Assert.AreEqual(expected, actual);

        }
    }
}