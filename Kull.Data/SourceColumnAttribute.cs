using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kull.Data
{
    /// <summary>
    /// Use this in conjunction with RowHelper only. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false,
Inherited = true)]
    public class SourceColumnAttribute : System.Attribute
    {
        /// <summary>
        /// Just creates the class without any change
        /// </summary>
        public SourceColumnAttribute()
        {
        }

        /// <summary>
        /// Use this constructor to set the name of the column in database.
        /// </summary>
        /// <param name="columnName"></param>
        public SourceColumnAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }

        /// <summary>
        /// Use this construtor with parameter = true to exclude the column when getting a class from db.
        /// </summary>
        /// <param name="nosource"></param>
        public SourceColumnAttribute(bool nosource)
        {
            this.NoSource = nosource;
        }

        /// <summary>
        /// The name on the database side.
        /// </summary>
        public string? ColumnName { get; set; }

        /// <summary>
        /// Do net get this column from database.
        /// </summary>
        public bool NoSource { get; set; }
    }
}
