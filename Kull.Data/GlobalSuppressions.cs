
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "Code is meant to run on a server, so no changing culture")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Code is meant to run on a server, so no changing culture")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "Code is meant to run on a server, so no changing culture")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "It's ok if they are readonly")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "Would be breaking")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations.", Justification = "Cannot be used with net35 and net45")]

