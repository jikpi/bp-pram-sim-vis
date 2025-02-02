﻿using PRAM_lib.Machine;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WPF_app.Resources;

namespace WPF_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PramMachine? exposedMachine { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            InitializeProgram();
        }

        public void InitializeProgram()
        {
            InitializeRichTextBox();

            //Exposing property for bindings
            exposedMachine = DataResources.PRAM;

            //Master processor instruction pointer binding
            Binding MPIPbinding = new Binding("Value");
            MPIPbinding.Source = DataResources.PRAM.MPIP;
            BindingOperations.SetBinding(LabelMasterProcessorInstructionPointer, Label.ContentProperty, MPIPbinding);

            RestartedMasterMemoryBindings();
        }

        public void RestartedMasterMemoryBindings()
        {
            //Input memory binding
            DataGridInput.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = DataResources.PRAM.GetInputMemory() });

            //Shared memory binding
            DataGridSharedMemory.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = DataResources.PRAM.GetSharedMemory() });

            //Output memory binding
            DataGridOutput.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = DataResources.PRAM.GetOutputMemory() });

            ResetCodeEditorColor();
        }

        private void DataGridMemory_IndexRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString();
        }

        private void ButtonCompile_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(RichTextBoxCode.Document.ContentStart, RichTextBoxCode.Document.ContentEnd);
            string text = textRange.Text;

            DataResources.PRAM.Compile(text);

            if (DataResources.PRAM.IsCompiled)
            {
                MessageBox.Show("Compilation successful");
                ResetCodeEditorColor();
            }
            else
            {
                MessageBox.Show("Compilation failed");
                MessageBox.Show(DataResources.PRAM.CompilationErrorMessage);
                if (DataResources.PRAM.CompilationErrorLineIndex >= 0)
                {
                    SetCodeEditorLineIndexColor((int)DataResources.PRAM.CompilationErrorLineIndex);
                }
            }

            DataGridInput.Items.Refresh();
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            DataResources.PRAM.Restart();
            ResetCodeEditorColor();
        }
        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            DataResources.PRAM.Clear();
            RestartedMasterMemoryBindings();
        }

        private void ResetCodeEditorColor()
        {
            foreach (Block block in RichTextBoxCode.Document.Blocks)
            {
                if (block is Paragraph paragraph)
                {
                    foreach (Inline inline in paragraph.Inlines)
                    {
                        if (inline is Run run)
                        {
                            run.Foreground = Brushes.Black;
                        }
                    }
                }
            }
        }

        private void SetCodeEditorLineIndexColor(int index)
        {
            if (index >= RichTextBoxCode.Document.Blocks.Count)
            {
                return;
            }

            if (index < 0)
            {
                return;
            }

            ResetCodeEditorColor();
            if (RichTextBoxCode.Document.Blocks.Count > 0)
            {
                Paragraph? paragraph = RichTextBoxCode.Document.Blocks.ElementAt(index) as Paragraph;

                if (paragraph != null && paragraph.Inlines.Count > 0)
                {

                    ((Run)paragraph.Inlines.FirstInline).Foreground = Brushes.Red;
                }
            }
        }

        private void ButtonNextExecution_Click(object sender, RoutedEventArgs e)
        {
            bool result = DataResources.PRAM.ExecuteNextInstruction();

            if (DataResources.PRAM.ParallelMachinesCount > 0)
            {
                int count = DataResources.PRAM.ParallelMachinesCount;

                ObservableCollection<PRAM_lib.Memory.MemoryCell>? memory = DataResources.PRAM.GetParallelMachinesMemory(0);
                if (memory != null)
                {
                    //Input memory binding
                    DataGridInParallelMemory1.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = memory });
                }

                if (count > 1)
                {
                    memory = DataResources.PRAM.GetParallelMachinesMemory(1);
                    if (memory != null)
                    {
                        //Input memory binding
                        DataGridInParallelMemory2.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = memory });
                    }
                }

                if (count > 2)
                {
                    memory = DataResources.PRAM.GetParallelMachinesMemory(2);
                    if (memory != null)
                    {
                        //Input memory binding
                        DataGridInParallelMemory3.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = memory });
                    }
                }

                if (count > 3)
                {
                    memory = DataResources.PRAM.GetParallelMachinesMemory(3);
                    if (memory != null)
                    {
                        //Input memory binding
                        DataGridInParallelMemory4.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = memory });
                    }
                }

                if (count > 4)
                {
                    memory = DataResources.PRAM.GetParallelMachinesMemory(4);
                    if (memory != null)
                    {
                        //Input memory binding
                        DataGridInParallelMemory5.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = memory });
                    }
                }
            }

            if (result)
            {
                SetCodeEditorLineIndexColor(DataResources.PRAM.GetCurrentCodeLineIndex());
            }
            else
            {
                MessageBox.Show("Execution failed");
                MessageBox.Show(DataResources.PRAM.ExecutionErrorMessage);
            }

            if (DataResources.PRAM.IllegalMemoryAccess)
            {
                MessageBox.Show("Illegal memory access");
                foreach (var access in DataResources.PRAM.IllegalMemoryAccesses)
                {
                    MessageBox.Show($"Global type: {(DataResources.PRAM.IllegalMemoryAccessType == PRAM_lib.Machine.Container.ParallelAccessError.Read ? " Write " : " Read ")}     Machine index: {access.ParallelMachineIndex}" + Environment.NewLine +
                        $"Memory index: {access.MemoryIndex}" + Environment.NewLine + $"Type: {(access.Type == PRAM_lib.Machine.Container.ParallelAccessType.Write ? " Write " : " Read ")}" + Environment.NewLine +
                    $"Parallel machine relevant code index: {access.CodeLineIndex}");
                }
            }
        }

        private void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RichTextBox richTextBox = (RichTextBox)sender;
                Paragraph newParagraph = new Paragraph();
                richTextBox.Document.Blocks.Add(newParagraph);
                richTextBox.CaretPosition = newParagraph.ContentStart;
                e.Handled = true;
            }
        }

        private void InitializeRichTextBox()
        {
            RichTextBoxCode.Document.Blocks.Clear();

            string program = "#Nastavit 5 jako vstup\r\n#Cteni\r\nS0 := READ()\r\n#Prirazeni\r\nS1 := S0\r\n#Prirazeni hodnoty a operace\r\nS2 := S1 + 3\r\nS3 := S1 - 3\r\nS4 := S1 * 3\r\nS5 := S1 / 3\r\nS6 := S1 % 3\r\n\r\n#Zapis do vystupu\r\nWRITE(23)\r\nWRITE(S1)\r\nWRITE(S1 + 2)\r\nWRITE([S1])\r\n\r\n#Prirazeni hodnoty naopak\r\nS7 := 3 + S7\r\nS8 := 3 - S7\r\nS9 := 3 * S7\r\nS10 := 3 / S7\r\nS11 := 3 % S7\r\n\r\n#Prirazeni hodnoty jako vysledek 2 bunek\r\nS12 := S0 + S1\r\nS13 := S0 - S1\r\nS14 := S0 * S1\r\nS15 := S0 / S1\r\nS16 := S0 % S1\r\n\r\n#Pointery\r\nS17 := [S2]\r\n[S17] := 1\r\n[S18] := S17\r\n\r\n#Skoky\r\ngoto :skok1\r\nS19 := S19 / 0\r\n:skok1\r\nif (S0 == S8) goto :skoks2\r\nS19 := S19 / 0\r\n:skoks2\r\n\r\nif (S0 != -191919) goto :skok2\r\nS19 := S19 / 0\r\n:skok2\r\nif (S0 != -1) goto :skok3\r\nS19 := S19 / 0\r\n:skok3\r\n\r\nS20 := 5\r\nS21 := 6\r\nif (S20 >= S21) goto :skok4\r\ngoto :skok5\r\n:skok4\r\nS0 := S0 / 0\r\n:skok5\r\n\r\nif (S21 <= S20) goto :skok6\r\ngoto :skok7\r\n:skok6\r\nS0 := S0 / 0\r\n:skok7\r\n\r\nif (S20 > S21) goto :skok8\r\ngoto :skok9\r\n:skok8\r\nS0 := S0 / 0\r\n:skok9\r\n\r\nif (S21 < S20) goto :skok10\r\ngoto :skok11\r\n:skok10\r\nS0 := S0 / 0\r\n:skok11\r\n\r\n:true\r\nS5 := 1\r\ngoto :end\r\n:end\r\nWRITE(0)\r\n";
            foreach (var instruction in program.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList())
            {
                Paragraph paragraph = new Paragraph(new Run(instruction));
                RichTextBoxCode.Document.Blocks.Add(paragraph);
            }
        }

        private void ButtonClearMemory_Click(object sender, RoutedEventArgs e)
        {
            DataResources.PRAM.ClearMemory();
        }

        private void checkBoxCRXW_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxCRXW.IsChecked == null)
            {
                return;
            }

            DataResources.PRAM.SetCRXW(checkBoxCRXW.IsChecked.Value);
        }

        private void checkBoxXRCW_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxXRCW.IsChecked == null)
            {
                return;
            }

            DataResources.PRAM.SetXRCW(checkBoxXRCW.IsChecked.Value);
        }
    }
}
