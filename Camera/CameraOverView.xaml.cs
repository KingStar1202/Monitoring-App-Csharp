using Camera.constants;
using Camera.models;
using Camera.net2access;
using Camera.repository;
using Camera.sensors.response;
using Camera.sensors.services;
using Camera.UserControls;
using Google.Protobuf.WellKnownTypes;
using LibVLCSharp.Shared;
using MonitoringApp.repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
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
    /// Interaction logic for CameraOverView.xaml
    /// </summary>
    public partial class CameraOverView : RadRibbonWindow
    {
        private PropertyUtil propertyUtil = PropertyUtil.getInstance();
        private double aspectRatio = 0.0;
        private RoomRepository roomRepository = RoomRepository.getInstance();
        private InetGasCodeRepository inetGasCodeRepository = InetGasCodeRepository.getInstance();
        //private INetSensorService inetSensorService = new INetSensorService();
        private List<CameraPanel> cameraPanelList = new List<CameraPanel>();
        private List<Room> roomList = new List<Room>();
        private List<InetGasCode> inetGasCodeArrayList = new List<InetGasCode>();
        //private List<MediaContainer> containerList = new List<MediaContainer>();
        //private OperatorAwarenessForm operatorAwarenessForm;
        private string loggedInUser;
        private int initialCount = 10;
        private System.Threading.Timer operatorAwarenessTimer { get; set; }

        private INetSensorService iNetSensorService = new INetSensorService();

        private OperatorAwarenessView operatorAwareness { get; set; }
        static CameraOverView()
        {
            CameraOverView.IsWindowsThemeEnabled = false;
        }

        public CameraOverView(string username)
        {
            InitializeComponent();
            loggedInUser = username;
            roomList = roomRepository.Get();
            this.UserName.Content = username;
            int amountOfRooms = roomList.Count;
            initialCount = amountOfRooms > initialCount ? amountOfRooms : initialCount;
            inetGasCodeArrayList = inetGasCodeRepository.getAllInetGasCodes();
            SetGridLayout(amountOfRooms);
            try
            {
                SetupPanels(amountOfRooms);
                SetupStreamsAndPlay();
            }
            catch (Exception ex)
            {

            }
            SetupINetLiveDataTimeline();
            SetupNet2AccessEntrantsMonitoring();
            SetupOperatorAwareness(this);

        }

        public string getLoggedInUser()
        {
            return loggedInUser;
        }

        private void SetGridLayout(int amountOfRooms)
        {
           
            if (roomRepository.getMaxRowsForGrid() > 1)
            {
                int columnsRow1 = roomRepository.getMaxColumnsForRow(1);
                int columnsRow2 = roomRepository.getMaxColumnsForRow(2);
                for (int i = 0; i < 2; i++)
                {
                    this.topSect.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star), });
                }
                int columnCount = Math.Max(columnsRow1, columnsRow2);
                for (int i = 0; i < columnCount; i++)
                {
                    this.topSect.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                }

            }
            else
            {
                this.topSect.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                for (int i = 0; i < amountOfRooms; i++)
                {
                    this.topSect.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                }
            }
        }

        public void UpdatePeopleInRooms()
        {
            foreach (CameraPanel cameraPanel in cameraPanelList)
            {
                cameraPanel.UpdateEntrantsAmount();
            }
        }

        private void SetupPanels(int amountOfRooms)
        {

            for (int i = 0; i < amountOfRooms; i++)
            {
                Room camera_room = roomList.ElementAt(i);

                CameraPanel cameraPanel = new CameraPanel(camera_room);
                cameraPanel.SetRoomData(roomList.ElementAt(i));
                cameraPanel.SetInetGasCodeArrayList(inetGasCodeArrayList);
                cameraPanel.setParent(this);
                if(camera_room.y > 1)
                {
                    Grid.SetRow(cameraPanel, 1);
                    Grid.SetColumn(cameraPanel, i % topSect.ColumnDefinitions.Count );
                }
                else
                {
                    Grid.SetRow(cameraPanel, 0);
                    Grid.SetColumn(cameraPanel, i);
                }
                //cameraPanel.setParent(this);
                
                
                cameraPanelList.Add(cameraPanel);
                this.topSect.Children.Add(cameraPanel);
                
            }
        }

        public void SetupStreamsAndPlay()
        {
            foreach(var cameraPanel in cameraPanelList)
            {
                cameraPanel.ShowCamera();
            }
        }
        private void SetupINetLiveDataTimeline()
        {
            int interval = propertyUtil.GetINetLiveInterval();//Convert.ToInt32(ConfigurationManager.AppSettings["Inetlive_Query_Interval"].ToString());
            List<string> equipmentSerialNumbers = new List<string>();
            roomList.Where(e => e.instrumentSn != "<unset>").ToList().ForEach(e => equipmentSerialNumbers.Add(e.instrumentSn));
            // Thread iNetLiveBackgroundTask = new Thread(new ThreadStart())
            System.Threading.Timer iNetLiveTimer = new System.Threading.Timer(iNetLiveBackgroundTask, equipmentSerialNumbers, 0, interval);
           

        }

        private  void iNetLiveBackgroundTask(object state)
        {
            List<string> equipmentSerialNumbers = (List<string>)state;
            INetLiveResponseDto iNetLiveData =  iNetSensorService.getINetLiveData(equipmentSerialNumbers);

            if (iNetLiveData == null)
            {
                
                return ;
            }
            foreach(var cameraPanel in cameraPanelList)
            {
                cameraPanel.FeedSensorData(iNetLiveData.instrument);
            }

            return ;
        }

        private void SetupNet2AccessEntrantsMonitoring()
        {
            
            System.Threading.Timer net2AccessAliveChecker = new System.Threading.Timer(net2AccessAliveActionListener, null, 1000, 1000);
            
            //File net2AccessDataFile = File
        }

        private void net2AccessAliveActionListener(object state)
        {
            string net2AccessLocation = propertyUtil.GetNet2AccessDataFileLocation();//ConfigurationManager.AppSettings["WHOSIN_FILE_LOCATION"].ToString();
            FileInfo net2AccessDataFile = new FileInfo(net2AccessLocation);
            FileWatcher fileWatcher = new FileWatcher(net2AccessDataFile, this);
            fileWatcher.Start();
            CameraOverView thisForm = this;
            if (fileWatcher.isStopped())
            {
                new FileWatcher(net2AccessDataFile, thisForm);
                fileWatcher.Start();
            }
        }
        public void RestartOperatorAwarenessTimer()
        {
            operatorAwareness = null;
        }
        private void SetupOperatorAwareness(CameraOverView cameraOverView)
        {
            int interval = propertyUtil.GetOperatorAwarenessTiming();
            operatorAwarenessTimer = new System.Threading.Timer(operatorAwarenessBackgroundTask, cameraOverView, interval, interval);
        }

        private void operatorAwarenessBackgroundTask(object state)
        {
            CameraOverView cameraOverView = (CameraOverView)state;
            if (operatorAwareness == null)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    operatorAwareness = new OperatorAwarenessView(loggedInUser, cameraOverView);
                    operatorAwareness.Activate();
                    operatorAwareness.Topmost = true;
                    operatorAwareness.ShowDialog();
                });
            }

            
        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            aspectRatio = this.ActualWidth / this.ActualHeight;
        }

        private void RadRibbonWindow_Closed(object sender, EventArgs e)
        {
            foreach(var panel in cameraPanelList)
                panel.Dispose();
        }
    }
}
