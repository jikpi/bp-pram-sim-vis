/*
 * Author: Jan Kopidol
 */

using Microsoft.AspNetCore.Components;
using PRAM_lib.Machine;
using System.Collections.ObjectModel;

namespace Blazor_app.Services
{
    /// <summary>
    /// Service handling the execution of the PRAM machine, and the UI logic during, and all relevant states.
    /// </summary>
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
        public event Action CommonControlsRefreshed;


        private Timer _timer;
        public bool IsAutoRunning { get; private set; } = false;
        private int _autoRunInterval = 1000;

        public int StepsTotal { get; private set; } = 0;
        public int StepsIncludingParallel { get; private set; } = 0;
        private bool ShownErrorMessage { get; set; } = false;



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
            _globalService.HistoryToggled += HistoryToggled;
            CommonControlsRefreshed += () => { };

            _timer = new Timer(TimerCallback, null, Timeout.Infinite, _autoRunInterval);

            _globalService.SetDefaultRegex(_pramMachine.InstructionRegex.SaveToText());
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

        public void RefreshPramView()
        {
            PramCodeRefreshed?.Invoke();
        }

        public void RefreshCommonControls()
        {
            CommonControlsRefreshed?.Invoke();
        }

        public bool CanMachineExecute()
        {
            return _pramMachine.IsCompiled && !_pramMachine.IsHalted && !_pramMachine.IsCrashed;
        }

        // Logic for UI in machine step, is used to update the UI, called after the machine has executed an instruction/s
        private void InteractiveMachineStep()
        {

            int currentLine;

            if (_pramMachine.IsRunningParallel)
            {
                _globalService.SetLastState(TextUIService.StateIndicatorRunningInParallel, GlobalService.LastStateUniform.Ok);
            }
            else
            {
                _globalService.SetLastState(TextUIService.StateIndicatorNextStep(_pramMachine.MPIP.Value), GlobalService.LastStateUniform.Ok);
            }

            currentLine = _pramMachine.GetCurrentCodeLineIndex();


            if (_pramMachine.IsRunningParallel)
            {
                if (IsRunningParallel == false)
                {
                    _globalService.SetLastState(TextUIService.StateIndicatorParallelExecutionStarted, GlobalService.LastStateUniform.Note);
                    IsRunningParallel = true;
                    _pramCodeViewService.SetPramCode(_pramMachine.GetCurrentParallelMachineCode() ?? "No code");
                    _navigationManager.NavigateTo("/pramview");
                }

                RefreshPramView();
            }
            else
            {
                if (IsRunningParallel == true)
                {
                    _globalService.SetLastState(TextUIService.StateIndicatorParallelExecutionStopped, GlobalService.LastStateUniform.Note);
                    IsRunningParallel = false;
                    _navigationManager.NavigateTo("/");
                }

                if (_pramMachine.IsHalted)
                {
                    _globalService.SetLastState(TextUIService.StateIndicatorExecutionFinished, GlobalService.LastStateUniform.Note);
                }
            }

            _codeEditorService.UpdateExecutingLine(currentLine);
            RefreshMemory();
        }

