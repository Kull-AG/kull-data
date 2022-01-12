# Kull.Data Database Access

This Library extends the .Net Standard DBA Classes like System.Data.Common.DbConnection or System.Data.Common.DbDataReader
with some extension methods that makes them easiert to use. You can use this a very simple ORM Mapper as well, without 
the need of a full-bown Entity Framework. A good alternative is [Dapper](https://github.com/StackExchange/Dapper). 

Install it using Nuget, Package "Kull.Data":

[![NuGet Badge](https://buildstats.info/nuget/Kull.Data)](https://www.nuget.org/packages/Kull.Data/)

## Some Examples

You always need to be `using Kull.Data` to use the extension methods.

### Call a Stored Procedure

```C#
using (var con = Kull.Data.DatabaseUtils.GetConnectionFromConfig("SomeConfigConnstr"))
{
    return con.CreateSPCommand("spGetSomeData")
        .AddCommandParameter("NameOfParameter", 1)
        .AddCommandParameter("NameofOtherPArameter", 2)
        .AsCollectionOf<SomeClassName>() // As of v6, a C# 9 record can be used here
}
```

### Call a Stored Procedure, but use default DataReader

```C#
System.Data.Common.DbCommand cmd = con.CreateSPCommand("spGetSomeData")
                    .AddCommandParameter("NameOfParameter", 1)
                    .AddCommandParameter("NameofOtherPArameter", 2);

using(var rdr = cmd.ExecuteReader())
{
    rdr.Read();
    return rdr.GetNInt16(2); // Use Kull.Data Extension method that handles null-values for you (No System.DBNull Checking anymore...)
}
```

## Abstraction of Config 

You can simple call `Kull.Data.DatabaseUtils.GetConnectionFromConfig("NAMEOFCONNECTIONSTRING")` and it will return your DbConnection. 

It works in the following order:

 - Look in System.Configuration.ConfigurationManager.ConnectionStrings (.Net Fx only)
 - Look in System.Configuration.ConfigurationManager.AppSettings (.Net Fx only)
 - Look in appsettings.json (.Net Core only)
 - Look in Environment Variables

When looking into Environment Variables, the ones of [Azure functions](https://azure.microsoft.com/en-us/blog/windows-azure-web-sites-how-application-strings-and-connection-strings-work/) are the base.
We support the following prefixes:

 - No prefix, meaning just NAMEOFCONNECTIONSTRING in the example above 
 - SQLCONNSTR_
 - SQLAZURECONNSTR_
 - MYSQLCONNSTR_
 - PostgreSQLCONNSTR_
 - CUSTOMCONNSTR_ 


## Other Feature: WrapperDataReader and ObjectDataReader

For [SQL Bulk Copy](https://docs.microsoft.com/de-de/dotnet/api/system.data.sqlclient.sqlbulkcopy?view=netframework-4.7.2) or other things it can
be useful to pass C# Data as a DataReader. You can achieve that by using `Kull.Data.DataReader.ObjectDataReader`. If you need to add some columns 
to a datareader, you can use `Kull.Data.DataReader.WrappedDataReader`.


# Powershell Module

You can use the Kull.Data.Powershell Module to use some simple cmdlets that allow for querying stored procedures(queries with parameters easily.

```powershell
Import-Module SqlServer # To load System.Data.SqlClient driver in Powershell 6.2+
Import-Module Kull.Data.Powershell 
$allAssemblies = [appdomain]::currentdomain.GetAssemblies() | ForEach-Object { [IO.Path]::GetFileName($_.Location) } # we try to be ready when SqlServer Module uses Microsoft.Data.SqlClient
if($allAssemblies.Contains("System.Data.SqlClient.dll") -and -not $allAssemblies.Contains("Microsoft.Data.SqlClient.dll")){
    $provider = "System.Data.SqlClient"
} else {
    $provider = "Microsoft.Data.SqlClient"
}
if($firstLoad -and $provider -eq "System.Data.SqlClient"){
    [System.Data.Common.DbProviderFactories]::RegisterFactory($provider, [System.Data.SqlClient.SqlClientFactory]::Instance)
}
else {
    [System.Data.Common.DbProviderFactories]::RegisterFactory($provider, [Microsoft.Data.SqlClient.SqlClientFactory]::Instance)
}
$sqlcon = Connect-Database $connectionstring $provider
Send-DbCommand $sqlcon $commandText $parameters ([System.Data.CommandType]::StoredProcedure)
# Or, use Receive-DbData to get result list
```