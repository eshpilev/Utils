using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
using System.Text;

namespace Utils.Helpers
{
    /// <summary> Генерация SQL скрипта создания таблиц из csv файла </summary>
    public class CreatorSqlScripts
    {
        private const string Dir = "c:\\Work";
        private const string SchemaName = "fedres";
        private const string DescCsvFile = $"{Dir}\\Descriptions.csv";
        private const string CreatorFile = $"{Dir}\\Creator.txt";
        private const string RollbackFile = $"{Dir}\\Rollback.txt";
        private const string TableDetector = "#";
        private const string Marked = "1";
        private Stack<string> _tables;
        private StringBuilder _fields;
        private StringBuilder _constaintPk;
        private StringBuilder _constaintFk;

        public static void Run()
        {
            new CreatorSqlScripts().Process();
        }

        public void Process()
        {
            using var writer = new StreamWriter(CreatorFile);
            _tables = new Stack<string>();
            _fields = new StringBuilder();
            _constaintPk = new StringBuilder();
            _constaintFk = new StringBuilder();
            var descriptions = ReadCsv();
            var table = string.Empty;
            foreach (var desc in descriptions)
            {
                if (desc.Name.StartsWith(TableDetector))
                {
                    if (_tables.Count > 0)
                        AddEndTable(writer);
                    table = desc.Name.Replace(TableDetector, "");
                    AddStartTable(writer, table);
                    _tables.Push(table);
                }
                else
                {
                    if (_fields.Length > 0) _fields.Append(",");
                    _fields.Append($"\n\t{desc.Name} {SqlType(desc.Type)} {SqlNull(desc.Mandatory)}");
                    if (!string.IsNullOrEmpty(desc.Identity)) _fields.Append($" {SqlIdentity(desc.Identity)}");
                    if (desc.PK == Marked)
                    {
                        if (_constaintPk.Length > 0)
                        {
                            _constaintPk.Replace(")", $", {desc.Name})");
                        }
                        else
                            _constaintPk.Append($"\n\tCONSTRAINT {table}_pkey PRIMARY KEY ({desc.Name})");
                    }
                    if (!string.IsNullOrEmpty(desc.FK))
                    {
                        if (_constaintFk.Length > 0) _constaintFk.Append(",");
                        _constaintFk.Append($"\n\tCONSTRAINT fk_{table}_{desc.Name} FOREIGN KEY ({desc.Name})");
                        _constaintFk.Append($"\n\t\tREFERENCES {SchemaName}.{desc.FK}");
                        if (!string.IsNullOrEmpty(desc.DeleteAction))
                            _constaintFk.Append($"\n\t\tON UPDATE {desc.DeleteAction} ON DELETE {desc.DeleteAction}");
                    }
                }
            }
            if (_tables.Count > 0)
            {
                AddEndTable(writer);
                using var writerRollback = new StreamWriter(RollbackFile);
                foreach (var tableName in _tables)
                {
                    writerRollback.WriteLine($"DROP TABLE {SchemaName}.{tableName} CASCADE;");
                }
            }
        }
        private void AddStartTable(TextWriter writer, string table)
        {
            _fields.Clear();
            _constaintPk.Clear();
            _constaintFk.Clear();
            writer.Write($"CREATE TABLE {SchemaName}.{table}(");
        }
        private void AddEndTable(TextWriter writer)
        {
            writer.Write(_fields);
            if (_constaintPk.Length > 0)
                writer.Write($",{_constaintPk}");
            if (_constaintFk.Length > 0)
                writer.Write($",{_constaintFk}");
            writer.WriteLine("\n);\n");
        }
        private IEnumerable<DescriptionFields> ReadCsv()
        {
            using var reader = new StreamReader(DescCsvFile);
            using var csvReader = new CsvReader(reader, new CsvConfiguration(new CultureInfo("ru-RU"))
            {
                Delimiter = ";",
                HasHeaderRecord = true
            });
            return csvReader.GetRecords<DescriptionFields>().ToList();
        }
        private static string SqlType(string value) =>
            value.Replace("string", "varchar").Replace("date", "timestamp");
        private static string SqlNull(string value) => value == Marked ? "NOT NULL" : "NULL";
        private static string SqlIdentity(string value) => value == Marked ? "GENERATED ALWAYS AS IDENTITY" : "";
    }
    public class DescriptionFields
    {
        [Name("Name")]
        public string Name { get; set; }
        [Name("Type")]
        public string Type { get; set; }
        [Name("Mandatory")]
        public string Mandatory { get; set; }
        [Name("Identity")]
        public string Identity { get; set; }
        [Name("PK")]
        public string PK { get; set; }
        [Name("FK")]
        public string FK { get; set; }
        [Name("Delete Action")]
        public string DeleteAction { get; set; }
    }
}
