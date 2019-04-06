using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using XrayCS;

namespace UnitTests
{
    [TestClass]
    public class EntityTest
    {
        private Entity entity;

        [TestInitialize]
        public void Setup()
        {
            entity = new Entity();
        }

        #region Constructor Tests
        [TestMethod]
        public void BasicConstructor()
        {
            Assert.AreEqual(entity.NumRegisteredComponents, 0);
            Assert.AreEqual(entity.NumComponents, 0);
            Assert.AreNotEqual(entity.MaxComponents, 0);
        }

        [TestMethod]
        public void ArgumentConstructor()
        {
            entity = new Entity(2);
            Assert.AreEqual(entity.NumRegisteredComponents, 0);
            Assert.AreEqual(entity.NumComponents, 0);
            Assert.AreEqual(entity.MaxComponents, 2);
        }
        #endregion
        #region Add<>() Tests
        [TestMethod]
        public void AddIncrementsComponentCount()
        {
            entity.Add<A>();
            Assert.AreEqual(entity.NumRegisteredComponents, 1);
            entity.Add<B>();
            Assert.AreEqual(entity.NumRegisteredComponents, 2);
            Assert.AreEqual(entity.NumComponents, 2);
        }

        [TestMethod]
        public void AddReturnsComponentRef()
        {
            A a = entity.Add<A>();
            Assert.AreEqual(a.GetType(), a.Clone().GetType());
        }

        [TestMethod]
        public void AddWithPreconstructedComponent()
        {
            A a = new A();
            var reference = entity.Add<A>(a);
            Assert.AreEqual(a, reference);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "throwOnError was set but no exception was thrown.")]
        public void PreventDoubleAddingComponents()
        {
            entity.Add<A>();
            entity.Add<A>();
        }

        [TestMethod]
        public void EnsureDoubleAddingHasNoSideEffects()
        {
            entity.Add<A>();
            try
            {
                entity.Add<A>();
                Assert.Equals(false, true);
            }
            catch (ArgumentException)
            {
                Assert.AreEqual(entity.NumComponents, 1);
                Assert.AreEqual(entity.NumRegisteredComponents, 1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Entity allowed addition of too many components")]
        public void PreventRegistrationOfTooManyComponents()
        {
            entity = new Entity(2);
            entity.Add<A>();
            entity.Add<B>();
            entity.Add<C>();
        }

        [TestMethod]
        public void AddAfterRemoval()
        {
            entity.Add<A>();
            entity.Add<B>();
            entity.Remove<A>();
            entity.Remove<B>();
            Assert.AreEqual(entity.NumComponents, 0);
            A reference = entity.Add<A>(new A());
            B anotherReference = entity.Add<B>();
            Assert.AreEqual(entity.NumComponents, 2);
            Assert.AreEqual(entity.NumRegisteredComponents, 2);
            Assert.AreNotEqual(reference, null);
            Assert.AreNotEqual(anotherReference, null);
        }

        #endregion
        #region Remove<>() Tests

        [TestMethod]
        public void RemoveDecrementsComponentCount()
        {
            entity.Add<A>();
            entity.Remove<A>();
            Assert.AreEqual(entity.NumComponents, 0);
            Assert.AreEqual(entity.NumRegisteredComponents, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Remove attempted to delete an unmapped component")]
        public void PreventRemovalOfNonExistantComponent()
        {
            entity.Add<A>();
            entity.Remove<B>();
        }

        [TestMethod]
        public void RemoveReturnsCorrectBool()
        {
            entity.Add<A>();
            Assert.AreEqual(entity.Remove<A>(), true);
            Assert.AreEqual(entity.Remove<A>(), false);
            Assert.AreEqual(entity.Remove<B>(false), false);
        }

        #endregion
        #region Get<>() Tests

        [TestMethod]
        public void GetReturnsExpectedValues()
        {
            A a = entity.Add<A>();
            entity.Add<B>();
            entity.Remove<B>();
            Assert.AreEqual(a, entity.Get<A>());
            Assert.AreNotEqual(new A(), entity.Get<A>());
            Assert.AreEqual(null, entity.Get<B>());
            Assert.AreEqual(null, entity.Get<C>(false));
        }

        [TestMethod]
        public void GetReturnsCorrectTypes()
        {
            entity.Add<A>();
            Assert.AreNotEqual(typeof(XrayCS.Component), entity.Get<A>().GetType());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PreventGettingNonRegisteredComponents()
        {
            entity.Get<A>();
        }

        #endregion
        #region Has<>() Tests

        [TestMethod]
        public void HasReturnsExpectedValues()
        {
            entity.Add<A>();
            Assert.AreEqual(entity.Has<A>(), true);
            Assert.AreEqual(entity.Has<B>(), false);
            entity.Remove<A>();
            Assert.AreEqual(entity.Has<A>(), false);
        }

        [TestMethod]
        public void HasLists()
        {
            entity.Add<A>();
            entity.Add<B>();
            Type[] arr1 = { typeof(A) };
            Type[] arr2 = { typeof(A), typeof(B) };
            Type[] arr3 = { typeof(A), typeof(B), typeof(C) };
            Type[] arr4 = { typeof(C) };
            Assert.AreEqual(entity.HasAll(arr1), true);
            Assert.AreEqual(entity.HasAll(arr2), true);
            Assert.AreEqual(entity.HasAll(arr3), false);
            Assert.AreEqual(entity.HasAny(arr1), true);
            Assert.AreEqual(entity.HasAny(arr2), true);
            Assert.AreEqual(entity.HasAny(arr3), true);
            Assert.AreEqual(entity.HasAny(arr4), false);

            entity.Remove<B>();
            Assert.AreEqual(entity.HasAll(arr2), false);
            Assert.AreEqual(entity.HasAny(arr3), true);

            entity.Remove<A>();
            Assert.AreEqual(entity.HasAny(arr3), false);
        }

        #endregion
    }
}
