namespace Blazor_app.Services
{
    /// <summary>
    /// Supplies text for UI elements.
    /// </summary>
    public class TextUIService
    {
        //General
        public static string Reset { get; private set; } = "Reset";
        public static string LoadTitle { get; private set; } = "Load";
        public static string Select { get; private set; } = "Select...";

        // Execution, common controls
        public static string ExecutionStepBack { get; private set; } = "🔺 Step back";
        public static string ExecutionToPresent { get; private set; } = "🔁 Step to present";
        public static string ExecutionStepForward { get; private set; } = "🟢 Step forward";
        public static string ExecutionStepUntilBreakpoint { get; private set; } = "🔴 Run Until Breakpoint";
        public static string ExecutionAutoRun { get; private set; } = "⏯ Auto Run";
        public static string ExecutionReset { get; private set; } = "🧹 Reset";
        public static string CurrentExecutionInfo(int stepsCount, int parallelStepsCount, int runningParallelMachines) => $"Running paralell machines: {(runningParallelMachines > 0 ? $"[{runningParallelMachines}] 🟥" : "[0]")} || Steps: [{stepsCount}] || Including parallel: [{parallelStepsCount}]";
        public static string CodeCannotBeEmpty { get; private set; } = "Code cannot be empty";

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
        public static string StateIndicatorRunningInParallel { get; private set; } = "Parallel machines running";
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
        public static string StateIndicatorMachineSavedForReimport { get; private set; } = "Machine saved for re-import";

        //Settings
        public static string SettingsTitleRunConfiguration { get; private set; } = "🟢 Run configuration";
        public static string SettingsAutoStepSpeed { get; private set; } = "Auto Step Speed";
        public static string SettingsHideUnsetMemoryCells { get; private set; } = "Hide unset memory cells";
        public static string SettingsHideUnsetMemoryCellsReHide { get; private set; } = "Re-hide";
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
        public static string SettingsFirstTimeUserManual { get; private set; } = "First time here? Read the user manual:";

