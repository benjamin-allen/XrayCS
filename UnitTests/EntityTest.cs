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
            catch (ArgumentException e)
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
    }
}
