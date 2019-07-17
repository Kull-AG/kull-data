using System;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using dt = System.Data;
using dba = System.Data.Common;


namespace Kull.Data
{


    /// <summary>
    /// Simple DB/POCO Mapper
    /// </summary>
    public class RowHelper
    {
        /// <summary>
        /// Use this if a TimeZoneField is null
        /// </summary>
        public TimeZoneInfo? DefaultTimeZone { get; set; }

        /// <summary>
        /// Ignore it when columns do not appear in the dataset
        /// </summary>
        public bool IgnoreMissingColumns { get; set; }

        [Obsolete("Use DbReader instead")]
        private readonly dt.DataRow? _row;

        private readonly IDataReader? reader;

        private string[]? columnNames;


        /// <summary>
        /// The row to operate on
        /// </summary>
        [Obsolete("Use DbReader instead")]
        public dt.DataRow? Row { get { return _row; } }


        /// <summary>
        /// Creates the RowHelper
        /// </summary>
        /// <param name="rw"></param>
        [Obsolete("Use DbReader instead")]
        public RowHelper(dt.DataRow rw)
        {
            this._row = rw;
        }

        /// <summary>
        /// Creates the RowHelper for the reader
        /// </summary>
        /// <param name="reader"></param>
        public RowHelper(IDataReader reader)
        {
            this.reader = reader;
        }



        /// <summary>
        /// Returns an array of the given type by using the given table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="ignoreMissingColumns"></param>
        /// <returns></returns>
        [Obsolete("Use DbReader instead")]
        public static T[] FromTable<T>(dt.DataTable dt, bool ignoreMissingColumns = false) where T : new()
        {
            T[] toReturn = new T[dt.Rows.Count];


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                toReturn[i] = FromRow(dt.Rows[i], new T(), ignoreMissingColumns);
            }
            return toReturn;
        }

        /// <summary>
        /// Returns an array of the given type by using the given table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="ignoreMissingColumns"></param>
        /// <returns></returns>
        public static T[] FromTable<T>(IDataReader dt, bool ignoreMissingColumns = false) where T : new()
        {
            var rh = new RowHelper(dt);
            rh.IgnoreMissingColumns = ignoreMissingColumns;
            var type = typeof(T);
            var infos = rh.GetInfos(type);

            List<T> toReturn = new List<T>();
            while (dt.Read())
            {
                toReturn.Add((T)rh.FromRow(new T(), type, infos));
            }
            return toReturn.ToArray();
        }

        /// <summary>
        /// Gets the value from the row
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rw"></param>
        /// <param name="toSet"></param>
        /// <param name="ignoreMissingColumns"></param>
        /// <returns></returns>
        [Obsolete("Use DbReader instead")]
        public static T FromRow<T>(dt.DataRow rw, T toSet, bool ignoreMissingColumns = false)
        {
            return new RowHelper(rw)
            {
                IgnoreMissingColumns = ignoreMissingColumns
            }.FromRow(toSet);
        }

        /// <summary>
        /// Gets the value from the row
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rw"></param>
        /// <param name="toSet"></param>
        /// <param name="ignoreMissingColumns"></param>
        /// <returns></returns>
        public static T FromRow<T>(System.Data.Common.DbDataReader rw, T toSet, bool ignoreMissingColumns = false)
        {
            return new RowHelper(rw)
            {
                IgnoreMissingColumns = ignoreMissingColumns
            }.FromRow(toSet);
        }

        /// <summary>
        /// Sets the given objects properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSet"></param>
        /// <returns></returns>
        public T FromRow<T>(T toSet)
        {
            return (T)FromRow(toSet, typeof(T));
        }

