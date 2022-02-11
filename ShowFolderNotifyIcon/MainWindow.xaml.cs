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

namespace ShowFolderNotifyIcon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int TaskbarHeight = 44;

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
            var startPos = GetWindowStartPos();
            this.Left = startPos.X;
            this.Top = startPos.Y;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Get the position the window should be in
        /// </summary>
        // Public, as to be unit testable
        public Point GetWindowStartPos()
        {
            return new Point(
                    System.Windows.SystemParameters.PrimaryScreenWidth - this.Width,
                    System.Windows.SystemParameters.PrimaryScreenHeight - this.Height - TaskbarHeight
                );
        }

        #endregion
    }
}
