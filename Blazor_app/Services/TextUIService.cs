namespace Blazor_app.Services
{
    /// <summary>
    /// Supplies text for UI elements.
    /// </summary>
    public class TextUIService
    {
        //General
        public static string Reset { get; private set; } = "Reset";

        // Execution, common controls
        public static string ExecutionStepBack { get; private set; } = "🔺 Step back";
        public static string ExecutionToPresent { get; private set; } = "🔁 Step to present";
        public static string ExecutionStepForward { get; private set; } = "🟢 Step forward";
        public static string ExecutionStepUntilBreakpoint { get; private set; } = "🔴 Run Until Breakpoint";
        public static string ExecutionAutoRun { get; private set; } = "⏯ Auto Run";
        public static string ExecutionReset { get; private set; } = "🧹 Reset";
        public static string CurrentExecutionInfo(int stepsCount, int parallelStepsCount, int runningParallelMachines) => $"Running paralell machines: {(runningParallelMachines > 0 ? $"[{runningParallelMachines}] 🟥" : "[0]")} Steps: [{stepsCount}] Steps including parallel: [{parallelStepsCount}]";

        //Home page
        public static string HomePageCodeEditorCancel { get; private set; } = "Cancel";
        public static string HomePageCompile { get; private set; } = "Compile";
        public static string HomePageEdit { get; private set; } = "Edit";

        //Memory titles
        public static string MemoryInputTitle { get; private set; } = "Input";
        public static string MemoryOutputTitle { get; private set; } = "Output";
        public static string MemorySharedTitle { get; private set; } = "Shared Memory";
        public static string MemoryAddCell { get; private set; } = "➕";
        public static string MemoryClear { get; private set; } = "Clear";


        //PRAM View
        public static string PRAMViewConcurrentRead { get; private set; } = "Concurrent Read";
        public static string PRAMViewConcurrentWrite { get; private set; } = "Concurrent Write";
        public static string PRAMViewMachineFilterTextEntry { get; private set; } = "Enter text";
        public static string PRAMViewMachineFilterTextSet { get; private set; } = "Set";
        public static string PRAMViewMachineFilterDisplayingAll { get; private set; } = "Displaying all. Enter indexes like '0,1,2,4-10,odd 10-20, even 20-30, ...'";
        public static string PRAMViewMachineFilterDisplayingMessage(string machines) => $"🟧 Displaying: {machines}; Begin with '+' to add.";
        public static string PRAMViewShowHalted { get; private set; } = "Show Halted";
        public static string PRAMViewCardHalted { get; private set; } = "Halted";
        public static string PRAMViewCardCrashed { get; private set; } = "Crashed";
        public static string PRAMViewCardIllegalMemoryAccess { get; private set; } = "IMA";
        public static string PRAmViewCodeTitle { get; private set; } = "Code";
        public static string PRAMViewMemoryTitle { get; private set; } = "Memory";
        public static string PRAMViewMachineTitle(int machineIndex) => $"Machine {machineIndex} :";


        // State indicator
        public static string StateIndicatorDefault { get; private set; } = "Ready";
        public static string StateIndicatorAllRegexReset { get; private set; } = "All regex have been reset";
        public static string StateIndicatorRegexImportedSuccessfully { get; private set; } = "Regex imported successfully";
        public static string StateIndicatorFailedToExportRegex { get; private set; } = "Failed to export regex";
        public static string StateIndicatorFailedToExportMachine { get; private set; } = "Failed to export machine";
        public static string StateIndicatorFileRefusedTooLarge { get; private set; } = "File refused, it might be too large.";
        public static string StateIndicatorFileFormatIncorrectOrCorrupted { get; private set; } = "File format is incorrect or file is corrupted.";
        public static string StateIndicatorFailedToImportMachine(string exMessage) => $"Failed to import machine: {exMessage}";
        public static string StateIndicatorMachineImportedSuccessfullyWarning { get; private set; } = "Machine imported successfully. Warning: The regex of the machine is not default.";
        public static string StateIndicatorMachineImportedSuccessfully { get; private set; } = "Machine imported successfully";
        public static string StateIndicatorFailedToImportMemory { get; private set; } = "Memory format is incorrect or file is corrupted.";
        public static string StateIndicatorInvalidRegexPattern(string key) => $"Invalid regex pattern for {key}";
        public static string StateIndicatorRegexPatternSavedSuccessfully(string key) => $"Regex pattern for {key} saved successfully";
        public static string StateIndicatorRegexAllReset { get; private set; } = "All regex have been reset";
        public static string StateIndicatorNextStep(int nextStep) => $"Next step: {nextStep}";
        public static string StateIndicatorParallelExecutionStarted { get; private set; } = "Parallel execution started";
        public static string StateIndicatorParallelExecutionStopped { get; private set; } = "Parallel execution stopped";
        public static string StateIndicatorExecutionFinished { get; private set; } = "Execution finished";
        public static string StateIndicatorExecutionStop(string? executionErrorMessage) => $"Execution stop: {executionErrorMessage ?? "Unknown error"}";
        public static string StateIndicatorCompilationSuccessful { get; private set; } = "Compilation successful";
        public static string StateIndicatorCompilationFailed(string? compilationErrorMessage) => $"Compilation failed: {compilationErrorMessage ?? "Unknown error"}";
        public static string StateIndicatorMemoryCleared { get; private set; } = "Memory cleared";
        public static string StateIndicatorHistoryReset(int historyOffset) => $"History reset: {historyOffset}";
        public static string StateIndicatorHistoryGoingForward(int historyOffset) => $"History going forward: {historyOffset}";
        public static string StateIndicatorNoHistory { get; private set; } = "No history";
        public static string StateIndicatorStartingHistory(int historyOffset) => $"Starting history: {historyOffset}";
        public static string StateIndicatorMaxHistoryReached(int historyOffset) => $"Max history reached: {historyOffset}";
        public static string StateIndicatorHistoryGoingBack(int historyOffset) => $"History going back: {historyOffset}";
        public static string StateIndicatorHistorySetToPresent { get; private set; } = "History set to present.";

        //Settings
        public static string SettingsTitleRunConfiguration { get; private set; } = "🟢 Run configuration";
        public static string SettingsAutoStepSpeed { get; private set; } = "Auto Step Speed";
        public static string SettingsHideUnsetMemoryCells { get; private set; } = "Hide unset memory cells";
        public static string SettingsSaveHistory { get; private set; } = "Save history";
        public static string SettingsFixParallelCodeInPlace { get; private set; } = "Fix parallel code in place";
        public static string SettingsCRCWAccessType { get; private set; } = "CRCW Access type";
        public static string SettingsTitleMachineImportExport { get; private set; } = "💾 Machine import/export";
        public static string SettingsTitleRegexImportExport { get; private set; } = "🔠 Regex import/export";
        public static string SettingsTitleEditingInstructionRegex { get; private set; } = "Edit instruction regex";
        public static string SettingsEditingInstructionRegexWarning { get; private set; } = "Editing the instruction regex of the machine is not officially supported and may expose critical aspects of its internal functionality. Please proceed with caution.";
        public static string SettingsEditingInstructionRegexContinue { get; private set; } = "I understand, continue";
        public static string SettingsTitleInformation { get; private set; } = "ℹ Information and examples";
        public static string SettingsAboutButton { get; private set; } = "About and User manual";

    }
}
