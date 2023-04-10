using Camera.constants;
using Camera.models;
using Camera.repository;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        private string loggedInUser;
        private PropertyUtil property = PropertyUtil.getInstance();
        private MediaPlayer mediaPlayer = new MediaPlayer();
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
            try
            {
                string workingDir = Environment.CurrentDirectory + @"\user\audio\";
                string buzzerPath = workingDir + "buzzer.wav";
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }
                mediaPlayer.Open(new Uri(buzzerPath));
                mediaPlayer.Position = (TimeSpan.FromSeconds(10));
                mediaPlayer.Play();
            }
            catch (Exception ex)
            {

            }
            
        }

        [STAThread]
        private void OperatorAwarenessClicked()
        {
            endDate = new DateTime();
            long timeTaken = (endDate.Ticks - startDate.Ticks) / 1000;
            operatorAwarenessRepository.SaveLogin(loggedInUser, timeTaken);

            if (mediaPlayer != null)
            {
                //_mediaPlayer.Stop();
                mediaPlayer.Stop();
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

