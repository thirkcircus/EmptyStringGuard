using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public static class InstructionPatterns
{
    public static void CallDebugAssertInstructions(List<Instruction> instructions, string message)
    {
        // Load empty string onto the stack
        instructions.Add(Instruction.Create(OpCodes.Ldsfld, ReferenceFinder.EmptyStringField));

        // Compare the top 2 items on the stack, and put the result back on the stack
        instructions.Add(Instruction.Create(OpCodes.Ceq));

        // Loads constant int32 0 onto the stack
        instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));

        // Compare the top 2 items on the stack, and put the result back on the stack
        instructions.Add(Instruction.Create(OpCodes.Ceq));

        // Load assert message onto the stack
        instructions.Add(Instruction.Create(OpCodes.Ldstr, message));

        // Call Debug.Assert
        instructions.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.DebugAssertMethod));
    }

    public static void DuplicateReturnValue(List<Instruction> instructions, TypeReference methodReturnType)
    {
        // Duplicate the stack (this should be the return value)
        instructions.Add(Instruction.Create(OpCodes.Dup));

        if (methodReturnType != null && methodReturnType.GetElementType().IsGenericParameter)
        {
            // Generic parameters must be boxed before access
            instructions.Add(Instruction.Create(OpCodes.Box, methodReturnType));
        }
    }

    public static void LoadArgumentOntoStack(List<Instruction> instructions, ParameterDefinition parameter)
    {
        // Load the argument onto the stack
        instructions.Add(Instruction.Create(OpCodes.Ldarg, parameter));

        var elementType = parameter.ParameterType.GetElementType();

        if (parameter.ParameterType.IsByReference)
        {
            if (elementType.IsGenericParameter)
            {
                // Loads an object reference onto the stack
                instructions.Add(Instruction.Create(OpCodes.Ldobj, elementType));

                // Box the type to an object
                instructions.Add(Instruction.Create(OpCodes.Box, elementType));
            }
            else
            {
                // Loads an object reference onto the stack
                instructions.Add(Instruction.Create(OpCodes.Ldind_Ref));
            }
        }
        else if (elementType.IsGenericParameter)
        {
            // Box the type to an object
            instructions.Add(Instruction.Create(OpCodes.Box, parameter.ParameterType));
        }
    }

    public static void LoadArgumentException(List<Instruction> instructions, string valueName, string errorString)
    {
        // Load the name of the argument onto the stack
        instructions.Add(Instruction.Create(OpCodes.Ldstr, valueName));

        // Load the exception text onto the stack
        instructions.Add(Instruction.Create(OpCodes.Ldstr, errorString));

        // Load the ArgumentException onto the stack
        instructions.Add(Instruction.Create(OpCodes.Newobj, ReferenceFinder.ArgumentExceptionConstructor));
    }

    public static void IfNull(List<Instruction> instructions, Instruction returnInstruction, Action<List<Instruction>> trueBlock)
    {
        /*
.method public hidebysig instance void  DoIt2(string blah) cil managed
{
  // Code size       20 (0x14)
  .maxstack  8
  IL_0000:  ldarg.1
  IL_0001:  brtrue.s   IL_0013
  IL_0003:  ldstr      "blah"
  IL_0008:  ldstr      "cannot accept empty string"
  IL_000d:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string,
                                                                                   string)
  IL_0012:  throw
  IL_0013:  ret
} // end of method Class1::DoIt2
         */

        // Branch if value on stack is true, not null or non-zero
        instructions.Add(Instruction.Create(OpCodes.Brtrue_S, returnInstruction));

        trueBlock(instructions);
    }

    public static void IfEmptyString(List<Instruction> instructions, Instruction returnInstruction, Action<List<Instruction>> trueBlock)
    {
        /* 
.method public hidebysig instance void  DoIt(string blah) cil managed
{
  // Code size       30 (0x1e)
  .maxstack  8
  IL_0000:  ldsfld     string [mscorlib]System.String::Empty
  IL_0005:  ldarg.1
  IL_0006:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                 string)
  IL_000b:  brfalse.s  IL_001d
  IL_000d:  ldstr      "cannot accept empty string"
  IL_0012:  ldstr      "blah"
  IL_0017:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string,
                                                                               string)
  IL_001c:  throw
  IL_001d:  ret
} // end of method Class1::DoIt
         */

        instructions.Add(Instruction.Create(OpCodes.Ldsfld, ReferenceFinder.EmptyStringField));
        instructions.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.StringEqualityMethod));

        // Branch if value on stack is false, null or zero
        instructions.Add(Instruction.Create(OpCodes.Brfalse_S, returnInstruction));

        trueBlock(instructions);
    }
}