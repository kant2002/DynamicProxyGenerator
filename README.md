# DynamicProxyGenerator

Generates dynamically loaded proxy for types from external assemblies.

Command line arguments
```
dotnet run -- --assembly C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Core\v4.0_4.0.0.0__b77a5c561934e089\System.Core.dll --type System.IO.Pipes.PipeSecurity --namespace sameplens --name PipeSecurityProxy --methods AddAccessRule --methods SetAccessRule
```