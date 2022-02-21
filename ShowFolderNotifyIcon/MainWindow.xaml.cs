using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
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

namespace ShowFolderNotifyIcon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FolderContentsWidgetViewModel _folderContentsWidgetViewModel;

        public const int TaskbarHeight = 44;

        #region Constructors

        public MainWindow(FolderContentsWidgetViewModel folderContentsWidgetViewModel)
        {
            _folderContentsWidgetViewModel = folderContentsWidgetViewModel;
            DataContext = _folderContentsWidgetViewModel;

            // Taskbar icon will handle all the taskbar duties
            this.ShowInTaskbar = false;

            InitializeComponent();
            Loaded += MainWindow_Loaded;
            taskbarIcon.TrayLeftMouseDown += TaskbarIcon_Click;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the position the window should be in
        /// </summary>
        public Point GetWindowStartPos()
        {
            return new Point(
                    System.Windows.SystemParameters.PrimaryScreenWidth - this.Width,
                    System.Windows.SystemParameters.PrimaryScreenHeight - this.Height - TaskbarHeight
                );
        }

        public void PopulateGrid(List<string>? files)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    mainGrid.RowDefinitions.Add(new RowDefinition());
                    var fileTextBox = new TextBox();
                    fileTextBox.Text = file;
                    fileTextBox.SetValue(Grid.RowProperty, mainGrid.RowDefinitions.Count - 1);
                    fileTextBox.SetValue(Grid.ColumnProperty, 0);
                    fileTextBox.Background = new SolidColorBrush(Color.FromRgb(19, 19, 19));
                    fileTextBox.FontSize = 16;
                    fileTextBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    mainGrid.Children.Add(fileTextBox);
                }
            }
        }

        #endregion

        #region Event handlers

        // So we can have "clean" unit tests that aren't calling a bunch of crazy methods because everything's in the constructor
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var startPos = GetWindowStartPos();
            this.Left = startPos.X;
            this.Top = startPos.Y;

            PopulateGrid(_folderContentsWidgetViewModel.FileList);
        }

        public void TaskbarIcon_Click(object sender, RoutedEventArgs e)
        {
            if(WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }
            else if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Minimized;
            }
        }

        #endregion
    }
}
