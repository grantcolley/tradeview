using DevelopmentInProgress.TradeView.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.TradeView.Core.Test
{
    [TestClass]
    public class InclusiveRangeTest
    {
        [TestMethod]
        public void Increment()
        {
            // Arrange
            var a = 1.00000000M;
            var b = 0.10000000M;
            var c = 0.01000000M;
            var d = 0.00100000M;
            var e = 0.00010000M;
            var f = 0.00001000M;
            var g = 0.00000100M;
            var h = 0.00000010M;
            var j = 0.00000001M;

            // Act
            var ai = new InclusiveRange { Increment = a };
            var bi = new InclusiveRange { Increment = b };
            var ci = new InclusiveRange { Increment = c };
            var di = new InclusiveRange { Increment = d };
            var ei = new InclusiveRange { Increment = e };
            var fi = new InclusiveRange { Increment = f };
            var gi = new InclusiveRange { Increment = g };
            var hi = new InclusiveRange { Increment = h };
            var ji = new InclusiveRange { Increment = j };

            // Assert
            Assert.AreEqual(ai.Increment, a);
            Assert.AreEqual(bi.Increment, b);
            Assert.AreEqual(ci.Increment, c);
            Assert.AreEqual(di.Increment, d);
            Assert.AreEqual(ei.Increment, e);
            Assert.AreEqual(fi.Increment, f);
            Assert.AreEqual(gi.Increment, g);
            Assert.AreEqual(hi.Increment, h);
            Assert.AreEqual(ji.Increment, j);

            Assert.AreEqual(ai.Increment.ToString(), "1");
            Assert.AreEqual(bi.Increment.ToString(), "0.1");
            Assert.AreEqual(ci.Increment.ToString(), "0.01");
            Assert.AreEqual(di.Increment.ToString(), "0.001");
            Assert.AreEqual(ei.Increment.ToString(), "0.0001");
            Assert.AreEqual(fi.Increment.ToString(), "0.00001");
            Assert.AreEqual(gi.Increment.ToString(), "0.000001");
            Assert.AreEqual(hi.Increment.ToString(), "0.0000001");
            Assert.AreEqual(ji.Increment.ToString(), "0.00000001");
        }
    }
}
