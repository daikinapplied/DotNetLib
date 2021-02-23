using System.Collections.Generic;
using System.Data;
using Serilog.Sinks.MSSqlServer;

namespace Daikin.DotNetLib.Serilog
{
    public static class LogColumns
    {
        #region Constants
        public const int MaxLengthApplication = 50;
        public const int MaxLengthVersion = 10;
        public const int MaxLengthRemoteIp = 40;
        public const int MaxLengthClientId = 40;
        public const int MaxLengthEnvironment = 10;
        public const int MaxLengthUser = 50;
        public const int MaxLengthSource = 50;
        public const int MaxLengthSession = 50;
        public const int MaxLengthRequestId = 25;
        public const int MaxLengthUserAgent = 200;
        //public const int MaxLengthData = 255;
        #endregion

        #region Functions

        public static ColumnOptions DefaultColumnOptions()
        {
            var columnOptions = new ColumnOptions {AdditionalColumns = DefaultColumns()};
            columnOptions.Store.Add(StandardColumn.LogEvent); // Include JSON
            columnOptions.Store.Remove(StandardColumn.MessageTemplate); 
            columnOptions.Store.Remove(StandardColumn.Properties); // Exclude XML
            return columnOptions;
        }

        public static List<SqlColumn> DefaultColumns()
        {
            return new List<SqlColumn>
            {
                // Order may matter - need to verify
                new SqlColumn { ColumnName = "EventId", DataType = SqlDbType.Int, AllowNull = true },
                new SqlColumn { ColumnName = "Application", DataType = SqlDbType.NVarChar, DataLength = MaxLengthApplication, AllowNull = true },
                new SqlColumn { ColumnName = "Version", DataType = SqlDbType.NVarChar, DataLength = MaxLengthVersion, AllowNull = true },
                new SqlColumn { ColumnName = "RemoteIp", DataType = SqlDbType.NVarChar, DataLength = MaxLengthRemoteIp, AllowNull = true },
                new SqlColumn { ColumnName = "ClientId", DataType = SqlDbType.NVarChar, DataLength = MaxLengthClientId, AllowNull = true },
                new SqlColumn { ColumnName = "Environment", DataType = SqlDbType.NVarChar, DataLength = MaxLengthEnvironment, AllowNull = true },
                new SqlColumn { ColumnName = "User", DataType = SqlDbType.NVarChar, DataLength = MaxLengthUser, AllowNull = true },
                new SqlColumn { ColumnName = "Source", DataType = SqlDbType.NVarChar, DataLength = MaxLengthSource, AllowNull = true },
                new SqlColumn { ColumnName = "Session", DataType = SqlDbType.NVarChar, DataLength = MaxLengthSession, AllowNull = true },
                new SqlColumn { ColumnName = "UserAgent", DataType = SqlDbType.NVarChar, DataLength = MaxLengthUserAgent, AllowNull = true },
                new SqlColumn { ColumnName = "RequestId", DataType = SqlDbType.NVarChar, DataLength = MaxLengthRequestId, AllowNull = true },
                new SqlColumn { ColumnName = "Data", DataType = SqlDbType.NVarChar, AllowNull = true }
            };
        }
        #endregion
    }
}
