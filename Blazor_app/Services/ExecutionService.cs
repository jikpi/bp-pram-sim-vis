using Microsoft.AspNetCore.Components;
using PRAM_lib.Machine;

namespace Blazor_app.Services
{
    public class ExecutionService
    {
        private readonly PramMachine _pramMachine;
        private readonly CodeEditorService _codeEditorService;
        private readonly PramCodeViewService _pramCodeViewService;
        private readonly NavigationManager _navigationManager;
        private readonly GlobalService _globalService;

        public event Action MemoryRefreshed;
        public event Action PramCodeRefreshed;
        public event Action<string> ShowPopup;


        private Timer _timer;
        public bool IsAutoRunning { get; private set; } = false;
        private int _autoRunInterval = 1000;





        public ExecutionService(PramMachine pramMachine,
            CodeEditorService codeEditorService,
            PramCodeViewService pramCodeViewService,
            NavigationManager navigationManager,
            GlobalService globalService)
        {
            _pramMachine = pramMachine;
            _codeEditorService = codeEditorService;
            _pramCodeViewService = pramCodeViewService;
            _navigationManager = navigationManager;
            _globalService = globalService;
            MemoryRefreshed += () => { };
            PramCodeRefreshed += () => { };
            ShowPopup += (message) => { };

            _timer = new Timer(TimerCallback, null, Timeout.Infinite, _autoRunInterval);
        }


        //## Parallel run state #############################
        public bool IsRunningParallel { get; set; } = false;
        //## Reset parallel running service state
        public void ResetParallelRunningState()
        {
            IsRunningParallel = false;
        }
        //---------------------------------------------

        public void RefreshMemory()
        {
            MemoryRefreshed?.Invoke();
        }

        public void RefreshPramCode()
        {
            PramCodeRefreshed?.Invoke();
        }

        public void StepMachine()
        {
            bool result = _pramMachine.ExecuteNextInstruction();
            if (!result)
            {
                //Stop auto run if machine is in bad state
                StopAutoRun();

                if (_pramMachine.IsCrashed)
                {
                    ShowPopup?.Invoke(_pramMachine.ExecutionErrorMessage ?? string.Empty);

                    _globalService.SetLastState($"Execution stop: {_pramMachine.ExecutionErrorMessage}", GlobalService.LastStateUniform.Error);
                }
                else
                {
                    //not shown if machine is halted, since the if for 'halt' will override
                    _globalService.SetLastState($"Execution stop: {_pramMachine.ExecutionErrorMessage}", GlobalService.LastStateUniform.Warning);
                }
            }
            else
            {
                _globalService.SetLastState($"Next step: {_pramMachine.MPIP.Value.ToString()}", GlobalService.LastStateUniform.Ok);
                int currentLine = _pramMachine.GetCurrentCodeLineIndex();
                _codeEditorService.UpdateExecutingLine(currentLine);
            }

            RefreshMemory();

            if (_pramMachine.IsRunningParallel)
            {
                if (IsRunningParallel == false)
                {
                    _globalService.SetLastState("Parallel execution started", GlobalService.LastStateUniform.Note);
                    IsRunningParallel = true;
                    _pramCodeViewService.SetPramCode(_pramMachine.GetCurrentParallelMachineCode() ?? "No code");
                    _navigationManager.NavigateTo("/pramview");
                }

                RefreshPramCode();
            }
            else
            {
                if (IsRunningParallel == true)
                {
                    _globalService.SetLastState("Parallel execution stopped", GlobalService.LastStateUniform.Note);
                    IsRunningParallel = false;
                    _navigationManager.NavigateTo("/");
                }

                if (_pramMachine.IsHalted)
                {
                    _globalService.SetLastState("Execution finished", GlobalService.LastStateUniform.Note);
                }
            }
        }

        public void ResetMachine()
        {
            _pramMachine.Restart();
            _globalService.SetLastState("Reset", GlobalService.LastStateUniform.Ok);
            _codeEditorService.UpdateExecutingLine(-1);
            ResetParallelRunningState();
        }

        public void ClearMemories()
        {
            _pramMachine.ClearMemory();
            RefreshMemory();
            _globalService.SetLastState("Memory cleared", GlobalService.LastStateUniform.Ok);
        }

        public void Clear()
        {
            _pramMachine.Clear();
            _globalService.SetLastState("Cleared all", GlobalService.LastStateUniform.Ok);
            ResetParallelRunningState();
            RefreshMemory();
            _codeEditorService.UpdateExecutingLine(-1);
        }

        //## Auto run ########################################
        private void TimerCallback(object? state)
        {
            if(!IsAutoRunning)
            {
                return;
            }
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            StepMachine();
            _timer.Change(_autoRunInterval, _autoRunInterval);
        }

        public void StartAutoRun()
        {
            if (IsAutoRunning)
            {
                return;
            }

            IsAutoRunning = true;
            _timer.Change(0, _autoRunInterval);
        }

        public void StopAutoRun()
        {
            if (!IsAutoRunning)
            {
                return;
            }

            IsAutoRunning = false;
            _timer.Change(Timeout.Infinite, _autoRunInterval);
        }

        public void ToggleAutoRun()
        {
            if (IsAutoRunning)
            {
                StopAutoRun();
            }
            else
            {
                StartAutoRun();
            }
        }
    }
}
