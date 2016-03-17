using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionParser
{
    public static class Extension
    {
        public static bool IsOperator(this char ch)
        {
            return ch == '+' || ch == '-' || ch == '*' || ch == '/';
        }

        public static bool IsNumber(this char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        public static Operator ToOperator(this char ch)
        {
            try
            {
                return (Operator)Enum.ToObject(typeof(Operator), ch);
            }
            catch (Exception)
            {
                return Operator.Invalid;
            }
        }

        public static bool IsValidMathExpression(this string expr)
        {
            // Check decimal point '.'
            if (!expr.ValidateDecimalPoint())
            {
                return false;
            }

            // Check brackets: '(' and ')'
            if (!expr.ValidateBracketsForExpression())
            {
                return false;
            }

            var temp = expr.Replace(" ", "");

            if (string.IsNullOrEmpty(temp))
            {
                throw new ArgumentNullException("expr", "Expression cannot be null or empty.");
            }

            // Check the first character. The 1st ch cannot be '*', '/'
            char first = temp[0];
            if (first == '*' || first == '/')
            {
                throw new ArgumentException(string.Format("Illeagal Math Expression. Character: '{0}'; Position: {1}", first, 0), "expr");
            }

            // Check the last character. The last one must be 0-9, or ')'
            char last = temp.Last();
            if (!last.IsNumber() && (last != ')'))
            {
                throw new ArgumentException(string.Format("Illeagal Math Expression. Character: '{0}'; Position: {1}", last, temp.Length - 1), "expr");
            }

            for (int i = 0; i < temp.Length; i++)
            {
                var ch = temp[i];
                if (ch.IsNumber() ||
                    (ch.IsOperator() || ch == '(' || ch == ')') ||
                    (ch == '.'))
                {
                    continue;
                }

                throw new ArgumentException(string.Format("Illeagal Math Expression. Character: '{0}'; Position: {1}", ch, i), "expr");
            }

            return true;
        }

        /// <summary>
        /// Validate brackets in a mathematical expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static bool ValidateBracketsForExpression(this string expr)
        {
            if (string.IsNullOrWhiteSpace(expr))
            {
                return true;
            }

            List<int> closeBracketIndexList = new List<int>();
            List<int> openBracketIndexList = new List<int>();

            int closeBracketIndex = 0;
            while (closeBracketIndex < expr.Length)
            {
                bool isCloseBracketFound = false;
                bool isOpenBracketFound = false;

                if (!closeBracketIndexList.Contains(closeBracketIndex) &&
                    expr[closeBracketIndex] == ')') // find the first ')'
                {
                    isCloseBracketFound = true;
                    closeBracketIndexList.Add(closeBracketIndex);

                    for (int j = closeBracketIndex - 1; j >= 0; j--)
                    {
                        if (!openBracketIndexList.Contains(j) && expr[j] == '(') // find the matched '(' with the first ')'
                        {
                            openBracketIndexList.Add(j);
                            isOpenBracketFound = true;
                            break;
                        }
                    }

                    if (!isOpenBracketFound) // If ')' exists while '(' is not found, it's an invalid expression
                    {
                        throw new ArgumentException(
                            string.Format("Illeagal Math Expression. Character: '{0}'; Position: {1}", expr[closeBracketIndex], closeBracketIndex), "expr");
                    }
                }

                closeBracketIndex++;
            }

            for (int i = 0; i < expr.Length; i++)
            {
                if (!openBracketIndexList.Contains(i) && expr[i] == '(')
                {
                    throw new ArgumentException(
                         string.Format("Illeagal Math Expression. Character: '{0}'; Position: {1}", '(', i), "expr");
                }
            }

            return true;
        }

        public static bool ValidateDecimalPoint(this string expr)
        {
            if (string.IsNullOrWhiteSpace(expr))
            {
                return true;
            }

            int index = 0;
            int len = expr.Length;
            while (index < len)
            {
                if (expr[index] == '.')
                {
                    // Check the character after the '.'
                    if (index == len - 1) // Indicates the last character of expression is '.'
                    {
                        throw new ArgumentException(
                            string.Format("ValidateDecimalPoint: Invalid decimal point. Position: {0}", index), "expr");
                    }

                    char next = expr[index + 1];
                    if (next < '0' || next > '9') // The character immediately after the '.' must be 0-9
                    {
                        throw new ArgumentException(
                            string.Format("ValidateDecimalPoint: Invalid decimal point. Position: {0}", index), "expr");
                    }

                    // Check the character before the '.'
                    if (index > 0)
                    {
                        bool whiteSpaceImmediatelyBeforePointExists = false;

                        for (int i = index - 1; i >= 0; i--)
                        {
                            char pre = expr[i];

                            if (pre == ' ') // Ignore white spaces before the '.'
                            {
                                whiteSpaceImmediatelyBeforePointExists = true;
                                continue;
                            }

                            if (pre.IsOperator() || pre == '(')
                            {
                                break; // It's a valid '.': 1+ .2; (.1+2)
                            }
                            else
                            {
                                if (pre.IsNumber() && !whiteSpaceImmediatelyBeforePointExists)
                                {
                                    break; // It's a valid '.': 1.13
                                }
                            }

                            // Illeagal '.' in any other situations: (1+1).2; 1 .2; ..1;
                            throw new ArgumentException(
                                string.Format("ValidateDecimalPoint: Invalid decimal point. Position: {0}", index), "expr");
                        }
                    }
                    // else index == 0 // the 1st character is '.', which is a leagal expression
                }

                index++;
            }

            return true;
        }
    }
}
