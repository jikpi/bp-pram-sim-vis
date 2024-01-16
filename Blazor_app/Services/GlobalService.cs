﻿namespace Blazor_app.Services
{
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

        public string TestedCode { get; private set; } = "#Nastavit 5 jako vstup\r\n#Cteni\r\nS0 := READ()\r\n#Prirazeni\r\nS1 := S0\r\n#Prirazeni hodnoty a operace\r\nS2 := S1 + 3\r\nS3 := S1 - 3\r\nS4 := S1 * 3\r\nS5 := S1 / 3\r\nS6 := S1 % 3\r\n\r\n#Zapis do vystupu\r\nWRITE(23)\r\nWRITE(S1)\r\nWRITE(S1 + 2)\r\nWRITE([S1])\r\n\r\n#Prirazeni hodnoty naopak\r\nS7 := 3 + S7\r\nS8 := 3 - S7\r\nS9 := 3 * S7\r\nS10 := 3 / S7\r\nS11 := 3 % S7\r\n\r\n#Prirazeni hodnoty jako vysledek 2 bunek\r\nS12 := S0 + S1\r\nS13 := S0 - S1\r\nS14 := S0 * S1\r\nS15 := S0 / S1\r\nS16 := S0 % S1\r\n\r\n#Pointery\r\nS17 := [S2]\r\n[S17] := 1\r\n[S18] := S17\r\n\r\n#Skoky\r\ngoto :skok1\r\nS19 := S19 / 0\r\n:skok1\r\nif (S0 == S8) goto :skoks2\r\nS19 := S19 / 0\r\n:skoks2\r\n\r\nif (S0 != -191919) goto :skok2\r\nS19 := S19 / 0\r\n:skok2\r\nif (S0 != -1) goto :skok3\r\nS19 := S19 / 0\r\n:skok3\r\n\r\nS20 := 5\r\nS21 := 6\r\nif (S20 >= S21) goto :skok4\r\ngoto :skok5\r\n:skok4\r\nS0 := S0 / 0\r\n:skok5\r\n\r\nif (S21 <= S20) goto :skok6\r\ngoto :skok7\r\n:skok6\r\nS0 := S0 / 0\r\n:skok7\r\n\r\nif (S20 > S21) goto :skok8\r\ngoto :skok9\r\n:skok8\r\nS0 := S0 / 0\r\n:skok9\r\n\r\nif (S21 < S20) goto :skok10\r\ngoto :skok11\r\n:skok10\r\nS0 := S0 / 0\r\n:skok11\r\n\r\n:true\r\nS5 := 1\r\ngoto :end\r\n:end\r\nWRITE(0)\r\n\r\n";
        
    }
}
