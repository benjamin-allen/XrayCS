using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using XrayCS;

namespace UnitTests
{
    [TestClass]
    public class PublisherTest
    {
        private Publisher publisher;
        private Entity entity1;
        private Entity entity2;
        private Entity entity3;

        [TestInitialize]
        public void Setup()
        {
            publisher = new Publisher();
            entity1 = new Entity();
            entity2 = new Entity();
            entity3 = new Entity();

            entity1.Add<A>();
            entity1.Add<PositionComponent>();

            entity2.Add<B>();
            entity2.Add<PositionComponent>();

            entity3.Add<C>();
        }

        [TestMethod]
        public void BasicConstructor()
        {
            Assert.AreEqual(publisher.NumEntities, 0);
        }

        [TestMethod]
        public void SimpleAddEntities()
        {
            publisher.AddEntity(entity1);
            publisher.AddEntity(entity2);
            publisher.AddEntity(entity3);
            Assert.AreEqual(publisher.NumEntities, 3);
        }

        [TestMethod]
        public void PreventDuplicateEntities()
        {
            Assert.AreEqual(publisher.AddEntity(entity1), true);
            Assert.AreEqual(publisher.AddEntity(entity2), true);
            Assert.AreEqual(publisher.NumEntities, 2);
            Assert.AreEqual(publisher.AddEntity(entity1), false);
            Assert.AreEqual(publisher.NumEntities, 2);
        }

        [TestMethod]
        public void RemoveEntities()
        {
            publisher.AddEntity(entity1);
            publisher.AddEntity(entity2);
            Assert.AreEqual(publisher.RemoveEntity(entity1), true);
            Assert.AreEqual(publisher.RemoveEntity(entity1), false);
            Assert.AreEqual(publisher.NumEntities, 1);
            Assert.AreEqual(publisher.RemoveEntity(entity2), true);
        }

        [TestMethod]
        public void SimpleAddEvent()
        {
            publisher.AddEvent(new MoveEvent());
        }

        [TestMethod]
        public void ProcessSingleEvent()
        {
            publisher.AddEntity(entity1);
            publisher.AddEntity(entity2);
            publisher.AddEntity(entity3);
            MoveEvent moveEvent = new MoveEvent(3, -3, null, new Type[] { typeof(B) });
            publisher.ProcessEvent(moveEvent);
            Assert.AreEqual(entity1.Get<PositionComponent>().X, 3);
            Assert.AreEqual(entity1.Get<PositionComponent>().Y, -3);
            Assert.AreEqual(entity2.Get<PositionComponent>().X, 0);
            Assert.AreEqual(entity2.Get<PositionComponent>().Y, 0);
        }

        [TestMethod]
        public void ProcessAllEvents()
        {
            publisher.AddEntity(entity1);
            publisher.AddEntity(entity2);
            publisher.AddEntity(entity3);
            for (int i = 0; i < 3; i++)
            {
                MoveEvent moveEvent = new MoveEvent(i, -i);
                publisher.AddEvent(moveEvent);
            }
            publisher.ProcessQueue();
            Assert.AreEqual(entity1.Get<PositionComponent>().X, 3);
            Assert.AreEqual(entity1.Get<PositionComponent>().Y, -3);
            Assert.AreEqual(entity2.Get<PositionComponent>().X, 3);
            Assert.AreEqual(entity2.Get<PositionComponent>().Y, -3);
        }

        [TestMethod]
        public void HonorsPriority()
        {
            publisher.AddEntity(entity1);
            publisher.AddEntity(entity2);
            publisher.AddEntity(entity3);
            for (int i = 0; i < 3; i++)
            {
                MoveEvent moveEvent = new MoveEvent(i+1, -i-1);
                moveEvent.Priority = -i;
                publisher.AddEvent(moveEvent);
            }
            PositionComponent pc1 = entity1.Get<PositionComponent>();
            publisher.ProcessTopEvent();
            Assert.AreEqual(pc1.X, 3);
            Assert.AreEqual(pc1.Y, -3);
            publisher.ProcessTopEvent();
            Assert.AreEqual(pc1.X, 5);
            Assert.AreEqual(pc1.Y, -5);
            publisher.ProcessTopEvent();
            Assert.AreEqual(pc1.X, 6);
            Assert.AreEqual(pc1.Y, -6);
        }
    }
}
