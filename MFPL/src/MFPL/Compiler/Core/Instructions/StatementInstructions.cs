using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace MFPL.Compiler.Core.Instructions
{
    public class StatementInstructions
    {
        public List<StatementInstruction> Instructions { get; set; }

        public void Emit(ILGenerator il)
        {
            foreach (var instruction in Instructions)
            {
                switch (instruction.GetStatementType())
                {
                    case StatementType.DeclareLocal:
                        var declareLocal = instruction as DeclareLocalInstruction;
                        var local = declareLocal.Emit(il);
                        throw new NotImplementedException("What's next?");
                    case StatementType.DefineLabel:
                        var defineLabel = instruction as DefineLabelInstruction;
                        var label = defineLabel.Emit(il);
                        throw new NotImplementedException("What's next?");
                    case StatementType.Expression:
                        var expression = instruction as ExpressionInstructions;
                        expression.EmitAll(il);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
