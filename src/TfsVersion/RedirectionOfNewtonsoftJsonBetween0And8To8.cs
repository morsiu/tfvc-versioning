using System;
using System.Collections;
using System.Reflection;

namespace TfsVersion
{
    /* This is done to workaround following error, when the project is built from within Visual Studio 2017:
        The "TfsVersion" task failed unexpectedly.
        System.IO.FileNotFoundException: Could not load file or assembly 'Newtonsoft.Json, Version=6.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed' or one of its dependencies. Nie można odnaleźć określonego pliku.
        File name: 'Newtonsoft.Json, Version=6.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed'
            at Microsoft.VisualStudio.Services.WebApi.VssJsonMediaTypeFormatter..ctor(Boolean bypassSafeArrayWrapping)
            at Microsoft.VisualStudio.Services.WebApi.VssHttpClientBase..ctor(Uri baseUrl, HttpMessageHandler pipeline, Boolean disposeHandler)
            at Microsoft.TeamFoundation.SourceControl.WebApi.TfvcHttpClient..ctor(Uri baseUrl, VssCredentials credentials)
            at TfsVersion.TfsVersion.Execute() in C:\Users\morsk\Documents\Projects\msbuild\tfsversion\src\TfsVersion\TfsVersion.cs:line 14
            at Microsoft.Build.BackEnd.TaskExecutionHost.Microsoft.Build.BackEnd.ITaskExecutionHost.Execute()
            at Microsoft.Build.BackEnd.TaskBuilder.<ExecuteInstantiatedTask>d__26.MoveNext()

        === Pre-bind state information ===
        LOG: DisplayName = Newtonsoft.Json, Version=6.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
         (Fully-specified)
        LOG: Appbase = file:///C:/Program Files (x86)/Microsoft Visual Studio/2017/Community/MSBuild/15.0/Bin/
        LOG: Initial PrivatePath = NULL
        Calling assembly : System.Net.Http.Formatting, Version=5.2.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35.
        ===
        LOG: This bind starts in LoadFrom load context.
        WRN: Native image will not be probed in LoadFrom context. Native image will only be probed in default load context, like with Assembly.Load().
        LOG: Using application configuration file: C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe.Config
        LOG: Using host configuration file: 
        LOG: Using machine configuration file from C:\Windows\Microsoft.NET\Framework\v4.0.30319\config\machine.config.
        LOG: Post-policy reference: Newtonsoft.Json, Version=6.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
        LOG: Attempting download of new URL file:///C:/Program Files (x86)/Microsoft Visual Studio/2017/Community/MSBuild/15.0/Bin/Newtonsoft.Json.DLL.
        LOG: Attempting download of new URL file:///C:/Program Files (x86)/Microsoft Visual Studio/2017/Community/MSBuild/15.0/Bin/Newtonsoft.Json/Newtonsoft.Json.DLL.
        LOG: Attempting download of new URL file:///C:/Program Files (x86)/Microsoft Visual Studio/2017/Community/MSBuild/15.0/Bin/Newtonsoft.Json.EXE.
        LOG: Attempting download of new URL file:///C:/Program Files (x86)/Microsoft Visual Studio/2017/Community/MSBuild/15.0/Bin/Newtonsoft.Json/Newtonsoft.Json.EXE.
        LOG: Attempting download of new URL file:///C:/Users/morsk/Documents/Projects/msbuild/tfsversion/src/TfsVersion/bin/Debug/Newtonsoft.Json.DLL.
        WRN: Comparing the assembly name resulted in the mismatch: Major Version
        LOG: Attempting download of new URL file:///C:/Users/morsk/Documents/Projects/msbuild/tfsversion/src/TfsVersion/bin/Debug/Newtonsoft.Json/Newtonsoft.Json.DLL.
        LOG: Attempting download of new URL file:///C:/Users/morsk/Documents/Projects/msbuild/tfsversion/src/TfsVersion/bin/Debug/Newtonsoft.Json.EXE.
        LOG: Attempting download of new URL file:///C:/Users/morsk/Documents/Projects/msbuild/tfsversion/src/TfsVersion/bin/Debug/Newtonsoft.Json/Newtonsoft.Json.EXE
     */
    internal sealed class RedirectionOfNewtonsoftJsonBetween0And8To8
    {
        static RedirectionOfNewtonsoftJsonBetween0And8To8()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            if (assemblyName.Name == "Newtonsoft.Json" &&
                assemblyName.Version < new Version(8, 0, 0) &&
                StructuralComparisons.StructuralEqualityComparer.Equals(
                    assemblyName.GetPublicKeyToken(),
                    new byte[] { 0x30, 0xad, 0x4f, 0xe6, 0xb2, 0xa6, 0xae, 0xed }))
            {
                assemblyName.Version = new Version(8, 0);
                return Assembly.Load(assemblyName);
            }
            return null;
        }
    }
}
