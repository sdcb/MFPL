using MFPL.Functional;
using MFPL.Parser.G4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using static MFPL.Parser.G4.MfplParser;
using System.Reflection.Emit;

namespace MFPL.Compiler.Details
{
    public class StatementVisitor : MfplBaseVisitor<Result>
    {
        private readonly ILGenerator il;

        public StatementVisitor(ILGenerator il)
        {
            this.il = il;
        }
    }
}
