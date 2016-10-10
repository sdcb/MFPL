using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace MFPL.Compiler.Core.Instructions
{
    public class DefineLabelInstruction : StatementInstruction
    {
        public static DefineLabelInstruction Create()
            => new DefineLabelInstruction();

        public Label Emit(ILGenerator il)
            => il.DefineLabel();
    }
}
