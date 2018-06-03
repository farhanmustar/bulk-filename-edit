using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using BulkFIlenameEdit.Core.Forms;
using System.Reflection;
using System.ComponentModel;
using Xceed.Wpf.Toolkit;
using BulkFIlenameEdit.Core.Filter;

namespace BulkFIlenameEdit.Utility
{
    public static class FormFactory
    {
        public static ListView CreateForm<T>(T data = null) where T : class, IFormValidator, new()
        {
            if (data == null)
            {
                data = new T();
            }

            var classType = data.GetType();
            string className = classType.GetDisplayName();
            string classTooltip = classType.GetDescription();

            var listView = new ListView();
            listView.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            var titleTxt = new TextBlock();
            titleTxt.HorizontalAlignment = HorizontalAlignment.Center;
            titleTxt.Text = className;
            titleTxt.ToolTip = classTooltip;

            listView.Items.Add(titleTxt);


            var properties = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance)
                                            .Where(p=>p.CanRead && p.CanWrite);
            foreach (var prop in properties)
            {
                var type = prop.PropertyType;
                string name = prop.GetDisplayName();

                string tooltip = prop.GetDescription();

                if (type == typeof(bool))
                {
                    bool value = (prop.GetValue(data) as bool?).GetValueOrDefault();
                    listView.Items.Add(value.GenForm(name, tooltip));
                }
                else if(type == typeof(string))
                {
                    string value = (prop.GetValue(data) as string);
                    if (string.IsNullOrEmpty(value)) value = "";

                    listView.Items.Add(value.GenForm(name, tooltip));
                }
                else if (type == typeof(int))
                {
                    int value = (prop.GetValue(data) as int?).GetValueOrDefault();
                    listView.Items.Add(value.GenForm(name, tooltip));
                }
            }

