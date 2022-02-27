using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
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

            _folderContentsWidgetViewModel.GetFileList();

            // Taskbar icon will handle all the taskbar duties
            ShowInTaskbar = false;

            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            SizeChanged += MainWindow_SizeChanged;
            taskbarIcon.TrayLeftMouseDown += TaskbarIcon_Click;
            
            OpenFolderDialog.MouseEnter += OpenFolderDialog_MouseEnter;
            OpenFolderDialog.MouseLeave += OpenFolderDialog_MouseLeave;
            OpenFolderDialog.MouseLeftButtonUp += OpenFolderDialog_MouseLeftUp;

            RefreshView.MouseEnter += RefreshView_MouseEnter;
            RefreshView.MouseLeave += RefreshView_MouseLeave;
            RefreshView.MouseLeftButtonUp += RefreshView_MouseLeftUp;

            CloseWindow.MouseEnter += CloseWindow_MouseEnter;
            CloseWindow.MouseLeave += CloseWindow_MouseLeave;
            CloseWindow.MouseLeftButtonUp += CloseWindow_MouseLeftUp;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the position the window should be in
        /// </summary>
        private Point GetWindowStartPos()
        {
            return new Point(
                    System.Windows.SystemParameters.PrimaryScreenWidth - this.Width,
                    System.Windows.SystemParameters.PrimaryScreenHeight - this.Height - TaskbarHeight
                );
        }

        private void PopulateGrid()
        {
            if (_folderContentsWidgetViewModel.FileList != null)
            {
                foreach (var file in _folderContentsWidgetViewModel.FileList)
                {
                    mainGrid.RowDefinitions.Add(new RowDefinition());
                    var fileTextBox = new TextBox();
                    fileTextBox.Text = file;
                    fileTextBox.SetValue(Grid.RowProperty, mainGrid.RowDefinitions.Count - 1);
                    fileTextBox.SetValue(Grid.ColumnProperty, 0);
                    fileTextBox.Background = new SolidColorBrush(Color.FromRgb(19, 19, 19));
                    fileTextBox.FontSize = 16;
                    fileTextBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    fileTextBox.BorderBrush = null;
                    fileTextBox.Cursor = Cursors.Hand;
                    fileTextBox.PreviewMouseLeftButtonDown += FileTextBox_PreviewMouseLeftButtonDown;

                    mainGrid.Children.Add(fileTextBox);
                }
            }
        }

        private void ClearGrid()
        {
            // For now we can just remove all the text boxes
            var elementsToRemove = new List<UIElement>();

            foreach (UIElement element in mainGrid.Children)
            {
                if(element is TextBox)
                {
                    elementsToRemove.Add(element);
                }
            }

            foreach(var element in elementsToRemove)
            {
                mainGrid.Children.Remove(element);
            }


            for (int i = 0; i < mainGrid.RowDefinitions.Count; i++)
            {
                mainGrid.RowDefinitions.RemoveAt(i);
            }
        }

        #endregion

        #region Event handlers

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var startPos = GetWindowStartPos();
            this.Left = startPos.X;
            this.Top = startPos.Y;

            PopulateGrid();
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            var appSettingsJson = _folderContentsWidgetViewModel.ExportAppSettings();
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ShowFolderNotifyIcon\\appsettings.json", appSettingsJson);
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CloseWindow.Margin = new Thickness(e.NewSize.Width - 35, 0, 0, 0);
            mainGrid.Width = e.NewSize.Width - 40;
            MainColumn.Width = new GridLength(e.NewSize.Width - 40);
            MainScrollViewer.Height = e.NewSize.Height - 35;
        }

        private void TaskbarIcon_Click(object sender, RoutedEventArgs e)
        {
            if(Visibility == Visibility.Visible)
            {
                Hide();
            }
            else if(Visibility == Visibility.Hidden)
            {
                Show();
            }
        }

        private void TaskBarIconMenuClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenFolderDialog_MouseEnter(object sender, MouseEventArgs e)
        {
            OpenFolderDialog.Background = new SolidColorBrush(Colors.Gray);
        }

        private void OpenFolderDialog_MouseLeave(object sender, MouseEventArgs e)
        {
            OpenFolderDialog.Background = new SolidColorBrush(Colors.Black);
        }

        // Opens file dialogue
        private void OpenFolderDialog_MouseLeftUp(object sender, MouseEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select folder to display",
                UseDescriptionForTitle = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + System.IO.Path.DirectorySeparatorChar, ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if(dialog.SelectedPath != _folderContentsWidgetViewModel.FolderPath)
                {
                    _folderContentsWidgetViewModel.FolderPath = dialog.SelectedPath; // Also calls GetFileList()
                    ClearGrid();
                    PopulateGrid();
                }
            }
        }

        private void RefreshView_MouseEnter(object sender, MouseEventArgs e)
        {
            RefreshView.Background = new SolidColorBrush(Colors.Gray);
        }

        private void RefreshView_MouseLeave(object sender, MouseEventArgs e)
        {
            RefreshView.Background = new SolidColorBrush(Colors.Black);
        }

        private void RefreshView_MouseLeftUp(object sender, MouseEventArgs e)
        {
            _folderContentsWidgetViewModel.GetFileList();
            ClearGrid();
            PopulateGrid();
        }

        private void CloseWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseWindow.Background = new SolidColorBrush(Colors.Gray);
        }

        private void CloseWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseWindow.Background = new SolidColorBrush(Colors.Black);
        }

        private void CloseWindow_MouseLeftUp(object sender, MouseEventArgs e)
        {
            Hide();
        }

        // Need to use *Preview*MouseLeftButtonDown as it captures any clicks before the internal parts of the control process the click
        private void FileTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBoxSender = sender as TextBox;
            if (textBoxSender != null && textBoxSender.Text != "This folder is empty.")
            {
                var fileOrFolderPath = _folderContentsWidgetViewModel.FolderPath + "\\" + textBoxSender.Text;
                Process.Start("explorer", $"/select, {fileOrFolderPath}");
            }
        }

        #endregion
    }
}
