using MFPL.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFPL.Compiler.Details
{
    public static class MfplTypeUtil
    {
        public static Result<MfplTypes> BinaryOperator(Result<MfplTypes> type1, Result<MfplTypes> type2, string op)
        {
            if (type1.IsFailure)
            {
                return type1;
            }
            else if (type2.IsFailure)
            {
                return type2;
            }
            else
            {
                return BinaryOperator(type1.Value, type2.Value, op);
            }
        }

        public static Result<MfplTypes> BinaryOperator(MfplTypes type1, MfplTypes type2, string op)
        {
            if (type1 != type2)
            {
                return Result.Fail<MfplTypes>("Binary operator must be same type.");
            }

            if (op == "+")
            {
                if (type1 == MfplTypes.String)
                    return Result.Ok(type1);
                else if (type1 == MfplTypes.Number)
                    return Result.Ok(type1);
                else
                    return Result.Fail<MfplTypes>("Binary operator + must be string or number type.");
            }
            else if (op == "-" || op == "*" || op == "/")
            {
                if (type1 == MfplTypes.Number)
                    return Result.Ok(type1);
                else
                    return Result.Fail<MfplTypes>("Binary operator - * / must be number type.");
            }
            else if (op == ">" || op == "<" || op == ">=" || op == "<=")
            {
                if (type1 == MfplTypes.Number)
                    return Result.Ok(MfplTypes.Bool);
                else
                    return Result.Fail<MfplTypes>("Binary operator > < >= <= must be number type.");
            }
            else if (op == "==" || op == "!=")
            {
                return Result.Ok(MfplTypes.Bool);
            }
            else if (op == "&&" || op == "||")
            {
                if (type1 == MfplTypes.Bool)
                    return Result.Ok(type1);
                else
                    return Result.Fail<MfplTypes>("Binary operator && || must be bool type.");
            }
            else
            {
                return Result.Fail<MfplTypes>($"Unknown binary operator '{op}'.");
            }
        }

        public static Result<MfplTypes> UnaryOperator(string op, Result<MfplTypes> type)
        {
            if (type.IsFailure)
            {
                return type;
            }
            else
            {
                return UnaryOperator(op, type.Value);
            }
        }

        public static Result<MfplTypes> UnaryOperator(string op, MfplTypes type)
        {
            if (op == "!")
            {
                if (type == MfplTypes.Bool)
                {
                    return Result.Ok(MfplTypes.Bool);
                }
                else
                {
                    return Result.Fail<MfplTypes>("Unary operator ! must be bool type.");
                }
            }
            else if (op == "-" || op == "+")
            {
                if (type == MfplTypes.Number)
                {
                    return Result.Ok(MfplTypes.Number);
                }
                else
                {
                    return Result.Fail<MfplTypes>("Unary operator + - must be number type.");
                }
            }
            else
            {
                return Result.Fail<MfplTypes>($"Unknown unary operator '{op}'.");
            }
        }
    }
}
