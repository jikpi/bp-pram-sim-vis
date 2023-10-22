using PRAM_simulator;
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
            exposedMachine = DataResources.pram;

            //Master processor instruction pointer binding
            Binding MPIPbinding = new Binding("Value");
            MPIPbinding.Source = DataResources.pram.MPIP;
            BindingOperations.SetBinding(LabelMasterProcessorInstructionPointer, Label.ContentProperty, MPIPbinding);

            RestartedMasterMemoryBindings();
        }

        public void RestartedMasterMemoryBindings()
        {
            //Input memory binding
            DataGridInput.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = DataResources.pram.GetInputMemory() });

            //Shared memory binding
            DataGridSharedMemory.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = DataResources.pram.GetSharedMemory() });

            //Output memory binding
            DataGridOutput.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = DataResources.pram.GetOutputMemory() });

            ResetCodeEditorColor();
        }

        private void ButtonCompile_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(RichTextBoxCode.Document.ContentStart, RichTextBoxCode.Document.ContentEnd);
            string text = textRange.Text;

            DataResources.pram.Compile(text);

            if (DataResources.pram.IsCompiled)
            {
                MessageBox.Show("Compilation successful");
                ResetCodeEditorColor();
            }
            else
            {
                MessageBox.Show("Compilation failed");
                MessageBox.Show(DataResources.pram.CompilationErrorMessage);
                if(DataResources.pram.CompilationErrorLineIndex >= 0)
                {
                    SetCodeEditorLineIndexColor((int)DataResources.pram.CompilationErrorLineIndex);
                }
            }

            DataGridInput.Items.Refresh();
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            DataResources.pram.Restart();
            ResetCodeEditorColor();
        }
        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            DataResources.pram.Clear();
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
            if (index < 0)
            {
                return;
            }

            ResetCodeEditorColor();
            if (RichTextBoxCode.Document.Blocks.Count > 0)
            {
                Paragraph paragraph = RichTextBoxCode.Document.Blocks.ElementAt(index) as Paragraph;

                if (paragraph != null && paragraph.Inlines.Count > 0)
                {
                    (paragraph.Inlines.FirstInline as Run).Foreground = Brushes.Red;
                }
            }
        }

        private void ButtonNextExecution_Click(object sender, RoutedEventArgs e)
        {
            bool result = DataResources.pram.ExecuteNextInstruction();

            if (result)
            {
                SetCodeEditorLineIndexColor(DataResources.pram.GetCurrentCodeLineIndex());
            }
            else
            {
                MessageBox.Show("Execution failed");
                MessageBox.Show(DataResources.pram.ExecutionErrorMessage);
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

            string[] instructions = {
                "S0 := READ()",
                "S1 := READ()",
                "S2 := S0 + S1",
                "S3 := S0 - S1",
                "S4 := S0 * S1",
                "S5 := S0 / S1",
                "S6 := S0 % S1",
                "S7 := [S2]",
                "S8 := S0 + 2",
                "S9 := S0 - 2",
                "S10 := S0 * 2",
                "S11 := S0 / 2",
                "S12 := S0 % 2",
                "[S13] := 10",
                ":test",
                "S14 := S14 + 1",
                "if (S0 == S1) goto :true",
                "S5 := 0",
                "goto :end",
                ":true",
                "S5 := 1",
                "goto :end",
                ":end",
                "goto :test"
            };

            foreach (string instruction in instructions)
            {
                Paragraph paragraph = new Paragraph(new Run(instruction));
                RichTextBoxCode.Document.Blocks.Add(paragraph);
            }
        }

        private void ButtonClearMemory_Click(object sender, RoutedEventArgs e)
        {
            DataResources.pram.ClearMemory();
        }
    }
}
