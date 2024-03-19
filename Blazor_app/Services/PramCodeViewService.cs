/*
 * Author: Jan Kopidol
 */

using Blazor_app.Assets;

namespace Blazor_app.Services
{
    /// <summary>
    /// Service for managing some PRAM UI state
    /// </summary>
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

            //Limit the string size
            foreach (var kvp in PramLines)
            {
                if (kvp.Value.Length > ProjectConfig.MaxPramViewCodeEditorLineSize)
                {
                    PramLines[kvp.Key] = kvp.Value.Substring(0, ProjectConfig.MaxPramViewCodeEditorLineSize) + "..";
                }
            }
        }

        //A property that decides whether to show halted parallel machines
        public bool ShowHaltedParallelMachines { get; set; } = false;

        // ## Selective PRAM view 
        public SortedSet<int> SelectedViewParallelMachines { get; private set; } = new SortedSet<int>();

        // Method to parse numbers and ranges from the input string
        public void SelectParallelMachines(string lines)
        {
            if (!lines.StartsWith("+"))
            {
                SelectedViewParallelMachines.Clear();
            }
            else
            {
                lines = lines.Substring(1);
            }

            try
            {
                string[] parts = lines.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string part in parts)
                {
                    if (part.StartsWith("odd") || part.StartsWith("even"))
                    {
                        bool isOdd = part.StartsWith("odd");
                        string rangePart = part.Substring(part.IndexOf(' ') + 1);
                        string[] rangeParts = rangePart.Split('-');
                        int start = int.Parse(rangeParts[0]);
                        int end = int.Parse(rangeParts[1]);

                        if (isOdd && start % 2 == 0) start++;
                        if (!isOdd && start % 2 != 0) start++;

                        for (int i = start; i <= end; i += 2)
                        {
                            SelectedViewParallelMachines.Add(i);
                        }
                    }
                    else if (part.Contains('-'))
                    {
                        string[] rangeParts = part.Split('-');
                        int start = int.Parse(rangeParts[0]);
                        int end = int.Parse(rangeParts[1]);

                        for (int i = start; i <= end; i++)
                        {
                            SelectedViewParallelMachines.Add(i);
                        }
                    }
                    else
                    {
                        SelectedViewParallelMachines.Add(int.Parse(part));
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
