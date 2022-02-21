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
    public class FolderContentsWidgetViewModelTests
    {
        private Fixture _fixture;

        public FolderContentsWidgetViewModelTests()
        {
            _fixture = new Fixture();
        }

        [SetUp]
        public void Setup()
        {
            
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

            var folderContentsWidgetVM = new FolderContentsWidgetViewModel(mockFileSystem.Object);

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

            var folderContentsWidgetVM = new FolderContentsWidgetViewModel(mockFileSystem.Object);

            // Act
            var resultFiles = folderContentsWidgetVM.GetFileList();

            // Assert
            Assert.AreEqual(0, resultFiles.Count);
        }
    }
}