using Aspose.Cells.Charts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Camera.net2access
{
    public class FileWatcher
    {
        private FileInfo file { get; set; }
        private bool shutdown = false;
        private Net2Service net2Service { get; set; }
        public FileWatcher(FileInfo file, CameraOverView cameraOverView)
        {
            this.file = file;
            net2Service = new Net2Service(cameraOverView);
            
        }

        public void Start()
        {
            Thread thread = new Thread(() =>
            {
                try
                {
                    //Path path = File(file).
                    string path = Path.GetDirectoryName(file.FullName);
                    FileSystemWatcher watcher = new FileSystemWatcher(path);
                    doOnChange();
                    watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;
                    
                    

                    watcher.Changed += OnChanged;
                    watcher.Created += OnCreated;

                }
                catch (Exception ex)
                {
                    Thread.CurrentThread.Interrupt();
                    shutdown = true;
                }
            });
            thread.Start();
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            doOnChange();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            doOnChange();
        }

        public bool isStopped() { return shutdown; }
        public void doOnChange()
        {
            net2Service.ReadFileAndSaveToDatabase();
        }
    }
}
