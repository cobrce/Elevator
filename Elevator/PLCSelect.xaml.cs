using Elevator.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Elevator
{
    /// <summary>
    /// Interaction logic for PLCSelect.xaml
    /// </summary>
    public partial class PLCSelect : Window
    {
        private IO[] _ioArray;
        private int _numberOfDoors;

        private PLCSelect()
        {
            InitializeComponent();
        }

        public PLCSelect(int numberOfDoors, IO[] ioarray) : this()
        {
            _ioArray = ioarray;
            _numberOfDoors = numberOfDoors;
        }

        public IO SelectedIO { get; private set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboPLC.ItemsSource = _ioArray;
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
                Close();
            }
        }
    }
}
