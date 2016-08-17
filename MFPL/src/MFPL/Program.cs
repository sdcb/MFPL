using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace MFPL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Assembly"), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");
            var typeBuilder = moduleBuilder.DefineType("Type", TypeAttributes.Public | TypeAttributes.Class);
            var methodBuilder = typeBuilder.DefineMethod("Method", MethodAttributes.Public | MethodAttributes.Static);

            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldstr, "Hello World");
            il.Emit(OpCodes.Call, typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(string) }));
            il.Emit(OpCodes.Ret);            

            var typeInfo = typeBuilder.CreateTypeInfo();
            var method = typeInfo.GetMethod(methodBuilder.Name);
            var action = (Action)method.CreateDelegate(typeof(Action));
            action();
        }
    }
}
