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
        public void AddIncrementsComponentCount()
        {
            entity.Add<A>();
            Assert.AreEqual(entity.NumComponents, 1);
            entity.Add<B>();
            Assert.AreEqual(entity.NumComponents, 2);
            try
            {
                entity.Add<B>();
                Assert.AreEqual(true, false);
            }
            catch(ArgumentException)
            {
                Assert.AreEqual(entity.NumComponents, 2);
            }
        }

        [TestMethod]
        public void AddReturnsComponentRef()
        {
            A a = entity.Add<A>();
            Assert.AreEqual(a.GetType(), a.Clone().GetType());
        }

        [TestMethod]
        public void AddWithComponentCopy()
        {
            A a = new A();
            A copy = entity.Add<A>(a);
            Assert.AreNotEqual(Object.ReferenceEquals(a, copy), true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "throwOnError was set but no exception was thrown.")]
        public void PreventDoubleAddingComponents()
        {
            entity.Add<A>();
            entity.Add<A>();
        }

        [TestMethod]
        public void AllowDoubleAddingComponentIfUnsetThrowOnError()
        {
            entity.Add<A>();
            entity.Add<A>(throwOnError = false);
        }
    }
}
