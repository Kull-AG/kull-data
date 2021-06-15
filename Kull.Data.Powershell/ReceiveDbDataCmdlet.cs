using System;
using System.Linq;
using System.Data.Common;
using System.Management.Automation;
using System.Collections.Generic;

namespace Kull.Data.Powershell
{
    [Cmdlet(VerbsCommunications.Receive, "DbData")]
    [OutputType(typeof(IEnumerable<PSObject>))]
    public class ReceiveDbDataCmdlet : Cmdlet
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
            if (Connection.State != System.Data.ConnectionState.Open)
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
            using var rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) return;
            string[] names = null;
            while (rdr.Read())
            {
                if (names == null)
                {
                    names = new string[rdr.FieldCount];
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        names[i] = rdr.GetName(i);
                    }
                }
                PSObject obj = new PSObject();
                for (int n = 0; n < names.Length; n++)
                {
                    obj.Members.Add(new PSNoteProperty(names[n],rdr.GetValue(n)));
                }
                WriteObject(obj);
            }
        }
    }
}
