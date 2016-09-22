using MFPL.Compiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFPL.Compiler.MfplLibs.Implements
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MfplFunctionDefAttribute : Attribute
    {
        public string Name { get; set; }

        public MfplTypes[] ArgumentTypes { get; set; }

        public MfplFunctionDefAttribute(string name, MfplTypes[] argumentTypes)
        {
            Name = name;
            ArgumentTypes = argumentTypes;
        }
    }
}
