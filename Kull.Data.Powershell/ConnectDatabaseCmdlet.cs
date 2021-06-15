using System;
using System.Data.Common;
using System.Management.Automation;

namespace Kull.Data.Powershell
{
    [Cmdlet(VerbsCommunications.Connect, "Database")]
    [OutputType(typeof(DbConnection))]
    public class ConnectDatabaseCmdlet : Cmdlet
    {

        [Parameter(Position = 0, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string ConnectionString { get; set; }

        [Parameter(Position = 1, Mandatory = false)]
        public string Provider { get; set; }


        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            var connStrEF = EFFallback.ConnectionStringParser.ParseEF(ConnectionString);
            if (connStrEF == null)
            {

            }
            var factory = connStrEF.Provider != null ? DbProviderFactories.GetFactory(connStrEF.Provider) :
                 !string.IsNullOrEmpty(Provider) ? DbProviderFactories.GetFactory(Provider) : null;
            factory = factory ?? Microsoft.Data.SqlClient.SqlClientFactory.Instance;
            var connection = factory.CreateConnection();
            connection.ConnectionString = connStrEF.ConnectionString;
            connection.Open();
            WriteObject(connection);
        }
    }
}
