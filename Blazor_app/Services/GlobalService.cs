/*
 * Author: Jan Kopidol
 */

namespace Blazor_app.Services
{
    /// <summary>
    /// Service for settings and global state
    /// </summary>
    public class GlobalService
    {
        //## State text
        public string LastState { get; private set; } = "None";
        public enum LastStateUniform { Ok, Warning, Note, Error }
        public LastStateUniform LastStateType { get; private set; } = LastStateUniform.Ok;

        public event Action LastStateUpdated;

        public GlobalService()
        {
            ShowPopup += (message) => { };
            LastStateUpdated += () => { };
            HistoryToggled += (value) => { };

            SetExampleMachines();
        }

        public void SetLastState(string state, LastStateUniform type)
        {
            LastState = state;
            LastStateType = type;
            LastStateUpdated?.Invoke();
        }
        //---------------------------------------------

        // ## Popup
        public event Action<string> ShowPopup;
        public void ShowPopupMessage(string message)
        {
            ShowPopup?.Invoke(message);
        }

        // ## Auto step speed
        public int AutoStepSpeed { get; private set; } = 1000;
        public int MinAutoStepSpeed { get; private set; } = 200;
        public int MaxAutoStepSpeed { get; private set; } = 2000;
        public void SetAutoStepSpeed(int speed)
        {
            int adjustedSpeed = (int)(Math.Round(speed / 100.0) * 100);

            if (adjustedSpeed < MinAutoStepSpeed)
            {
                AutoStepSpeed = MinAutoStepSpeed;
            }
            else if (adjustedSpeed > MaxAutoStepSpeed)
            {
                AutoStepSpeed = MaxAutoStepSpeed;
            }
            else
            {
                AutoStepSpeed = adjustedSpeed;
            }
        }

        //---------------------------------------------

        // Fixed code toggle
        public bool FixedCode { get; set; } = false;

        public int FixedCodeLength { get; private set; } = 10;
        public int MaxFixedCodeLength { get; private set; } = 20;
        public int MinFixedCodeLength { get; private set; } = 5;
        public void SetFixedCodeLength(int length)
        {
            if (length < MinFixedCodeLength)
            {
                FixedCodeLength = MinFixedCodeLength;
            }
            else if (length > MaxFixedCodeLength)
            {
                FixedCodeLength = MaxFixedCodeLength;
            }
            else
            {
                FixedCodeLength = length;
            }
        }

        //Memory cell hiding
        public bool HideMemoryCells { get; set; } = false;

        //History toggle
        public event Action<bool> HistoryToggled;
        private bool _saveHistory = true;
        public bool SaveHistory
        {
            get => _saveHistory;
            set
            {
                _saveHistory = value;
                HistoryToggled?.Invoke(value);
            }
        }


        // ## Regex state management
        public string DefaultRegex { get; private set; } = "";
        public void SetDefaultRegex(string regex)
        {
            DefaultRegex = regex;
        }

        // ## Machine import management

        public string? LastImportedMachine { get; set; } = null;


        // ## User manual popup
        public bool ShowUserManual { get; set; } = true;



        //## Example code
        public Dictionary<string, string> ExampleMachines = new Dictionary<string, string> { };

        public void SetExampleMachines()
        {
            ExampleMachines = TextUIService.CreateMachineExampleDictionary();
        }

        //---------------------------------------------
    }
}
