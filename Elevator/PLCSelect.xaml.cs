using Elevator.Automation;
using Elevator.Automation.IOReadWrite;
using Elevator.Plugins;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Elevator
{
    /// <summary>
    /// Interaction logic for PLCSelect.xaml
    /// </summary>
    public partial class PLCSelect : Window
    {
        public string test { get; set; }
        public PLCSelect()
        {
            InitializeComponent();
            DataContext = this;

        }

        public IO SelectedIO { get; private set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboPLC.ItemsSource = PluginsLoader.PluginsList;
        }

        private void comboPLC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboPLC.SelectedItem is IO io)
            {
                btnSelect.IsEnabled = btnLoad.IsEnabled = btnSave.IsEnabled = true;
                DataContext = io;
                doorsDataGrid.ItemsSource = io.IOContext.Doors;
            }
        }

        private void doorsDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = $"Door {e.Row.GetIndex() + 1}";
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (comboPLC.SelectedItem is IO io)
            {
                SelectedIO = io;
                DialogResult = true;
                Close();
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (comboPLC.SelectedItem is IO io)
                {
                    if (SaverLoader.LoadCopyTo(io.IOContext))
                    {
                        UpdateGUI();
                        MessageBox.Show(this, "Loaded successfuly");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, (ex.InnerException ?? ex).Message);
            }
        }

        private void UpdateGUI()
        {
            foreach (object item in doorsDataGrid.ItemsSource)
                if (doorsDataGrid.ItemContainerGenerator.ContainerFromItem(item) is DataGridRow row)
                    UpdateBindingsExpressionTarget(row.BindingGroup.BindingExpressions);

            UpdateTextBox(txtDown);
            UpdateTextBox(txtUP);
        }

        private void UpdateTextBox(TextBox textbox)
        {
            textbox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        private void UpdateBindingsExpressionTarget(Collection<BindingExpressionBase> bindingExpressions)
        {
            foreach (var binding in bindingExpressions)
                binding.UpdateTarget();

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (comboPLC.SelectedItem is IO io)
                    MessageBox.Show(this, SaverLoader.Save(io.IOContext) ? "Saved successfuly" : "Error saving");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }
    }
}
