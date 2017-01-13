using MFPL.Compiler.Core;
using MFPL.Functional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MFPL.KnownFunctions
{
    public static class KnownFunction
    {
        public static Result<MethodInfo> Get(string name, List<MfplTypes> arguments)
        {
            switch (name)
            {
                case "abs":
                    if (arguments.Count == 1 && arguments[0] == MfplTypes.Number)
                    {
                        return GetMethod(typeof(Math), nameof(Math.Abs), typeof(double));
                    }
                    break;
                case "print":
                    if (arguments.Count == 1)
                    {
                        if (arguments[0] == MfplTypes.Number)
                        {
                            return GetMethod(typeof(Console), nameof(Console.WriteLine), typeof(double));
                        }
                        if (arguments[0] == MfplTypes.String)
                        {
                            return GetMethod(typeof(Console), nameof(Console.WriteLine), typeof(string));
                        }
                        else if (arguments[0] == MfplTypes.Bool)
                        {
                            return GetMethod(typeof(Console), nameof(Console.WriteLine), typeof(bool));
                        }
                    }
                    break;
            }
            return Result.Fail<MethodInfo>(
                $"Unknown method: '{name}' with {arguments.Count} arguments.");
        }

        private static Result<MethodInfo> GetMethod(Type type, string name)
        {
            var method = type.GetTypeInfo().GetMethod(name);
            if (method == null) throw new NullReferenceException();
            return Result.Ok(method);
        }

        private static Result<MethodInfo> GetMethod(Type type, string name, params Type[] types)
        {
            var method = type.GetTypeInfo().GetMethod(name, types);
            if (method == null) throw new NullReferenceException();
            return Result.Ok(method);
        }
    }
}
