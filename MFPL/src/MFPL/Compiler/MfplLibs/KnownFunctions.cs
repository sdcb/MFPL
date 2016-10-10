using MFPL.Compiler.Core;
using MFPL.Compiler.Core.Instructions;
using MFPL.Compiler.MfplLibs.Implements;
using MFPL.Compiler.Visitors;
using MFPL.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace MFPL.Compiler.MfplLibs
{
    public class KnownFunction
    {
        public static Result<ExpressionInstructions> GetFunction(string name, MfplTypes[] parameters)
        {
            var mi = typeof(KnownFunctionsImplement)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Select(x => new
                {
                    Method = x, 
                    Attribute = x.GetCustomAttribute<MfplFunctionDefAttribute>()
                })
                .Where(x => x.Attribute != null)
                .Where(x => x.Attribute.Name == name && x.Attribute.ArgumentTypes.SequenceEqual(parameters))
                .FirstOrDefault();

            if (mi == null)
            {
                return Result.Fail<ExpressionInstructions>(
                    $"Cannot found function '{name}' with parameter '{string.Join(",", parameters)}'.");
            }

            var emiter = (Func<ExpressionInstructions>)mi.Method.CreateDelegate(typeof(Func<ExpressionInstructions>));
            return Result.Ok(emiter());
        }
    }
}
