using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Kull.Data
{
    /// <summary>
    /// Simple helper extensions for Db transactions
    /// </summary>
    public static class TransactionUtils
    {


        /// <summary>
        /// Returns a DbCommand Object for a Procedure
        /// </summary>
        /// <param name="transaction">The transaction</param>
        /// <param name="text">The text to execute</param>
        /// <param name="commandType">The type of the command</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static DbCommand CreateCommand(this DbTransaction transaction, string text, CommandType commandType = CommandType.Text)
        {
            var cmd = transaction.Connection.CreateCommand();
            cmd.CommandText = text;
            cmd.Transaction = transaction;
            cmd.CommandType = commandType;
            return cmd;
        }


        /// <summary>
        /// Returns a DbCommand Object for a Procedure
        /// </summary>
        /// <param name="transaction">The transaction to run the command in</param>
        /// <param name="nameOfStoredProcedure"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public static DbCommand CreateSPCommand(this DbTransaction transaction, DBObjectName nameOfStoredProcedure)
        {
            var cmd = transaction.Connection.CreateCommand();
            cmd.CommandText = nameOfStoredProcedure.ToString(false, true);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = transaction;
            return cmd;
        }


    }
}
