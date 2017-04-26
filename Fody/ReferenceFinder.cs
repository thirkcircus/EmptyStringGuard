using System.Linq;
using Mono.Cecil;

public static class ReferenceFinder
{
    public static MethodReference ArgumentExceptionConstructor;
    public static MethodReference StringEqualityMethod;
    public static FieldReference EmptyStringField;
    public static MethodReference DebugAssertMethod;

    public static void FindReferences(IAssemblyResolver assemblyResolver, ModuleDefinition moduleDefinition)
    {
        var baseLib = assemblyResolver.Resolve(new AssemblyNameReference("mscorlib", null));
        var baseLibTypes = baseLib.MainModule.Types;

        var winrt = baseLibTypes.All(type => type.Name != "Object");
        if (winrt)
        {
            baseLib = assemblyResolver.Resolve(new AssemblyNameReference("System.Runtime",null));
            baseLibTypes = baseLib.MainModule.Types;
        }

        var argumentException = baseLibTypes.First(x => x.Name == "ArgumentException");
        ArgumentExceptionConstructor = moduleDefinition.ImportReference(argumentException.Methods.First(x =>
            x.IsConstructor &&
            x.Parameters.Count == 2 &&
            x.Parameters[0].ParameterType.Name == "String" &&
            x.Parameters[1].ParameterType.Name == "String"));

        var debugLib = !winrt ? assemblyResolver.Resolve(new AssemblyNameReference("System", null)) : assemblyResolver.Resolve(new AssemblyNameReference("System.Diagnostics.Debug", null));
        var debugLibTypes = debugLib.MainModule.Types;

        var debug = debugLibTypes.First(x => x.Name == "Debug");
        DebugAssertMethod = moduleDefinition.ImportReference(debug.Methods.First(x =>
            x.IsStatic &&
            x.Parameters.Count == 2 &&
            x.Parameters[0].ParameterType.Name == "Boolean" &&
            x.Parameters[1].ParameterType.Name == "String"));

        var stringTypeDefinition = moduleDefinition.TypeSystem.String.Resolve();
        StringEqualityMethod = moduleDefinition.ImportReference(stringTypeDefinition.Methods.First(x => x.Name == "op_Equality"));
        EmptyStringField = moduleDefinition.ImportReference(stringTypeDefinition.Fields.First(x => x.Name == "Empty"));
    }
}