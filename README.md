# Kull.Data Database Access

This Library extends the .Net Standard DBA Classes like System.Data.Common.DbConnection or System.Data.Common.DbDataReader
with some extension methods that makes them easiert to use. You can use this a very simple ORM Mapper as well, without 
the need of a full-bown Entity Framework. A good alternative is [Dapper](https://github.com/StackExchange/Dapper). 

## Some Examples

You always need to be `using Kull.Data` to use the extension methods.

### Call a Stored Procedure

```C#
using (var con = Kull.Data.DatabaseUtils.GetConnectionFromConfig("SomeConfigConnstr"))
{
    return con.CreateSPCommand("spGetSomeData")
        .AddCommandParameter("NameOfParameter", 1)
        .AddCommandParameter("NameofOtherPArameter", 2)
        .AsArrayOf<SomeClassName>()
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


## Other Feature: WrapperDataReader and ObjectDataReader

For [SQL Bulk Copy](https://docs.microsoft.com/de-de/dotnet/api/system.data.sqlclient.sqlbulkcopy?view=netframework-4.7.2) or other things it can
be useful to pass C# Data as a DataReader. You can achieve that by using `Kull.Data.DataReader.ObjectDataReader`. If you need to add some columns 
to a datareader, you can use `Kull.Data.DataReader.WrappedDataReader`.