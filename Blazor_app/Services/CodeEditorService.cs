namespace Blazor_app.Services
{
    public class CodeEditorService
    {
        public string Code { get; set; } = "";
        public event Action SaveCode;

        public void Save()
        {
            SaveCode?.Invoke();
        }
    }
}
