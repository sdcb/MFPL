using MFPL.Compiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace MFPL.Compiler.MfplLibs.Implements
{
    public class KnownFunctionsImplement
    {
        [MfplFunctionDef(MfplTypes.Void, "printHelloWorld", new MfplTypes[] { })]
        public static void PrintHelloWorld(ILGenerator il)
        {
            il.Emit(OpCodes.Ldstr, "Hello World");
            il.Emit(OpCodes.Call, typeof(Console).GetMethod(nameof(Console.WriteLine), Type.EmptyTypes));
        }
    }
}
