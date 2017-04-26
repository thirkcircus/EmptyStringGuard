using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using EmptyStringGuard;

public class MethodProcessor
{
    private const string IsEmptyString = "[EmptyStringGuard] {0} is an empty string.";
    private readonly ValidationFlags _validationFlags;

    public MethodProcessor(ValidationFlags validationFlags)
    {
        _validationFlags = validationFlags;
    }

    public void Process(MethodDefinition method)
    {
        try
        {
            if (method.IsGeneratedCode())
            {
                return;
            }
            InnerProcess(method);
        }
        catch (Exception exception)
        {
            throw new WeavingException($"An error occurred processing method '{method.FullName}'.", exception);
        }
    }

    private void InnerProcess(MethodDefinition method)
    {
        var localValidationFlags = _validationFlags;

        var attribute = method.DeclaringType.GetEmptyStringGuardAttribute();
        if (attribute != null)
        {
            localValidationFlags = (ValidationFlags)attribute.ConstructorArguments[0].Value;
        }

        if ((!localValidationFlags.HasFlag(ValidationFlags.NonPublic) && (!method.IsPublic || !method.DeclaringType.IsPublic))
            || method.IsProperty()
            )
            return;

        var body = method.Body;
        body.SimplifyMacros();

        if (localValidationFlags.HasFlag(ValidationFlags.Methods))
        {
            InjectMethodArgumentGuards(method, body);
        }

        body.InitLocals = true;
        body.OptimizeMacros();
    }

    private void InjectMethodArgumentGuards(MethodDefinition method, MethodBody body)
    {
        var guardInstructions = new List<Instruction>();

        foreach (var parameter in method.Parameters.Reverse()) {
            if (parameter.ParameterType.FullName != "System.String")
                continue;

            if (!parameter.MayNotBeEmptyString())
                continue;

            if (CheckForExistingGuard(body.Instructions, parameter))
                continue;

            var entry = body.Instructions.First();

            guardInstructions.Clear();

            InstructionPatterns.LoadArgumentOntoStack(guardInstructions, parameter);
            
            InstructionPatterns.IfEmptyString(guardInstructions, entry, i =>
            {
                InstructionPatterns.LoadArgumentException(i, String.Format(CultureInfo.InvariantCulture, IsEmptyString, parameter.Name), parameter.Name);

                // Throw the top item off the stack
                i.Add(Instruction.Create(OpCodes.Throw));
            });

            body.Instructions.Prepend(guardInstructions);
        }
    }

    private static bool CheckForExistingGuard(Collection<Instruction> instructions, ParameterDefinition parameter)
    {
        for (var i = 1; i < instructions.Count - 1; i++)
        {
            if (instructions[i].OpCode == OpCodes.Newobj)
            {
                var newObjectMethodRef = instructions[i].Operand as MethodReference;

                if (newObjectMethodRef == null || instructions[i + 1].OpCode != OpCodes.Throw)
                    continue;

                // Checks for throw new ArgumentException("x");
                if (newObjectMethodRef.FullName == ReferenceFinder.ArgumentExceptionConstructor.FullName &&
                    instructions[i - 1].OpCode == OpCodes.Ldstr &&
                    (string)(instructions[i - 1].Operand) == parameter.Name)
                    return true;
            }
        }

        return false;
    }
}