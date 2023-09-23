using PRAM_lib.Processor;
using PRAM_simulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        }
        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            DataResources.pram.Clear();
            RestartedMasterMemoryBindings();
        }

        private void ButtonNextExecution_Click(object sender, RoutedEventArgs e)
        {
            bool result = DataResources.pram.ExecuteNextInstruction();

            if(!result)
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
    }
}
