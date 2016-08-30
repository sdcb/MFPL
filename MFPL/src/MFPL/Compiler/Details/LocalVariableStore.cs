using MFPL.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace MFPL.Compiler.Details
{
    public class LocalVariableStore
    {
        Dictionary<string, LocalBuilder> vars = new Dictionary<string, LocalBuilder>();

        // TODO:
        // LocalVariableStore ParentLocalVarialbleStore { get; set; }

        public Result<LocalBuilder> Get(string syntax)
        {
            if (vars.ContainsKey(syntax))
            {
                return Result.Ok(vars[syntax]);
            }
            else
            {
                return Result.Fail<LocalBuilder>($"Local variable '{syntax}' not declared.");
            }
        }

        public Result Set(string syntax, LocalBuilder localBuilder)
        {
            if (vars.ContainsKey(syntax))
            {
                return Result.Fail($"Local variable '{syntax}' existed.");
            }
            else
            {
                vars[syntax] = localBuilder;
                return Result.Ok();
            }            
        }

        public static LocalVariableStore CreateEmpty()
        {
            return new LocalVariableStore();
        }

        public static LocalVariableStore CreateInScope()
        {
            throw new NotImplementedException();
        }
    }
}
