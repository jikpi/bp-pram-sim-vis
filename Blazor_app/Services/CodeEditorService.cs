namespace Blazor_app.Services
{
    public class CodeEditorService
    {
        public string Code { get; set; } = "";
        public string CompiledCode { get; set; } = "";
        public bool EditMode { get; set; } = true;
        public int CurrentExecutingLine { get; set; } = -1;
        public Dictionary<int, string> CompiledLines { get; private set; } = new Dictionary<int, string>();

        public event Action EditorStateChanged;

        public HashSet<int> Breakpoints = new HashSet<int>();

        public void ChangeCode(string code)
        {
            Code = code;
            EditorStateChanged?.Invoke();
        }

        public void CodeToCompiledMode()
        {
            EditMode = false;
            CompiledLines = Code.Split('\n')
                                .Select((line, index) => new KeyValuePair<int, string>(index, line += " "))
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            CompiledCode = new string(Code);
            CurrentExecutingLine = -1;
            Breakpoints.Clear();

            EditorStateChanged?.Invoke();
        }


        public void EditCode()
        {
            EditMode = true;
            EditorStateChanged?.Invoke();
        }

        public void UpdateExecutingLine(int lineNumber)
        {
            CurrentExecutingLine = lineNumber;
            EditorStateChanged?.Invoke();
        }

        public void CancelEditMode()
        {
            EditMode = false;
            Code = CompiledCode;
            EditorStateChanged?.Invoke();
        }
    }
}
