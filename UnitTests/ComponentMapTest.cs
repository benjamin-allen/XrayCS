﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using XrayCS;

class A : Component
{
    protected override Component _Clone() { return this.Clone(); }
    public new A Clone() { return new A(); }
}

class B : Component
{
    protected override Component _Clone() { return this.Clone(); }
    public new B Clone() { return new B(); }
}

class C : Component
{
    protected override Component _Clone() { return this.Clone(); }
    public new C Clone() { return new C(); }
}


namespace UnitTests
{
    [TestClass]
    public class ComponentMapTest
    {
        private ComponentMap map;

        [TestInitialize]
        public void Setup()
        {
            map = new ComponentMap();
        }

        [TestMethod]
        public void DefaultConstructor()
        {
            Assert.AreEqual(map.Size, 0);
            Assert.AreEqual(map.MaximumSize, (uint)255);
        }

        [TestMethod]
        public void ArgumentConstructor()
        {
            map = new ComponentMap(3);
            Assert.AreEqual(map.Size, 0);
            Assert.AreEqual(map.MaximumSize, (uint)3);
        }

        [TestMethod]
        public void RegistersComponents()
        {
            map.Register<A>();
            map.Register(typeof(C));
        }

        [TestMethod]
        public void RegisterReportsCorrectSize()
        {
            map.Register<A>();
            map.Register<B>();
            map.Register<C>();
            Assert.AreEqual(map.Size, 3);
        }

        [TestMethod]
        public void RegisterReturnsExpectedIndices()
        {
            int[] indices = new int[3];
            indices[0] = map.Register<A>();
            indices[1] = map.Register<B>();
            indices[2] = map.Register<C>();
            for(int i = 0; i < 3; i++)
            {
                Assert.AreEqual(indices[i], i);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "A component was inappropriately mapped twice.")]
        public void PreventDoubleRegistration()
        {
            map.Register<A>();
            map.Register(typeof(A));        // Registering the same component twice should throw an exception
        }

        [TestMethod]
        [ExpectedException(typeof(System.OutOfMemoryException), "More mappings were allowed than MaxSize specifies.")]
        public void PreventTooManyComponents()
        {
            map = new ComponentMap(1);      // Construct a map where only one component is allowed
            map.Register<A>();
            map.Register<B>();              // Registering 2 component should throw an exception
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "A component was registered that was not a subclass of XrayCS.Component")]
        public void PreventsNonSubclassRegistration()
        {
            map.Register(typeof(int));
        }

        [TestMethod]
        public void ContainsReturnsExpected()
        {
            map.Register<A>();
            Assert.AreEqual(map.Contains<A>(), true);
            Assert.AreEqual(map.Contains<B>(), false);
        }

        [TestMethod]
        public void LookupByGeneric()
        {
            map.Register<A>();
            map.Register(typeof(B));
            Assert.AreEqual(map.Lookup<B>(), 1);
            Assert.AreEqual(map.Lookup<A>(), 0);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "throwOnError was set, and a component was not in the map, but an exception was not thrown.")]
        public void PreventLookupOfNonRegisteredComponent()
        {
            Assert.AreEqual(map.Lookup<A>(false), -1);
            map.Lookup<A>();
        }

        [TestMethod]
        public void LookupByType()
        {
            map.Register<A>();
            map.Register<B>();
            Assert.AreEqual(map.Lookup(Type.GetType("A")), 0);
            Assert.AreEqual(map.Lookup(Type.GetType("B")), 1);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "throwOnError was set, and a component was not in the map, but an exception was not thrown.")]
        public void PreventLookupByTypeOfNonRegisteredComponent()
        {
            Assert.AreEqual(map.Lookup(Type.GetType("A"), false), -1);
            map.Lookup(Type.GetType("A"));       // Should throw an ArgumentException
        }

        [DataTestMethod]
        [DataRow(typeof(int))]
        [DataRow(typeof(Object))]
        [ExpectedException(typeof(System.ArgumentException), "A non-subclass of XrayCS.Component was not prevented from begin looked up.")]
        public void PreventLookupOfNonSubclass(Type type)
        {
            map.Lookup(type);    // We have to check this one directly because it's not enforced by the compiler
        }

        [TestMethod]
        public void AllKeysIterator()
        {
            map.Register(typeof(A));
            map.Register(typeof(B));
            int i = 0;
            foreach (Type key in map.AllKeys())
            {
                i += 1;
            }
            Assert.AreEqual(i, 2);
        }
    }
}
