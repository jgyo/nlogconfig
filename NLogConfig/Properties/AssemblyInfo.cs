using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("NLogConfig")]
#if DEBUG

[assembly:
 AssemblyDescription("This assembly is debug code and should not be used in production.")]
[assembly: AssemblyConfiguration("Debug")]

#else

[assembly:
 AssemblyDescription("Helper utilities to facilitate programmatic NLog configuration.")]
[assembly: AssemblyConfiguration("Release")]

#endif

[assembly: AssemblyCompany("Yoder Zone")]
[assembly: AssemblyProduct("NLogConfig")]
[assembly: AssemblyCopyright("Copyright © Gil Yoder 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("9341caac-2071-4c35-bcae-b988babb8ff3")]

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
[assembly: AssemblyVersion("1.1.5386.1")]
[assembly: AssemblyFileVersion("1.1.14273.0050")]
[assembly: AssemblyInformationalVersion("2014.9.30.0050")]