        // Method that executes the next instruction and optionally handles the UI
        private bool ExecuteNext(bool manual = true)
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
                    }
                    else if (!IsRunningParallel && _pramMachine.IsRunningParallel)
                    {
                        _pramCodeViewService.SetPramCode(_pramMachine.GetCurrentParallelMachineCode() ?? "No code");
                        _navigationManager.NavigateTo("/pramview");
                        RefreshPramView();
                    }
                    else if (IsRunningParallel && _pramMachine.IsRunningParallel)
                    {
                        RefreshPramView();
                    }

                    if (!ShownErrorMessage)
                    {
                        _globalService.ShowPopupMessage(_pramMachine.ExecutionErrorMessage ?? string.Empty);
                    }
                    ShownErrorMessage = true;

                    _globalService.SetLastState(TextUIService.StateIndicatorExecutionStop(_pramMachine.ExecutionErrorMessage), GlobalService.LastStateUniform.Error);
                }
                else
                {
                    //not shown if machine is halted, since the if for 'halt' will override
                    _globalService.SetLastState(TextUIService.StateIndicatorExecutionStop(_pramMachine.ExecutionErrorMessage), GlobalService.LastStateUniform.Warning);
                    if (IsRunningParallel)
                    {
                        _navigationManager.NavigateTo("/");
                        ResetParallelRunningState();
                    }
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
            else
            {
                //Count steps
                StepsIncludingParallel++;
                StepsTotal++;
                StepsIncludingParallel += _pramMachine.ParallelMachinesCount;

                if (_globalService.SaveHistory)
                {
                    _historyMemoryService.SaveState(_pramMachine);
                }
            }

            if (manual && result)
            {
                InteractiveMachineStep();
            }

            return result;
        }

        //Called when user wants to reset the machine
        public void ResetMachine()
        {
            if (IsAutoRunning)
            {
                StopAutoRun();
            }

            _pramMachine.Restart();
            _globalService.SetLastState(TextUIService.Reset, GlobalService.LastStateUniform.Ok);
            _codeEditorService.UpdateExecutingLine(-1);
            ResetParallelRunningState();
            _navigationManager.NavigateTo("/");
            _historyMemoryService.Reset();
            HistoryOffset = null;
            ShownErrorMessage = false;

            StepsIncludingParallel = 0;
            StepsTotal = 0;
        }

        //Compiles the code in the code editor
        public void CompileCode()
        {
            if (IsAutoRunning)
            {
                StopAutoRun();
            }

            if (string.IsNullOrWhiteSpace(_codeEditorService.Code))
            {
                _globalService.ShowPopupMessage(TextUIService.CodeCannotBeEmpty);
                return;
            }

            _pramMachine.Compile(_codeEditorService.Code);
            CRCWChanged();
            ResetParallelRunningState();
            _historyMemoryService.Reset();
            HistoryOffset = null;
            ShownErrorMessage = false;

            StepsIncludingParallel = 0;
            StepsTotal = 0;


            if (_pramMachine.IsCompiled)
            {
                _globalService.SetLastState(TextUIService.StateIndicatorCompilationSuccessful, GlobalService.LastStateUniform.Ok);
                _codeEditorService.CodeToViewMode();
            }
            else
            {
                _globalService.SetLastState(TextUIService.StateIndicatorCompilationFailed(_pramMachine.CompilationErrorMessage), GlobalService.LastStateUniform.Error);
                _globalService.ShowPopupMessage(TextUIService.StateIndicatorCompilationFailed(_pramMachine.CompilationErrorMessage));
                _codeEditorService.CodeToViewMode(_pramMachine.CompilationErrorLineIndex ?? -1);
            }

            RefreshCommonControls();
            RefreshMemory();
        }

        public void ClearMemories()
        {
            _pramMachine.ClearMemory();
            RefreshMemory();
            _globalService.SetLastState(TextUIService.StateIndicatorMemoryCleared, GlobalService.LastStateUniform.Ok);
        }

        //Unused
        public void Clear()
        {
            _pramMachine.Clear();
            _globalService.SetLastState("Cleared all", GlobalService.LastStateUniform.Ok);
            ResetParallelRunningState();
            RefreshMemory();
            _codeEditorService.UpdateExecutingLine(-1);
            _historyMemoryService.Reset();
            HistoryOffset = null;
            StepsIncludingParallel = 0;
            StepsTotal = 0;
        }

        //## Auto run ########################################
        private void TimerCallback(object? state)
        {
            if (!IsAutoRunning)
            {
                return;
            }
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            StepForward();
            RefreshCommonControls();
            _timer.Change(_autoRunInterval, _autoRunInterval);
        }

        public void StartAutoRun()
        {
            if (IsAutoRunning)
            {
                return;
            }

            _autoRunInterval = _globalService.AutoStepSpeed;
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
            //HistoryOffset = null;
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
            HistoryOffset = null;
            for (int i = 0; i < 10000; i++)
            {
                // Stop if machine is in bad state
                result = ExecuteNext(false);
                if (!result)
                {
                    break;
                }

                //Check if breakpoints exist
                if (_codeEditorService.Breakpoints.Count > 0)
                {
                    //Stop if breakpoint in RAM is hit
                    if (_codeEditorService.Breakpoints.Contains(_pramMachine.GetCurrentCodeLineIndex()) && !_pramMachine.IsRunningParallel)
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

                            //Stop if breakpoint in parallel RAM is hit
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
        //Contains getters for the memory and code context, switching which is used based on the history state.

        int? HistoryOffset = null;
        public bool IsInHistory => HistoryOffset != null;

        public ObservableCollection<PRAM_lib.Memory.MemoryCell> GetMemoryContextInput()
        {
            if (HistoryOffset == null)
            {
                return _pramMachine.GetInputMemory();
            }
            else
            {
                return _historyMemoryService.GetInputAt(_historyMemoryService.HistoryIndex + HistoryOffset.Value) ?? _pramMachine.GetInputMemory();
            }
        }

        public ObservableCollection<PRAM_lib.Memory.MemoryCell> GetMemoryContextOutput()
        {
            if (HistoryOffset == null)
            {
                return _pramMachine.GetOutputMemory();
            }
            else
            {
                return _historyMemoryService.GetOutputAt(_historyMemoryService.HistoryIndex + HistoryOffset.Value) ?? _pramMachine.GetOutputMemory();
            }
        }

        public ObservableCollection<PRAM_lib.Memory.MemoryCell> GetMemoryContextShared()
        {
            if (HistoryOffset == null)
            {
                return _pramMachine.GetSharedMemory();
            }
            else
            {
                return _historyMemoryService.GetSharedMemoryAt(_historyMemoryService.HistoryIndex + HistoryOffset.Value) ?? _pramMachine.GetSharedMemory();
            }
        }

        public int GetMasterCodeIndex()
        {
            if (HistoryOffset == null)
            {
                return _pramMachine.GetCurrentCodeLineIndex();
            }
            else
            {
                return _historyMemoryService.GetMasterCodeIndexAt(_historyMemoryService.HistoryIndex + HistoryOffset.Value) ?? _pramMachine.GetCurrentCodeLineIndex();
            }
        }

        public int GetParallelMachineCount()
        {
            if (HistoryOffset == null)
            {
                return _pramMachine.ParallelMachinesCount;
            }
            else
            {
                return _historyMemoryService.GetParallelMachineCountAt(_historyMemoryService.HistoryIndex + HistoryOffset.Value);
            }
        }

        public int GetRunningParallelMachineCount()
        {
            return _pramMachine.RunningParallelMachinesCount;
        }

        public ObservableCollection<PRAM_lib.Memory.MemoryCell>? GetParallelMachineMemory(int machineIndex)
        {
            if (HistoryOffset == null)
            {
                return _pramMachine.GetParallelMachinesMemory(machineIndex);
            }
            else
            {
                var result = _historyMemoryService.GetParallelMemoryAt(_historyMemoryService.HistoryIndex + HistoryOffset.Value);
                if (result == null)
                {
                    return null;
                }
                else
                {
                    return result[machineIndex];
                }
            }
        }

        public bool GetParallelMachineIsHalted(int machineIndex)
        {
            if (HistoryOffset == null)
            {
                return _pramMachine.GetParallelMachineIsHalted(machineIndex);
            }
            else
            {
                return _historyMemoryService.GetParallelMachineHaltAt(_historyMemoryService.HistoryIndex + HistoryOffset.Value, machineIndex);
            }
        }

        public bool GetParallelMachineIsAfterHalted(int machineIndex)
        {
            if (HistoryOffset == null)
            {
                return _pramMachine.GetParallelMachineIsAfterHalted(machineIndex);
            }
            else
            {
                bool wasHalted = _historyMemoryService.GetParallelMachineHaltAt(_historyMemoryService.HistoryIndex + HistoryOffset.Value - 1, machineIndex);
                bool isHalted = _historyMemoryService.GetParallelMachineHaltAt(_historyMemoryService.HistoryIndex + HistoryOffset.Value, machineIndex);

                if (!wasHalted && isHalted)
                {
                    return false;
                }
                else
                {
                    return isHalted;
                }
            }
        }

        public int GetParallelMachineCodeIndex(int machineIndex)
        {
            if (HistoryOffset == null)
            {
                return _pramMachine.GetParallelMachineCodeLineIndex(machineIndex) ?? -1;
            }
            else
            {
                var result = _historyMemoryService.GetParallelCodeIndexAt(_historyMemoryService.HistoryIndex + HistoryOffset.Value);
                if (result == null)
                {
                    return -1;
                }
                else
                {
                    return result[machineIndex];
                }
            }
        }

        public int GetParallelMachineErrorLineIndex(int machineIndex)
        {
            if (HistoryOffset == null)
            {
                return _pramMachine.GetParallelMachineErrorLineIndex(machineIndex);
            }
            else
            {
                return -1;
            }
        }

        //Switches to parallel history view if the history offset is set
        private int? _lastBatchIndex = -1;
        private void ResolveUIParallelHistory()
        {
            if (HistoryOffset == null)
            {
                return;
            }

            int? batchIndex = _historyMemoryService.GetParallelBatchIndexAt(_historyMemoryService.HistoryIndex + HistoryOffset.Value);

            if (batchIndex == null)
            {
                batchIndex = -1;
            }

            if (batchIndex == -1)
            {
                if (_lastBatchIndex != batchIndex)
                {
                    _navigationManager.NavigateTo("/");
                    IsRunningParallel = false;
                }

                _lastBatchIndex = batchIndex;
                return;
            }

            string code = _pramMachine.GetParallelMachineCode((int)batchIndex);


            if (_lastBatchIndex != batchIndex)
            {
                _navigationManager.NavigateTo("/pramview");
                IsRunningParallel = true;
            }

            if (!IsRunningParallel)
            {
                _navigationManager.NavigateTo("/pramview");
                IsRunningParallel = true;
            }


            _pramCodeViewService.SetPramCode(code);
            _lastBatchIndex = batchIndex;
            RefreshPramView();
        }

        // Main method(s) to step the machine forward (backward). Handles history.
        public void StepForward()
        {
            if (HistoryOffset >= -1)
            {
                HistoryOffset = null;
                _globalService.SetLastState(TextUIService.StateIndicatorHistoryReset(HistoryOffset ?? 0), GlobalService.LastStateUniform.Note);
                RefreshMemory();
                _codeEditorService.UpdateExecutingLine(GetMasterCodeIndex());
            }

            if (!_globalService.SaveHistory || HistoryOffset == null)
            {
                _ = ExecuteNext();
            }
            else
            {
                HistoryOffset++;
                _globalService.SetLastState(TextUIService.StateIndicatorHistoryGoingForward(HistoryOffset ?? 0), GlobalService.LastStateUniform.Note);
                RefreshMemory();
                _codeEditorService.UpdateExecutingLine(GetMasterCodeIndex());
                ResolveUIParallelHistory();
            }
        }

        public void StepBackward()
        {
            if (IsAutoRunning)
            {
                StopAutoRun();
            }

            if (!_globalService.SaveHistory || _historyMemoryService.HistoryIndex == 0)
            {
                _globalService.SetLastState(TextUIService.StateIndicatorNoHistory, GlobalService.LastStateUniform.Warning);
                return;
            }

            if (HistoryOffset == null)
            {
                HistoryOffset = -2;
                _globalService.SetLastState(TextUIService.StateIndicatorStartingHistory(HistoryOffset ?? 0), GlobalService.LastStateUniform.Note);
                RefreshMemory();
                _codeEditorService.UpdateExecutingLine(GetMasterCodeIndex());
                ResolveUIParallelHistory();
            }
            else if (_historyMemoryService.HistoryIndex + HistoryOffset <= 0)
            {
                _globalService.SetLastState(TextUIService.StateIndicatorMaxHistoryReached(HistoryOffset ?? 0), GlobalService.LastStateUniform.Note);
            }
            else
            {
                HistoryOffset--;
                _globalService.SetLastState(TextUIService.StateIndicatorHistoryGoingBack(HistoryOffset ?? 0), GlobalService.LastStateUniform.Note);
                RefreshMemory();
                _codeEditorService.UpdateExecutingLine(GetMasterCodeIndex());
                ResolveUIParallelHistory();
            }
        }

        public void StepToPresent()
        {
            if (IsAutoRunning)
            {
                StopAutoRun();
            }

            HistoryOffset = null;
            _globalService.SetLastState(TextUIService.StateIndicatorHistorySetToPresent, GlobalService.LastStateUniform.Note);
            RefreshMemory();
            RefreshCommonControls();
            RefreshPramView();
            _codeEditorService.UpdateExecutingLine(GetMasterCodeIndex());

            if (_pramMachine.IsRunningParallel)
            {
                _navigationManager.NavigateTo("/pramview");
                IsRunningParallel = true;
            }
            else
            {
                _navigationManager.NavigateTo("/");
                IsRunningParallel = false;
            }
        }

        private void HistoryToggled(bool value)
        {
            if (!value)
            {
                _historyMemoryService.Reset();
            }
        }

        //Returns true if the current history offset is the latest parallel batch, or if the history offset is null
        //Useful when a machine is crashed / halted / accessed memory illegally, and the user still wants to see which one caused it.
        public bool IsHistoryInLatestParallelBatch()
        {
            if (HistoryOffset == null)
            {
                return true;
            }

            int? batchIndex = _historyMemoryService.GetParallelBatchIndexAt(_historyMemoryService.HistoryIndex + HistoryOffset.Value);
            if (batchIndex == null)
            {
                return false;
            }

            int? lastBatchIndex = _historyMemoryService.GetParallelBatchIndexAt(_historyMemoryService.HistoryIndex - 1);

            if (lastBatchIndex == null)
            {
                return false;
            }

            return batchIndex == lastBatchIndex;
        }

        //## Exclusive write / read control

        private void CRCWChanged()
        {
            _pramMachine.SetCRXW(_concurrentRead);
            _pramMachine.SetXRCW(_concurrentWrite);
        }

        private bool _concurrentRead = true;
        public bool ConcurrentRead
        {
            get => _concurrentRead;
            set
            {
                _concurrentRead = value;

                //Dont allow ERCW, as it is not properly implemented in the machine
                if (_concurrentRead == false && _concurrentWrite == true)
                {
                    _concurrentWrite = false;
                }

                CRCWChanged();
            }
        }
        private bool _concurrentWrite = false;
        public bool ConcurrentWrite
        {
            get => _concurrentWrite;
            set
            {
                _concurrentWrite = value;

                //Dont allow ERCW
                if (_concurrentRead == false && _concurrentWrite == true)
                {
                    _concurrentRead = true;
                }

                CRCWChanged();
            }
        }

        public void ResetZeroValuedMemoryCellsFlags()
        {
            foreach (var cell in _pramMachine.GetInputMemory())
            {
                if(cell.Value == 0)
                {
                    cell.HasBeenWrittenTo = false;
                }
            }

            foreach (var cell in _pramMachine.GetOutputMemory())
            {
                if (cell.Value == 0)
                {
                    cell.HasBeenWrittenTo = false;
                }
            }

            foreach (var cell in _pramMachine.GetSharedMemory())
            {
                if (cell.Value == 0)
                {
                    cell.HasBeenWrittenTo = false;
                }
            }
        }
    }
}
