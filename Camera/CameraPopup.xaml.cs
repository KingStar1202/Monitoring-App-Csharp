using Camera.models;
using Camera.UserControls;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace Camera
{
    /// <summary>
    /// Interaction logic for CameraPopup.xaml
    /// </summary>
    public partial class CameraPopup : RadRibbonWindow
    {
        private string roomName { get;set; }
        private string cameraIp { get; set; }
        private CameraPanel cameraPanel { get; set; }  
        private bool outside { get; set; }

        private LibVLC libvlc = new LibVLC();
        public CameraPopup(string roomName, string cameraIp, CameraPanel cameraPanel, bool outside)
        {
            InitializeComponent();
            this.roomName = roomName;
            this.cameraIp = cameraIp;
            this.cameraPanel = cameraPanel;
            this.outside = outside;
            this.Title = roomName;
            string options = "?compression=30&resolution=1920x1080&fps=30&videocodec=h264";
            CameraView.MediaPlayer = new LibVLCSharp.Shared.MediaPlayer(libvlc);
            var media = new Media(libvlc, "rtsp://root:root@" + cameraIp + "/axis-media/media.amp" + options, FromType.FromLocation);
            media.AddOption(":no-sout-rtp-sap");
            media.AddOption(":no-sout-standard-sap");
            media.AddOption(":sout-all");
            media.AddOption(":sout-keep");
            this.CameraView.MediaPlayer.Play(media);
            //outside_media.AddOption(":no-sout-standard-sap" + ":sout-all" + ":sout-keep");
        }

        private void RadRibbonWindow_Closed(object sender, EventArgs e)
        {
            CameraView.MediaPlayer.Stop();
            CameraView.MediaPlayer.Dispose();
            if (outside)
            {
                cameraPanel.clearOutsidePopup();
            }
            else
            {
                cameraPanel.clearInsidePopup();
            }
        }
    }
}
