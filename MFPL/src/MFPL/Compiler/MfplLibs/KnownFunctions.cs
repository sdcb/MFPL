using MFPL.Compiler.Core;
using MFPL.Compiler.MfplLibs.Implements;
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
        public static Result<MfplFuntionEmiter> GetFunction(string name, MfplTypes[] parameters)
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
                return Result.Fail<MfplFuntionEmiter>(
                    $"Cannot found function '{name}' with parameter '{string.Join(",", parameters)}'.");
            }

            var emiter = (Emiter)mi.Method.CreateDelegate(typeof(Emiter));
            return Result.Ok(new MfplFuntionEmiter(emiter, mi.Attribute.ReturnType));
        }
    }
}
