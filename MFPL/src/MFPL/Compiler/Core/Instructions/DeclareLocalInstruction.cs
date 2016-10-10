using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace MFPL.Compiler.Core.Instructions
{
    public class DeclareLocalInstruction : StatementInstruction
    {
        public MfplTypes MfplType { get; set; }

        public static DeclareLocalInstruction Create(MfplTypes type)
            => new DeclareLocalInstruction { MfplType = type };

        public LocalBuilder Emit(ILGenerator il)
            => il.DeclareLocal(MfplTypeUtil.MfplTypeToType(MfplType));
    }
}
