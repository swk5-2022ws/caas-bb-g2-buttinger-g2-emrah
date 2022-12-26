using System.Data;

namespace CaaS.Common.Mappings
{
    public static class BaseMappingMethods
    {
        public static int GetIntByName(this IDataRecord record, string propertyName) =>
           (int)record[propertyName];
        public static uint GetUIntByName(this IDataRecord record, string propertyName) =>
           Convert.ToUInt32((int)record[propertyName]);
        public static int? GetNullableIntByName(this IDataRecord record, string propertyName) =>
           record[propertyName] == DBNull.Value ? null : Convert.ToInt32(record[propertyName]);
        public static string GetStringByName(this IDataRecord record, string propertyName) =>
            (string)record[propertyName];
        public static string? GetNullableStringByName(this IDataRecord record, string propertyName) =>
            record[propertyName] == DBNull.Value ? null : (string)record[propertyName];
        public static double GetDoubleByName(this IDataRecord record, string propertyName) =>
            (double)record[propertyName];
        public static Guid GetGuidByName(this IDataRecord record, string propertyName) =>
            (Guid)record[propertyName];
        public static DateTime GetDateTimeByName(this IDataRecord record, string propertyName) =>
          Convert.ToDateTime(record[propertyName]);
        public static DateTime? GetNullableDateTimeByName(this IDataRecord record, string propertyName) =>
            record[propertyName] == DBNull.Value ? null : Convert.ToDateTime(record[propertyName]); 
    }
}
