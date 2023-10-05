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
            }
            else
            {
                MessageBox.Show("Compilation failed");
                MessageBox.Show(DataResources.pram.CompilationErrorMessage);
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
            if(index < 0)
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
            //RichTextBoxCode.PreviewKeyDown += RichTextBox_PreviewKeyDown;
        }

        private void ButtonClearMemory_Click(object sender, RoutedEventArgs e)
        {
            DataResources.pram.ClearMemory();
        }
    }
}
