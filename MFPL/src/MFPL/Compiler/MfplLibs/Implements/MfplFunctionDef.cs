using MFPL.Compiler.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFPL.Compiler.MfplLibs.Implements
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MfplFunctionDefAttribute : Attribute
    {
        public MfplTypes ReturnType { get; set; }

        public string Name { get; set; }

        public MfplTypes[] ArgumentTypes { get; set; }

        public MfplFunctionDefAttribute(MfplTypes returnType, string name, MfplTypes[] argumentTypes)
        {
            ReturnType = returnType;
            Name = name;
            ArgumentTypes = argumentTypes;
        }
    }
}