        public static Dictionary<string, string> CreateMachineExampleDictionary()
        {
            Dictionary<string,string> ExampleMachines = new Dictionary<string, string>
            {
                { "Calculate prefix sums of n numbers in O(log n)", "MACHINECODE\r\n# Calculate prefix sums of n numbers\r\n# in O(log n)\r\n# in a balanced binary tree format\r\n# n is a power of 2\r\n\r\n# Input\r\n# First array (A) size at S9, starts S10\r\n# the array must be in the format:\r\n# An array representing a \r\n# balanced binary tree, where the\r\n# input numbers are at the last\r\n# level of it, with space reserved\r\n# for all levels above.\r\n# (starts with ((n+1) / 2) - 1) zeroes)\r\n\r\n#Output\r\n# A second array (B) also in the\r\n# representation of a balanced \r\n# binary tree starting at \r\n# (11 + the size of array A)\r\n# of the first array containing\r\n# in each node the sum of A + B\r\n# on identical indexes, in B on the \r\n# following node index on each level.\r\n# The array A with 2 summed\r\n# numbers from connected\r\n# nodes from of the previous level.\r\n\r\n\r\n# Calculate the square root of n + 1\r\n# to get the number of tree levels\r\nS8 := S9 + 1\r\nS7 := 0\r\n\r\n:sqrt_loop\r\nS8 := S8 / 2\r\nS7 := S7 + 1\r\nif (S8 == 1) goto :sqrt_done\r\ngoto :sqrt_loop\r\n:sqrt_done\r\n\r\n# The total number of levels is now\r\n# saved in S7.\r\n# In S5, save the no/2 of level's nodes\r\n# for the first iteration\r\nS5 := S9 + 1\r\nS5 := S5 / 4\r\n\r\n# Launch no/2 of level's nodes processors\r\n# from the bottom\r\n:summation_cycle\r\npardo S5\r\nL0 := {i}\r\n# Get the origin index for the current\r\n# level and add processors index to it\r\n# dont add offset to allow calculations\r\n#L1 => A[i]\r\nL1 := S5 + L0\r\n\r\n# Get the address of the\r\n# pair of numbers to be added\r\n# and add offset to both\r\n# L2 => A[2 ∗ i] , (A[2 ∗ i + 1])\r\nL2 := L1 * 2\r\nL1 := L1 + 9\r\nL2 := L2 + 9\r\n# Save the values to local memory, sum them\r\n# and save to shared memory\r\n# to the precalculated address\r\n# in the tree\r\nL4 := S{L2}\r\nL5 := S{L2 + 1}\r\nS{L1} := L4 + L5\r\nhalt\r\nparend\r\n\r\n# Refresh the number of nodes\r\n# in S5 to be half of the previous count\r\nS5 := S5 / 2\r\n\r\n# if S5 is 0, first array is summed\r\nif (S5 == 0) goto :next_stage\r\ngoto :summation_cycle\r\n:next_stage\r\n\r\n# Offset for array 2\r\nS4 := S9 + 10\r\n\r\n# First level has 1 node\r\nS5 := 1\r\n\r\n# Launch no/2 of level's nodes processors\r\n# from the top\r\n:calc_cycle\r\npardo S5\r\nL0 := {i}\r\n\r\n# L1 => B[i]\r\nL1 := S5 + L0\r\n\r\n# L2 => B[2 * i], (B[2 * i + 1])\r\nL2 := L1 * 2\r\n# Add offsets to second array\r\nL1 := L1 + S4\r\nL2 := L2 + S4\r\n\r\n# L3 => A[2 * i]\r\nL3 := S5 + L0\r\nL3 := L3 * 2\r\nL3 := L3 + 9\r\n\r\n# B[2 * i] := B[i]\r\nS{L2} := S{L1}\r\n\r\n# B[2 * i + 1] := B[i] + A[2 * i]\r\nL4 := S{L1}\r\nL5 := S{L3}\r\nS{L2 + 1} := L4 + L5\r\nhalt\r\nparend\r\n\r\n# Refresh the number of nodes\r\n# in S5 to be double the previous count\r\nS5 := S5 * 2\r\n# if S5 is >S7, second array B is calculated\r\nif (S5 > S7) goto :end\r\ngoto :calc_cycle\r\n:end\r\nMEMORYINPUT\r\nMEMORYSHARED\r\n9:15\r\n10:0\r\n11:0\r\n12:0\r\n13:0\r\n14:0\r\n15:0\r\n16:0\r\n17:2\r\n18:18\r\n19:5\r\n20:12\r\n21:21\r\n22:8\r\n23:10\r\n24:7\r\n" },
                { "Calculate conjunction array in O(1)", "MACHINECODE\r\n# Calculate conjunction array\r\n# CRCW any required\r\n# Outputs 1 if all is 1\r\n# Outputs 0 of any is not 1\r\n\r\n# Input\r\n# Size of array (n) at S1\r\n# S2 to Sn are array values\r\n# Value: 1 = true, \r\n# other = false\r\nS0 := 1\r\n\r\npardo S1\r\nL0 := {i}\r\nL0 := L0 + 2\r\nL1 := S{L0}\r\nif (L1 == 1) goto :not\r\nS0 := 0\r\n:not\r\nhalt\r\nparend\r\n\r\nWRITE(S0)\r\nhalt\r\nMEMORYINPUT\r\nMEMORYSHARED\r\n0:1\r\n1:5\r\n2:1\r\n3:1\r\n4:1\r\n5:1\r\n6:1\r\n" },
                { "Calculate disjunction array in O(1)", "MACHINECODE\r\n# Calculate disjunction array\r\n# in O(1)\r\n# CRCW any required\r\n# Outputs 1 if any is 1\r\n# Outputs 0 if all are 0\r\n\r\n# Input:\r\n# Size of array (n) at S1\r\n# S2 to Sn are array values\r\n# Value: 1 = true, \r\n# other = false\r\nS0 := 0\r\n\r\npardo S1\r\nL0 := {i}\r\nL0 := L0 + 2\r\nL1 := S{L0}\r\nif (L1 == 1) goto :yes\r\ngoto :end\r\n:yes\r\nS0 := 1\r\n:end\r\nhalt\r\nparend\r\n\r\nWRITE(S0)\r\nhalt\r\n\r\nMEMORYINPUT\r\nMEMORYSHARED\r\n0:1\r\n1:5\r\n2:0\r\n3:0\r\n4:0\r\n5:0\r\n6:1\r\n" },
                { "Find the smallest number in O(log n)", "MACHINECODE\r\n# Find the smallest number\r\n# in O(log n)\r\n# EREW is sufficent\r\n\r\n# Input:\r\n# Amount of numbers at S5\r\n# Array of numbers from S6 to Sn\r\n\r\n# Output:\r\n# The smallest number\r\n\r\n# Copy amount of values into S4\r\nS4 := S5\r\n\r\n:cycle\r\n# Always divide an even NO of machines\r\nS3 := S4 % 2\r\nif (S3 == 0) goto :preparePardo\r\n# If not even, make it even\r\nS4 := S4 + 1\r\n# Create a dummy cell\r\nS{S4 + 5} := S{S4 + 4}\r\n:preparePardo\r\n\r\n# Launch n / 2 machines\r\nS4 := S4 / 2\r\npardo S4\r\n# Set to get index\r\nL0 := {i}\r\n# Calculate addrs of first cell to get\r\nL1 := L0 * 2\r\n# Add offset of 6\r\nL1 := L1 + 6\r\n# Calculate addrs of second cell to get\r\nL2 := L1 + 1\r\n# Copy memory from shared to local\r\nL3 := S{L1}\r\nL4 := S{L2}\r\n# Set shared to zero\r\nS{L1} := 0\r\nS{L2} := 0\r\n# Compare the numbers\r\nif (L3 < L4) goto :L3smaller\r\ngoto :L4smaller\r\n:L3smaller\r\nL5 := L3\r\ngoto :writeBack\r\n:L4smaller\r\nL5 := L4\r\n:writeBack\r\n# Calculate the position in shared memory\r\nL1 := L1 - L0\r\n# Write the smaller number to shared memory\r\nS{L1} := L5\r\nhalt\r\nparend\r\n\r\n# If only 1 parallel machine was launched, end\r\nif (S4 == 1) goto :end\r\ngoto :cycle\r\n\r\n:end\r\n# Write out result\r\nWRITE(S6)\r\nhalt\r\nMEMORYINPUT\r\nMEMORYSHARED\r\n0:0\r\n1:0\r\n2:0\r\n3:0\r\n4:1\r\n5:5\r\n6:-6\r\n7:2\r\n8:10000\r\n9:0\r\n10:800\r\n" },
                { "Find the smallest number in O(1)", "MACHINECODE\r\n# Find the smallest number in O(1)\r\n# CRCW common required\r\n# Pairs of: 0 <= i < j < n \r\n\r\n# Input:\r\n# Amount of values (n) at S3\r\n# Array of numbers from S6 to Sn\r\n# S4 to Sn + 3 are values of array\r\n\r\n# S1 is the offset for bool array\r\nS1 := S3 + 6\r\n# Launch n^2 par machines\r\nS2 := S3 * S3\r\n\r\npardo S2\r\nL0 := {i}\r\n#Calculate i in L1:\r\nL1 := L0 / S3\r\nL1 := L1 + 4\r\n#Calculate j in L2\r\nL2 := L0 % S3\r\nL2 := L2 + 4\r\n#i < j\r\nif (L1 < L2) goto :jIndexBigger\r\nhalt\r\n:jIndexBigger\r\n\r\n# Compare numbers at indexes\r\n# and write result into bool array\r\nL3 := S{L1}\r\nL4 := S{L2}\r\nif (L3 < L4) goto :jBigger\r\nS{S1 + L1} := 1\r\ngoto :endComparison\r\n:jBigger\r\nS{S1 + L2} := 1\r\n:endComparison\r\nhalt\r\nparend\r\n\r\n# Launch n parallel machines and\r\n# find the index of the bool array\r\n# with 0\r\npardo S3\r\nL0 := {i}\r\nL1 := S{L0 + 4}\r\nL2 := L0 + 4\r\nL2 := S{S1 + L2}\r\nif (L2 == 1) goto :isNot\r\nS0 := L1\r\n:isNot\r\nhalt\r\nparend\r\n\r\nWRITE(S0)\r\nhalt\r\nMEMORYINPUT\r\nMEMORYSHARED\r\n3:10\r\n4:6\r\n5:10\r\n6:3\r\n7:444\r\n8:500\r\n9:-2\r\n10:-555\r\n11:99999\r\n12:4\r\n13:110000\r\n" },
                { "Sum all numbers in O(log n)", "MACHINECODE\r\n# Sum all numbers\r\n# in O(log n)\r\n# EREW is sufficent\r\n\r\n# Input:\r\n# Amount of numbers at S5\r\n# Array of numbers from S6 to Sn\r\n# Memory after array must be\r\n# set to 0.\r\n\r\n# Output:\r\n# The sum of numbers.\r\n\r\n\r\n# Copy amount of values into S4\r\nS4 := S5\r\n\r\n:cycle\r\n# Always divide an even NO of machines\r\nS3 := S4 % 2\r\nif (S3 == 0) goto :preparePardo\r\n# If not even, make it even\r\nS4 := S4 + 1\r\n:preparePardo\r\n\r\n# Launch n / 2 machines\r\nS4 := S4 / 2\r\npardo S4\r\n#Set to get index\r\nL0 := {i}\r\n#Calculate addrs of first cell to get\r\nL1 := L0 * 2\r\n#Add offset of 6\r\nL1 := L1 + 6\r\n#Calculate addrs of second cell to get\r\nL2 := L1 + 1\r\n#Copy memory from shared to local\r\nL3 := S{L1}\r\nL4 := S{L2}\r\n#Set shared to zero\r\nS{L1} := 0\r\nS{L2} := 0\r\n#Add the numbers\r\nL5 := L3 + L4\r\n#Calculate the position\r\n#in shared memory\r\nL1 := L1 - L0\r\n#Write to shared memory\r\nS{L1} := L5\r\nhalt\r\nparend\r\n\r\n# If only 1 parallel machine was launched, end\r\nif (S4 == 1) goto :end\r\ngoto :cycle\r\n\r\n:end\r\n#Write out result\r\nWRITE(S6)\r\nhalt\r\nMEMORYINPUT\r\nMEMORYSHARED\r\n5:10\r\n6:2\r\n7:2\r\n8:2\r\n9:2\r\n10:2\r\n11:2\r\n12:2\r\n13:2\r\n14:2\r\n15:2\r\n" }
            };

            return ExampleMachines;
        }

    }
}
