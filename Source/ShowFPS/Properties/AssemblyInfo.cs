using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ShowFPS /L Unleashed")]
[assembly: AssemblyDescription("Parking vessels in unusual places and situations on KSP")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(ShowFPS.LegalMamboJambo.Company)]
[assembly: AssemblyProduct(ShowFPS.LegalMamboJambo.Product)]
[assembly: AssemblyCopyright(ShowFPS.LegalMamboJambo.Copyright)]
[assembly: AssemblyTrademark(ShowFPS.LegalMamboJambo.Trademark)]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("e91b1460-61e0-4c7e-be6d-2abc51d3c14e")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(ShowFPS.Version.Number)]
[assembly: AssemblyFileVersion(ShowFPS.Version.Number)]
[assembly: KSPAssembly("ShowFPS", ShowFPS.Version.major, ShowFPS.Version.minor)]

[assembly: KSPAssemblyDependency("KSPe", 2, 5)]
[assembly: KSPAssemblyDependency("KSPe.UI", 2, 5)]
