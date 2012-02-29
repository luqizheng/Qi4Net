using System.Collections.Generic;
using MvcTest.Models;
using Qi.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Qi.Web.Mvc;

namespace Qi.Test
{


    /// <summary>
    ///This is a test class for NHModelBinderTest and is intended
    ///to contain all NHModelBinderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NHModelBinderTest
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
        ///A test for IsPersistentType
        ///</summary>
        [TestMethod()]
        public void IsPersistentTypeTest()
        {

            Assert.IsTrue(NHModelBinder.IsPersistentType(typeof(User)));
            Assert.IsTrue(NHModelBinder.IsPersistentType(typeof(User[])));
            Assert.IsTrue(NHModelBinder.IsPersistentType(typeof(IList<User>[])));
            Assert.IsTrue(NHModelBinder.IsPersistentType(typeof(List<User>[])));
            Assert.IsTrue(NHModelBinder.IsPersistentType(typeof(Iesi.Collections.Generic.ISet<User>[])));
            Assert.IsTrue(NHModelBinder.IsPersistentType(typeof(Iesi.Collections.Generic.HashedSet<User>[])));
            Assert.IsTrue(NHModelBinder.IsPersistentType(typeof(Iesi.Collections.Generic.ImmutableSet<User>[])));
            Assert.IsTrue(NHModelBinder.IsPersistentType(typeof(Iesi.Collections.Generic.DictionarySet<User>[])));
            Assert.IsTrue(NHModelBinder.IsPersistentType(typeof(Iesi.Collections.Generic.OrderedSet<User>[])));
            Assert.IsTrue(NHModelBinder.IsPersistentType(typeof(Iesi.Collections.Generic.Set<User>[])));
            Assert.IsTrue(NHModelBinder.IsPersistentType(typeof(Iesi.Collections.Generic.SortedSet<User>[])));
            Assert.IsTrue(NHModelBinder.IsPersistentType(typeof(Iesi.Collections.Generic.SynchronizedSet<User>[])));
            Assert.IsTrue(NHModelBinder.IsPersistentType(typeof(ISet<User>[])));
        }

        [TestMethod()]
        public void IsCollectionType()
        {

            Type parameterType;
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof(User[]), out parameterType));
            Assert.AreEqual(typeof(User),parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof(IList<User>[]), out parameterType));
            Assert.AreEqual(typeof(User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof(List<User>[]), out parameterType));
            Assert.AreEqual(typeof(User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof(Iesi.Collections.Generic.ISet<User>[]), out parameterType));
            Assert.AreEqual(typeof(User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof(Iesi.Collections.Generic.HashedSet<User>[]), out parameterType));
            Assert.AreEqual(typeof(User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof(Iesi.Collections.Generic.ImmutableSet<User>[]), out parameterType));
            Assert.AreEqual(typeof(User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof(Iesi.Collections.Generic.DictionarySet<User>[]), out parameterType));
            Assert.AreEqual(typeof(User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof(Iesi.Collections.Generic.OrderedSet<User>[]), out parameterType));
            Assert.AreEqual(typeof(User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof(Iesi.Collections.Generic.Set<User>[]), out parameterType));
            Assert.AreEqual(typeof(User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof(Iesi.Collections.Generic.SortedSet<User>[]), out parameterType));
            Assert.AreEqual(typeof(User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof(Iesi.Collections.Generic.SynchronizedSet<User>[]), out parameterType));
            Assert.AreEqual(typeof(User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof(ISet<User>[]), out parameterType));
            Assert.AreEqual(typeof(User), parameterType);
        }
    }
}
