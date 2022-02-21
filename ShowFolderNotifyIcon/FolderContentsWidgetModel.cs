using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShowFolderNotifyIcon
{
    public class FolderContentsWidgetModel
    {
        public string FolderPath { get; set; }
        public List<string>? FileList { get; set; }
    }
}
