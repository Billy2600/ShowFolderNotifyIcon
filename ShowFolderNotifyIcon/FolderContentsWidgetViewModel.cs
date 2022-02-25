using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.IO.Abstractions;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ShowFolderNotifyIcon
{
    public class FolderContentsWidgetViewModel : INotifyPropertyChanged
    {
        private FolderContentsWidgetModel _folderContentsWidget;
        private readonly IFileSystem _fileSystem;
        private readonly AppSettings _appSettings;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Members

        public string FolderPath
        {
            get { return _folderContentsWidget.FolderPath;}
            set
            {
                if(_folderContentsWidget.FolderPath != value)
                {
                    _folderContentsWidget.FolderPath = value;
                    _appSettings.FolderPath = value;
                    _folderContentsWidget.FileList = GetFileList();
                    OnPropertyChanged(nameof(_folderContentsWidget.FolderPath));
                }
            }
        }

        public List<string>? FileList
        {
            get { return _folderContentsWidget.FileList; }
            set
            {
                if(_folderContentsWidget.FileList != value)
                {
                    _folderContentsWidget.FileList = value;
                    OnPropertyChanged(nameof(_folderContentsWidget.FileList));
                }
            }
        }

        #endregion

        #region Public Methods
        public FolderContentsWidgetViewModel(IFileSystem fileSystem, IOptions<AppSettings> appSettings)
        {
            _folderContentsWidget = new FolderContentsWidgetModel();
            _fileSystem = fileSystem;
            _appSettings = appSettings.Value;

            _folderContentsWidget.FolderPath = _appSettings.FolderPath ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            _folderContentsWidget.FileList = GetFileList();
        }

        public List<string> GetFileList()
        {
            var directoryPaths = _fileSystem.Directory.GetDirectories(_folderContentsWidget.FolderPath);
            var filePaths = _fileSystem.Directory.GetFiles(_folderContentsWidget.FolderPath);

            var fullPathList = directoryPaths.Concat(filePaths).ToList();

            var namesOnly = new List<string>();
            foreach(var path in fullPathList)
            {
                namesOnly.Add(_fileSystem.Path.GetFileName(path));
            }

            namesOnly.RemoveAll(x => x.ToLower() == "desktop.ini");

            return namesOnly;
        }

        public string ExportAppSettings()
        {
            return "{ \"AppSettings\": " + JsonSerializer.Serialize(_appSettings) + " }";
        }

        #endregion

        #region Protected Methods
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