            return listView;
        }
        public static ListView CreateForm<T>(IEnumerable<Filter> filters, T data = null) where T : class, IFormValidator, new()
        {
            var lv = CreateForm(data);

            var properties = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance)
                                            .Where(p => p.CanRead && p.CanWrite);
            foreach (var prop in properties)
            {
                var type = prop.PropertyType;
                string name = type.GetDisplayName();
                string tooltip = type.GetDescription();

                if (type == typeof(Filter))
                {
                    Filter value = (prop.GetValue(data) as Filter);
                    lv.Items.Add(value.GenForm(name, tooltip, filters));
                }
            }
            return lv;
        }

        public static T ParseForm<T>(ListView formLV) where T: class, IFormValidator, new()
        {
            var data = new T();

            int i = 1;
            int propNum = formLV.Items.Count;
            //for now assume this will always produce same ordered as form gen method
            var properties = data.GetType().GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                                            .Where(p => p.CanRead && p.CanWrite);
            foreach (var prop in properties)
            {
                var type = prop.PropertyType;
                if (type == typeof(bool))
                {
                    prop.SetValue(data, ParseBool(formLV.Items[i] as DockPanel));
                }
                else if (type == typeof(string))
                {
                    prop.SetValue(data, ParseString(formLV.Items[i] as DockPanel));
                }
                else if (type == typeof(int))
                {
                    prop.SetValue(data, ParseInteger(formLV.Items[i] as DockPanel));
                }
                else
                {
                    continue;
                }
                i++;

                if (i>=propNum)
                {
                    break;
                }
            }
            return data;
        }
        //TODO: need more elegan solution. 
        public static T ParseForm<T>(IEnumerable<Filter> filters, ListView formLV) where T : class, IFormValidator, new()
        {
            T data = ParseForm<T>(formLV);

            int propNum = formLV.Items.Count;
            //for now assume this will always produce same ordered as form gen method
            var properties = data.GetType().GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                                            .Where(p => p.CanRead && p.CanWrite);
            //TODO: need to refactor should not be like this shit.. maybe setvalue got library or something 
            int i = properties.Where(prop => prop.PropertyType == typeof(bool) || 
                                            prop.PropertyType == typeof(string) ||
                                            prop.PropertyType == typeof(int))
                                            .Count() + 1;

            foreach (var prop in properties)
            {
                var type = prop.PropertyType;
                if (type == typeof(Filter))
                {
                    prop.SetValue(data, ParseFilter(formLV.Items[i] as DockPanel));
                }
                else
                {
                    continue;
                }
                i++;

                if (i>=propNum)
                {
                    break;
                }
            }
            return data;
        }


        //TODO: this kind a repetetive. should refactor. :(
        static DockPanel GenForm(this bool value, string name, string desc)
        {

            var checkBox = new CheckBox();
            checkBox.IsChecked = value;
            checkBox.FlowDirection = FlowDirection.RightToLeft;
            checkBox.HorizontalAlignment = HorizontalAlignment.Right;
            //checkBox.ToolTip = desc;

            var des = new TextBlock();
            des.Text = name;
            des.HorizontalAlignment = HorizontalAlignment.Left;


            var dockPanel = new DockPanel();
            dockPanel.Children.Add(des);
            dockPanel.Children.Add(checkBox);
            if(!string.IsNullOrEmpty(desc))
                dockPanel.ToolTip = desc;
            return dockPanel;
        }

        static bool ParseBool(DockPanel panel)
        {
            if (panel.Children.Count != 2)
            {
                throw new ArgumentException("Argument format error");
            }
            var cb = panel.Children[1] as CheckBox;
            if (cb == null)
            {
                throw new FormatException("Fail to parse the argument");
            }
            return cb.IsChecked.GetValueOrDefault();
        }

        static DockPanel GenForm(this string value, string name, string desc)
        {
            var des = new TextBlock();
            des.Text = name;
            des.HorizontalAlignment = HorizontalAlignment.Left;


            var input = new TextBox();
            input.Text = value;
            input.HorizontalAlignment = HorizontalAlignment.Right;
            input.MinWidth = 120;

            var dockPanel = new DockPanel();
            dockPanel.Children.Add(des);
            dockPanel.Children.Add(input);
            if (!string.IsNullOrEmpty(desc))
                dockPanel.ToolTip = desc;

            return dockPanel;
        }

        static string ParseString(DockPanel panel)
        {
            if (panel.Children.Count != 2)
            {
                throw new ArgumentException("Argument format error");
            }
            var tb = panel.Children[1] as TextBox;

            if (tb == null)
            {
                throw new FormatException("Fail to parse the argument");
            }

            return tb.Text;
        }

        static DockPanel GenForm(this int value, string name, string desc)
        {
            var des = new TextBlock();
            des.Text = name;
            des.HorizontalAlignment = HorizontalAlignment.Left;


            var input = new IntegerUpDown();
            input.DisplayDefaultValueOnEmptyText = true;
            input.Value = value;
            input.HorizontalAlignment = HorizontalAlignment.Right;
            input.MinWidth = 120;

            var dockPanel = new DockPanel();
            DockPanel.SetDock(des, Dock.Left);
            DockPanel.SetDock(input, Dock.Right);
            dockPanel.Children.Add(des);
            dockPanel.Children.Add(input);
            if (!string.IsNullOrEmpty(desc))
                dockPanel.ToolTip = desc;

            return dockPanel;
        }

        static int ParseInteger(DockPanel panel)
        {
            if (panel.Children.Count != 2)
            {
                throw new ArgumentException("Argument format error");
            }
            var ud = panel.Children[1] as IntegerUpDown;

            if (ud == null)
            {
                throw new FormatException("Fail to parse the argument");
            }

            return ud.Value.GetValueOrDefault();
        }

        static DockPanel GenForm(this Filter filter, string name, string desc, IEnumerable<Filter> filters)
        {
            var des = new TextBlock();
            des.Text = name;
            des.HorizontalAlignment = HorizontalAlignment.Left;


            var cb = new ComboBox();
            cb.ItemsSource = filters;
            cb.MinWidth = 120;
            cb.HorizontalAlignment = HorizontalAlignment.Right;
            cb.SelectedItem = filter;

            var dockPanel = new DockPanel();
            DockPanel.SetDock(des, Dock.Left);
            DockPanel.SetDock(cb, Dock.Right);
            dockPanel.Children.Add(des);
            dockPanel.Children.Add(cb);
            if (!string.IsNullOrEmpty(desc))
                dockPanel.ToolTip = desc;

            return dockPanel;
        }
        static Filter ParseFilter(DockPanel panel)
        {
            if (panel.Children.Count != 2)
            {
                throw new ArgumentException("Argument format error");
            }
            var ud = panel.Children[1] as ComboBox;

            if (ud == null)
            {
                throw new FormatException("Fail to parse the argument");
            }

            return ud.SelectedItem as Filter;
        }
    }

    public class FormHandler<T> : IFormHandler where T : class, IFormValidator, new()
    {
        T data;
        public FormHandler(T data)
        {
            this.data = data;
        }

        public ListView GetForm()
        {
            return FormFactory.CreateForm<T>(data);
        }
        public void UpdateData(ListView form)
        {
            data = FormFactory.ParseForm<T>(form);
        }
        public bool Validate(ref string errorMsg)
        {
            return data.Validate(ref errorMsg);
        }
        public T GetValue()
        {
            return data;
        }
    }

    public interface IFormHandler : IFormValidator
    {
        ListView GetForm();
        void UpdateData(ListView form);
    }
    
    public class BuilderFormHandler<T> : IFormHandler where T : class, IFormValidator, new()
    {
        T data;
        IEnumerable<Filter> filter;
        public BuilderFormHandler(T data, IEnumerable<Filter> filter)
        {
            this.data = data;
            this.filter = filter;
        }

        public ListView GetForm()
        {
            return FormFactory.CreateForm<T>(filter, data);
        }
        public void UpdateData(ListView form)
        {
            data = FormFactory.ParseForm<T>(filter, form);
        }
        public bool Validate(ref string errorMsg)
        {
            return data.Validate(ref errorMsg);
        }
        public T GetValue()
        {
            return data;
        }
    }
}
