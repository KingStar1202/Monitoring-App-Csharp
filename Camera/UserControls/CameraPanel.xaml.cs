using Camera.alarmsetup.service;
using Camera.constants;
using Camera.models;
using Camera.repository;
using Camera.sensors.response;
using Camera.services;
using LibVLCSharp.Shared;
using MonitoringApp.repository;
using OfficeOpenXml.Style;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Vlc.DotNet.Wpf;
using static OpenTK.Graphics.OpenGL.GL;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Camera.UserControls
{
    /// <summary>
    /// Interaction logic for CameraPanel.xaml
    /// </summary>
    public partial class CameraPanel : RadTransitionControl
    {
        // Static values
        private static string HIGH_ALARM = "HIGH_ALARM";
        public PropertyUtil propertyUtil = PropertyUtil.getInstance();
        private string[] gasSensorStates;
        private double[] gasSensorValues;
        private string[] gasSensorMeasurementTypes;
        private string[] gasSensorTypes;

        // DB.....
        private RoomDataRepository roomDataRepository = RoomDataRepository.getInstance();
        private RoomRepository roomRepository = RoomRepository.getInstance();
        private GasSensorReadingRepository gasSensorReadingRepository = GasSensorReadingRepository.getInstance();

        //Lists
        private List<InetGasCode> inetGasCodeArrayList = new List<InetGasCode>();
        private List<GasSensorReading> currentSensorReadings = new List<GasSensorReading>();
        //Media...
        private LibVLC libvlc = new LibVLC("--input-repeat=2");

        //public string stream_Resolution = propertyUtil.GetIntercomMonitoringInterval();//ConfigurationManager.AppSettings["Stream_Resolution"].ToString();
        public string options;

        private Room room { get; set; }

        //Flags....
        private bool intercomOutsideActive = false;
        private bool intercomInsideActive = false;
        private bool highAlarmStateAlarmIsTriggered = false;
        private bool alarmIsTriggered = false;


        private Entrants entrantsForm { get; set; }

        //Icons
        private BitmapImage disableAlarmIcon { get; set; }
        private BitmapImage triggerAlarmIcon { get; set; }
        private BitmapImage startCallIcon { get; set; }
        private BitmapImage stopCallIcon { get; set; }

        //Service...
        private AlarmSetupService alarmSetupService { get; set; }

        //Timer
        private System.Threading.Timer alertInsideBlinkingTimer;
        private System.Threading.Timer alertOutsideBlinkingTimer;
        // Popup
        private CameraPopup outsidePopup { get; set; }
        private CameraPopup insidePopup { get; set; }
        private CameraOverView parent { get; set; }

        public CameraPanel(Room room)
        {
            InitializeComponent();
            Core.Initialize();
            this.room = room;
            //this.options = "?compression=30&resolution=" + stream_Resolution + "&fps=30&videocodec=h264";
            //ShowCamera();
            GasPanel.Visibility = Visibility.Hidden;
            RoomName.Content = room.name;
            SetupSnapshotButtons();
            SetupPpeIcons();
            SetupAlarmButtons();
        }

        public void clearEntrantsPopup()
        {
            this.entrantsForm = null;
        }
        public void clearInsidePopup()
        {
            this.insidePopup = null;
        }
        public void clearOutsidePopup()
        {
            this.outsidePopup = null;
        }

        private void SetupPpeIcons()
        {
            if (room != null)
            {
                ppePanel.Children.Clear();
                int index = 0;
                //for (int i = 0; i < room.ppeIcons.Count; i++)
                //{
                //    this.ppePanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength() });
                //}

                //this.ppePanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                foreach (PpeIcon ppeIcon in room.ppeIcons)
                {
                    PpeIconPanel ppeIconPanel = new PpeIconPanel(ppeIcon);
                    ppeIconPanel.setEnabled(false);
                    Grid.SetColumn(ppeIconPanel, 0);
                    Grid.SetRow(ppeIconPanel, index++);
                    ppePanel.Children.Add(ppeIconPanel);
                }
            }
        }

        private void SetupAlarmButtons()
        {
            if (alarmIconSetupSucceeded())
            {
                if (room.alarmSetup != null)
                {
                    alarmSetupService = new AlarmSetupService(room.alarmSetup.outsideAlamIp, room.alarmSetup.insideAlarmIp);
                    ShowTriggerAlarmButton();
                    ShowCallButton();
                    SetupIntercomMonitoring();
                }
                else
                {
                    btnOutsideTriggerAlarm.Visibility = Visibility.Hidden;
                    btnInsideTriggerAlarm.Visibility = Visibility.Hidden;
                    btnCallInside.Visibility = Visibility.Hidden;
                    btnCallOutside.Visibility = Visibility.Hidden;
                }
            }
        }

        public void Dispose()
        {
            OutSideCameraView.MediaPlayer.Dispose();
            InSideCameraView.MediaPlayer.Dispose();
        }
        private bool alarmIconSetupSucceeded()
        {
            try
            {
                disableAlarmIcon = new BitmapImage(new Uri("pack://application:,,,/resources/Image/" + "disable-alarm.png"));
            }
            catch (Exception ex)
            {
                return false;
            }
            try
            {
                triggerAlarmIcon = new BitmapImage(new Uri("pack://application:,,,/resources/Image/" + "active-alarm.png"));
            }
            catch (Exception ex)
            {
                return false;
            }
            try
            {
                startCallIcon = new BitmapImage(new Uri("pack://application:,,,/resources/Image/" + "start-call.png"));
            }
            catch (Exception ex)
            {
                return false;
            }
            try
            {
                stopCallIcon = new BitmapImage(new Uri("pack://application:,,,/resources/Image/" + "stop-call.png"));
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        public void ShowCamera()
        {
            string options = "?compression=30&resolution=1920x1080&fps=30&videocodec=h264";

            var outside_media = new Media(libvlc, "rtsp://root:root@" + room.outsideCamera + "/axis-media/media.amp" + options, FromType.FromLocation);
            var inside_media = new Media(libvlc, "rtsp://root:root@" + room.insideCamera + "/axis-media/media.amp" + options, FromType.FromLocation);
            OutSideCameraView.MediaPlayer = new LibVLCSharp.Shared.MediaPlayer(outside_media);
            InSideCameraView.MediaPlayer = new LibVLCSharp.Shared.MediaPlayer(inside_media);
            //if (OutSideCameraView.MediaPlayer.IsPlaying) { OutSideCameraView.MediaPlayer.Stop(); }
            this.OutSideCameraView.MediaPlayer.Play(outside_media);

            this.InSideCameraView.MediaPlayer.Play(inside_media);
        }



        private void ShowTriggerAlarmButton()
        {
            btnOutsideTriggerAlarm.Visibility = Visibility.Visible;
            btnInsideTriggerAlarm.Visibility = Visibility.Visible;
        }

        private void ShowCallButton()
        {
            btnCallOutside.Visibility = Visibility.Visible;
            btnCallInside.Visibility = Visibility.Visible;
        }

        private void SetupIntercomMonitoring()
        {

            int interval = propertyUtil.GetINetLiveInterval();
            System.Threading.Timer intercomMonitorTimer = new System.Threading.Timer(InterComMonitorTimerEvent, null, 1000, interval);
        }

        private async void InterComMonitorTimerEvent(object state)
        {
            if (await alarmSetupService.isOutsideBeingCalled())
            {
                if (!IsIntercomOutsideActive())
                {
                    SetIntercomOutsideActive(true);
                    ShowStopCallOutSide();
                }
            }
            else
            {
                if (IsIntercomOutsideActive())
                {
                    SetIntercomOutsideActive(false);
                    ShowStopCallOutSide();
                }
            }

            if (await alarmSetupService.isInsideBeingCalled())
            {
                if (!IsIntercomInsideActive())
                {
                    SetIntercomInsideActive(true);
                    ShowStopCallOutSide();
                }
            }
            else
            {
                if (IsIntercomInsideActive())
                {
                    SetIntercomInsideActive(false);
                    ShowStopCallOutSide();
                }
            }
        }

        private void ShowStartCallOutSide()
        {
            ImgCallOutside.Source = startCallIcon;
        }

        private void ShowStopCallOutSide()
        {
            ImgCallOutside.Source = stopCallIcon;
        }

        private void ShowStartCallInSide()
        {
            imgCallInside.Source = startCallIcon;
        }

        private void ShowStopCallInSide()
        {
            imgCallInside.Source = stopCallIcon;
        }

        private void SetIntercomInsideActive(bool intercomInsideActive)
        {
            this.intercomInsideActive = intercomInsideActive;
        }

        private bool IsIntercomOutsideActive()
        {
            return intercomOutsideActive;
        }

        private void SetupSnapshotButtons()
        {

        }

        private bool HandleGasSensorsAndCheckForHighAlarm(List<INetLiveSensorDto> sensors, bool highAlarmShouldBeTriggered)
        {
            currentSensorReadings.Clear();
            for (int i = 0; i < sensors.Count; i++)
            {
                INetLiveSensorDto sensor = sensors.ElementAt(i);
                gasSensorStates[i] = sensor.state;
                gasSensorValues[i] = sensor.gasReading;
                gasSensorMeasurementTypes[i] = sensor.measurementTypeCode;
                gasSensorTypes[i] = getINetGasCodeDescription(sensor);
                highAlarmShouldBeTriggered = HIGH_ALARM.Equals(sensor.state) ? true : false;

                GasSensorReading gasSensorReading = new GasSensorReading
                {
                    room = room,
                    type = gasSensorTypes[i],
                    state = gasSensorStates[i],
                    value = gasSensorValues[i],
                    measurementType = gasSensorMeasurementTypes[i]

                };
                currentSensorReadings.Add(gasSensorReading);
                gasSensorReadingRepository.save(gasSensorReading);
            }

            return highAlarmShouldBeTriggered;
        }

        private string getINetGasCodeDescription(INetLiveSensorDto gasSensor)
        {
            string type;
            var optionalInetGasCode = inetGasCodeArrayList.Where(e => gasSensor.gasCode.Equals(e.gasCode)).FirstOrDefault();


            if (optionalInetGasCode != null)
            {
                type = optionalInetGasCode.description;
            }
            else
            {
                type = gasSensor.gasCode;
            }
            return type;
        }

        private void disableGasPanelsBecauseNoInstruments(String instrumentSerialNumber)
        {
            lblGasHeader1.Dispatcher.Invoke(new Action(() => {
                lblGasHeader1.IsEnabled = false;
            }));
            lblGasHeader2.Dispatcher.Invoke(new Action(() => {
                lblGasHeader1.IsEnabled = false;
            }));
            lblGasHeader3.Dispatcher.Invoke(new Action(() => {
                lblGasHeader3.IsEnabled = false;
            }));
            lblGasHeader4.Dispatcher.Invoke(new Action(() => {
                lblGasHeader4.IsEnabled = false;
            }));
            lblGasHeader5.Dispatcher.Invoke(new Action(() => {
                lblGasHeader5.IsEnabled = false;
            }));
            lblGasValue1.Dispatcher.Invoke(new Action(() => {
                lblGasValue1.Content = ("No data for: " + instrumentSerialNumber);
                lblGasValue1.Background = Brushes.Red;
            }));
            lblGasValue2.Dispatcher.Invoke(new Action(() => {
                lblGasValue2.IsEnabled = (false);
            }));
            lblGasValue3.Dispatcher.Invoke(new Action(() => {
                lblGasValue3.IsEnabled = (false);
            }));
            lblGasValue4.Dispatcher.Invoke(new Action(() => {
                lblGasValue4.IsEnabled = (false);
            }));
            lblGasValue5.Dispatcher.Invoke(new Action(() => {
                lblGasValue5.IsEnabled = (false);
            }));
            

            //pAlarm1.setBackground(RED);
            //pnlGas.setVisible(true);

        }

        private void takeSnapshot(bool outsideSnapshotTaken)
        {
            string workingDir = Environment.CurrentDirectory + @"\user\";



            string roomName = room.name;
            string snapshotDir = workingDir + @"snapshots\" + roomName;
            if (!Directory.Exists(snapshotDir))
            {
                Directory.CreateDirectory(snapshotDir);
            }
            if (outsideSnapshotTaken)
            {
                string timeStamp = DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss");
                string location = "out";
                string fileName = location + "-" + timeStamp + ".png";
                string snapInfo = System.IO.Path.Combine(snapshotDir, fileName);
                Random rand = new Random();
                bool test = OutSideCameraView.MediaPlayer.TakeSnapshot(0, snapInfo, (uint)1920, (uint)1080);
            }
            else
            {
                string timeStamp = DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss");
                string location = "in";
                string fileName = location + "-" + timeStamp + ".png";
                string snapInfo = System.IO.Path.Combine(snapshotDir, fileName);
                Random rand = new Random();
                InSideCameraView.MediaPlayer.TakeSnapshot((uint)rand.Next(), snapInfo, 0, 0);
            }

        }
        private void setGasSensorPanels()
        {
            GasPanel.Dispatcher.Invoke(new Action(() => {
                GasPanel.Visibility = Visibility.Visible;
            }));
            
            for (int i = 0; i < gasSensorTypes.Count(); i++)
            {

                switch (gasSensorStates[i])
                {
                    case "NORMAL":
                        ShowGasPanelColor(i, Brushes.Green);
                        break;
                    case "LOW_ALARM":
                        ShowGasPanelColor(i, Brushes.Yellow);
                        break;
                    case "HIGH_ALARM":
                        ShowGasPanelColor(i, Brushes.Red);
                        break;
                    default:
                        ShowGasPanelColor(i, Brushes.Blue);
                        break;
                }



                if (i == 0)
                {
                    lblGasValue1.Dispatcher.Invoke(new Action(() => {
                        lblGasHeader1.Content = (gasSensorTypes[i]);
                        lblGasValue1.Content = gasSensorValues[i] + " " + gasSensorMeasurementTypes[i];
                    }));
                    
                }
                else if (i == 1)
                {
                    lblGasValue2.Dispatcher.Invoke(new Action(() => {
                        lblGasHeader2.Content = (gasSensorTypes[i]);
                        lblGasValue2.Content = gasSensorValues[i] + " " + gasSensorMeasurementTypes[i];
                    }));
                }
                else if (i == 2)
                {
                    lblGasValue3.Dispatcher.Invoke(new Action(() => {
                        lblGasHeader3.Content = (gasSensorTypes[i]);
                        lblGasValue3.Content = gasSensorValues[i] + " " + gasSensorMeasurementTypes[i];
                    }));
                }
                else if (i == 3)
                {
                    lblGasValue4.Dispatcher.Invoke(new Action(() => {
                        lblGasHeader4.Content = (gasSensorTypes[i]);
                        lblGasValue4.Content = gasSensorValues[i] + " " + gasSensorMeasurementTypes[i];
                    }));
                }
                if (i == 4)
                { // if there are 5 sensors, make the fifth one visible
                    
                    lblGasValue5.Dispatcher.Invoke(new Action(() => {
                        lblGasHeader5.IsEnabled = true;
                        lblGasValue5.IsEnabled = true;
                    }));
                }
            }
        }

        private void ShowGasPanelColor(int i, SolidColorBrush color)
        {
           
            switch (i)
            {
                case 0:
                    lblGasValue1.Dispatcher.Invoke(new Action(() => { lblGasValue1.Background = color; }));
                    break;
                case 1:
                    lblGasValue2.Dispatcher.Invoke(new Action(() => { lblGasValue2.Background = color; }));
                    break;
                case 2:
                    lblGasValue3.Dispatcher.Invoke(new Action(() => { lblGasValue3.Background = color; }));
                    break;
                case 3:
                    lblGasValue4.Dispatcher.Invoke(new Action(() => { lblGasValue4.Background = color; }));
                    break;
                case 4:
                    lblGasValue5.Dispatcher.Invoke(new Action(() => { lblGasValue5.Background = color; }));
                    break;

            }
        }

        private void HandleHighAlarmState(bool highAlarmShouldBeTriggered)
        {
            if (highAlarmShouldBeTriggered)
            {
                if (!highAlarmStateAlarmIsTriggered)
                {
                    highAlarmStateAlarmIsTriggered = true;
                    TriggerBothAlarms();
                }
            }
            else
            {
                if (highAlarmStateAlarmIsTriggered)
                {
                    highAlarmStateAlarmIsTriggered = false;
                    DisableBothAlarms();
                }
            }
        }

        private void TriggerBothAlarms()
        {
            alarmIsTriggered = true;
            TriggerOutsideAlarm();
            TriggerInsideAlarm();
            ShowDisableAlarmButton();
        }



        private void DisableBothAlarms()
        {
            alarmIsTriggered = false;
            DisableOutsideAlarm();
            DisableInsideAlarm();
            ShowTriggerAlarmButton();
        }
        private void ShowDisableAlarmButton()
        {
            btnOutsideTriggerAlarm.Visibility = Visibility.Hidden;
            btnInsideTriggerAlarm.Visibility = Visibility.Hidden;
        }

        private void TriggerInsideAlarm()
        {
            alarmSetupService.triggerInsideAlarm();

            //alertInsideBlinkingTimer = new System.Threading.Timer(BlinkingActionListener, null, 0, 1000);
        }

        private void TriggerOutsideAlarm()
        {
            alarmSetupService.triggerOutsideAlarm();

            //alertOutsideBlinkingTimer = new System.Threading.Timer(BlinkingActionListener, null, 0, 1000);

        }

        private void BlinkingActionListener(object state)
        {
            throw new NotImplementedException();
        }

        private void DisableInsideAlarm()
        {
            alarmSetupService.disableInsideAlarm();

            if (alertInsideBlinkingTimer != null)
            {
                alertInsideBlinkingTimer.Dispose();
                alertInsideBlinkingTimer = null;
            }
        }

        private void DisableOutsideAlarm()
        {
            alarmSetupService.disableOutsideAlarm();
            if (alertOutsideBlinkingTimer != null)
            {
                alertOutsideBlinkingTimer.Dispose();
                alertOutsideBlinkingTimer = null;
            }
        }

        public void SetIntercomOutsideActive(bool intercomOutsideActive)
        {
            this.intercomOutsideActive = intercomOutsideActive;
        }

        public bool IsIntercomInsideActive()
        {
            return intercomInsideActive;
        }
        public void SetInetGasCodeArrayList(List<InetGasCode> inetGasCodeArrayList)
        {
            this.inetGasCodeArrayList = inetGasCodeArrayList;
        }
        public void setParent(CameraOverView cameraOverView)
        {
            this.parent = cameraOverView;
        }

        public void SetRoomData(Room room)
        {
            this.room = room;

            RoomName.Content = (room.name);
        }

        public void ClearEntrantsPopup()
        {
            this.entrantsForm = null;
        }

        public void ClearOutsidePopup()
        {
            //this.outsidePopup = null;
        }

        public void ClearInsidePopup()
        {
            //this.insidePopup = null;
        }

        public void UpdateEntrantsAmount()
        {
            
            string room_name = "";
            int amount  = roomDataRepository.getAmountOfPeopleInRoomWithName(room.name);
            PeopleInRoom.Dispatcher.BeginInvoke(new Action(() =>
            {
                PeopleInRoom.Content = ("People in room: " + amount);
            }));
            
        }

        private void btnCallInside_Click(object sender, RoutedEventArgs e)
        {
            if (intercomInsideActive)
            {
                // we're already calling to stop the call
                alarmSetupService.closeIntercomInside();

                // show the start call button again
                ShowStartCallInSide();

                // set the flag
                intercomInsideActive = false;
            }
            else
            {
                // we're not already calling, check if the other intercom is active and if so show the start button again
                if (intercomOutsideActive)
                {
                    ShowStartCallOutSide();
                    intercomOutsideActive = false;
                }

                alarmSetupService.openIntercomInside();
                ShowStopCallInSide();
                intercomInsideActive = true;
            }
        }

        private void btnCallOutside_Click(object sender, RoutedEventArgs e)
        {
            if (intercomOutsideActive)
            {
                // we're already calling to stop the call
                alarmSetupService.closeIntercomOutside();

                // show the start call button again
                ShowStartCallOutSide();

                // set the flag
                intercomOutsideActive = false;
            }
            else
            {
                // we're not already calling, check if the other intercom is active and if so show the start button again
                if (intercomInsideActive)
                {
                    ShowStartCallInSide();
                    intercomInsideActive = false;
                }

                alarmSetupService.openIntercomOutside();
                ShowStopCallOutSide();
                intercomOutsideActive = true;
            }
        }

        public void FeedSensorData(List<INetLiveInstrumentDto> instruments)
        {
            if (room == null) { return; }
            string instrumentSerialNumber = room.instrumentSn;
            if (instrumentSerialNumber == null || instrumentSerialNumber == "") { return; }
            INetLiveInstrumentDto instrument = instruments.Where(e => e != null).Where(e => e.sn == instrumentSerialNumber).FirstOrDefault();
            if (instrument == null) { disableGasPanelsBecauseNoInstruments(instrumentSerialNumber); return; }
            List<INetLiveSensorDto> sensors = instrument.sensors;
            sensors.OrderBy(e => e.gasCode);
            int amountOfSensors = sensors.Count();
            gasSensorStates = new string[amountOfSensors];
            gasSensorMeasurementTypes = new string[amountOfSensors];
            gasSensorTypes = new string[amountOfSensors];
            gasSensorValues = new double[amountOfSensors];
            bool highAlarmShouldBeTriggered = false;
            highAlarmShouldBeTriggered = HandleGasSensorsAndCheckForHighAlarm(sensors, highAlarmShouldBeTriggered);
            setGasSensorPanels();
            HandleHighAlarmState(highAlarmShouldBeTriggered);
        }

        private void btnSnapshotOutside_Click(object sender, RoutedEventArgs e)
        {
            takeSnapshot(true);
        }

        private void btnSnapshotInside_Click(object sender, RoutedEventArgs e)
        {
            takeSnapshot(false);
        }



        private void SafeReport_Click(object sender, RoutedEventArgs e)
        {
            string workingDir = Environment.CurrentDirectory + @"\user\";
            string roomName = room.name;
            string snapshotDir = workingDir + @"snapshots\" + roomName;
            string timeStamp = DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss");
            string outsideFilename = timeStamp + "_Safety_report_outside.png";
            string insideFilename = timeStamp + "Safety_report_inside.png";
            if (!Directory.Exists(snapshotDir))
            {
                Directory.CreateDirectory(snapshotDir);
            }
            string outsideFile = System.IO.Path.Combine(snapshotDir, outsideFilename);
            string insideFile = System.IO.Path.Combine(snapshotDir, insideFilename);

            saveSnapshotToFile(true, outsideFile);
            saveSnapshotToFile(false, insideFile);


            TemplateService templateService = new TemplateService();
            try
            {
                templateService.WriteToTemplate(parent.getLoggedInUser(),
                        room.name,
                        currentSensorReadings,
                        outsideFile,
                        insideFile);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error creating safety report!");
            }
        }

        private void saveSnapshotToFile(bool outsideTaken, string snapInfo)
        {
            try
            {
                if (outsideTaken)
                {
                    OutSideCameraView.MediaPlayer.TakeSnapshot(0, snapInfo, (uint)OutSideCameraView.ActualWidth, (uint)OutSideCameraView.ActualHeight);

                }
                else
                {
                    InSideCameraView.MediaPlayer.TakeSnapshot(0, snapInfo, (uint)OutSideCameraView.ActualWidth, (uint)OutSideCameraView.ActualHeight);
                }
            }
            catch (Exception ex)
            {
                // todo narrow down correct exception
                //LOGGER.error("Could not save {}", file.toString());
            }
        }



        private void OutSideCameraView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (outsidePopup == null)
            {
                outsidePopup = new CameraPopup(room.name, room.outsideCamera, this, true); ;
                outsidePopup.ShowDialog();

            }
            else
            {

                outsidePopup.ShowDialog();
            }
        }

        private void InSideCameraView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (insidePopup == null)
            {
                insidePopup = new CameraPopup(room.name, room.outsideCamera, this, false);
                insidePopup.ShowDialog();
            }
            else
            {
                insidePopup.ShowDialog();
            }
        }
    }
}
