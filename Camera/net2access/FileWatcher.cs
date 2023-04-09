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
                }
                catch (Exception ex)
                {

                }
            });
            thread.Start();
        }
        public bool isStopped() { return shutdown; }
        public void doOnChange()
        {
            net2Service.ReadFileAndSaveToDatabase();
        }
    }
}
