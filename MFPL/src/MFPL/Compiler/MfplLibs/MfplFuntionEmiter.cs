using MFPL.Compiler.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace MFPL.Compiler.MfplLibs
{
    public delegate void Emiter(ILGenerator ilGenerator);

    public class MfplFuntionEmiter
    {
        public Emiter Emiter { get; }

        public MfplTypes ReturnType { get; }

        public MfplFuntionEmiter(Emiter emiter, MfplTypes returnType)
        {
            Emiter = emiter;
            ReturnType = returnType;
        }
    }
}
