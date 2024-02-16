/*
 * Author: Jan Kopidol
 */

namespace Blazor_app.Services
{
    /// <summary>
    /// Service to manage the code editor state
    /// </summary>
    public class CodeEditorService
    {
        // Property bound to the code editor
        public string Code { get; set; } = "";
        // Property to store the compiled code
        public string CompiledCode { get; set; } = "";
        // Property to store the current state of the editor
        public bool EditMode { get; set; } = true;
        // Property to store the current executing line
        public int CurrentExecutingLine { get; set; } = -1;
        // Property to store the compiled lines, with the line number as the key. Used to highlight the executing line
        public Dictionary<int, string> CompiledLines { get; private set; } = new Dictionary<int, string>();
        // Event to notify the editor state change and call the UI update
        public event Action EditorStateChanged;
        // Property to store the breakpoints
        public HashSet<int> Breakpoints = new HashSet<int>();

        public CodeEditorService()
        {
            EditorStateChanged += () => { };
        }

        public void ChangeCode(string code)
        {
            Code = code;
            EditorStateChanged?.Invoke();
        }
        // Change the editor state to view mode (read-only, and showing the current executing line). Logic to keep the whitespace in html.
        public void CodeToViewMode(int setExecutingLine = -1)
        {
            EditMode = false;
            CompiledLines = Code.Split('\n')
                                .Select((line, index) => new KeyValuePair<int, string>(index, line += " "))
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            CompiledCode = new string(Code);
            CurrentExecutingLine = setExecutingLine;
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
