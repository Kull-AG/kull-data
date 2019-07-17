using System;

namespace Kull.Data
{
    /// <summary>
    /// Some small extensions for the DbDataReader class
    /// </summary>
    public static class DbDataReaderExtensions
    {

        /// <summary>
        /// Gets a string column. Slower then GetOrdinal + GetString
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string? GetNString(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        /// <summary>
        /// Gets a string column.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public static string? GetNString(this System.Data.Common.DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }


        /// <summary>
        /// Gets an int16 column. Slower then GetOrdinal + GetInt16
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Int16? GetNInt16(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (Int16?)null : reader.GetInt16(ordinal);
        }

        /// <summary>
        /// Gets an int16 column. 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ordinal">The column number</param>
        /// <returns></returns>
        public static Int16? GetNInt16(this System.Data.Common.DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (Int16?)null : reader.GetInt16(ordinal);
        }


        /// <summary>
        /// Gets an int column. Slower then GetOrdinal + GetInt32
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Int32 GetInt32(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.GetInt32(ordinal);
        }


        /// <summary>
        /// Gets an int column. Slower then GetOrdinal + GetInt32
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Int32? GetNInt32(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
        }

        /// <summary>
        /// Gets an int column. 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public static Int32? GetNInt32(this System.Data.Common.DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
        }


        /// <summary>
        /// Gets an int64 column. Slower then GetOrdinal + GetInt64
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Int64? GetNInt64(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (long?)null : reader.GetInt64(ordinal);
        }

        /// <summary>
        /// Gets an int64 column. 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public static Int64? GetNInt64(this System.Data.Common.DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (long?)null : reader.GetInt64(ordinal);
        }


        /// <summary>
        /// Gets a Double column. Slower then GetOrdinal + GetDouble
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static double? GetNDouble(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (double?)null : reader.GetDouble(ordinal);
        }

        /// <summary>
        /// Gets a Double column. 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ordinal">The column number</param>
        /// <returns></returns>
        public static double? GetNDouble(this System.Data.Common.DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (double?)null : reader.GetDouble(ordinal);
        }


        /// <summary>
        /// Gets a datetime column. Slower then GetOrdinal + GetDateTime
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DateTime? GetNDateTime(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
        }

        /// <summary>
        /// Gets a datetime column. 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public static DateTime? GetNDateTime(this System.Data.Common.DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
        }

        /// <summary>
        /// Gets a datetime column. Slower then GetOrdinal + GetDateTime
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.GetDateTime(ordinal);
        }


        /// <summary>
        /// Gets a bool column. Slower then GetOrdinal + GetBool
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool? GetNBoolean(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (bool?)null : reader.GetBoolean(ordinal);
        }

        /// <summary>
        /// Gets a bool column.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ordinal">The column number</param>
        /// <returns></returns>
        public static bool? GetNBoolean(this System.Data.Common.DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (bool?)null : reader.GetBoolean(ordinal);
        }


        /// <summary>
        /// Gets a bool column. Slower then GetOrdinal + GetBoolean
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool GetBoolean(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.GetBoolean(ordinal);
        }


        /// <summary>
        /// Gets a float column. Slower then GetOrdinal + GetFloat
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static float? GetNFloat(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (float?)null : reader.GetFloat(ordinal);
        }

        /// <summary>
        /// Gets a float column.
        /// </summary>
        /// <param name="reader">The db reader</param>
        /// <param name="ordinal">The column number</param>
        /// <returns></returns>
        public static float? GetNFloat(this System.Data.Common.DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (float?)null : reader.GetFloat(ordinal);
        }

        /// <summary>
        /// Gets a double column. Slower then GetOrdinal + GetDouble
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static double GetDouble(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.GetDouble(ordinal);
        }

        /// <summary>
        /// Gets a float column. Slower then GetOrdinal + GetFloat
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static float GetFloat(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.GetFloat(ordinal);
        }


        /// <summary>
        /// Gets a decimal column. Slower then GetOrdinal + GetDecimal
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static decimal? GetNDecimal(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
        }

        /// <summary>
        /// Gets a decimal column. 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public static decimal? GetNDecimal(this System.Data.Common.DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
        }

        /// <summary>
        /// Gets a byte array column. 
        /// Use this for small byte array only, such as timestamps
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public static byte[]? GetByteArray(this System.Data.Common.DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : (byte[])reader.GetValue(ordinal);
        }

        /// <summary>
        /// Gets a byte array column. 
        /// Use this for small byte array only, such as timestamps
        /// 
        /// Slower then GetOrdinal + GetByteArray(ordinal)
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name">The name of the column</param>
        /// <returns></returns>
        public static byte[]? GetByteArray(this System.Data.Common.DbDataReader reader, string name)
        {
            int ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? null : (byte[])reader.GetValue(ordinal);
        }


    }
}
