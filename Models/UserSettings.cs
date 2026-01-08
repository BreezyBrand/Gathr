namespace Gathr.Models
{
    public class UserSettings
    {
        public int Id { get; set; }
        public string Option { get; set; }
        public string Value { get; set; }
    }

    public class GoogleSheetExtractView
    {
        public string SpreadsheetId { get; set; }
        public string SheetRange { get; set; }
    }
}
