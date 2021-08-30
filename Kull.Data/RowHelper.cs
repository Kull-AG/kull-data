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
        [Obsolete("Use DbDataReader instead")]
        public static IReadOnlyCollection<T> FromTable<T>(dt.DataTable dt, bool ignoreMissingColumns = false) 
        {
            T[] toReturn = new T[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                toReturn[i] = FromRow(dt.Rows[i], default(T), ignoreMissingColumns);
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
        public static IReadOnlyCollection<T> FromTable<T>(IDataReader dt, bool ignoreMissingColumns = false)
        {
            var rh = new RowHelper(dt);
            rh.IgnoreMissingColumns = ignoreMissingColumns;
            var type = typeof(T);

            var infos = rh.GetInfos(type);
            List<T> toReturn = new List<T>();
            while (dt.Read())
            {
                toReturn.Add((T)rh.FromRow(null, type, infos));
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
        public static T FromRow<T>(dt.DataRow rw, T? toSet, bool ignoreMissingColumns = false)
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
        public T FromRow<T>(T? toSet)
        {
            return (T)FromRow(toSet, typeof(T));
        }

        private bool TryGetValue(string propName, Type propertyType, int fieldIndex, out object? value)
        {
            if (propertyType == (typeof(int?)))
            {
                value =  GetNIntFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(int))
            {
                value =  GetIntFieldValue(fieldIndex);
            }
            else if (propertyType == (typeof(System.Nullable<long>)))
            {
                value =  GetNLongFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(long))
            {
                value =  GetLongFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(System.Nullable<double>))
            {
                value =  GetNDoubleFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(DateTimeOffset))
            {
                value =  GetDateTimeOffsetFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(DateTimeOffset?))
            {
                value =  GetDateTimeOffsetFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(TimeZoneInfo))
            {
                value = 
                    GetTimeZoneFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(double))
            {
                value =  GetDoubleFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(string))
            {
                value =  GetStringFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(DateTime?))
            {
                value =  GetDateTimeFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(DateTime))
            {
                value =  GetDateTimeFieldValue(fieldIndex)!.Value;
            }
            else if (propertyType == typeof(System.Drawing.Color?))
            {
                value =  GetColorFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(System.Drawing.Color))
            {
                value =  GetColorFieldValue(fieldIndex)!.Value;
            }
            else if (propertyType == typeof(byte[]))
            {
                value =  GetByteValue(fieldIndex);
            }
            else if (propertyType == typeof(bool?))
            {
                value =  GetNBoolValue(fieldIndex);
            }
            else if (propertyType == typeof(bool))
            {
                value =  GetBoolValue(fieldIndex);
            }
            else if (propertyType == typeof(System.Nullable<byte>))
            {
                int? intValue = GetNIntFieldValue(fieldIndex);
                if (intValue == null)
                {
                    value =  null;
                }
                else
                { value =  (byte)intValue; }
            }
            else if (propertyType == typeof(byte))
            {
                value =  (byte)GetIntFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(Guid?))
            {
                value =  GetNGuidFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(Guid))
            {
                value =  (Guid)GetNGuidFieldValue(fieldIndex)!;
            }
            else if (propertyType == typeof(decimal?))
            {
                value =  GetNDecimalFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(decimal))
            {
                value =  GetDecimalFieldValue(fieldIndex);
            }
            else if (propertyType == typeof(short?))
            {
                value =  GetNInt16FieldValue(fieldIndex);
            }
            else if (propertyType == typeof(short))
            {
                value =  GetInt16FieldValue(fieldIndex);
            }
            else if (propertyType == typeof(object))
            {
                value = GetValue(fieldIndex);
            }
            else if (propertyType == typeof(IDataReader))
            {
                if (reader == null) throw new ArgumentException("Must call this with a Datareader");
                value = reader;
            }
            else if (propertyType == typeof(System.Data.Common.DbDataReader))
            {
                if (reader == null) throw new ArgumentException("Must call this with a Datareader");
                value = (System.Data.Common.DbDataReader)reader;
            }
            else if (
                   propertyType.IsGenericType
                || propertyType.IsArray)
            {//Generic types and array cannot be set by design an therefore we do not set them
                value = null;
                return false;
            }
            else
            {
                throw new ArgumentException("Property " + propName + " is of type " + propertyType.Name + " which is not allowed");
            }
            return true;
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
            if(TryGetValue(property.Name, property.PropertyType, fieldIndex, out var value))
            {
                property.SetValue(toSet, value, null);
                return true;
            }
            return false;
        }

        private struct SetInfo
        {
            public readonly MemberSetInfo[] PropertySetInfo;
            public readonly ConstructorInfo? constructorInfo;

            public readonly bool IsRecord { get; }

            public SetInfo(MemberSetInfo[] propertySetInfos)
            {
                this.IsRecord = false;
                this.PropertySetInfo = propertySetInfos;
                this.constructorInfo = null;
            }

            public SetInfo(MemberSetInfo[] propertySetInfos, ConstructorInfo constructorInfo)
            {
                this.IsRecord = true;
                this.PropertySetInfo = propertySetInfos;
                this.constructorInfo = constructorInfo;
            }
        }


        private struct MemberSetInfo
        {
            public readonly Type Type;
            public readonly string Name;
            public readonly int FieldIndex;
            public readonly bool NoSource=false;
            public Action<object, object?>? SetValue;

            public MemberSetInfo(PropertyInfo property, int fieldIndex, Action<object, object?> setValue)
            {
                this.Type = property.PropertyType;
                this.Name = property.Name!;
                this.FieldIndex = fieldIndex;
                this.SetValue = setValue;
            }

            public MemberSetInfo(ParameterInfo property, int fieldIndex, bool noSource)
            {
                this.Type = property.ParameterType;
                this.Name = property.Name!;
                this.FieldIndex = fieldIndex;
                this.NoSource = noSource;
                this.SetValue = null;
            }
        }


        private SetInfo GetInfos(Type type)
        {
            bool isRecord = !type.GetConstructors().Any(c => !c.GetParameters().Any());
            if (isRecord)
            {
                var constructor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Count()).First();

                var prms = constructor.GetParameters().ToArray();

                var prmInfos = prms.Select(p =>
                {
                    var attributes = p.GetCustomAttributes(typeof(SourceColumnAttribute), true).Cast<SourceColumnAttribute>().ToArray();
                    return new { ParameterInfo = p, Attrs = attributes.Length > 0 ? attributes[0] : null };
                })
                // Get column name
                .Select(p => new { p.ParameterInfo, p.Attrs, ColumnName = p.Attrs?.ColumnName ?? p.ParameterInfo!.Name })
                .Select(p =>
                {
                    bool noSource = p.Attrs != null && p.Attrs.NoSource;
                    if(noSource)
                    {
                        return new { NoSource = true, p.ParameterInfo, p.ColumnName };
                    }
                    if (this.IgnoreMissingColumns ||
                       (
                       p.ParameterInfo!.ParameterType.IsGenericType
                       && p.ParameterInfo.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        if (!HasColumn(p.ColumnName!))
                            return new { NoSource = true, p.ParameterInfo, p.ColumnName };
                    }
                    return new { NoSource = false, p.ParameterInfo, p.ColumnName };
                })
                .Select(p => new MemberSetInfo(p.ParameterInfo, GetOrdinal(p.ColumnName!), p.NoSource)).ToArray();
                return new SetInfo(prmInfos, constructor);
            }
            
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite)
                    .ToArray();

            var PropertySetInfos = properties.Select(p =>
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
            .Select(p => new MemberSetInfo(p.PropertyInfo!, GetOrdinal(p.ColumnName!), p.PropertyInfo.SetValue)).ToArray();
            return new SetInfo(PropertySetInfos);
        }

        private static object? GetDefaultValueForType(Type type) => type.IsValueType ? Activator.CreateInstance(type):null;

        private object FromRow(object? toSet, Type? type, SetInfo infos)
        {
            if (type == null && toSet == null)
                throw new ArgumentNullException(nameof(type));
            if (type == null)
                type = toSet!.GetType();
            if (infos.IsRecord)
            {
                var values = infos.PropertySetInfo.Select(s =>
                {
                    if (s.NoSource) return GetDefaultValueForType(s.Type);
                    if (TryGetValue(s.Name, s.Type, s.FieldIndex, out var value))
                    {
                        return value;
                    }
                    throw new InvalidOperationException($"Cannot get value of type {s.Type.FullName} for field {s.Name}");
                }).ToArray();
                return infos.constructorInfo!.Invoke(values);
                //constr.Invoke()
            }
            if (toSet == null)
            {
                toSet = Activator.CreateInstance(type);
            }

            foreach (var property in infos.PropertySetInfo)
            {
                if (TryGetValue(property.Name, property.Type, property.FieldIndex, out var value))
                {
                    property.SetValue!(toSet!, value);
                }

            }
            return toSet!;
        }

        private SetInfo? cached_infos;
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
            SetInfo setInfo;
            if (type == cached_type)
            {
                setInfo = cached_infos!.Value;
            }
            else
            {
                setInfo = GetInfos(type);
                cached_infos = setInfo;
                cached_type = type;
                
            }
            return FromRow(toSet, type, setInfo);
        }

        /// <summary>
        /// Gets an nullable int field. Returns null if the column does not exist
        /// </summary>
        /// <param name="fieldIndex"></param>
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
                return int.Parse(val.ToString()!);
            }
            else
                return null;
        }


        public int? GetNIntFieldValue(string columnName) => GetNIntFieldValue(GetOrdinal(columnName));

        /// <summary>
        /// Gets a Guid Col. Returns null if the field does not exists
        /// </summary>
        /// <param name="fieldIndex">The field index</param>
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
                return Guid.Parse(val.ToString()!);
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
        /// <param name="fieldIndex">The index of the column</param>
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
                return long.Parse(val.ToString()!);
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
#pragma warning disable CS0618 // Type or member is obsolete
                return _row!.Table.Columns.IndexOf(columnName);
#pragma warning restore CS0618 // Type or member is obsolete
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
        /// <param name="fieldIndex"></param>
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
                return double.Parse(val.ToString()!);
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
                    return double.Parse(val.ToString()!);
                }
            }
            return null;
        }


        public decimal GetDecimalFieldValue(int fieldIndex)
        {
            object val = GetValue(fieldIndex)!;
            if (val is decimal d)
                return d;
            return decimal.Parse(val.ToString()!);
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
                return decimal.Parse(val.ToString()!);
            }
            return null;
        }
        public decimal? GetNDecimalFieldValue(string columnName) => GetNDecimalFieldValue(GetOrdinal(columnName));

        public short GetInt16FieldValue(int fieldIndex)
        {
            object val = GetValue(fieldIndex)!;
            if (val is short d)
                return d;
            return short.Parse(val.ToString()!);
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
                return short.Parse(val.ToString()!);
            }
            return null;
        }
        public short? GetNInt16FieldValue(string columnName) => GetNInt16FieldValue(GetOrdinal(columnName));

        public double GetDoubleFieldValue(int fieldIndex)
        {
            object val = GetValue(fieldIndex)!;
            if (val is double d)
                return d;
            return double.Parse(val.ToString()!);
        }
        public double GetDoubleFieldValue(string columnName) => GetDoubleFieldValue(GetOrdinal(columnName));


        public int GetIntFieldValue(int fieldIndex)
        {
            var vl = GetValue(fieldIndex);
            if (vl is int i)
                return i;
            if (vl is Int64 l)
                return (int)l;
            if (vl == DBNull.Value || vl == null)
            {
                throw new ArgumentNullException($"Field with index {fieldIndex} may not be null");
            }
            string toParse = vl.ToString()!;
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
            string toParse = val.ToString()!;
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
                return DateTime.Parse(val!.ToString()!);
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
            return Convert.FromBase64String(val.ToString()!);
        }
        public byte[]? GetByteValue(string columnName) => GetByteValue(GetOrdinal(columnName));


        public bool GetBoolValue(int fieldIndex)
        {
            object valueO = GetValue(fieldIndex)!;
            if (valueO is bool)
                return (bool)valueO;
            string value = valueO.ToString()!.Trim();
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
        /// <param name="fieldIndex"></param>
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
        /// <param name="fieldIndex"></param>
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
                    string value = val.ToString()!;
                    return TimezoneMapping.GetTimeZone(value);
                }
            }
            return null;
        }
        public TimeZoneInfo? GetTimeZoneFieldValue(string columnName) => GetTimeZoneFieldValue(GetOrdinal(columnName));

        /// <summary>
        /// Gets a Datetimeoffset field value
        /// </summary>
        /// <param name="fieldIndex">The index of the field</param>
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
                    var dateTime = DateTime.Parse(value.ToString()!);
                    return new DateTimeOffset(dateTime, this.DefaultTimeZone?.GetUtcOffset(dateTime) ?? TimeSpan.Zero);
                }
            }
            else
                return null;
        }
        public DateTimeOffset? GetDateTimeOffsetFieldValue(string columnName) => GetDateTimeOffsetFieldValue(GetOrdinal(columnName));
    }
}
