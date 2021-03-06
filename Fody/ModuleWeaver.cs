﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using EmptyStringGuard;

public class ModuleWeaver
{
    public ValidationFlags ValidationFlags { get; set; }
    public List<string> DefineConstants { get; set; }
    public Action<string> LogInfo { get; set; }
    public Action<string> LogError { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogError = s => { };
        ValidationFlags = ValidationFlags.AllPublic;
        DefineConstants = new List<string>();
    }

    public void Execute()
    {
        var emptyStringGuardAttribute = ModuleDefinition.GetEmptyStringGuardAttribute() ?? ModuleDefinition.Assembly.GetEmptyStringGuardAttribute();

        if (emptyStringGuardAttribute != null) {
            ValidationFlags = (ValidationFlags) emptyStringGuardAttribute.ConstructorArguments[0].Value;
        }

        ReferenceFinder.FindReferences(AssemblyResolver, ModuleDefinition);
        var types = new List<TypeDefinition>(ModuleDefinition.GetTypes());

        CheckForBadAttributes(types);
        ProcessAssembly(types);
        RemoveAttributes(types);
        RemoveReference();
    }

    private void CheckForBadAttributes(List<TypeDefinition> types)
    {
        foreach (var typeDefinition in types)
        {
            foreach (var method in typeDefinition.AbstractMethods())
            {
                if (method.ContainsAllowEmptyAttribute())
                {
                    LogError($"Method '{method.FullName}' is abstract but has an [AllowEmptyAttribute]. Remove this attribute.");
                }
                foreach (var parameter in method.Parameters)
                {
                    if (parameter.ContainsAllowEmptyAttribute())
                    {
                        LogError($"Method '{method.FullName}' is abstract but has an [AllowEmptyAttribute]. Remove this attribute.");
                    }
                }
            }
        }
    }

    private void ProcessAssembly(List<TypeDefinition> types)
    {
        var methodProcessor = new MethodProcessor(ValidationFlags);
        var propertyProcessor = new PropertyProcessor(ValidationFlags);

        foreach (var type in types)
        {
            if (type.IsInterface || type.ContainsAllowEmptyAttribute() || type.IsGeneratedCode() || type.HasInterface("Windows.UI.Xaml.Markup.IXamlMetadataProvider"))
                continue;

            foreach (var method in type.MethodsWithBody())
                methodProcessor.Process(method);

            foreach (var property in type.ConcreteProperties())
                propertyProcessor.Process(property);
        }
    }

    private void RemoveAttributes(List<TypeDefinition> types)
    {
        ModuleDefinition.Assembly.RemoveAllEmptyStringGuardAttributes();
        ModuleDefinition.RemoveAllEmptyStringGuardAttributes();
        foreach (var typeDefinition in types)
        {
            typeDefinition.RemoveAllEmptyStringGuardAttributes();

            foreach (var method in typeDefinition.Methods)
            {
                method.MethodReturnType.RemoveAllEmptyStringGuardAttributes();

                foreach (var parameter in method.Parameters)
                {
                    parameter.RemoveAllEmptyStringGuardAttributes();
                }
            }

            foreach (var property in typeDefinition.Properties)
            {
                property.RemoveAllEmptyStringGuardAttributes();
            }
        }
    }

    private void RemoveReference()
    {
        var referenceToRemove = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "EmptyStringGuard");
        if (referenceToRemove == null)
        {
            LogInfo("\tNo reference to 'EmptyStringGuard.dll' found. References not modified.");
            return;
        }

        ModuleDefinition.AssemblyReferences.Remove(referenceToRemove);
        LogInfo("\tRemoving reference to 'EmptyStringGuard.dll'.");
    }
}