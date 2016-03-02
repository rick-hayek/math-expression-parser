using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathExpressionParser;

namespace Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            var str = "-1*2+5-6/2*3";
            Expr expr = new Expr(str);
            var result = expr.Calculate();
            Assert.IsTrue(result == -6);
        }
    }
}
