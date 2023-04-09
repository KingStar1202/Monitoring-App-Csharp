using Camera.constants;
using Camera.models;
using Camera.repository;
using LibVLCSharp.Shared;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for OperatorAwarenessView.xaml
    /// </summary>
    
    public partial class OperatorAwarenessView : RadRibbonWindow
    {
        private OperatorAwarenessRepository operatorAwarenessRepository = OperatorAwarenessRepository.getInstance();
        private CameraOverView cameraOverView { get; set; }
        private LibVLC libvlc = new LibVLC();
        private LibVLCSharp.Shared.MediaPlayer _mediaPlayer;
        private string loggedInUser;
        private PropertyUtil property = PropertyUtil.getInstance();

        private DateTime startDate;
        private DateTime endDate;
 
        public OperatorAwarenessView(string loggedInUser, CameraOverView cameraOverView)
        {
            InitializeComponent();
            this.loggedInUser = loggedInUser;
            this.cameraOverView = cameraOverView;
            SetupAudioAlarm();
            ShowLogo();
            startDate = new DateTime();
        }

        private void ShowLogo()
        {
            string path = property.GetOperatorAwarenessLgo();
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();


                // Set the source of the ImgAwarnessLogo Image control to the selected image file
                ImgAwarnessLogo.Source = bitmap;
            }
            catch (Exception ex)
            {

            }
        }
        [STAThread]
        private void SetupAudioAlarm()
        {
            string workingDir = Environment.CurrentDirectory + @"\user\audio\";
            string buzzerPath = workingDir  + "buzzer.wav";
            if(!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }
            
                _mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(libvlc);
                var outside_media = new Media(libvlc, buzzerPath, FromType.FromLocation);
                _mediaPlayer.Play(outside_media);
           
           
        }
        [STAThread]
        private void OperatorAwarenessClicked()
        {
            endDate = new DateTime();
            long timeTaken = (endDate.Ticks - startDate.Ticks) / 1000;
            operatorAwarenessRepository.SaveLogin(loggedInUser, timeTaken);

            if (_mediaPlayer != null)
            {
                //_mediaPlayer.Stop();
                _mediaPlayer.Dispose();
            }
            cameraOverView.RestartOperatorAwarenessTimer();
        }

        private void logoButton_Click(object sender, RoutedEventArgs e)
        {
            OperatorAwarenessClicked();
            this.Close();
        }

        private void Windows_Closed(object sender, EventArgs e)
        {
            OperatorAwarenessClicked();
        }
    }
}

