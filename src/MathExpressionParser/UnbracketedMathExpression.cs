using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionParser
{
    /// <summary>
    /// Represent a raw mathematical expression without any brackets '()'
    /// </summary>
    public class UnbracketedMathExpression
    {
        private string expr;
        private bool isParsed;

        public UnbracketedMathExpression(string expr)
        {
            this.expr = expr;

            this.Validate();

            this.isParsed = false;
        }

        private UnbracketedMathExpression _left;
        public UnbracketedMathExpression Left
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

        private UnbracketedMathExpression _right;
        public UnbracketedMathExpression Righ
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
                double result = 0;

                if (double.TryParse(RawValue, out result))
                {
                    return result;
                }
                else
                {
                    throw new ArgumentException(string.Format("'{0}' is not a valid double value", RawValue), "expr");
                }
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

                    this.Left = new UnbracketedMathExpression(expr.Remove(operatorPosition));
                    this.Righ = new UnbracketedMathExpression(expr.Remove(0, operatorPosition + 1));

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
}
