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
        private readonly IFileSystem _fileSystem;

        public const int TaskbarHeight = 44;

        #region Constructors

        public MainWindow(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;

            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        #endregion

        // Most methods are public, as to be unit testable
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

        public List<string> GetFileList()
        {
            const string path = "D:\\documents\\QA stuff";
            var directories = _fileSystem.Directory.GetDirectories(path);
            var files = _fileSystem.Directory.GetFiles(path);

            return directories.Concat(files).ToList();
        }

        // For unit testing
        public Grid GetMainGrid()
        {
            return mainGrid;
        }

        public void PopulateGrid(List<string> files)
        {
            foreach(var file in files)
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

        #endregion

        #region Private methods

        // So we can have "clean" unit tests that aren't calling a bunch of crazy methods because everything's in the constructor
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var startPos = GetWindowStartPos();
            this.Left = startPos.X;
            this.Top = startPos.Y;

            var files = GetFileList();
            PopulateGrid(files);
        }

        #endregion
    }
}
