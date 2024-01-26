﻿using Microsoft.AspNetCore.Components;
using PRAM_lib.Machine;
using System.Collections.ObjectModel;
using System.Reflection.PortableExecutable;

namespace Blazor_app.Services
{
    public class ExecutionService
    {
        private readonly PramMachine _pramMachine;
        private readonly CodeEditorService _codeEditorService;
        private readonly PramCodeViewService _pramCodeViewService;
        private readonly NavigationManager _navigationManager;
        private readonly GlobalService _globalService;
        private readonly HistoryMemoryService _historyMemoryService;

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
            GlobalService globalService,
            HistoryMemoryService historyMemoryService)
        {
            _pramMachine = pramMachine;
            _codeEditorService = codeEditorService;
            _pramCodeViewService = pramCodeViewService;
            _navigationManager = navigationManager;
            _globalService = globalService;
            _historyMemoryService = historyMemoryService;
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

        // Logic for UI in machine step, only for manual stepping or after auto step is done
        private void InteractiveMachineStep()
        {

            int currentLine;
            _globalService.SetLastState($"Next step: {_pramMachine.MPIP.Value.ToString()}", GlobalService.LastStateUniform.Ok);
            currentLine = _pramMachine.GetCurrentCodeLineIndex();

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

            _codeEditorService.UpdateExecutingLine(currentLine);
            RefreshMemory();
        }

        public bool StepMachine(bool manual = true)
        {
            bool result = _pramMachine.ExecuteNextInstruction();
            if (!result)
            {
                //Stop auto run if machine is in bad state
                StopAutoRun();

                if (_pramMachine.IsCrashed)
                {
                    //View switching logic
                    if (IsRunningParallel && !_pramMachine.IsRunningParallel)
                    {
                        _navigationManager.NavigateTo("/");
                        ShowPopup?.Invoke(_pramMachine.ExecutionErrorMessage ?? string.Empty);
                    }
                    else if(!IsRunningParallel && _pramMachine.IsRunningParallel)
                    {
                        _pramCodeViewService.SetPramCode(_pramMachine.GetCurrentParallelMachineCode() ?? "No code");
                        _navigationManager.NavigateTo("/pramview");
                    }
                    else if(IsRunningParallel && _pramMachine.IsRunningParallel)
                    {
                        RefreshPramCode();
                    }
                    else if (!IsRunningParallel)
                    {
                        ShowPopup?.Invoke(_pramMachine.ExecutionErrorMessage ?? string.Empty);
                    }

                    _globalService.SetLastState($"Execution stop: {_pramMachine.ExecutionErrorMessage}", GlobalService.LastStateUniform.Error);
                }
                else
                {
                    //not shown if machine is halted, since the if for 'halt' will override
                    _globalService.SetLastState($"Execution stop: {_pramMachine.ExecutionErrorMessage}", GlobalService.LastStateUniform.Warning);
                }

                int currentLine;
                if (_pramMachine.ExecutionErrorLineIndex != null)
                {
                    currentLine = (int)_pramMachine.ExecutionErrorLineIndex;
                }
                else
                {
                    currentLine = -1;
                }

                _codeEditorService.UpdateExecutingLine(currentLine);
                RefreshMemory();
            }

            if (manual && result)
            {
                InteractiveMachineStep();
            }

            return result;
        }

        public void ResetMachine()
        {
            _pramMachine.Restart();
            _globalService.SetLastState("Reset", GlobalService.LastStateUniform.Ok);
            _codeEditorService.UpdateExecutingLine(-1);
            ResetParallelRunningState();
            _navigationManager.NavigateTo("/");
        }

        public void ClearMemories()
        {
            _pramMachine.ClearMemory();
            RefreshMemory();
            _globalService.SetLastState("Memory cleared", GlobalService.LastStateUniform.Ok);
        }

        //Unused
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
            if (!IsAutoRunning)
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
        // ----------------------------------------------------

        //## Instant run #####################################

        public void RunUntilBreakpoint()
        {
            bool result = false;
            for (int i = 0; i < 10000; i++)
            {
                // Stop if machine is in bad state
                result = StepMachine(false);
                if (!result)
                {
                    break;
                }

                //Check if breakpoints exist
                if (_codeEditorService.Breakpoints.Count > 0)
                {
                    //Stop if breakpoint in RAM is hit
                    if (_codeEditorService.Breakpoints.Contains(_pramMachine.GetCurrentCodeLineIndex()))
                    {
                        break;
                    }

                    bool isParallelBreakpoint = false;
                    if (_pramMachine.IsRunningParallel)
                    {
                        int lastRamCodeLineIndex = _pramMachine.PreviousCodeLineIndex;
                        //Add all parallel machines code line indexes to the ram code line index
                        for (int j = 0; j < _pramMachine.ParallelMachinesCount; j++)
                        {
                            int parallelMachineCodeLineIndex = _pramMachine.GetParallelMachineCodeLineIndex(j) ?? 0;
                            int parallelMachineRamCodeLineIndex = parallelMachineCodeLineIndex + lastRamCodeLineIndex + 1;

                            //Stop if breakpoint in parallel machine RAM is hit
                            if (_codeEditorService.Breakpoints.Contains(parallelMachineRamCodeLineIndex))
                            {
                                isParallelBreakpoint = true;
                                break;
                            }
                        }
                    }
                    if (isParallelBreakpoint)
                    {
                        break;
                    }
                }
            }

            if (result)
            {
                InteractiveMachineStep();
            }
        }

        // ----------------------------------------------------

        // ## History #########################################

        public ObservableCollection<PRAM_lib.Memory.MemoryCell> GetMemoryContextInput()
        {
            return _pramMachine.GetInputMemory();
        }

        public ObservableCollection<PRAM_lib.Memory.MemoryCell> GetMemoryContextOutput()
        {
            return _pramMachine.GetOutputMemory();
        }

        public ObservableCollection<PRAM_lib.Memory.MemoryCell> GetMemoryContextShared()
        {
            return _pramMachine.GetSharedMemory();
        }
    }
}
