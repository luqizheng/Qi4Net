using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcTest.Models;
using Qi.Web.Mvc;

namespace Qi.Test.NHibernate
{
    /// <summary>
    ///This is a test class for NHModelBinderTest and is intended
    ///to contain all NHModelBinderTest Unit Tests
    ///</summary>
    [TestClass]
    public class NHModelBinderTest
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
        ///A test for IsPersistentType
        ///</summary>
        [TestMethod]
        public void IsPersistentTypeTest()
        {
            NHModelBinder binder=new NHModelBinder();
            Assert.IsTrue(binder.IsPersistentType(typeof (User)));
            Assert.IsTrue(binder.IsPersistentType(typeof (User[])));
            Assert.IsTrue(binder.IsPersistentType(typeof (IList<User>[])));
            Assert.IsTrue(binder.IsPersistentType(typeof (List<User>[])));
            Assert.IsTrue(binder.IsPersistentType(typeof (Iesi.Collections.Generic.ISet<User>[])));
            Assert.IsTrue(binder.IsPersistentType(typeof (HashedSet<User>[])));
            Assert.IsTrue(binder.IsPersistentType(typeof (ImmutableSet<User>[])));
            Assert.IsTrue(binder.IsPersistentType(typeof (DictionarySet<User>[])));
            Assert.IsTrue(binder.IsPersistentType(typeof (OrderedSet<User>[])));
            Assert.IsTrue(binder.IsPersistentType(typeof (Set<User>[])));
            Assert.IsTrue(binder.IsPersistentType(typeof (Iesi.Collections.Generic.SortedSet<User>[])));
            Assert.IsTrue(binder.IsPersistentType(typeof (SynchronizedSet<User>[])));
            Assert.IsTrue(binder.IsPersistentType(typeof (System.Collections.Generic.ISet<User>[])));
        }

        [TestMethod]
        public void IsCollectionType()
        {
            Type parameterType;
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof (User[]), out parameterType));
            Assert.AreEqual(typeof (User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof (IList<User>[]), out parameterType));
            Assert.AreEqual(typeof (User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof (List<User>[]), out parameterType));
            Assert.AreEqual(typeof (User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof (Iesi.Collections.Generic.ISet<User>[]),
                                                            out parameterType));
            Assert.AreEqual(typeof (User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof (HashedSet<User>[]), out parameterType));
            Assert.AreEqual(typeof (User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof (ImmutableSet<User>[]), out parameterType));
            Assert.AreEqual(typeof (User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof (DictionarySet<User>[]), out parameterType));
            Assert.AreEqual(typeof (User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof (OrderedSet<User>[]), out parameterType));
            Assert.AreEqual(typeof (User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof (Set<User>[]), out parameterType));
            Assert.AreEqual(typeof (User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof (Iesi.Collections.Generic.SortedSet<User>[]),
                                                            out parameterType));
            Assert.AreEqual(typeof (User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof (SynchronizedSet<User>[]), out parameterType));
            Assert.AreEqual(typeof (User), parameterType);
            Assert.IsTrue(CollectionHelper.IsCollectionType(typeof (System.Collections.Generic.ISet<User>[]),
                                                            out parameterType));
            Assert.AreEqual(typeof (User), parameterType);
        }
    }
}