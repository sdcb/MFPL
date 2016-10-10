using MFPL.Compiler.Core;
using MFPL.Compiler.Core.Instructions;
using MFPL.Compiler.Visitors;
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
        [MfplFunctionDef("printHelloWorld", new MfplTypes[] { })]
        public static ExpressionInstructions PrintHelloWorld()
        {
            return ExpressionInstructions.Create(new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldstr, "Hello World"),
                Instruction.Create(OpCodes.Call,
                typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(string) }))
            }, MfplTypes.Void);
        }
    }
}
