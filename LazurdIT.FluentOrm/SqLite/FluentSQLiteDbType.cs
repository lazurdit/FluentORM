namespace LazurdIT.FluentOrm.SQLite
{
    public static class FluentSQLiteDbType
    {
        public const int Integer = 0;       // Maps to BigInt, Int, SmallInt, TinyInt, etc.
        public const int Blob = 1;          // Maps to Binary, VarBinary, Image
        public const int Boolean = 2;       // Maps to Bit
        public const int Text = 3;          // Maps to Char, NChar, NText, NVarChar, Text, VarChar
        public const int Real = 4;          // Maps to Decimal, Float, Money, SmallMoney
        public const int Date = 5;          // Maps to Date, DateTime, DateTime2, SmallDateTime
        public const int Timestamp = 6;     // Maps to Timestamp
        public const int UniqueIdentifier = 7; // Maps to UniqueIdentifier (GUID in SQLite)
        public const int DateTimeOffset = 8; // Maps to DateTimeOffset
        public const int Null = 9;          // Maps to Variant, Xml, Udt, Structured, etc.
    }
}