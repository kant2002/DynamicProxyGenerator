open Argu
open System.Linq

type Arguments =
    | Assembly of path: string
    | Type of fullName: string
    | Namespace of name: string
    | Name of name: string
    | Methods of methods: string list

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Assembly _ -> "specify a path to assembly where get type."
            | Type _ -> "specify full name to the type."
            | Namespace _ -> "specify namespace for the generated proxy."
            | Name _ -> "specify type name for the generated proxy."
            | Methods _ -> "Methods to proxy."

let reader = EnvironmentVariableConfigurationReader() :> IConfigurationReader
let parser = ArgumentParser.Create<Arguments>(programName = "dynproxygen")

let csharpType (typeInfo: System.Type) =
    if typeInfo.FullName = "System.Void" then "void"
    else typeInfo.FullName

[<EntryPoint>]
let main argv =
    let results = parser.Parse argv
    let assemmblyPath = results.GetResult Assembly
    let typeName = results.GetResult Type
    let asm = System.Reflection.Assembly.LoadFile(assemmblyPath)
    let methods = results.GetResult Methods
    let implType = asm.GetType(typeName)

    let searchFlag = System.Reflection.BindingFlags.Static + System.Reflection.BindingFlags.Instance + System.Reflection.BindingFlags.Public + System.Reflection.BindingFlags.NonPublic
    let implMethods =  implType.GetMembers searchFlag
    let implMethods2 =
        implMethods
            |> Seq.filter (fun method -> methods |> Seq.contains method.Name)
            |> Seq.cast<System.Reflection.MethodInfo>
    let implCtors =
        implMethods
            |> Seq.filter (fun method -> method.Name = ".ctor")
            |> Seq.cast<System.Reflection.ConstructorInfo>

    let nsName = results.GetResult Namespace
    let name = results.GetResult Name
    let assemblyName = System.IO.Path.GetFileNameWithoutExtension assemmblyPath
    printfn "namespace %s;" nsName
    printfn ""
    printfn "public class %s" name
    printfn "{"
    printfn "    private static readonly Type __generated_type = Type.GetType(\"%s, %s\");" typeName assemblyName
    printfn "    private readonly object __generated_instance;"
    for ctor in implCtors do
        let ctorParams = ctor.GetParameters() |> Seq.map (fun p -> sprintf "%s %s" p.ParameterType.FullName p.Name) |> String.concat ", "
        let ctorParamNames = ctor.GetParameters() |> Seq.map (fun p -> sprintf "%s" p.Name) |> String.concat ", "
        printfn ""
        printfn "    public %s(%s)" name ctorParams
        printfn "    {"
        if ctorParamNames.Length = 0 then
            printfn "        __generated_instance = global::System.Activator.CreateInstance(__generated_type)"
        else
            printfn "        __generated_instance = global::System.Activator.CreateInstance(__generated_type, %s)" ctorParamNames

        printfn "    }"
    for meth in implMethods2 do
        let methParams = meth.GetParameters() |> Seq.map (fun p -> sprintf "%s %s" p.ParameterType.FullName p.Name) |> String.concat ", "
        let methParamNames = meth.GetParameters() |> Seq.map (fun p -> sprintf "%s" p.Name) |> String.concat ", "
        let retType = csharpType meth.ReturnType
        printfn ""
        printfn "    public %s %s(%s)" retType meth.Name methParams
        printfn "    {"
        printfn "        var __generated_method = _type.GetMethod(\"%s\", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod);" meth.Name
        if methParamNames.Length = 0 then
            printfn "        var __generated_result = __generated_method.Invoke(__generated_instance);"
        else
            printfn "        var __generated_result = __generated_method.Invoke(__generated_instance, %s);" methParamNames

        printfn "        return __generated_result as %s;" retType
        printfn "    }"
    printfn "}"
    printfn ""
    0
