namespace Blazor_app.Services
{
    public class PramCodeViewService
    {
        public string PramCode { get; set; } = "";
        public Dictionary<int, string> PramLines { get; private set; } = new Dictionary<int, string>();

        public void SetPramCode(string code)
        {
            PramCode = code;
            PramLines = PramCode.Split('\n')
                                .Select((line, index) => new KeyValuePair<int, string>(index, line += " "))
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
