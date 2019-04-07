using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrayCS;

class MoveEvent : XrayCS.Event
{
    private int _dx;
    private int _dy;

    public int Dx { get => _dx; set => _dx = value; }
    public int Dy { get => _dy; set => _dy = value; }

    public MoveEvent()
        : base(null, new Type[] {typeof(PositionComponent)}, null) { }

    public MoveEvent(int dx, int dy, Entity source = null, Type[] excludes = null)
        : this(source, new Type[] { typeof(PositionComponent) }, excludes)
    {
        Dx = dx;
        Dy = dy;
    }

    public MoveEvent(Entity source = null, Type[] matches = null, Type[] excludes = null)
        : base(source, matches, excludes) { }

    public override void CallOnMatch(Entity entity)
    {
        PositionComponent pc = entity.Get<PositionComponent>();
        pc.X += Dx;
        pc.Y += Dy;
    }
}

namespace UnitTests
{
    [TestClass]
    public class EventTest
    {
        private Entity entity;
        private Event[] events;

        [TestInitialize]
        public void Setup()
        {
            events = new Event[3];
            entity = new Entity();
            PositionComponent p = new PositionComponent();
            entity.Add<PositionComponent>(p);
        }

        [TestMethod]
        public void BasicConstructor()
        {
            MoveEvent moveEvent = new MoveEvent();
            Assert.AreEqual(moveEvent.Dx, 0);
            Assert.AreEqual(moveEvent.Dy, 0);
            Assert.AreEqual(moveEvent.Source, null);
            Assert.AreEqual(moveEvent.Matches[0], typeof(PositionComponent));
            Assert.AreEqual(moveEvent.Matches.Length, 1);
            Assert.AreEqual(moveEvent.Excludes.Length, 0);
        }

        [TestMethod]
        public void ArgumentConstructors()
        {
            MoveEvent moveEvent = new MoveEvent(5, 3);
            Assert.AreEqual(moveEvent.Dx, 5);
            Assert.AreEqual(moveEvent.Dy, 3);

            MoveEvent moveEvent2 = new MoveEvent(3, 5);
            Assert.AreEqual(moveEvent2.Dx, 3);
            Assert.AreEqual(moveEvent2.Dy, 5);

            MoveEvent moveEvent3 = new MoveEvent(entity);
            Assert.AreEqual(moveEvent3.Source, entity);

            MoveEvent moveEvent4 = new MoveEvent(null, new Type[] { typeof(PositionComponent) }, new Type[] {typeof(A)});
            Assert.AreEqual(moveEvent4.Matches[0], typeof(PositionComponent));
            Assert.AreEqual(moveEvent4.Matches.Length, 1);
            Assert.AreEqual(moveEvent4.Excludes[0], typeof(A));
            Assert.AreEqual(moveEvent4.Excludes.Length, 1);

            MoveEvent moveEvent5 = new MoveEvent(0, 2, entity, null);
            Assert.AreEqual(moveEvent5.Dx, 0);
            Assert.AreEqual(moveEvent5.Dy, 2);
            Assert.AreEqual(moveEvent5.Source, entity);
            Assert.AreEqual(moveEvent5.Matches.Length, 1);
            Assert.AreEqual(moveEvent5.Excludes.Length, 0);
        }

        [DataTestMethod]
        [DataRow(1, 4)]
        [DataRow(-1, -490)]
        [DataRow(0, 0)]
        [DataRow(-1, 1)]
        public void DispatchAppliesResults(int dx, int dy)
        {
            MoveEvent moveEvent = new MoveEvent(dx, dy);
            moveEvent.DispatchToEntity(entity);
            PositionComponent pc = entity.Get<PositionComponent>();
            Assert.AreEqual(pc.X, dx);
            Assert.AreEqual(pc.Y, dy);
            moveEvent.DispatchToEntity(entity);
            Assert.AreEqual(pc.X, 2*dx);
            Assert.AreEqual(pc.Y, 2*dy);

            MoveEvent moveEvent2 = new MoveEvent(dx, dy, null, new Type[] {typeof(C)});
            moveEvent.DispatchToEntity(entity);
            Assert.AreEqual(pc.X, 3*dx);
            Assert.AreEqual(pc.Y, 3*dy);
        }

        [TestMethod]
        public void PreventIllegalDispatch()
        {
            PositionComponent pre = entity.Get<PositionComponent>().Clone();
            MoveEvent moveEvent = new MoveEvent(pre.X + 1,pre.Y - 1,null, new Type[] {typeof(PositionComponent)});
            moveEvent.DispatchToEntity(entity);
            PositionComponent pc = entity.Get<PositionComponent>();
            Assert.AreEqual(pre.X, pc.X);
            Assert.AreEqual(pre.Y, pc.Y);
        }

        [TestMethod]
        public void PreventCallsWithNoMatchesNoExcludes()
        {
            MoveEvent moveEvent = new MoveEvent(null, null, null);
            moveEvent.Dx = 5;
            moveEvent.Dy = 5;
            moveEvent.DispatchToEntity(entity);
            Assert.AreEqual(entity.Get<PositionComponent>().X, 0);
            Assert.AreEqual(entity.Get<PositionComponent>().Y, 0);
        }

        [TestMethod]
        public void ArrayOfEvents()
        {
            events[0] = new MoveEvent(3, 4);
            events[1] = new MoveEvent(5, 5);
            events[2] = new MoveEvent(6, -1);
            foreach (Event @event in events)
            {
                @event.DispatchToEntity(entity);
            }
            Assert.AreEqual(entity.Get<PositionComponent>().X, 14);
            Assert.AreEqual(entity.Get<PositionComponent>().Y, 8);
        }
    }
}
