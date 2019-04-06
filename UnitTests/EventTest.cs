using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrayCS;

class MoveEvent : XrayCS.Event
{
    private int _dx;
    private int _dy;

    public int Dx { get => _dx; set => _dx = value; }
    public int Dy { get => _dy; set => _dy = value; }

    public MoveEvent(Entity source = null, Type[] excludes = null)
        : base(source, new Type[] {typeof(PositionComponent)}, excludes) { }

    public MoveEvent(int dx, int dy, Entity source = null, Type[] excludes = null)
        : this(source, excludes)
    {
        Dx = dx;
        Dy = dy;
    }

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

            MoveEvent moveEvent4 = new MoveEvent(null, new Type[] {typeof(A)});
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
    }
}
