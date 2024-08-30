open Argu
open System

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

let isVoid (typeInfo: System.Type) =
    typeInfo.FullName = "System.Void"

let csharpType typeInfo =
    if isVoid typeInfo then "void"
    else typeInfo.FullName

let parameterInfo (p: Reflection.ParameterInfo) =
    let out = if p.IsOut then "out " else ""
    sprintf "%s%s %s" out p.ParameterType.FullName p.Name

[<EntryPoint>]
let main argv =
    let results = parser.Parse argv
    let assemmblyPath = results.GetResult Assembly
    let typeName = results.GetResult Type
    System.AppDomain.CurrentDomain.BaseDirectory = IO.Path.GetDirectoryName(assemmblyPath) |> ignore
    let asm = System.Reflection.Assembly.LoadFrom(assemmblyPath)
    let methods = results.GetResult Methods
    let implType = asm.GetType(typeName)

    let searchFlag = System.Reflection.BindingFlags.Static + System.Reflection.BindingFlags.Instance + System.Reflection.BindingFlags.Public + System.Reflection.BindingFlags.NonPublic
    let implMethods =  implType.GetMembers searchFlag
    let implMethods2 =
        implMethods
            |> Seq.filter (fun method -> methods |> Seq.contains method.Name)
            |> Seq.cast<System.Reflection.MethodInfo>
            |> Seq.toArray
    let implCtors =
        implMethods
            |> Seq.filter (fun method -> method.Name = ".ctor")
            |> Seq.cast<System.Reflection.ConstructorInfo>
            |> Seq.toArray

    let nsName = results.GetResult Namespace
    let name = results.GetResult Name
    let assemblyName = System.IO.Path.GetFileNameWithoutExtension assemmblyPath
    printfn "namespace %s;" nsName
    printfn ""
    printfn "public class %s" name
    printfn "{"
    printfn "    private static readonly Type __generated_type = Type.GetType(\"%s, %s\");" typeName assemblyName
    if implCtors.Length > 0 then
        printfn "    private readonly object __generated_instance;"
    for ctor in implCtors do
        let ctorParams = ctor.GetParameters() |> Seq.map (fun p -> sprintf "%s %s" p.ParameterType.FullName p.Name) |> String.concat ", "
        let ctorParamNames = ctor.GetParameters() |> Seq.map (fun p -> sprintf "%s" p.Name) |> String.concat ", "
        printfn ""
        printfn "    public %s(%s)" name ctorParams
        printfn "    {"
        if ctorParamNames.Length = 0 then
            printfn "        __generated_instance = global::System.Activator.CreateInstance(__generated_type);"
        else
            printfn "        __generated_instance = global::System.Activator.CreateInstance(__generated_type, %s);" ctorParamNames

        printfn "    }"
    for meth in implMethods2 do
        let methParams = meth.GetParameters() |> Seq.map parameterInfo |> String.concat ", "
        let methParamNames = meth.GetParameters() |> Seq.map (fun p -> sprintf "%s" p.Name) |> String.concat ", "
        let retType = csharpType meth.ReturnType
        printfn ""
        printfn "    public %s %s(%s)" retType meth.Name methParams
        printfn "    {"
        printfn "        var __generated_method = __generated_type.GetMethod(\"%s\", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod);" meth.Name
        let saveResult = if isVoid meth.ReturnType then "" else "var __generated_result = "
        let instance = if meth.IsStatic then "null" else "__generated_instance"
        if methParamNames.Length = 0 then
            printfn "        %s__generated_method.Invoke(%s);" saveResult instance
        else
            printfn "        %s__generated_method.Invoke(%s, %s);" saveResult instance methParamNames

        if not (isVoid meth.ReturnType) then
            printfn "        return __generated_result as %s;" retType
        printfn "    }"
    printfn "}"
    printfn ""
    0
