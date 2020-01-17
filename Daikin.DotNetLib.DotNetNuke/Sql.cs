using System;
using System.Data.SqlClient;
using DotNetNuke.Common.Utilities;

namespace Daikin.DotNetLib.DotNetNuke
{
    public static class Sql
    {
        public static SqlConnection GetSqlConnection()
        {
            //return new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString);
            //return new SqlConnection(ConfigurationManager.AppSettings["SiteSqlServer"]);           
            return new SqlConnection(Config.GetConnectionString());
        }

        public static T ReadColumn<T>(SqlDataReader sqlDataReader, string columnName)
        {
            int fieldIndex;
            try { fieldIndex = sqlDataReader.GetOrdinal(columnName); } catch { return default(T); }
            if (sqlDataReader.IsDBNull(fieldIndex)) { return default; }
            var readData = sqlDataReader.GetValue(fieldIndex);
            if (readData is T) { return (T)readData; }
            try { return (T)Convert.ChangeType(readData, typeof(T)); } catch (InvalidCastException) { return default; }
        }
    }
}
