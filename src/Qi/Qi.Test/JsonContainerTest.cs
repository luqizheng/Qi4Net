using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qi.Web;

namespace Qi.Test
{
    /// <summary>
    ///This is a test class for JsonContainerTest and is intended
    ///to contain all JsonContainerTest Unit Tests
    ///</summary>
    [TestClass]
    public class JsonContainerTest
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

        public void NumberHelper<T>(T max, T min, string invokeMethod)
        {
            var content = new Dictionary<string, object> { { "Max", max.ToString() }, { "Min", min.ToString() } };
            var target = new JsonContainer(content);

            Type t = target.GetType();
            MethodInfo method = t.GetMethod(invokeMethod);

            var actual = (T)method.Invoke(target, new[] { "Max" });
            Assert.AreEqual(actual, max);

            actual = (T)method.Invoke(target, new[] { "Min" });
            Assert.AreEqual(actual, min);
        }

        /// <summary>
        ///A test for ToUInt32
        ///</summary>
        [TestMethod]
        public void ToUInt32Test()
        {
            NumberHelper(UInt32.MaxValue, UInt32.MinValue, "ToUInt32");
        }

        /// <summary>
        ///A test for ToUInt16
        ///</summary>
        [TestMethod]
        public void ToUInt16Test()
        {
            NumberHelper(UInt16.MaxValue, UInt16.MinValue, "ToUInt16");
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod]
        public void ToStringTest()
        {
            const string expected = "stringExpression";
            const string key = "key";
            var content = new Dictionary<string, object>
                              {
                                  {key, expected}
                              };
            var target = new JsonContainer(content);
            string actual = target.ToString(key);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for ToJsonContainer
        ///</summary>
        [TestMethod]
        public void ToJsonContainerTest()
        {
            string key = "key";


            var content = new Dictionary<string, object>
                              {
                                  {
                                      "key", new Dictionary<string, object>
                                                 {
                                                     {"childKey", Int32.MaxValue}
                                                 }
                                      }
                              };
            var target = new JsonContainer(content);
            JsonContainer actual;
            actual = target.ToJsonContainer(key);
            Assert.AreEqual(Int32.MaxValue, actual.ToInt32("childKey"));
        }

        /// <summary>
        ///A test for ToInt64
        ///</summary>
        [TestMethod]
        public void ToInt64Test()
        {
            NumberHelper(Int64.MaxValue, Int64.MinValue, "ToInt64");
        }

        /// <summary>
        ///A test for ToInt32
        ///</summary>
        [TestMethod]
        public void ToInt32Test()
        {
            NumberHelper(Int32.MaxValue, Int32.MinValue, "ToInt32");
        }

        /// <summary>
        ///A test for ToInt16
        ///</summary>
        [TestMethod]
        public void ToInt16Test()
        {
            NumberHelper(Int16.MaxValue, Int16.MinValue, "ToInt16");
        }

        /// <summary>
        ///A test for ToGuid
        ///</summary>
        [TestMethod]
        public void ToGuidTest()
        {
            Guid expected = Guid.NewGuid();

            var content = new Dictionary<string, object> { { "key", expected.ToString() } };
            var target = new JsonContainer(content);
            string key = "key";

            Guid actual;
            actual = target.ToGuid(key);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ToDouble
        ///</summary>
        [TestMethod]
        public void ToDoubleTest()
        {
            double max = 1.222222;
            double min = 1.33333333333333;
            NumberHelper(max, min, "ToDouble");
        }

        /// <summary>
        ///A test for ToDateTime
        ///</summary>
        [TestMethod]
        public void ToDateTimeTest()
        {
            var max = new DateTime(1980, 1, 1);
            var min = new DateTime(1911, 1, 1);
            var content = new Dictionary<string, object> { { "Max", max }, { "Min", min } };
            var target = new JsonContainer(content);

            DateTime actual = target.ToDateTime("Max");
            Assert.AreEqual(actual, max);

            actual = target.ToDateTime("Min");
            Assert.AreEqual(actual, min);
        }

        /// <summary>
        ///A test for ToBoolean
        ///</summary>
        [TestMethod]
        public void ToBooleanTest()
        {
            string key = "key";
            bool expected = true;

            var content = new Dictionary<string, object> { { key, expected } };
            var target = new JsonContainer(content);

            bool actual;
            actual = target.ToBoolean(key);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void ToArrayTest_Types()
        {
            string json = @"{a:[1,2,3,4,5]}";
            JsonContainer a = JsonContainer.Create(json);
            int[] actual = a.ToArray<int>("a", Convert.ToInt32);
            var except = new[] { 1, 2, 3, 4, 5 };
            Assert.AreEqual(actual.Length, except.Length);
            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(except[i], actual[i]);
            }
        }

        [TestMethod]
        public void ToArrayTest_JsonContains()
        {
            var o = new Dictionary<string, object>();
            o.Add("child1", "a");
            o.Add("child2", "b");

            var o2 = new Dictionary<string, object>();
            o2.Add("child1", "a");
            o2.Add("child2", "b");

            var s = new[] { o, o2 };

            var content = new Dictionary<string, object>();
            content.Add("key", s);
            var jsonContainer = new JsonContainer(content);

            JsonContainer[] target = jsonContainer.ToArray<JsonContainer>("key", JsonContainer.ConvertToJsonContainer);

            Assert.AreEqual("a", target[0].ToString("child1"));
            Assert.AreEqual("b", target[0].ToString("child2"));

            Assert.AreEqual("a", target[1].ToString("child1"));
            Assert.AreEqual("b", target[1].ToString("child2"));
        }

        [TestMethod]
        public void ToArrayTest_JsonContains_ArrayList()
        {
            var o = new Dictionary<string, object>();
            o.Add("child1", "a");
            o.Add("child2", "b");

            var o2 = new Dictionary<string, object>();
            o2.Add("child1", "a");
            o2.Add("child2", "b");

            var s = new List<Dictionary<string, object>> { o, o2 };

            var content = new Dictionary<string, object>();
            content.Add("key", s);
            var jsonContainer = new JsonContainer(content);

            JsonContainer[] target = jsonContainer.ToArray<JsonContainer>("key", sa => new JsonContainer((Dictionary<string, object>)sa));

            Assert.AreEqual("a", target[0].ToString("child1"));
            Assert.AreEqual("b", target[0].ToString("child2"));

            Assert.AreEqual("a", target[1].ToString("child1"));
            Assert.AreEqual("b", target[1].ToString("child2"));
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod]
        public void CreateTest1()
        {
            string jsonData = @"{
    string:'string',
    boolean:true,
    int:32
}";
            var stream = new MemoryStream(Encoding.Default.GetBytes(jsonData));
            JsonContainer actual;
            actual = JsonContainer.Create(stream);

            Assert.AreEqual(32, actual.ToInt32("int"));
            Assert.AreEqual("string", actual.ToString("string"));
            Assert.IsTrue(actual.ToBoolean("boolean"));
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod]
        public void CreateTest()
        {
            string jsonData = @"{
    string:'string',
    boolean:true,
    int:32
}";

            JsonContainer actual;
            actual = JsonContainer.Create(jsonData);


            Assert.AreEqual(32, actual.ToInt32("int"));
            Assert.AreEqual("string", actual.ToString("string"));
            Assert.IsTrue(actual.ToBoolean("boolean"));
        }

        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod]
        public void ContainsTest()
        {
            var content = new Dictionary<string, object>
                              {
                                  {
                                      "l1", new Dictionary<string, object>
                                                {
                                                    {
                                                        "l2", new Dictionary<string, object>
                                                                  {
                                                                      {"value1", Int32.MaxValue}
                                                                  }
                                                        }
                                                }
                                      }
                              };

            var current = new JsonContainer(content);
            Assert.IsTrue(current.Contains("l1.l2.value1"));
            Assert.IsFalse(current.Contains("l1.l2.value"));
            Assert.IsFalse(current.Contains("l1.l.value"));
        }

        [TestMethod]
        public void AllKeyTest()
        {
            var content = new Dictionary<string, object>
                              {
                                  {
                                      "l1", new Dictionary<string, object>
                                                {
                                                    {
                                                        "l2", new Dictionary<string, object>
                                                                  {
                                                                      {"value1", Int32.MaxValue}
                                                                  }
                                                        }
                                                }
                                      }
                              };

            var current = new JsonContainer(content);
            string[] actual = current.AllSubKeys;

            Assert.AreEqual(("l1.l2.value1"), actual[0]);
        }

        ///// <summary>
        /////A test for AnaylzTheKey
        /////</summary>
        //[TestMethod]
        //[DeploymentItem("Qi.Web.dll")]
        //public void AnaylzTheKeyTest()
        //{
        //    var content = new Dictionary<string, object>
        //                      {
        //                          {
        //                              "l1", new Dictionary<string, object>
        //                                        {
        //                                            {
        //                                                "l2", new Dictionary<string, object>
        //                                                          {
        //                                                              {"value", Int32.MaxValue}
        //                                                          }
        //                                                }
        //                                        }
        //                              }
        //                      };

        //    var current = new JsonContainer(content);
        //    string keyPath = "l1.l2.value";
        //    string lastKey;
        //    string lastKeyExpect = "value";

        //    JsonContainer actual = JsonContainer_Accessor.AnaylzTheKey(keyPath, out lastKey, current);
        //    Assert.AreEqual(lastKeyExpect, lastKey);
        //    Assert.AreEqual(Int32.MaxValue, actual.ToInt32(lastKey));
        //}

        [TestMethod]
        public void ArrayJson()
        {
            string json = @"
{
    a:[
         {name:""abc1""},
         {number:""11""}         
      ]
}";
            JsonContainer a = JsonContainer.Create(json);
            JsonContainer[] actual = a.ToArray<JsonContainer>("a", JsonContainer.ConvertToJsonContainer);
            Assert.AreEqual("abc1", actual[0].ToString("name"));
            Assert.AreEqual(11, actual[1].ToInt32("number"));
        }

        /// <summary>
        ///A test for SetValue
        ///</summary>
        [TestMethod]
        public void SetValueTest()
        {
            var target = new JsonContainer();
            string key = "a.b";
            string val = "11";

            target.SetValue(key, val);

            Assert.AreEqual(val, target.ToString("a.b"));

            target.SetValue("b", "11");
            Assert.AreEqual("11", target.ToString("b"));

        }



        /// <summary>
        ///A test for SetValue
        ///</summary>
        [TestMethod]
        public void SetValueTest_set_val_which_is_array()
        {
            var target = new JsonContainer();
            string key = "a.b[]";
            string val = "11";
            ;
            target.SetValue(key, val);
            target.SetValue("a.b[]", "12");

            string[] actual = target.ToArray<string>("a.b", Convert.ToString);
            Assert.AreEqual("11", actual[0]);
            Assert.AreEqual("12", actual[1]);
        }

        /// <summary>
        ///A test for SetValue
        ///</summary>
        [TestMethod]
        public void SetValue_set_ary_objects()
        {
            var target = new JsonContainer();
            string key = "a.b[0].c";
            string val = "11";
            ;
            target.SetValue(key, val);
            target.SetValue("a.b[1].c", "12");

            JsonContainer[] actual = target.ToArray<JsonContainer>("a.b",JsonContainer.ConvertToJsonContainer);
            Assert.AreEqual("11", actual[0].ToString("c"));
            Assert.AreEqual("12", actual[1].ToString("c"));
        }


        /// <summary>
        ///A test for SetValue
        ///</summary>
        [TestMethod]
        public void SetValue_set_ary_objects_once()
        {
            var target = new JsonContainer();
            string key = "a.b[]";
            string[] val = new[] { "11", "12" };
            ;
            target.SetValue(key, val);


            var actual = target.ToArray<string>("a.b", Convert.ToString);
            Assert.AreEqual("11", actual[0]);
            Assert.AreEqual("12", actual[1]);
        }

        /// <summary>
        ///A test for SetValue
        ///</summary>
        [TestMethod]
        public void SetValue_set_ary_objectValue()
        {
            var target = new JsonContainer();
            string key = "a[0].b.c";
            string val = "11"; ;
            target.SetValue(key, val);
            var actual = target.ToArray<string>("a[0].b.c", Convert.ToString);
            Assert.AreEqual("11", actual);

        }
    }
}