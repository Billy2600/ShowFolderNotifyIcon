using AutoFixture;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using ShowFolderNotifyIcon;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using System.Threading;

namespace ShowFolderNotifyIconTest
{
    [Apartment(ApartmentState.STA)]
    public class FolderContentsWidgetViewModelTests
    {
        private Fixture _fixture;
        private Mock<IOptions<AppSettings>> _mockOptions;
        private AppSettings _appSettings;

        public FolderContentsWidgetViewModelTests()
        {
            _fixture = new Fixture();
        }

        [SetUp]
        public void Setup()
        {
            _mockOptions = new Mock<IOptions<AppSettings>>();
            _appSettings = new AppSettings();

            _mockOptions.Setup(x => x.Value).Returns(_appSettings);
        }

        [Test]
        public void GetFileList_HappyPath()
        {
            // Arrange
            var mockDirectoryPaths = new string[]
            {
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>()
            };

            var mockFilePaths = new string[]
            {
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>()
            };

            var mockFileSystem = new Mock<IFileSystem>();
            mockFileSystem.Setup(x => x.Directory.GetDirectories(It.IsAny<string>())).Returns(mockDirectoryPaths);
            mockFileSystem.Setup(x => x.Directory.GetFiles(It.IsAny<string>())).Returns(mockFilePaths);
            mockFileSystem.Setup(x => x.Path.GetFileName(It.IsAny<string>())).Returns<string>(x => x);

            var folderContentsWidgetVM = new FolderContentsWidgetViewModel(mockFileSystem.Object, _mockOptions.Object);

            // Act
            var resultFiles = folderContentsWidgetVM.GetFileList();

            // Assert
            var allMockPaths = new List<string>();
            allMockPaths.AddRange(mockDirectoryPaths);
            allMockPaths.AddRange(mockFilePaths);

            for (int i = 0; i < resultFiles.Count; i++)
            {
                Assert.AreEqual(allMockPaths[i], resultFiles[i]);
            }
        }

        [Test]
        public void GetFileList_NoFiles()
        {
            // Arrange
            var mockPaths = new string[] { };

            var mockFileSystem = new Mock<IFileSystem>();
            mockFileSystem.Setup(x => x.Directory.GetDirectories(It.IsAny<string>())).Returns(mockPaths);
            mockFileSystem.Setup(x => x.Directory.GetFiles(It.IsAny<string>())).Returns(mockPaths);
            mockFileSystem.Setup(x => x.Path.GetFileName(It.IsAny<string>())).Returns<string>(x => x);

            var folderContentsWidgetVM = new FolderContentsWidgetViewModel(mockFileSystem.Object, _mockOptions.Object);

            // Act
            var resultFiles = folderContentsWidgetVM.GetFileList();

            // Assert
            Assert.AreEqual(0, resultFiles.Count);
        }

        [Test]
        public void ExportAppSettings_HappyPath()
        {
            // Arrange
            var folderPath = "C:\\folders\\" + _fixture.Create<string>();

            var mockFileSystem = new Mock<IFileSystem>();
            _appSettings.FolderPath = folderPath;

            var folderContentsWidgetVM = new FolderContentsWidgetViewModel(mockFileSystem.Object, _mockOptions.Object);

            // Act
            var resultJson = folderContentsWidgetVM.ExportAppSettings();

            // Assert
            Assert.AreEqual("{ \"AppSettings\": {\"FolderPath\":\"" + Regex.Escape(folderPath) + "\"} }", resultJson);
        }

        [Test]
        public void ExportAppSettings_NullFolderPath()
        {
            // Arrange
            var mockFileSystem = new Mock<IFileSystem>();
            _appSettings.FolderPath = null;

            var folderContentsWidgetVM = new FolderContentsWidgetViewModel(mockFileSystem.Object, _mockOptions.Object);

            // Act
            var resultJson = folderContentsWidgetVM.ExportAppSettings();

            // Assert
            Assert.AreEqual("{ \"AppSettings\": {\"FolderPath\":null} }", resultJson);
        }

        [Test]
        public void ExportAppSettings_EmptyFolderPath()
        {
            // Arrange
            var mockFileSystem = new Mock<IFileSystem>();
            _appSettings.FolderPath = string.Empty;

            var folderContentsWidgetVM = new FolderContentsWidgetViewModel(mockFileSystem.Object, _mockOptions.Object);

            // Act
            var resultJson = folderContentsWidgetVM.ExportAppSettings();

            // Assert
            Assert.AreEqual("{ \"AppSettings\": {\"FolderPath\":\"\"} }", resultJson);
        }
    }
}