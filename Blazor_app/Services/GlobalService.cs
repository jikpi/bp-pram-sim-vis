namespace Blazor_app.Services
{
    //Service for settings and global state
    public class GlobalService
    {
        //## State text
        public string LastState { get; private set; } = "None";
        public enum LastStateUniform { Ok, Warning, Note, Error }
        public LastStateUniform LastStateType { get; private set; } = LastStateUniform.Ok;

        public event Action LastStateUpdated;
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

        public GlobalService()
        {
            ShowPopup += (message) => { };
            LastStateUpdated += () => { };
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

        //

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



        //## Test code
        public string TestedCode { get; private set; } = "#Nastavit 5 jako vstup\r\n#Cteni\r\nS0 := READ()\r\n#Prirazeni\r\nS1 := S0\r\n#Prirazeni hodnoty a operace\r\nS2 := S1 + 3\r\nS3 := S1 - 3\r\nS4 := S1 * 3\r\nS5 := S1 / 3\r\nS6 := S1 % 3\r\n\r\n#Zapis do vystupu\r\nWRITE(23)\r\nWRITE(S1)\r\nWRITE(S1 + 2)\r\nWRITE([S1])\r\n\r\n#Prirazeni hodnoty naopak\r\nS7 := 3 + S7\r\nS8 := 3 - S7\r\nS9 := 3 * S7\r\nS10 := 3 / S7\r\nS11 := 3 % S7\r\n\r\n#Prirazeni hodnoty jako vysledek 2 bunek\r\nS12 := S0 + S1\r\nS13 := S0 - S1\r\nS14 := S0 * S1\r\nS15 := S0 / S1\r\nS16 := S0 % S1\r\n\r\n#Pointery\r\nS17 := [S2]\r\n[S17] := 1\r\n[S18] := S17\r\n\r\n#Skoky\r\ngoto :skok1\r\nS19 := S19 / 0\r\n:skok1\r\nif (S0 == S8) goto :skoks2\r\nS19 := S19 / 0\r\n:skoks2\r\n\r\nif (S0 != -191919) goto :skok2\r\nS19 := S19 / 0\r\n:skok2\r\nif (S0 != -1) goto :skok3\r\nS19 := S19 / 0\r\n:skok3\r\n\r\nS20 := 5\r\nS21 := 6\r\nif (S20 >= S21) goto :skok4\r\ngoto :skok5\r\n:skok4\r\nS0 := S0 / 0\r\n:skok5\r\n\r\nif (S21 <= S20) goto :skok6\r\ngoto :skok7\r\n:skok6\r\nS0 := S0 / 0\r\n:skok7\r\n\r\nif (S20 > S21) goto :skok8\r\ngoto :skok9\r\n:skok8\r\nS0 := S0 / 0\r\n:skok9\r\n\r\nif (S21 < S20) goto :skok10\r\ngoto :skok11\r\n:skok10\r\nS0 := S0 / 0\r\n:skok11\r\n\r\n:true\r\nS5 := 1\r\ngoto :end\r\n:end\r\nWRITE(0)\r\n\r\n";

        //---------------------------------------------
    }
}
