using Elevator.Automation;
using Elevator.Plugins;
using System.Windows;
using System.Windows.Controls;

namespace Elevator
{
    /// <summary>
    /// Interaction logic for PLCSelect.xaml
    /// </summary>
    public partial class PLCSelect : Window
    {
        public PLCSelect()
        {
            InitializeComponent();
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
                btnSelect.IsEnabled = true;
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
    }
}