        /// <summary>
        /// Sets the property of an object by using the Value from the given Column in the DataRow.
        /// Throws ArgumentException for unknown property types
        /// </summary>
        /// <param name="property"></param>
        /// <param name="toSet"></param>
        /// <param name="fieldIndex"></param>
        /// <returns>true for success, false for arrays and most generics</returns>
        public bool SetPropertyInfo(PropertyInfo property, Object toSet, int fieldIndex)
        {

            if (property.PropertyType == (typeof(int?)))
            {
                property.SetValue(toSet, GetNIntFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(int))
            {
                property.SetValue(toSet, GetIntFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == (typeof(System.Nullable<long>)))
            {
                property.SetValue(toSet, GetNLongFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(long))
            {
                property.SetValue(toSet, GetLongFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(System.Nullable<double>))
            {
                property.SetValue(toSet, GetNDoubleFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(DateTimeOffset))
            {
                property.SetValue(toSet, GetDateTimeOffsetFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(DateTimeOffset?))
            {
                property.SetValue(toSet, GetDateTimeOffsetFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(TimeZoneInfo))
            {
                property.SetValue(toSet,
                    GetTimeZoneFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(double))
            {
                property.SetValue(toSet, GetDoubleFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetValue(toSet, GetStringFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(DateTime?))
            {
                property.SetValue(toSet, GetDateTimeFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                property.SetValue(toSet, GetDateTimeFieldValue(fieldIndex)!.Value, null);
            }
            else if (property.PropertyType == typeof(System.Drawing.Color?))
            {
                property.SetValue(toSet, GetColorFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(System.Drawing.Color))
            {
                property.SetValue(toSet, GetColorFieldValue(fieldIndex)!.Value, null);
            }
            else if (property.PropertyType == typeof(byte[]))
            {
                property.SetValue(toSet, GetByteValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(bool?))
            {
                property.SetValue(toSet, GetNBoolValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(bool))
            {
                property.SetValue(toSet, GetBoolValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(System.Nullable<byte>))
            {
                int? value = GetNIntFieldValue(fieldIndex);
                if (value == null)
                {
                    property.SetValue(toSet, null, null);
                }
                else
                { property.SetValue(toSet, (byte)value, null); }
            }
            else if (property.PropertyType == typeof(byte))
            {
                property.SetValue(toSet, (byte)GetIntFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(Guid?))
            {
                property.SetValue(toSet, GetNGuidFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(Guid))
            {
                property.SetValue(toSet, (Guid)GetNGuidFieldValue(fieldIndex)!, null);
            }
            else if (property.PropertyType == typeof(decimal?))
            {
                property.SetValue(toSet, GetNDecimalFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(decimal))
            {
                property.SetValue(toSet, GetDecimalFieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(short?))
            {
                property.SetValue(toSet, GetNInt16FieldValue(fieldIndex), null);
            }
            else if (property.PropertyType == typeof(short))
            {
                property.SetValue(toSet, GetInt16FieldValue(fieldIndex), null);
            }
            else if (
                   property.PropertyType.IsGenericType
                || property.PropertyType.IsArray)
            {//Generic types and array cannot be set by design an therefore we do not set them
                return false;
            }
            else
            {
                throw new ArgumentException("Property " + property.Name + " is of type " + property.PropertyType.Name + " which is not allowed");
            }
            return true;
        }

        private struct ToSetInfo
        {
            public readonly PropertyInfo PropertyInfo;
            public readonly int FieldIndex;

            public ToSetInfo(PropertyInfo property, int fieldIndex)
            {
                this.PropertyInfo = property;
                this.FieldIndex = fieldIndex;
            }
        }


        private ToSetInfo[] GetInfos(Type type)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            var properties = type.GetProperties(flags)
                    .Where(p => p.CanRead && p.CanWrite)
                    .ToArray();

            var toSetInfos = properties.Select(p =>
            {
                var attributes = p.GetCustomAttributes(typeof(SourceColumnAttribute), true).Cast<SourceColumnAttribute>().ToArray();
                return new { PropertyInfo = p, Attrs = attributes.Length > 0 ? attributes[0] : null };
            })
            .Where(p => p.Attrs == null || !p.Attrs.NoSource)
            // Get column name
            .Select(p => new { p.PropertyInfo, p.Attrs, ColumnName = p.Attrs?.ColumnName ?? p.PropertyInfo!.Name })
            .Where(p =>
            {
                if (this.IgnoreMissingColumns ||
                   (
                   p.PropertyInfo!.PropertyType.IsGenericType
                   && p.PropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    if (!HasColumn(p.ColumnName!))
                        return false;
                }
                return true;
            })
            .Select(p => new ToSetInfo(p.PropertyInfo!, GetOrdinal(p.ColumnName!))).ToArray();
            return toSetInfos;
        }


        private object FromRow(object? toSet, Type? type, ToSetInfo[] infos)
        {
            if (type == null && toSet == null)
                throw new ArgumentNullException(nameof(type));
            if (type == null)
                type = toSet!.GetType();
            if (toSet == null)
                toSet = Activator.CreateInstance(type);

            foreach (var property in infos)
            {
                this.SetPropertyInfo(property.PropertyInfo, toSet, property.FieldIndex);

            }
            return toSet;
        }

        private ToSetInfo[]? cached_infos;
        private Type? cached_type;


        /// <summary>
        /// Sets the given objects properties
        /// </summary>
        /// <param name="toSet">The object whose properties will be set</param>
        /// <param name="type">The type of the object. Can be null if toSet is specified</param>
        /// <returns>The given object</returns>
        public object FromRow(object? toSet, Type? type)
        {
            if (type == null && toSet == null)
                throw new ArgumentNullException(nameof(type));
            if (type == null)
                type = toSet!.GetType();
            ToSetInfo[] props;
            if (type == cached_type)
            {
                props = cached_infos!;
            }
            else
            {
                props = GetInfos(type);
                cached_infos = props;
                cached_type = type;
            }
            FromRow(toSet, type, props);
            return toSet!;
        }

        /// <summary>
        /// Gets an nullable int field. Returns null if the column does not exist
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public int? GetNIntFieldValue(int fieldIndex)
        {
            if (fieldIndex != -1)
            {
                object? val = GetValue(fieldIndex);
                if (val is int i)
                    return i;
                if (val == null)
                    return null;
                return int.Parse(val.ToString());
            }
            else
                return null;
        }
        public int? GetNIntFieldValue(string columnName) => GetNIntFieldValue(GetOrdinal(columnName));

        /// <summary>
        /// Gets a Guid Col. Returns null if the field does not exists
        /// </summary>
        /// <param name="fieldName">The field name</param>
        /// <returns></returns>
        public Guid? GetNGuidFieldValue(int fieldIndex)
        {
            if (fieldIndex != -1)
            {
                object? val = GetValue(fieldIndex);
                if (val is Guid g)
                    return g;
                if (val == null)
                    return null;
#if !NET35
                return Guid.Parse(val.ToString());
#else
                throw new PlatformNotSupportedException("To use GUID Parsing, use .Net 45+");
#endif
            }
            else
                return null;
        }
        public Guid? GetNGuidFieldValue(string columnName) => GetNGuidFieldValue(GetOrdinal(columnName));

        /// <summary>
        /// Returns a long value, null if column does not exist
        /// </summary>
        /// <param name="fieldName">The name of the column</param>
        /// <returns></returns>
        public long? GetNLongFieldValue(int fieldIndex)
        {
            if (fieldIndex != -1)
            {
                object? val = GetValue(fieldIndex);

                if (val is long l)
                    return l;
                else if (val is int i)
                    return i;
                if (val == null)
                    return null;
                return long.Parse(val.ToString());
            }
            else
                return null;
        }

        public long? GetNLongFieldValue(string columnName) => GetNLongFieldValue(GetOrdinal(columnName));

        /// <summary>
        /// Gets the column Index of the given name or -1
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <returns>The index, or -1</returns>
        public int GetOrdinal(string columnName)
        {
            if (reader != null)
            {
                try
                {

                    return reader.GetOrdinal(columnName);
                }
                catch (IndexOutOfRangeException) { return -1; }
            }
            else
            {
                return _row.Table.Columns.IndexOf(columnName);
            }
        }

        /// <summary>
        /// Returns true if the column exists in the Dataset
        /// </summary>
        /// <param name="columnName">The name of the Column</param>
        /// <returns>A boolean indicating if it has the column</returns>
        public bool HasColumn(string columnName)
        {
            if (reader != null)
            {
                if (columnNames == null)
                {
                    columnNames = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columnNames[i] = this.reader.GetName(i).ToLower();
                    }
                }
                return columnNames.Contains(columnName.ToLower());
            }
            else
#pragma warning disable CS0618 // Type or member is obsolete
                return _row!.Table.Columns.Contains(columnName);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        private object? GetValue(int ordinal)
        {
            if (reader != null)
            {
                if (reader.IsDBNull(ordinal))
                    return null;
                return reader[ordinal];
            }
            else
            {

#pragma warning disable CS0618 // Type or member is obsolete
                if (_row![ordinal] == DBNull.Value)
                    return null;
                return _row![ordinal];
#pragma warning restore CS0618 // Type or member is obsolete
            }

        }

        /// <summary>
        /// Returns a double? value or null if the column does not exist
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public double? GetNDoubleFieldValue(int fieldIndex)
        {
            if (fieldIndex != -1)
            {
                object? val = GetValue(fieldIndex);
                if (val is double d)
                    return d;
                if (val == null)
                    return null;
                return double.Parse(val.ToString());
            }
            return null;
        }
        public double? GetNDoubleFieldValue(string columnName) => GetNDoubleFieldValue(GetOrdinal(columnName));

        public double? GetNDoubleFieldValue(params string[] fieldNames)
        {
            foreach (string fieldName in fieldNames)
            {
                var fieldIndex = GetOrdinal(fieldName);
                if (fieldIndex != -1)
                {
                    object? val = GetValue(fieldIndex);
                    if (val is double d)
                        return d;
                    if (val == null)
                        return null;
                    return double.Parse(val.ToString());
                }
            }
            return null;
        }


        public decimal GetDecimalFieldValue(int fieldIndex)
        {
            object val = GetValue(fieldIndex)!;
            if (val is decimal d)
                return d;
            return decimal.Parse(val.ToString());
        }
        public decimal GetDecimalFieldValue(string columnName) => GetDecimalFieldValue(GetOrdinal(columnName));

        public decimal? GetNDecimalFieldValue(int fieldIndex)
        {
            if (fieldIndex != -1)
            {
                object? val = GetValue(fieldIndex);
                if (val is decimal d)
                    return d;
                if (val == null)
                    return null;
                return decimal.Parse(val.ToString());
            }
            return null;
        }
        public decimal? GetNDecimalFieldValue(string columnName) => GetNDecimalFieldValue(GetOrdinal(columnName));

        public short GetInt16FieldValue(int fieldIndex)
        {
            object val = GetValue(fieldIndex)!;
            if (val is short d)
                return d;
            return short.Parse(val.ToString());
        }
        public short GetInt16FieldValue(string columnName) => GetInt16FieldValue(GetOrdinal(columnName));


        public short? GetNInt16FieldValue(int fieldIndex)
        {
            if (fieldIndex != -1)
            {
                object? val = GetValue(fieldIndex);
                if (val is short d)
                    return d;
                if (val == null)
                    return null;
                return short.Parse(val.ToString());
            }
            return null;
        }
        public short? GetNInt16FieldValue(string columnName) => GetNInt16FieldValue(GetOrdinal(columnName));

        public double GetDoubleFieldValue(int fieldIndex)
        {
            object val = GetValue(fieldIndex)!;
            if (val is double d)
                return d;
            return double.Parse(val.ToString());
        }
        public double GetDoubleFieldValue(string columnName) => GetDoubleFieldValue(GetOrdinal(columnName));


        public int GetIntFieldValue(int fieldIndex)
        {
            var vl = GetValue(fieldIndex);
            if (vl is int i)
                return i;
            if (vl is Int64 l)
                return (int)l;
            string toParse = vl.ToString();
            return int.Parse(toParse);
        }
        public int GetIntFieldValue(string columnName) => GetIntFieldValue(GetOrdinal(columnName));


        public long GetLongFieldValue(int fieldIndex)
        {
            var val = GetValue(fieldIndex)!;
            if (val is long l)
                return l;
            else if (val is int i)
                return i;
            string toParse = val.ToString();
            return long.Parse(toParse);
        }
        public long GetLongFieldValue(string columnName) => GetLongFieldValue(GetOrdinal(columnName));

        public string? GetStringFieldValue(int fieldIndex)
        {
            if (fieldIndex != -1)
            {
                return GetValue(fieldIndex)?.ToString();
            }
            else
                return null;
        }
        public string? GetStringFieldValue(string columnName) => GetStringFieldValue(GetOrdinal(columnName));

        public string? GetStringFieldValue(params string[] fieldNames)
        {
            foreach (string fieldName in fieldNames)
            {
                var fieldIndex = GetOrdinal(fieldName);
                if (fieldIndex != -1)
                {
                    return GetValue(fieldIndex)?.ToString();
                }
            }
            return null;
        }

        public DateTime? GetDateTimeFieldValue(int fieldIndex)
        {
            if (fieldIndex != -1)
            {
                object? val = GetValue(fieldIndex);

                if (val is DateTime d)
                    return d;
                if (val == null)
                    return null;
                return DateTime.Parse(val.ToString());
            }
            else
                return null;
        }
        public DateTime? GetDateTimeFieldValue(string columnName) => GetDateTimeFieldValue(GetOrdinal(columnName));

        public System.Drawing.Color? GetColorFieldValue(int fieldIndex)
        {
            var strVl = this.GetStringFieldValue(fieldIndex);
            return strVl == null ? (System.Drawing.Color?)null :
                System.Drawing.Color.FromName(strVl);
        }
        public System.Drawing.Color? GetColorFieldValue(string columnName) => GetColorFieldValue(GetOrdinal(columnName));


        public byte[]? GetByteValue(int fieldIndex)
        {
            var val = GetValue(fieldIndex);
            if (val is byte[] b)
                return b;
            if (val == null)
                return null;
            return Convert.FromBase64String(val.ToString());
        }
        public byte[]? GetByteValue(string columnName) => GetByteValue(GetOrdinal(columnName));


        public bool GetBoolValue(int fieldIndex)
        {
            object valueO = GetValue(fieldIndex)!;
            if (valueO is bool)
                return (bool)valueO;
            string value = valueO.ToString().Trim();
            if (value == "1")
                return true;
            else if (value == "0")
                return false;
            return bool.Parse(value);
        }
        public bool GetBoolValue(string columnName) => GetBoolValue(GetOrdinal(columnName));


        /// <summary>
        /// Gets a boolean value
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool? GetNBoolValue(int fieldIndex)
        {
            if (fieldIndex != -1)
            {
                object? valueO = GetValue(fieldIndex);
                if (valueO == null || valueO == DBNull.Value)
                    return null;
                return GetBoolValue(fieldIndex);
            }
            else
                return null;
        }
        public bool? GetNBoolValue(string columnName) => GetNBoolValue(GetOrdinal(columnName));

        /// <summary>
        /// Gets the TimeZoneInfo from a Field. Can be an old one as well (eg Europe/Zurich)
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public TimeZoneInfo? GetTimeZoneFieldValue(int fieldIndex)
        {
            if (fieldIndex != -1)
            {
                var val = GetValue(fieldIndex);
                if (val == null)
                    return this.DefaultTimeZone;
                else if (val is TimeZoneInfo i)
                    return i;
                else
                {
                    string value = val.ToString();
                    return TimezoneMapping.GetTimeZone(value);
                }
            }
            return null;
        }
        public TimeZoneInfo? GetTimeZoneFieldValue(string columnName) => GetTimeZoneFieldValue(GetOrdinal(columnName));

        /// <summary>
        /// Gets a Datetimeoffset field value
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public DateTimeOffset? GetDateTimeOffsetFieldValue(int fieldIndex)
        {

            if (fieldIndex != -1)
            {
                var value = GetValue(fieldIndex);
                if (value == null)
                    return null;
                else if (value is DateTimeOffset o)
                {
                    return o;
                }
                else
                {
                    var dateTime = DateTime.Parse(value.ToString());
                    return new DateTimeOffset(dateTime, this.DefaultTimeZone?.GetUtcOffset(dateTime) ?? TimeSpan.Zero);
                }
            }
            else
                return null;
        }
        public DateTimeOffset? GetDateTimeOffsetFieldValue(string columnName) => GetDateTimeOffsetFieldValue(GetOrdinal(columnName));
    }
}
