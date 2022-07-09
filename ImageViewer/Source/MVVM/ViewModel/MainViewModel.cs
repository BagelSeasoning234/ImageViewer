using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ImageViewer.Core;
using ImageViewer.Source.MVVM.Model;
using Microsoft.Win32;

namespace ImageViewer.Source.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        #region Commands

        public RelayCommand BrowseFileCommand { get; set; }

        public RelayCommand NextFileCommand { get; set; }
        public RelayCommand PreviousFileCommand { get; set; }

        #endregion

        #region Private Members

        // The file manager class
        private FileManager m_FileManager;

        #endregion

        #region Public Members

        //The file manager class
        public FileManager FileManager { get { return m_FileManager; } }

        #endregion

        // Constructor
        public MainViewModel()
        {
            m_FileManager = new FileManager();

            FileManager.FilesInCurrentFolder = new string[0];

            BrowseFileCommand = new RelayCommand(o => { m_FileManager.BrowseFiles(); 
                OnPropertyChanged(); });

            NextFileCommand = new RelayCommand(o => { m_FileManager.ChangeFile(true); });
            PreviousFileCommand = new RelayCommand(o => { m_FileManager.ChangeFile(false); });
        }
    }
}
