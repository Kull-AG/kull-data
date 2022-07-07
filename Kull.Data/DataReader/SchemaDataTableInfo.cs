using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kull.Data.DataReader
{
    public record SchemaDataTableInfo(string ColumnName, 
        int ColumnOrdinal, int ColumnSize, int? NumericPrecision, int? NumericScale,
        string DataType,
        object ProviderType ,
        bool IsLong=true,
        bool AllowDBNull=true,
        bool IsReadOnly=false,
        bool IsUnique = false,
        bool IsKey = true,
        bool IsAutoIncrement=false,
        string? BaseCatalogName = null,
        string? BaseSchemaName=null,
        string? BaseTableName=null,
        string? BaseColumnName=null,
        int? AutoIncrementSeed = null,
        int? AutoIncrementStep = null,
        object? DefaultValue=null,
        object? Expression = null,
        MappingType ColumnMapping = MappingType.Element,
        string? BaseTableNamespace=null,
        string? BaseColumnNamespace = null
        )
    {
        public bool IsRowVersion => false;
    }
}
