using DevelopmentInProgress.TradeView.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.TradeView.Wpf.Trading.Test
{
    [TestClass]
    public class DecimalExtensionsTest
    {
        [TestMethod]
        public void DecimalTrim()
        {
            // Arrange
            var a = 1.123456789M;
            var b = 1.123456789M;
            var c = 1.123456789M;
            var d = 1.123456789M;
            var e = 1.123456789M;
            var f = 1.123456789M;
            var g = 1.123456789M;
            var h = 1.123456789M;
            var i = 1.123456789M;
            var j = 0.000123M;

            // Act
            var ai = a.Trim(0);
            var bi = b.Trim(1);
            var ci = c.Trim(2);
            var di = d.Trim(3);
            var ei = e.Trim(4);
            var fi = f.Trim(5);
            var gi = g.Trim(6);
            var hi = h.Trim(7);
            var ii = i.Trim(8);
            var ji = j.Trim(8);

            // Assert
            Assert.IsTrue(ai.Equals(1M));
            Assert.IsTrue(bi.Equals(1.1M));
            Assert.IsTrue(ci.Equals(1.12M));
            Assert.IsTrue(di.Equals(1.123M));
            Assert.IsTrue(ei.Equals(1.1234M));
            Assert.IsTrue(fi.Equals(1.12345M));
            Assert.IsTrue(gi.Equals(1.123456M));
            Assert.IsTrue(hi.Equals(1.1234567M));
            Assert.IsTrue(ii.Equals(1.12345678M));
            Assert.IsTrue(ji.Equals(0.000123M));
        }
        
        [TestMethod]
        public void HasRemainder()
        {
            // Arrange
            var a = 0.123M;
            var b = -0.123M;
            var c = 123.00000000M;
            var d = -123.00000000M;
            var e = 0.00000000001M;

            // Act

            // Assert
            Assert.IsTrue(a.HasRemainder());
            Assert.IsTrue(b.HasRemainder());
            Assert.IsFalse(c.HasRemainder());
            Assert.IsFalse(d.HasRemainder());
            Assert.IsTrue(e.HasRemainder());
        }

        [TestMethod]
        public void GetPrecision()
        {
            // Arrange
            var a = 1M;
            var b = -0.123M;
            var c = 123.00000001M;
            var d = -123.00001000M;
            var e = 0.10000000000M;
            var f = 0.00000000000M;

            // Act

            // Assert
            Assert.AreEqual(a.GetPrecision(), 0);
            Assert.AreEqual(b.GetPrecision(), 3);
            Assert.AreEqual(c.GetPrecision(), 8);
            Assert.AreEqual(d.GetPrecision(), 8);
            Assert.AreEqual(e.GetPrecision(), 11);
            Assert.AreEqual(f.GetPrecision(), 11);
        }
    }
}
