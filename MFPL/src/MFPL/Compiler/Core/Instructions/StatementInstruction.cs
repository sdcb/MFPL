using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFPL.Compiler.Core.Instructions
{
    public abstract class StatementInstruction
    {
        public StatementType GetStatementType()
        {
            if (this is DeclareLocalInstruction)
                return StatementType.DeclareLocal;
            else if (this is DefineLabelInstruction)
                return StatementType.DefineLabel;
            else if (this is ExpressionInstructions)
                return StatementType.Expression;
            else
                throw new IndexOutOfRangeException($"Unknown StatementInstruction type.");
        }
    }

    public enum StatementType
    {
        Expression,
        DeclareLocal,
        DefineLabel,
    }
}
