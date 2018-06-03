using BulkFIlenameEdit.Core.Forms;
using BulkFIlenameEdit.Utility;
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
    /// Interaction logic for PopupForm.xaml
    /// </summary>
    public partial class PopupForm : Window
    {
        bool isOK = false;
        public bool IsOK { get { return isOK; } }
        IFormHandler formHandler;
        public ListView FormItem { get; set; }
        public PopupForm(IFormHandler _formHandler)
        {
            this.formHandler = _formHandler;
            InitializeComponent();
            FormItem = formHandler.GetForm();
            FormView.Children.Add(FormItem);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            isOK = false;
            Close();
        }

        public IFormHandler GetHandler()
        {
            return formHandler;
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            ErrorPanel.Visibility = Visibility.Collapsed;
            formHandler.UpdateData(FormItem);
            string error = "";
            if (formHandler.Validate(ref error))
            {
                isOK = true;
                Close();
            }
            else
            {
                ErrorTxt.Text = error;
                ErrorPanel.Visibility = Visibility.Visible;
            }
        }

        private void ErrorExitBtn_Click(object sender, RoutedEventArgs e)
        {
            ErrorPanel.Visibility = Visibility.Collapsed;
        }
    }
}
