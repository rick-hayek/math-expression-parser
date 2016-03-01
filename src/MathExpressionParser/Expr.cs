using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionParser
{
    public class Expr
    {
        private string expr;
        private bool isParsed;

        public Expr(string expr)
        {
            this.expr = expr.Replace(" ", ""); // Remove white spaces

            this.Validate();

            this.isParsed = false;
            //this.Parse();
        }

        private Expr _left;
        public Expr Left
        {
            get
            {
                return _left;
            }

            private set
            {
                _left = value;
            }
        }

        private Expr _right;
        public Expr Righ
        {
            get
            {
                return _right;
            }

            private set
            {
                _right = value;
            }
        }

        private Operator? _op;
        public Operator? Op
        {
            get
            {
                return _op;
            }

            private set
            {
                _op = value;
            }
        }

        private ExpressionType _exprType;
        public ExpressionType ExprType
        {
            get
            {
                return _exprType;
            }

            private set
            {
                _exprType = value;
            }
        }

        public string RawValue
        {
            get
            {
                return this.expr;
            }
        }

        public double Calculate()
        {
            if (!isParsed)
            {
                this.Parse();
            }

            if (ExprType == ExpressionType.SingleValue)
            {
                //double result = 0;

                //if (double.TryParse(RawValue, out result))
                //{
                //    return result;
                //}
                //else
                //{
                //    throw new ArgumentException(string.Format("'{0}' is not a valid mathematical string.", RawValue), "expr");
                //}

                return double.Parse(this.RawValue);
            }

            var left = this.Left.Calculate();
            var right = this.Righ.Calculate();

            switch (Op)
            {
                case Operator.Add:
                    return left + right;
                case Operator.Subtract:
                    return left - right;
                case Operator.Multiply:
                    return left * right;
                case Operator.Divide:
                    if (right == 0)
                    {
                        throw new DivideByZeroException();
                    }

                    return left / right;
                default:
                    break;
            }

            throw new Exception("Invalid Operator.");
        }

        private void Parse()
        {
            this.isParsed = true;

            if (expr.Any(ch => ch.IsOperator()))
            {
                char? operatorChar = null;

                // Find all negative and positive signs
                int operatorPosition = -1;
                List<int> signIndexList = new List<int>();
                int startIndex = expr.Length - 1; // Find from last occurrence

                while (expr.Any(ch => !signIndexList.Contains(expr.IndexOf(ch)) &&
                                      (ch.ToOperator() == Operator.Add || ch.ToOperator() == Operator.Subtract)))
                {
                    for (int i = startIndex; i >= 0; i--)
                    {
                        char ch = expr[i];
                        if (!signIndexList.Contains(i) &&
                            ch.IsOperator() &&
                            (ch.ToOperator() == Operator.Add || ch.ToOperator() == Operator.Subtract)) // +, -
                        {
                            operatorChar = ch;
                            operatorPosition = i;
                            break;
                        }
                    }

                    if (operatorPosition == 0 || expr[operatorPosition - 1].IsOperator() || expr[operatorPosition - 1] == '(')
                    {
                        // The character is a sign, skip it and find a next '+' or '-'
                        operatorChar = null;
                        startIndex = operatorPosition;

                        signIndexList.Add(operatorPosition);
                        continue;
                    }
                    else
                    {
                        // Find a valid operator '+' or '-'
                        break;
                    }
                }

                if (operatorChar == null)
                {
                    // Cannot find operator '+' or '-', try to find '*' or '/'
                    for (int i = expr.Length - 1; i >= 0; i--)
                    {
                        char ch = expr[i];
                        if (ch.ToOperator() == Operator.Multiply || ch.ToOperator() == Operator.Divide)
                        {
                            operatorChar = ch;
                            operatorPosition = i;
                            break;
                        }
                    }
                }

                if (operatorChar != null && operatorChar.HasValue && operatorChar.Value != '\0')
                {
                    this.ExprType = ExpressionType.Expression;
                    this.Op = operatorChar.Value.ToOperator();

                    this.Left = new Expr(expr.Remove(operatorPosition));
                    this.Righ = new Expr(expr.Remove(0, operatorPosition + 1));

                    return;
                }
            }

            this.ExprType = ExpressionType.SingleValue;
            this.Left = null;
            this.Righ = null;

            return;
        }

        private bool Validate()
        {
            return this.expr.IsValidMathExpression();
        }
    }

    public enum Operator
    {
        Invalid = -1,

        Add = '+',
        Subtract = '-',
        Multiply = '*',
        Divide = '/',
    }

    public enum ExpressionType
    {
        Expression,
        SingleValue,
        //SignedSingleValue,
    }

    public static class Extensions2
    {
        public static bool IsOperator(this char ch)
        {
            return ch == '+' || ch == '-' || ch == '*' || ch == '/';
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
            expr = expr.Replace(" ", "");

            if (string.IsNullOrEmpty(expr))
            {
                throw new ArgumentNullException("expr", "Expression cannot be null or empty.");
            }

            // Check the first character. The 1st ch cannot be '.', '*', or '/'
            char first = expr[0];
            if (first == '.' || first == '*' || first == '/')
            {
                throw new ArgumentException(string.Format("Illeagal Math Expression. Character: {0}; Position: {1}", first, 0), "expr");
            }

            // Check brackets

            for (int i = 0; i < expr.Length; i++)
            {
                var ch = expr[i];
                if ((ch >= '0' && ch <= '9') ||
                    (ch == '+' || ch == '-' || ch == '*' || ch == '/' || ch == '(' || ch == ')') ||
                    (ch == '.'))
                {
                    continue;
                }

                throw new ArgumentException(string.Format("Illeagal Math Expression. Character: {0}; Position: {1}", ch, i), "expr");
            }

            return true;
        }
    }
}
