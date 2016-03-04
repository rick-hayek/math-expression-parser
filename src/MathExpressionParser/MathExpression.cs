using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionParser
{
    public class MathExpression
    {
        string expr;
        public MathExpression(string expr)
        {
            this.expr = expr;
        }

        public double Resolve()
        {
            bool bracketExists = false;
            var len = expr.Length;
            int indexFirstCloseBracket = 0, // index of first ')'
                indexMatchedOpenBracket = 0;  // index of '(' which matches with the first ')'

            string tempExpr = this.expr,
                   beforeBracketExpr, // expression before the '('
                   insideBracketExpr, // expression insided the '()'
                   afterBracketExpr; // expression after the ')'
            double resultOfFirstBracketedExpression = 0;

            for (indexFirstCloseBracket = 0; indexFirstCloseBracket < len; indexFirstCloseBracket++)
            {
                var ch = expr[indexFirstCloseBracket];
                if (bracketExists = (ch == ')')) // find the 1st ')'
                {
                    for (indexMatchedOpenBracket = indexFirstCloseBracket - 1; indexMatchedOpenBracket >= 0; indexMatchedOpenBracket--)
                    {
                        if (expr[indexMatchedOpenBracket] == '(') // find the matched '(' with the first ')'
                        {
                            if (indexMatchedOpenBracket == 0)
                            {
                                beforeBracketExpr = "";
                            }
                            else
                            {
                                beforeBracketExpr = expr.Substring(0, indexMatchedOpenBracket);
                            }

                            insideBracketExpr = expr.Substring(indexMatchedOpenBracket + 1, indexFirstCloseBracket - indexMatchedOpenBracket - 1);
                            afterBracketExpr = expr.Substring(indexFirstCloseBracket + 1, len - indexFirstCloseBracket - 1);

                            UnbracketedMathExpression math = new UnbracketedMathExpression(insideBracketExpr);
                            resultOfFirstBracketedExpression = math.Calculate();

                            tempExpr = beforeBracketExpr + resultOfFirstBracketedExpression.ToString() + afterBracketExpr;

                            return new MathExpression(tempExpr).Resolve();
                        }
                    }
                }
            }

            if (!bracketExists)
            {
                return new UnbracketedMathExpression(expr).Calculate();
            }

            throw new ArgumentException(string.Format("'{0}' is not a valid mathematical expression.", this.expr), "expr");
        }
    }
}
