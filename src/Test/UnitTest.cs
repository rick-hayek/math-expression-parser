using System;
using MathExpressionParser;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace Test
{
    public class UnitTest
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]
        [InlineData("1+2", 3)]
        [InlineData("-1+2", 1)]
        [InlineData("1+2-1", 2)]
        [InlineData("-1+2--3", 4)]
        [InlineData("-1+2*+3-4/2*-2-+3", 6)]
        [InlineData("1+2-3*4+6/3*2", -5)]
        public void UnbraketedExpressionTest(string expr, double expectedResult)
        {
            MathExpression math = new MathExpression(expr);
            var actualResult = math.Evaluate();

            System.Diagnostics.Debug.WriteLine("Actual Result: {0}; Expected Result: {1}", actualResult, expectedResult);
            Assert.Equal(actualResult, expectedResult);
        }

        [Theory]
        [InlineData("(1)", 1)]
        [InlineData("(-1)", -1)]
        [InlineData("-(1)", -1)]
        [InlineData("1+(2-3)*4+6/(3*2)", -2)]
        [InlineData("1+(2-3)*(4+6/(3*2))", -4)]
        [InlineData("1+(.3-.2)*4", 1.4)]
        [InlineData("(6)/((1+2)*4)", 0.5)]
        public void BraketedExpressionTest(string expr, double expectedResult)
        {
            MathExpression math = new MathExpression(expr);
            var actualResult = math.Evaluate();

            System.Diagnostics.Debug.WriteLine("Actual Result: {0}; Expected Result: {1}", actualResult, expectedResult);
            Assert.Equal(actualResult, expectedResult);
        }

        [Theory]
        [InlineData("1+")]
        [InlineData("1*")]
        [InlineData("1+*1")]
        [InlineData("1)")]
        [InlineData("(1+2")]
        [InlineData("1+0 .1")]
        [InlineData("(1+2)*3)")]
        [InlineData("1+..2")]
        [InlineData("*1-2")]
        [InlineData("(1+2).3")]
        [InlineData(" ")]
        [InlineData("6.6.6+1")]
        public void ErrorHandling(string expr)
        {
            try
            {
                // Should throw exceptions
                MathExpression math = new MathExpression(expr); 
                math.Evaluate(); 

                Assert.True(false); // Code should never hit here
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error Occurs: {0}", e.Message);
                Assert.True(true);
            }
        }
    }
}
