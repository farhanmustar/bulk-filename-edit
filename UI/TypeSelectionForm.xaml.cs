using BulkFIlenameEdit.Core.Filter;
using BulkFIlenameEdit.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for FilterSelectionForm.xaml
    /// </summary>
    public partial class TypeSelectionForm : Window
    {
        public Type SelectedFilter { get; set; }
        public bool IsSelected { get; set; }
        public TypeSelectionForm(Type type)
        {
            InitializeComponent();
            IsSelected = false;
            var filters = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(type) && !t.IsAbstract);
            foreach (var filter in filters)
            {
                var btn = new Button();
                btn.Content = filter.GetDisplayName();
                btn.ToolTip = filter.GetDescription();
                btn.Click += (o, e) => 
                {
                    CreateFilter(filter);
                };
                FilterList.Items.Add(btn);
            }
        }



        void CreateFilter(Type type)
        {
            SelectedFilter = type;
            IsSelected = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
