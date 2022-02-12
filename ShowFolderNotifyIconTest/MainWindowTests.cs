using AutoFixture;
using Moq;
using NUnit.Framework;
using ShowFolderNotifyIcon;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading;

namespace ShowFolderNotifyIconTest
{
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private Fixture _fixture;

        public MainWindowTests()
        {
            _fixture = new Fixture();
        }

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void GetWindowStartPos_HappyPath()
        {
            // Arrange
            var mockFileSystem = new Mock<IFileSystem>();
            var mainWindow = new MainWindow(mockFileSystem.Object);
            var expectedX = System.Windows.SystemParameters.PrimaryScreenWidth - mainWindow.Width;
            var expectedY = System.Windows.SystemParameters.PrimaryScreenHeight - mainWindow.Height - MainWindow.TaskbarHeight;

            // Act
            var startPos = mainWindow.GetWindowStartPos();

            // Assert
            Assert.AreEqual(expectedX, startPos.X);
            Assert.AreEqual(expectedY, startPos.Y);
        }

        [Test]
        public void GetFileList_HappyPath()
        {
            // Arrange
            var mockFiles = new string[]
            {
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>()
            };

            var mockFileSystem = new Mock<IFileSystem>();
            mockFileSystem.Setup(x => x.Directory.GetDirectories(It.IsAny<string>())).Returns(mockFiles);

            var mainWindow = new MainWindow(mockFileSystem.Object);

            // Act
            var resultFiles = mainWindow.GetFileList();

            // Assert
            for(int i = 0; i < resultFiles.Count; i++)
            {
                Assert.AreEqual(mockFiles[i], resultFiles[i]);
            }
        }

        [Test]
        public void GetFileList_NoFiles()
        {
            // Arrange
            var mockFiles = new string[] { };

            var mockFileSystem = new Mock<IFileSystem>();
            mockFileSystem.Setup(x => x.Directory.GetDirectories(It.IsAny<string>())).Returns(mockFiles);

            var mainWindow = new MainWindow(mockFileSystem.Object);

            // Act
            var resultFiles = mainWindow.GetFileList();

            // Assert
            Assert.AreEqual(0, resultFiles.Count);
        }

        [Test]
        public void PopulateGrid_HappyPath()
        {
            // Arrange
            var mockFiles = new List<string>()
            {
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>()
            };

            var mockFileSystem = new Mock<IFileSystem>();
            var mainWindow = new MainWindow(mockFileSystem.Object);

            // Act 
            mainWindow.PopulateGrid(mockFiles);

            // Assert
            Assert.AreEqual(mockFiles.Count, mainWindow.GetMainGrid().RowDefinitions.Count);
        }

        [Test]
        public void PopulateGrid_NoFiles()
        {
            // Arrange
            var mockFiles = new List<string>() { };

            var mockFileSystem = new Mock<IFileSystem>();
            var mainWindow = new MainWindow(mockFileSystem.Object);

            // Act 
            mainWindow.PopulateGrid(mockFiles);

            // Assert
            Assert.AreEqual(0, mainWindow.GetMainGrid().RowDefinitions.Count);
        }
    }
}