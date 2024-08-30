namespace sameplens;

public class PipeSecurityProxy
{
    private static readonly Type __generated_type = Type.GetType("System.IO.Pipes.PipeSecurity, System.Core");
    private readonly object __generated_instance;

    public PipeSecurityProxy()
    {
        __generated_instance = global::System.Activator.CreateInstance(__generated_type);
    }

    public PipeSecurityProxy(Microsoft.Win32.SafeHandles.SafePipeHandle safeHandle, System.Security.AccessControl.AccessControlSections includeSections)
    {
        __generated_instance = global::System.Activator.CreateInstance(__generated_type, safeHandle, includeSections);
    }

    public void SetAccessRule(System.IO.Pipes.PipeAccessRule rule)
    {
        var __generated_method = __generated_type.GetMethod("SetAccessRule", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod, [global::System.Type.GetType("System.IO.Pipes.PipeAccessRule")]);
        __generated_method.Invoke(__generated_instance, [rule]);
    }

    public void SetAccessRule(System.Security.AccessControl.AccessRule rule)
    {
        var __generated_method = __generated_type.GetMethod("SetAccessRule", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod, [global::System.Type.GetType("System.Security.AccessControl.AccessRule")]);
        __generated_method.Invoke(__generated_instance, [rule]);
    }
}

