namespace Blazor_app.Services
{
    public class CodeEditorService
    {
        public string Code { get; set; } = "";
        public event Action SaveCode;
        public event Action <string> CodeChanged;

        public void Save()
        {
            SaveCode?.Invoke();
        }

        public void ChangeCode(string code)
        {
            Code = code;
            CodeChanged?.Invoke(code);
        }
    }
}
