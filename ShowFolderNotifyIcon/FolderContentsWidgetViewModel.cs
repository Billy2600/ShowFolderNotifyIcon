using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.IO.Abstractions;

namespace ShowFolderNotifyIcon
{
    public class FolderContentsWidgetViewModel : INotifyPropertyChanged
    {
        private FolderContentsWidgetModel _folderContentsWidget;
        private readonly IFileSystem _fileSystem;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Members

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
        public FolderContentsWidgetViewModel(IFileSystem fileSystem)
        {
            _folderContentsWidget = new FolderContentsWidgetModel();
            _fileSystem = fileSystem;

            _folderContentsWidget.FileList = GetFileList();
        }

        public List<string> GetFileList()
        {
            const string path = "D:\\steamapps\\steamapps\\common";
            var directories = _fileSystem.Directory.GetDirectories(path);
            var files = _fileSystem.Directory.GetFiles(path);

            return directories.Concat(files).ToList();
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
