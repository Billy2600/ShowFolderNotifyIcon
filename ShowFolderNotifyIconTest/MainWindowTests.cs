using NUnit.Framework;
using ShowFolderNotifyIcon;
using System.Threading;

namespace ShowFolderNotifyIconTest
{
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetWindowStartPos()
        {
            // Arrange
            var mainWindow = new MainWindow();
            var expectedX = System.Windows.SystemParameters.PrimaryScreenWidth - mainWindow.Width;
            var expectedY = System.Windows.SystemParameters.PrimaryScreenHeight - mainWindow.Height - MainWindow.TaskbarHeight;

            // Act
            var startPos = mainWindow.GetWindowStartPos();

            // Assert
            Assert.AreEqual(expectedX, startPos.X);
            Assert.AreEqual(expectedY, startPos.Y);
        }
    }
}