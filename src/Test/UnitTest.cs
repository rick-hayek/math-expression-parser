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
            UnbracketedMathExpression expr = new UnbracketedMathExpression(str);
            var result = expr.Calculate();
            Assert.IsTrue(result == -6);
        }

        [TestMethod]
        public void BracketedExpression()
        {
            var str = "((1))-((2*(-2+5))+6)/(2*3)";
            MathExpression expr = new MathExpression(str);
            var result = expr.Resolve();
            Assert.IsTrue(result == -1);
        }
    }
}
