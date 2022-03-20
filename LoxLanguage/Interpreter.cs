﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxLanguage {
    internal class Interpreter : Visitor<Object> {
        public object VisitBinaryExpr(Binary expr) {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Oper.Type) {
                case TokenType.GREATER:
                    CheckNumberOperands(expr.Oper, left, right);
                    return Convert.ToDouble(left) > Convert.ToDouble(right);

                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.Oper, left, right);
                    return Convert.ToDouble(left) >= Convert.ToDouble(right);

                case TokenType.LESS:
                    CheckNumberOperands(expr.Oper, left, right);
                    return Convert.ToDouble(left) < Convert.ToDouble(right);

                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.Oper, left, right);
                    return Convert.ToDouble(left) <= Convert.ToDouble(right);

                case TokenType.MINUS:
                    CheckNumberOperands(expr.Oper, left, right);
                    return Convert.ToDouble(left) - Convert.ToDouble(right);                    

                case TokenType.SLASH:
                    CheckNumberOperands(expr.Oper, left, right);                    

                    if (Convert.ToDouble(right) == 0.0) {
                        throw new RuntimeError(expr.Oper, "Division by constant zero.");
                    }
                    return Convert.ToDouble(left) / Convert.ToDouble(right);

                case TokenType.STAR:
                    CheckNumberOperands(expr.Oper, left, right);
                    return Convert.ToDouble(left) * Convert.ToDouble(right);

                case TokenType.PLUS:
                    if (left is double && right is double) return Convert.ToDouble(left) + Convert.ToDouble(right);
                    if ((left is double && right is string) 
                        || (left is string && right is double) 
                        || (left is string && right is string)) return Convert.ToString(left) + Convert.ToString(right);


                    throw new RuntimeError(expr.Oper, "Operands (" + left + ", " + right + ") must be numbers or strings.");                    
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);

                case TokenType.EQUAL_EQUAL:
                    return !IsEqual(left, right);
            }

            return null;
        }

        public void Interpret(Expr expression) {
            try {
                Object value = Evaluate(expression);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError error) {
                Lox.RuntimeError(error);
            }
        }

        public object VisitGroupingExpr(Grouping expr) {
            return Evaluate(expr.Expression);
        }

        private object Evaluate(Expr expr) {
            return expr.Accept(this);
        }

        public object VisitLiteralExpr(Literal expr) {
            return expr.Value;
        }

        public object VisitTernaryExpr(Ternary expr) {            
            object left = Evaluate(expr.Left);
            object middle = Evaluate(expr.Middle);
            object right = Evaluate(expr.Right);

            Token oper = expr.Oper;
            Token oper2 = expr.Oper2;

            if (oper.Type == TokenType.QUESTION_MARK && oper2.Type == TokenType.COLON) {
                if (Convert.ToBoolean(left)) {
                    return middle;
                }
                else {
                    return right;
                }
            }

            return null;
        }

        public object VisitUnaryExpr(Unary expr) {
            object right = Evaluate(expr.Right);

            switch (expr.Operator.Type) {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    CheckNumberOperand(expr.Operator, right);
                    return -(double)right;
            }

            // Unreachable
            return null;
        }
        private void CheckNumberOperand(Token oper, object operand) {
            if (operand is double) return;
            throw new RuntimeError(oper, "Operand '" + operand + "' must be a number");
        }

        private void CheckNumberOperands(Token oper, object left, object right) {
            if (left is double && right is double) return;
            throw new RuntimeError(oper, "Operands (" + left + ", " + right + ") must be numbers.");
        }

        private bool IsTruthy(object defaultValue) {
            if (defaultValue == null) return false;
            if (defaultValue is bool) return Convert.ToBoolean(defaultValue);            
            return true;
        }

        private bool IsEqual (object a, object b) {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        private string Stringify(Object obj) {
            if (obj == null) return "nil";

            if (obj is double) {
                string text = Convert.ToString(obj);

                if (text.EndsWith(".0")) {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return Convert.ToString(obj);
        }
    }
}