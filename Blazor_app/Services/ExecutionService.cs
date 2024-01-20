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
                if (_pramMachine.IsCrashed)
                {
                    ShowPopup(_pramMachine.ExecutionErrorMessage!);
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

        public bool IsRunning { get; private set; }

        private void Step()
        {

        }

        public void Start()
        {

        }

        public void Stop()
        {

        }
    }
}
