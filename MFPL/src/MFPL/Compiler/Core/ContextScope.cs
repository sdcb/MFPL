using MFPL.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace MFPL.Compiler.Core
{
    public class ContextScope<T>
    {
        Dictionary<string, T> Datas = new Dictionary<string, T>();

        Maybe<ContextScope<T>> ParentScope { get; set; }

        public ContextScope(Maybe<ContextScope<T>> parentStore)
        {
            ParentScope = parentStore;
        }

        public Result<T> Get(string syntax)
        {
            if (Datas.ContainsKey(syntax))
            {
                return Result.Ok(Datas[syntax]);
            }
            else if (ParentScope.HasValue)
            {
                return ParentScope.Value.Get(syntax);
            }
            else
            {
                return Result.Fail<T>($"Syntax '{syntax}' not declared.");
            }
        }

        public Result<T> Declare(string syntax, T data)
        {
            if (Contains(syntax))
            {
                return Result.Fail<T>($"Syntax '{syntax}' already declared.");
            }
            else
            {
                Datas[syntax] = data;
                return Result.Ok(data);
            }
        }

        public bool Contains(string syntax)
        {
            if (Datas.ContainsKey(syntax))
            {
                return true;
            }
            else if (ParentScope.HasValue)
            {
                return ParentScope.Value.Contains(syntax);
            }
            else
            {
                return false;
            }
        }
    }

    public static class ContextScope
    {
        public static ContextScope<T> CreateEmpty<T>() where T : class
        {
            return new ContextScope<T>(Maybe.Empty<ContextScope<T>>());
        }

        public static ContextScope<T> CreateInScope<T>(ContextScope<T> scope)
        {
            return new ContextScope<T>(scope);
        }
    }
}
