using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrayCS;

class PositionComponent : XrayCS.Component
{
    private int x;
    private int y;

    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }

    public PositionComponent(int x, int y)
    {
        X = x;
        Y = y;
    }

    public PositionComponent() : this(0, 0) { }

    protected override Component _Clone() { return this.Clone(); }
    public new PositionComponent Clone() { return new PositionComponent(X, Y); }
}

namespace UnitTests
{
    [TestClass]
    public class ComponentTest
    {
        private PositionComponent position1;
        private PositionComponent position2;
        private Component[] cs;

        [TestInitialize]
        public void Setup()
        {
            position1 = new PositionComponent(0, 1);
            position2 = new PositionComponent(5, 5);
            cs = new Component[2];
            cs[0] = position1;
            cs[1] = position2;
        }

        [TestMethod]
        public void ArgumentConstructor()
        {
            Assert.AreEqual(position1.X, 0);           // This test simply checks that the constructors work
            Assert.AreEqual(position1.Y, 1);
            Assert.AreEqual(position2.X, position2.Y);
        }

        [TestMethod]
        public void SuccessfulClone()
        {
            Assert.AreNotEqual(position1.X, position2.X);     // position1 and position2 start with different values
            position2 = position1.Clone();
            Assert.AreEqual(position1.X, position2.X);        // After Clone() the objects should have equal values...
            Assert.AreEqual(position1.Y, position2.Y);
            position2.X += 1;
            Assert.AreNotEqual(position1.X, position2.X);     // ...But they should not be the same object (so changes don't stick)
            Assert.AreNotEqual(position1, position2);         // Curious if this works
        }

        [TestMethod]
        public void SuccessfulRefCopy()
        {
            position2 = position1;      // Assign the ref of position1
            position1.X = 42;           // The object can be edited...
            position2.Y = 24;           // ...from either ref
            Assert.AreEqual(position1.X, position2.X);
            Assert.AreEqual(position1.Y, position2.Y);
        }

        [TestMethod]
        public void ArrayInsertion()
        {
            Assert.AreEqual(cs[0].GetType(), position1.GetType());
        }
        
        [TestMethod]
        public void ArrayRefRetrieval()
        {
            PositionComponent position3 = cs[0] as PositionComponent;  // This should be a ref to position1, not a copy...
            position3.X = -1;
            position3.Y = -2;
            Assert.AreEqual(position1.X, position3.X);                  // ... So we'll change values and see if they stick
            Assert.AreEqual(position1.Y, position3.Y);
        }

        [TestMethod]
        public void ArrayCopyRetrieval()
        {
            PositionComponent position4 = cs[1].Clone() as PositionComponent;    // This should be a copy of position2, not a ref...
            position4.X = position2.X + 1;
            position4.Y = position2.Y + 1;
            Assert.AreNotEqual(position4.X, position2.X);     // ... So we'll change the values and check that they don't stick
            Assert.AreNotEqual(position4.Y, position2.Y);
        }
}
}
