using System;
using System.Linq;
using System.Data.Common;
using System.Management.Automation;

namespace Kull.Data.Powershell
{
    [Cmdlet(VerbsCommunications.Send, "DbCommand")]
    [OutputType(typeof(int))]
    public class SendDbCommandCmdlet : Cmdlet
    {

        [Parameter(Position = 0, Mandatory = true)]
        [ValidateNotNull]
        public DbConnection Connection { get; set; }

        [Parameter(Position = 1, Mandatory = false)]
        public string Command { get; set; }


        [Parameter(Position = 2, Mandatory = false)]
        public System.Collections.IDictionary Parameters { get; set; }

        [Parameter(Position = 3, Mandatory = false)]
        public System.Data.CommandType CommandType { get; set; } = System.Data.CommandType.StoredProcedure;


        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            using var cmd = Connection.CreateCommand();
            if(Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.Open();
            }
            cmd.CommandText = Command;
            cmd.CommandType = this.CommandType;

            foreach (var key in Parameters.Keys)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = key.ToString().StartsWith("@") ? key.ToString() : "@" + key;
                p.Value = Parameters[key] ?? DBNull.Value;
            }
            WriteObject(cmd.ExecuteNonQuery());
        }
    }
}
