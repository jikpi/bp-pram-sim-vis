namespace Blazor_app.Services
{
    public class CodeEditorService
    {
        public string Code { get; set; } = "";
        public bool EditMode { get; set; } = true;
        public int CurrentExecutingLine { get; set; } = -1;
        public Dictionary<int, string> CompiledLines { get; private set; } = new Dictionary<int, string>();

        public event Action EditorStateChanged;

        public void ChangeCode(string code)
        {
            Code = code;
            EditorStateChanged?.Invoke();
        }

        public void CodeToCompiledMode()
        {
            EditMode = false;
            CompiledLines = Code.Split('\n')
                                .Select((line, index) => new KeyValuePair<int, string>(index, line.Replace("\r", " ")))
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            EditorStateChanged?.Invoke();
        }


        public void EditCode()
        {
            EditMode = true;
            CurrentExecutingLine = -1;
            EditorStateChanged?.Invoke();
        }

        public void UpdateExecutingLine(int lineNumber)
        {
            CurrentExecutingLine = lineNumber;
            EditorStateChanged?.Invoke();
        }

        public void CancelExecution()
        {
            CurrentExecutingLine = -1;
            EditorStateChanged?.Invoke();
        }
    }
}
