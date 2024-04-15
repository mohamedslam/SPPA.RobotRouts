namespace Stimulsoft.System.Data
{
    internal static class Strings
    {
        public static string DataSetLinq_InvalidEnumerationValue(string name, string value)
        {
            return $"The {name} enumeration value, {value}, is not valid.";
        }
        
        public static string DataSetLinq_EmptyDataRowSource => "The source contains no DataRows.";

        public static string DataSetLinq_NullDataRow => "The source contains a DataRow reference that is null.";

        public static string DataSetLinq_CannotLoadDeletedRow => "The source contains a deleted DataRow that cannot be copied to the DataTable.";

        public static string DataSetLinq_CannotLoadDetachedRow => "The source contains a detached DataRow that cannot be copied to the DataTable.";

        public static string DataSetLinq_CannotCompareDeletedRow => "The DataRowComparer does not work with DataRows that have been deleted since it only compares current values.";

        public static string DataSetLinq_NonNullableCast(string type)
        {
            return $"Cannot cast DBNull.Value to type '{type}'. Please use a nullable type.";
        }

        public static string ToLDVUnsupported => "Can not create DataView after using projection";

        public static string LDV_InvalidNumOfKeys(string count)
        {
            return $"Must provide '{count}' keys to find";
        }

        public static string LDVRowStateError => "DataViewRowState must be DataViewRowState.CurrentRows";
    }
}
