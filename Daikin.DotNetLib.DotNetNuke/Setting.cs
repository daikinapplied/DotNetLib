using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DotNetNuke.Services.Exceptions;

namespace Daikin.DotNetLib.DotNetNuke
{
    public class Setting
    {
        #region Constants
        public const string DefaultSettingTableName = "DaikinApplied_Master_Setting";
        public const string DefaultStoredProcGetSetting = "DaikinApplied_GetSetting";
        public const string DefaultStoredProcSetSetting = "DaikinApplied_SetSetting";
        #endregion

        #region Properties
        public string SettingTableName { get; set; }
        public string StoredProcGetSetting { get; set; }
        public string StoredProcSetSetting { get; set; } 

        public long SettingRef { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public int? CacheDuration { get; set; }
        public DateTime? Modified { get; set; }
        public int? PortalId { get; set; }
        public int? ModuleId { get; set; }
        public int? UserId { get; set; }
        #endregion

        #region Constructors
        public Setting()
        {
            SettingTableName = DefaultSettingTableName;
            StoredProcGetSetting = DefaultStoredProcGetSetting;
            StoredProcSetSetting = DefaultStoredProcSetSetting;
        }

        public Setting(string settingName, string settingValue = "", int? cacheDuration = null, DateTime? modified = null, string settingTable = DefaultSettingTableName, string storedProcGetSetting = DefaultStoredProcGetSetting, string storedProcSetSetting = DefaultStoredProcSetSetting)
        {
            SettingTableName = settingTable;
            StoredProcGetSetting = storedProcGetSetting;
            StoredProcSetSetting = storedProcSetSetting;

            SettingName = settingName;
            SettingValue = settingValue;
            CacheDuration = cacheDuration;
            if (modified == null)
            {
                modified = DateTime.Now;
            }
            Modified = modified;
        }
        #endregion

        #region Methods
        /// <summary>
        /// This should be called once before a bunch of settings are saved, to make sure the data structures and stored procedures are in place
        /// </summary>
        public void VerifyStorage()
        {
            var sqlConn = Sql.GetSqlConnection();
            sqlConn.Open();

            // Master Setting table
            var createTable = 
                  $"IF NOT EXISTS(SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{SettingTableName}]') AND OBJECTPROPERTY(id, N'IsTable') = 1)"
                +  "BEGIN"
                + $"  CREATE TABLE [{SettingTableName}] ("
                +  "    [SettingRef][BIGINT] IDENTITY(1,1) NOT NULL,"
                +  "    [SettingName] [NVARCHAR] (50) NOT NULL,"
                +  "    [SettingValue] [NVARCHAR] (255) NULL,"
                +  "    [PortalId] [INT] NULL,"
                +  "    [ModuleId] [INT] NULL,"
                +  "    [UserId] [INT] NULL,"
                +  "    [CacheDuration] [INT] NULL,"
                +  "    [Created] [DATETIME] NULL,"
                +  "    [CreatedBy] [NVARCHAR] (20) NULL,"
                +  "    [Updated] [DATETIME] NULL,"
                +  "    [UpdatedBy] [NVARCHAR] (20) NULL,"
                +  "    [Modified] AS(ISNULL([Updated],[Created])),"
                +  "    [ModifiedBy] AS(ISNULL([UpdatedBy],[CreatedBy])),"
                + $"	 CONSTRAINT[PK_{SettingTableName}] PRIMARY KEY CLUSTERED ([SettingRef] ASC)"
                +  "    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY])"
                +  "    ON [PRIMARY]"
                +  "END";

            var sqlCmd = new SqlCommand(createTable, sqlConn);
            sqlCmd.ExecuteNonQuery();

            // Set Setting Stored Procedure
            var createSetStoredProc =
                  $"IF OBJECT_ID(N'[{StoredProcSetSetting}]') IS NULL"
                +  "BEGIN"
                + $"  EXEC('CREATE PROCEDURE [{StoredProcSetSetting}] AS SELECT 1');"
                +  "END"
                +  ""
                + $"ALTER PROCEDURE [{StoredProcSetSetting}]"
                +  "  -- Parameters"
                +  "  @SettingName AS NVARCHAR(50),"
                +  "  @SettingValue AS NVARCHAR(255),"
                +  "  @PortalId AS BIGINT,"
                +  "  @ModuleId AS BIGINT,"
                +  "  @UserId AS BIGINT,"
                +  "  @CacheDuration AS INT,"
                +  "  @By AS NVARCHAR(20)"
                +  "AS"
                +  "  BEGIN"
                +  "    -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements."
                +  "    SET NOCOUNT ON;"
                +  "    SELECT @SettingName = LOWER(@SettingName);"
                +  "    DECLARE @SettingRef BIGINT;"
                + $"    SELECT TOP 1 @SettingRef = Setting.SettingRef FROM [{SettingTableName}] AS Setting"
                +  "      WHERE Setting.SettingName = @SettingName"
                +  "        AND ((@PortalId IS NULL AND Setting.PortalId IS NULL) OR(@PortalId IS NOT NULL AND Setting.PortalId = @PortalId))"
                +  "        AND ((@ModuleId IS NULL AND Setting.ModuleId IS NULL) OR(@ModuleId IS NOT NULL AND Setting.ModuleId = @ModuleId))"
                +  "        AND ((@UserId IS NULL AND Setting.UserId IS NULL) OR(@UserId IS NOT NULL AND Setting.UserId = @UserId))"
                +  "      ORDER BY Setting.SettingRef;"
                +  "    IF (@SettingRef IS NULL)"
                +  "       BEGIN"
                + $"        INSERT [{SettingTableName}] (SettingName, SettingValue, PortalId, ModuleId, UserId, CacheDuration, CreatedBy, Created)"
                +  "          VALUES(@SettingName, @SettingValue, @PortalId, @ModuleId, @UserId, @CacheDuration, @By, GETDATE());"
                +  "      END"
                +  "    ELSE"
                +  "      BEGIN"
                + $"        UPDATE [{SettingTableName}] SET SettingValue = @SettingValue, UpdatedBy = @By, Updated = GETDATE()"
                +  "          WHERE SettingRef = @SettingRef;"
                +  "      END"
                +  "  END";

            sqlCmd = new SqlCommand(createSetStoredProc, sqlConn);
            sqlCmd.ExecuteNonQuery();

            // Get Setting Stored Procedure
            var createGetStoredProc =
                  $"IF OBJECT_ID(N'[{StoredProcGetSetting}]') IS NULL"
                +  "BEGIN"
                + $"  EXEC('CREATE PROCEDURE [{StoredProcGetSetting}] AS SELECT 1');"
                +  "END"
                +  ""
                + $"ALTER PROCEDURE [{StoredProcGetSetting}]"
                +  "  -- Parameters"
                +  "  @SettingName AS NVARCHAR(50),"
                +  "  @PortalId AS BIGINT,"
                +  "  @ModuleId AS BIGINT,"
                +  "  @UserId AS BIGINT"
                +  "AS"
                +  "  BEGIN"
                +  "    -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements."
                +  "    SET NOCOUNT ON;"
                +  "    SELECT @SettingName = LOWER(@SettingName);"
                +  "    SELECT TOP 1 Setting.SettingRef, Setting.SettingName, Setting.SettingValue, Setting.CacheDuration"
                + $"      FROM {SettingTableName} AS Setting"
                +  "      WHERE Setting.SettingName = @SettingName"
                +  "        AND ((@PortalId IS NULL AND Setting.PortalId IS NULL) OR(@PortalId IS NOT NULL AND Setting.PortalId = @PortalId))"
                +  "        AND ((@ModuleId IS NULL AND Setting.ModuleId IS NULL) OR(@ModuleId IS NOT NULL AND Setting.ModuleId = @ModuleId))"
                +  "        AND ((@UserId IS NULL AND Setting.UserId IS NULL) OR(@UserId IS NOT NULL AND Setting.UserId = @UserId))"
                +  "      ORDER BY Setting.SettingRef DESC;"
                +  "  END";

            sqlCmd = new SqlCommand(createGetStoredProc, sqlConn);
            sqlCmd.ExecuteNonQuery();

            sqlConn.Close();
        }

        public int DaysPassed()
        {
            var modified = Modified ?? DateTime.Now;
            return (DateTime.Now - modified).Days;
        }

        public bool HasExpired(int maxDays = 0)
        {
            return (DaysPassed() >= maxDays);
        }
        #endregion

        #region Functions
        /// <summary>
        /// Get Master Setting at the desired resolution (by Portal or Module or User, or a combination)
        /// </summary>
        /// <param name="settingName">Setting reference name to obtain</param>
        /// <param name="defaultSetting">Default Setting Object if no setting is found</param>
        /// <param name="portalId">Portal ID (Optional)</param>
        /// <param name="moduleId">Module ID (Optional)</param>
        /// <param name="userId">User ID (Optional)</param>
        public void GetSetting(string settingName, Setting defaultSetting, int? portalId = null, int? moduleId = null, int? userId = null)
        {
            try
            {
                // SEARCH
                var sqlConn = Sql.GetSqlConnection();
                var sqlCmd = new SqlCommand(StoredProcGetSetting, sqlConn) { CommandType = CommandType.StoredProcedure };
                sqlCmd.Parameters.Add("@SettingName", SqlDbType.NVarChar).Value = settingName;
                sqlCmd.Parameters.Add("@PortalId", SqlDbType.BigInt).Value = (object)portalId ?? DBNull.Value;
                sqlCmd.Parameters.Add("@ModuleId", SqlDbType.BigInt).Value = (object)moduleId ?? DBNull.Value;
                sqlCmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = (object)userId ?? DBNull.Value;
                sqlConn.Open();
                var sqlReader = sqlCmd.ExecuteReader();

                // RESULTS
                if (sqlReader.Read())
                {
                    SettingRef = Sql.ReadColumn<long>(sqlReader, "SettingRef");
                    SettingName = Sql.ReadColumn<string>(sqlReader, "SettingName");
                    SettingValue = Sql.ReadColumn<string>(sqlReader, "SettingValue");
                    CacheDuration = Sql.ReadColumn<int>(sqlReader, "CacheDuration");
                    Modified = Sql.ReadColumn<DateTime>(sqlReader, "Modified");
                }
                else
                {
                    SettingRef = defaultSetting.SettingRef;
                    SettingName = defaultSetting.SettingName;
                    SettingValue = defaultSetting.SettingValue;
                    CacheDuration = defaultSetting.CacheDuration;
                    Modified = defaultSetting.Modified;
                }
                sqlConn.Close();

            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
        }

        /// <summary>
        /// Get Master Setting at the desired resolution (by Portal or Module or User, or a combination)
        /// </summary>
        /// <param name="settingName">Setting reference name to obtain</param>
        /// <param name="defaultSettingValue">Default setting if no setting is found</param>
        /// <param name="portalId">Portal ID (Optional)</param>
        /// <param name="moduleId">Module ID (Optional)</param>
        /// <param name="userId">User ID (Optional)</param>
        public void GetSetting(string settingName, string defaultSettingValue, int? portalId = null, int? moduleId = null, int? userId = null)
        {
            GetSetting(settingName, new Setting(settingName, defaultSettingValue), portalId, moduleId, userId);
        }

        /// <summary>
        /// Set Master Setting at the desired resolution (by Portal or Module or User, or a combination)
        /// </summary>
        /// <param name="settingName">Setting reference name to assign</param>
        /// <param name="settingValue">Value to assign to the setting</param>
        /// <param name="portalId">Portal</param>
        /// <param name="moduleId">Module</param>
        /// <param name="userId">User</param>
        /// <param name="cacheDuration">Length of cache</param>
        /// <param name="by">User that set the setting</param>
        public void SetSetting(string settingName, string settingValue, int? portalId = null, int? moduleId = null, int? userId = null, int? cacheDuration = null, string by = null)
        {
            var sqlConn = Sql.GetSqlConnection();
            SetSetting(sqlConn, settingName, settingValue, portalId, moduleId, userId, cacheDuration, by);
        }

        /// <summary>
        /// Set Master Settings collection)
        /// </summary>
        /// <param name="settings">List of settings to set</param>
        /// <param name="by">User that set the setting</param>
        public void SetSetting(List<Setting> settings, string by = null)
        {
            var sqlConn = Sql.GetSqlConnection();
            foreach (var setting in settings)
            {
                SetSetting(sqlConn, setting.SettingName, setting.SettingValue, setting.PortalId, setting.ModuleId, setting.UserId, setting.CacheDuration, by);
            }
        }

        /// <summary>
        /// Set Master Setting at the desired resolution (by Portal or Module or User, or a combination)
        /// </summary>
        /// <param name="sqlConn">SQL Connection</param>
        /// <param name="settingName">Setting reference name to assign</param>
        /// <param name="settingValue">Value to assign the setting</param>
        /// <param name="portalId">Portal</param>
        /// <param name="moduleId">Module</param>
        /// <param name="userId">User</param>
        /// <param name="cacheDuration">Length of cache</param>
        /// <param name="by">User that set the setting</param>
        public void SetSetting(SqlConnection sqlConn, string settingName, string settingValue, int? portalId = null, int? moduleId = null, int? userId = null, int? cacheDuration = null, string by = null)
        {
            try
            {
                if (string.IsNullOrEmpty(by)) { by = Info.GetCurrentUserName(); }
                var sqlCmd = new SqlCommand(StoredProcSetSetting, sqlConn) { CommandType = CommandType.StoredProcedure };
                sqlCmd.Parameters.Add("@SettingName", SqlDbType.NVarChar).Value = settingName;
                sqlCmd.Parameters.Add("@SettingValue", SqlDbType.NVarChar).Value = settingValue;
                sqlCmd.Parameters.Add("@PortalId", SqlDbType.Int).Value = (object)portalId ?? DBNull.Value;
                sqlCmd.Parameters.Add("@ModuleId", SqlDbType.Int).Value = (object)moduleId ?? DBNull.Value;
                sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = (object)userId ?? DBNull.Value;
                sqlCmd.Parameters.Add("@CacheDuration", SqlDbType.Int).Value = (object)cacheDuration ?? DBNull.Value;
                sqlCmd.Parameters.Add("@By", SqlDbType.NVarChar).Value = by;
                sqlConn.Open();
                sqlCmd.ExecuteNonQuery();
                sqlConn.Close();
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
        }
        #endregion
    }
}
