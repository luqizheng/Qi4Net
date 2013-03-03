using System;
using Qi.Sms;
using Qi.Sms.Protocol.SmsEncodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Test.Qi.Sms.Core
{


    /// <summary>
    ///This is a test class for AbstractEncoderTest and is intended
    ///to contain all AbstractEncoderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AbstractEncoderTest
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
        ///A test for PhoneEncode
        ///</summary>
        [TestMethod()]
        public void PhoneEncodeTest()
        {
            string phone = "13600368080";
            string expected = "3106308680F0";
            string actual;
            actual = AbstractEncoder.ParityChange(phone);
            Assert.AreEqual(expected, actual);
        }

        internal virtual AbstractEncoder_Accessor CreateAbstractEncoder_Accessor()
        {
            // TODO: Instantiate an appropriate concrete class.
            AbstractEncoder_Accessor target = null;
            return target;
        }

        /// <summary>
        ///A test for GetLengthForPdu
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Qi.Sms.Core.dll")]
        public void GetLengthForPduTest()
        {
            PrivateObject param0 = new PrivateObject(new TestAbstractEncoder());
            AbstractEncoder_Accessor target = new AbstractEncoder_Accessor(param0);
            string str = "13800138000";
            string expected = "06";
            string actual;
            actual = target.GetLengthForPdu(str);
            Assert.AreEqual(expected, actual);

        }

        public class TestAbstractEncoder : AbstractEncoder
        {
            public override string Name
            {
                get { throw new NotImplementedException(); }
            }

            public override bool Decode(string receiveName, out ReceiveSms sms)
            {
                throw new NotImplementedException();
            }

            protected override string PduEncode(string phone, string smsContent)
            {
                throw new NotImplementedException();
            }

            protected override string FormatPhone(string phone)
            {
                throw new NotImplementedException();
            }
        }
    }
}
