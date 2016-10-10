using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace MFPL.Compiler.Core
{
    public struct Instruction
    {
        public OpCode OpCode { get; }

        public InstructionType InstructionType { get; }

        public object Value { get; }

        private Instruction(OpCode opCode, object value, InstructionType instructionType)
        {
            OpCode = opCode;
            Value = value;
            InstructionType = instructionType;
        }

        public void Emit(ILGenerator il)
        {
            switch (InstructionType)
            {
                case InstructionType.Void:
                    il.Emit(OpCode);
                    break;
                case InstructionType.Type:
                    il.Emit(OpCode, (Type)Value);
                    break;
                case InstructionType.String:
                    il.Emit(OpCode, (string)Value);
                    break;
                case InstructionType.Float:
                    il.Emit(OpCode, (float)Value);
                    break;
                case InstructionType.SByte:
                    il.Emit(OpCode, (sbyte)Value);
                    break;
                case InstructionType.MethodInfo:
                    il.Emit(OpCode, (MethodInfo)Value);
                    break;
                case InstructionType.FieldInfo:
                    il.Emit(OpCode, (FieldInfo)Value);
                    break;
                case InstructionType.SignatureHelper:
                    il.Emit(OpCode, (SignatureHelper)Value);
                    break;
                case InstructionType.LocalBuilder:
                    il.Emit(OpCode, (LocalBuilder)Value);
                    break;
                case InstructionType.Label:
                    il.Emit(OpCode, (Label)Value);
                    break;
                case InstructionType.ConstructorInfo:
                    il.Emit(OpCode, (ConstructorInfo)Value);
                    break;
                case InstructionType.Long:
                    il.Emit(OpCode, (long)Value);
                    break;
                case InstructionType.Int:
                    il.Emit(OpCode, (int)Value);
                    break;
                case InstructionType.Short:
                    il.Emit(OpCode, (short)Value);
                    break;
                case InstructionType.Double:
                    il.Emit(OpCode, (double)Value);
                    break;
                case InstructionType.Byte:
                    il.Emit(OpCode, (byte)Value);
                    break;
                case InstructionType.Labels:
                    il.Emit(OpCode, (Label[])Value);
                    break;
            }
        }

        public static Instruction Create(OpCode opCode) => new Instruction(opCode, null, InstructionType.Void);

        public static Instruction Create(OpCode opCode, Type value) 
            => new Instruction(opCode, value, InstructionType.Type);

        public static Instruction Create(OpCode opCode, String value) 
            => new Instruction(opCode, value, InstructionType.String);

        public static Instruction Create(OpCode opCode, float value) 
            => new Instruction(opCode, value, InstructionType.Float);

        public static Instruction Create(OpCode opCode, SByte value) 
            => new Instruction(opCode, value, InstructionType.SByte);

        public static Instruction Create(OpCode opCode, MethodInfo value) 
            => new Instruction(opCode, value, InstructionType.MethodInfo);

        public static Instruction Create(OpCode opCode, FieldInfo value) 
            => new Instruction(opCode, value, InstructionType.FieldInfo);

        public static Instruction Create(OpCode opCode, SignatureHelper value) 
            => new Instruction(opCode, value, InstructionType.SignatureHelper);

        public static Instruction Create(OpCode opCode, LocalBuilder value) 
            => new Instruction(opCode, value, InstructionType.LocalBuilder);

        public static Instruction Create(OpCode opCode, Label value) 
            => new Instruction(opCode, value, InstructionType.Label);

        public static Instruction Create(OpCode opCode, ConstructorInfo value) 
            => new Instruction(opCode, value, InstructionType.ConstructorInfo);

        public static Instruction Create(OpCode opCode, long value) 
            => new Instruction(opCode, value, InstructionType.Long);

        public static Instruction Create(OpCode opCode, int value) 
            => new Instruction(opCode, value, InstructionType.Int);

        public static Instruction Create(OpCode opCode, short value) 
            => new Instruction(opCode, value, InstructionType.Short);

        public static Instruction Create(OpCode opCode, Double value) 
            => new Instruction(opCode, value, InstructionType.Double);

        public static Instruction Create(OpCode opCode, Byte value) 
            => new Instruction(opCode, value, InstructionType.Byte);

        public static Instruction Create(OpCode opCode, Label[] value) 
            => new Instruction(opCode, value, InstructionType.Labels);
    }

    public enum InstructionType
    {
        Void,
        Type,
        String,
        Float,
        SByte,
        MethodInfo,
        FieldInfo,
        SignatureHelper,
        LocalBuilder,
        Label,
        ConstructorInfo,
        Long,
        Int,
        Short,
        Double,
        Byte,
        Labels, 
    }
}
