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
using System.Windows.Shapes;

namespace BulkFIlenameEdit.UI
{
    /// <summary>
    /// Interaction logic for PreviewOutputForm.xaml
    /// </summary>
    public partial class PreviewOutputForm : Window
    {
        public bool IsOk { get; set; }
        public PreviewOutputForm(IEnumerable<string> inputFile, IEnumerable<string> outputFile)
        {
            InitializeComponent();
            IsOk = false;
            InputFileLV.ItemsSource = inputFile;
            OutputFileLV.ItemsSource = outputFile;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExecuteBtn_Click(object sender, RoutedEventArgs e)
        {
            IsOk = true;
            Close();
        }
    }
}
