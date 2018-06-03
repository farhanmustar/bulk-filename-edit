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
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using BulkFIlenameEdit.UI;
using BulkFIlenameEdit.Utility;
using BulkFIlenameEdit.Core.Filter;
using System.Collections.ObjectModel;
using BulkFIlenameEdit.Core.Builder;
using Path = System.IO.Path;

namespace BulkFIlenameEdit
{
    //TODO: this should be purely ui only. need to refactor split core logic.
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Filter> filters;
        ObservableCollection<Builder> builders;
        public MainWindow()
        {
            InitializeComponent();
            filters = new ObservableCollection<Filter>();
            FilterLV.ItemsSource = filters;
            builders = new ObservableCollection<Builder>();
            BuilderLV.ItemsSource = builders;
        }

        private void InputDirectoryBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Multiselect = false;
                dialog.InitialDirectory = Directory.GetCurrentDirectory();
                dialog.Title = "Input Directory";

                if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                InputDirectoryTxt.Text = dialog.FileName;

            }
        }

        private void OutputDirectoryBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Multiselect = false;
                dialog.InitialDirectory = Directory.GetCurrentDirectory();
                dialog.Title = "Output Directory";

                if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                OutputDirectoryTxt.Text = dialog.FileName;

            }
        }

        private void AddFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectionForm = new TypeSelectionForm(typeof(Filter));
            selectionForm.ShowDialog();
            if (!selectionForm.IsSelected)
            {
                return;
            }
            var filterType = selectionForm.SelectedFilter;
            object filter = Activator.CreateInstance(filterType);

            Type handlerType = typeof(FormHandler<>).MakeGenericType(filterType);
            object formHandler = Activator.CreateInstance(handlerType, filter);

            var createForm = new PopupForm(formHandler as IFormHandler);
            createForm.ShowDialog();

            if (!createForm.IsOK)
            {
                return;
            }
            
            var cleanFilter = formHandler.GetType().GetMethod("GetValue").Invoke(formHandler, null) as Filter;
            if (cleanFilter == null)
            {
                return;
            }
            filters.Add(cleanFilter);
        }

        private void EditFilter_Click(object sender, RoutedEventArgs e)
        {
            var filter = filters[FilterLV.SelectedIndex];

            Type handlerType = typeof(FormHandler<>).MakeGenericType(filter.GetType());
            object formHandler = Activator.CreateInstance(handlerType, filter);

            var createForm = new PopupForm(formHandler as IFormHandler);
            createForm.ShowDialog();

            if (!createForm.IsOK)
            {
                return;
            }


            var cleanFilter = formHandler.GetType().GetMethod("GetValue").Invoke(formHandler, null) as Filter;
            if (cleanFilter == null)
            {
                return;
            }
            cleanFilter.CopyProperties(filters[FilterLV.SelectedIndex]);

            //TODO: each filter should implement INotifyPropertyChange. But for now too lazy to do that...:(.
            //This is workaround.
            FilterLV.ItemsSource = null;
            FilterLV.ItemsSource = filters;

            //need to update builder also to update ui info.. this doesnt need to happen if implement notify prop change huhu
            BuilderLV.ItemsSource = null;
            BuilderLV.ItemsSource = builders;

        }

        private void RemoveFilter_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete?\nWarning : This will also remove builder that depend on it.", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }
            var filter = filters[FilterLV.SelectedIndex];

            var deleteBuilder = builders.Where(b => 
            {
                var builder = b as BuilderFromSource;
                if (builder == null)
                {
                    return false;
                }
                if (builder.Source != filter)
                {
                    return false;
                }
                return true;
            });


            try
            {
                //TODO: investigate this. always throw erro. it seems like it enumerate to the nothingness. weard behaviour.
                foreach (var builder in deleteBuilder)
                {
                    builders.Remove(builder);
                }

            }
            catch (Exception)
            {

            }
            filters.Remove(filter);
        }

        private void AddBuilderBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectionForm = new TypeSelectionForm(typeof(Builder));
            selectionForm.ShowDialog();
            if (!selectionForm.IsSelected)
            {
                return;
            }
            var builderType = selectionForm.SelectedFilter;
            object builder = Activator.CreateInstance(builderType);

            Type handlerType = typeof(BuilderFormHandler<>).MakeGenericType(builderType);
            object formHandler = Activator.CreateInstance(handlerType, builder, filters);

            var createForm = new PopupForm(formHandler as IFormHandler);
            createForm.ShowDialog();

            if (!createForm.IsOK)
            {
                return;
            }

            var cleanBuilder = formHandler.GetType().GetMethod("GetValue").Invoke(formHandler, null) as Builder;
            if (cleanBuilder == null)
            {
                return;
            }
            builders.Add(cleanBuilder);

        }

        private void EditBuilder_Click(object sender, RoutedEventArgs e)
        {

            object builder = builders[BuilderLV.SelectedIndex];

            Type handlerType = typeof(BuilderFormHandler<>).MakeGenericType(builder.GetType());
            object formHandler = Activator.CreateInstance(handlerType, builder, filters);

            var createForm = new PopupForm(formHandler as IFormHandler);
            createForm.ShowDialog();

            if (!createForm.IsOK)
            {
                return;
            }

            var cleanBuilder = formHandler.GetType().GetMethod("GetValue").Invoke(formHandler, null) as Builder;
            if (cleanBuilder == null)
            {
                return;
            }
            cleanBuilder.CopyProperties(builders[BuilderLV.SelectedIndex]);
            //TODO: each builder should implement INotifyPropertyChange. But for now too lazy to do that...:(.
            //This is workaround.
            BuilderLV.ItemsSource = null;
            BuilderLV.ItemsSource = builders;
        }

        private void RemoveBuilder_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            builders.RemoveAt(BuilderLV.SelectedIndex);
        }

        private void ExecuteBtn_Click(object sender, RoutedEventArgs e)
        {
            var ShowError = new Action<string>((errMsg) => 
                                MessageBox.Show(errMsg, "Error", 
                                                MessageBoxButton.OK, 
                                                MessageBoxImage.Error));
            var inputDir = InputDirectoryTxt.Text;
            var outputDir = OutputDirectoryTxt.Text;
            if (!Directory.Exists(inputDir))
            {
                ShowError("Input directory does not exist");
                return;
            }
            if (!Directory.Exists(outputDir))
            {
                ShowError("Output directory does not exist");
                return;
            }

            var isIncludeExtension = IsIncludeFileTypeCB.IsChecked.GetValueOrDefault();
            var isIgnoreExtensionCheck = IsIgnoreExtensionCheck.IsChecked.GetValueOrDefault();
            var inputFiles = Directory.GetFiles(inputDir);

            var outputFiles = new List<string>();
            foreach (var file in inputFiles)
            {
                var filename = isIncludeExtension ? 
                                    Path.GetFileName(file) : 
                                    Path.GetFileNameWithoutExtension(file);
                var outputName = BuildFilename(filename);


                var outputFile = $"{outputDir}/{outputName}" +
                                (isIncludeExtension ?
                                "" : Path.GetExtension(file));
                if (isIncludeExtension && !isIgnoreExtensionCheck)
                {
                    //verify output file got extension
                    if (!Path.HasExtension(outputFile))
                    {
                        ShowError($"Resulting output filename does not have extension.\nOutput: {outputFile} for InputFile: {filename}");
                    }
                }
                outputFiles.Add(outputFile);
            }

            var previewForm = new PreviewOutputForm(inputFiles, outputFiles);
            previewForm.ShowDialog();
            if (!previewForm.IsOk)
            {
                return;
            }

            var outputEnum = outputFiles.GetEnumerator();
            var actionList = inputFiles.Select((s) => {
                                                var outputfile = outputEnum.Current;
                                                outputEnum.MoveNext();
                                                return new { input = s, output = outputEnum.Current };
                                            });

            if (IsMoveOperationCB.IsChecked.GetValueOrDefault())
            {
                foreach (var action in actionList)
                {
                    File.Move(action.input, action.output);
                }
            }
            else
            {
                foreach (var action in actionList)
                {
                    File.Copy(action.input, action.output);
                }
            }
        }

        private string BuildFilename(string filename)
        {
            foreach (var filter in filters)
            {
                filter.Execute(filename);
            }
            var outputName = "";
            foreach (var builder in builders)
            {
                outputName = builder.Execute(outputName);
            }
            return outputName;
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            var ShowError = new Action<string>((errMsg) =>
                                MessageBox.Show(errMsg, "Error",
                                                MessageBoxButton.OK,
                                                MessageBoxImage.Error));
            var isIncludeExtension = IsIncludeFileTypeCB.IsChecked.GetValueOrDefault();
            var isIgnoreExtensionCheck = IsIgnoreExtensionCheck.IsChecked.GetValueOrDefault();
            var file = TestInputTxt.Text;

            var filename = isIncludeExtension ?
                                Path.GetFileName(file) :
                                Path.GetFileNameWithoutExtension(file);
            var outputName = BuildFilename(filename);

            var outputFile = outputName +
                            (isIncludeExtension ?
                            "" : Path.GetExtension(file));
            if (isIncludeExtension && !isIgnoreExtensionCheck)
            {
                //verify output file got extension
                if (!Path.HasExtension(outputFile))
                {
                    ShowError($"Resulting output filename does not have extension.\nOutput: {outputFile} for InputFile: {filename}");
                }
            }
            TestOutputTxt.Text = outputFile;
        }
    }
}
