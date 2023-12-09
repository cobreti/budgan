namespace Budgan.Options;

public class CsvSettings
{
    public Dictionary<string, FileLayout>? Layouts { get; set; }

    public int? MinColumnsRequired { get; set; }
}
