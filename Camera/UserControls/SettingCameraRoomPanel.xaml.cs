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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using MySql.Data.MySqlClient;
using System.IO;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using Camera.models;
using System.Configuration;
using Camera.repository;
using Camera.context;
using MonitoringApp.repository;
using System.Threading;
using System.Runtime.InteropServices.WindowsRuntime;
using Camera.constants;

namespace Camera.UserControls
{
    /// <summary>
    /// Interaction logic for SettingCameraRoomPanel.xaml
    /// </summary>
    public partial class SettingCameraRoomPanel : RadTransitionControl
    {
        static SettingCameraRoomPanel()
        {

            //Load currently saved setup from database.
        }
        private PropertyUtil propertyUtil = PropertyUtil.getInstance();
        public static string UNSET_SETUP = "<unset>";
        private Room room { get; set; }
        private List<Room> rooms { get; set; }
        private int panelId { get; set; }
        private bool setupIsDone = false;
        private AdditionalSetup parent;
        private List<AlarmSetup> alarmSetups { get; set; }
        private List<CameraCombo> cameraCombos { get; set; }
        public int alarmSetupCount = 12;
        private RoomRepository roomRepository = RoomRepository.getInstance();
        public SettingCameraRoomPanel()
        {
            InitializeComponent();
        }

        public void setRooms(List<Room> rooms)
        {
            this.rooms = rooms;
        }

        public void setSetupIsDone(bool setupIsDone)
        {
            this.setupIsDone = setupIsDone;
        }
        public void setPanelId(int panelId)
        {
            this.panelId = panelId;
        }
        public Room getRoom()
        {
            return this.room;
        }

        public List<AlarmSetup> getAlarmSetups()
        {
            return alarmSetups;
        }
        public void setSettingsForm(AdditionalSetup additionalSetup)
        {
            this.parent = additionalSetup;
        }

        public void setRoom(Room room)
        {
            this.room = room;
            string roomName = room != null ? room.name : "";
            
            RoomName.Text = roomName;
        }

        public void setGasSerialNumbers()
        {
            if (room != null && room.instrumentSn != null)
            {
                cmbGasSerialNumberSelection.Content = room.instrumentSn;
            }
            else
            {
                cmbGasSerialNumberSelection.Content = UNSET_SETUP;
            }
        }

        public void setCameraCombos()
        {
            if(new CameraComboData().CameraCombos.Count > 0)
            {
                if (room != null && room.outsideCamera != null)
                {
                    CameraList.SelectedItem = room.outsideCamera;
                    cmbCameraCombo.Content = room.outsideCamera;
                }
                else
                {
                    CameraList.SelectedItem = UNSET_SETUP;
                    cmbCameraCombo.Content = UNSET_SETUP;
                }
            }
            
        }

        public void setAlarmSetups()
        {
            bool intercom_enabled = propertyUtil.IsIntercomEnabled();//ConfigurationManager.AppSettings["Intercom_Enabled"].ToString();
            if (intercom_enabled == true)
            {
               
                if (room != null && room.alarmSetup != null)
                {
                    AlarmSetupList.SelectedItem = room.alarmSetup;
                    cmbAlarmSetup.Content = room.alarmSetup.name;
                }
                else
                {
                    AlarmSetupList.SelectedItem = UNSET_SETUP;
                    cmbAlarmSetup.Content = UNSET_SETUP;
                }
            }
            else
            { 
                cmbAlarmSetup.IsEnabled = false;
            }
        }

        public Room SaveAndGetRoom()
        {


            if (UNSET_SETUP.Equals(cmbCameraCombo.Content)|| "".Equals(cmbCameraCombo.Content))
            {
                return null;
            }
            if(room == null)
            {
                room = new Room();
            }
            if (RoomName.Text == null || RoomName.Text == "") { room.name = cmbCameraCombo.Content.ToString(); } else { room.name = RoomName.Text; }


            if (!UNSET_SETUP.Equals(cmbGasSerialNumberSelection.Content) || !"".Equals(cmbGasSerialNumberSelection.Content))
            {
                room.instrumentSn = cmbGasSerialNumberSelection.Content.ToString();

            }

            if (!UNSET_SETUP.Equals(cmbAlarmSetup.Content))
            {
                room.alarmSetup = (AlarmSetup)AlarmSetupList.SelectedItem;

            }

            if(room.outsideCamera != cmbCameraCombo.Content.ToString())
            {
                string insideCamera = cmbCameraCombo.Content.ToString();
                string oldNumber = insideCamera.Split('.')[2];
                string newNumber = (Convert.ToInt32(oldNumber) + 1).ToString();
                room.outsideCamera = insideCamera.Split('.')[0] + "." + insideCamera.Split('.')[1] + "." + newNumber + "." + insideCamera.Split('.')[3];
                room.insideCamera = cmbCameraCombo.Content.ToString();
            }


            room.roomId = panelId;


            setXY();
            return room;
        }

        public void setXY()
        {
            int x, y;
            y = panelId > (alarmSetupCount /2) ? 2 : 1;
            x = panelId > (alarmSetupCount / 2) ? panelId - (alarmSetupCount / 2) : panelId;
            room.x = x;
            room.y = y;
        }

        private void btnPpe_Click(object sender, RoutedEventArgs e)
        {
            if(room == null) return;
            SetupPPE ppeSetupPopup = new SetupPPE(this.room);
            ppeSetupPopup.ShowDialog();
        }

        private void cmbAlarmSetup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CameraList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (Camera.models.CameraCombo)CameraList.SelectedItem;
            if (item != null)
            {
                int count = rooms.Where(m => m.outsideCamera == item.outsideIp).Count();
                if(count > 0)
                {
                    MessageBox.Show("Already Set the Outsid IP");
                    return;
                }
                cmbCameraCombo.Content= item.name.ToString();
            }
            cmbCameraCombo.CloseOnPopupMouseLeftButtonUp = true;
        }

        private void SerialNumberList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (GasDetection)SerialNumberList.SelectedItem;
            if (item != null)
            {
                cmbGasSerialNumberSelection.Content = item.serialNumber.ToString();
            }
            cmbGasSerialNumberSelection.CloseOnPopupMouseLeftButtonUp = true;  
        }

        private void AlarmSetupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (AlarmSetup)AlarmSetupList.SelectedItem;
            if (item != null)
            {
                cmbAlarmSetup.Content = item.name.ToString();
            }
            cmbAlarmSetup.CloseOnPopupMouseLeftButtonUp = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if(room == null || room.id < 1)
            {
                return;
            }
            //Thread deleteThread = new Thread(new ParameterizedThreadStart(DeleteThread));
            //deleteThread.Start(room);
            roomRepository.Delete(room);

            this.parent.RefreshData();
        }

        
    }

    public class SerialNumberData
    {
        private GasDetectionRepository gasDetectionRepository = GasDetectionRepository.getInstance();
        public ObservableCollection<GasDetection> SerialNumbers
        {
            get;
            set;
        }
        public SerialNumberData()
        {

            SerialNumbers = new ObservableCollection<GasDetection>();

            var gasDetections = gasDetectionRepository.GetGasDetections();
            gasDetections.Add(new GasDetection { serialNumber = "<unset>" });

            foreach (var gasDetection in gasDetections)
            {
                SerialNumbers.Add(gasDetection);
            }
        }
    }

    public class CameraComboData
    {
        private CameraComboRepository cameraComboRepository = CameraComboRepository.getInstance();
        public ObservableCollection<Camera.models.CameraCombo> CameraCombos
        {
            get;
            set;
        }
        public CameraComboData()
        {
            CameraCombos = new ObservableCollection<Camera.models.CameraCombo>();
            var cameracombos = cameraComboRepository.getConfiguredCamreaCombos();
            cameracombos.Add(new Camera.models.CameraCombo { name = "<unset>" });
            foreach (var camera in cameracombos)
            {
                CameraCombos.Add(camera);
            }
        }
    }

    public class AlarmSetupData
    {
        private AlarmSetupRepository alarmSetupRepository = AlarmSetupRepository.getInstance();
        public ObservableCollection<AlarmSetup> AlarmSetups
        {
            get;
            set;
        }
        public AlarmSetupData()
        {
            AlarmSetups = new ObservableCollection<AlarmSetup>();
            var alarmSetups = alarmSetupRepository.Get();
            alarmSetups.Add(new AlarmSetup { name = "<unset>"});
            foreach (var alarmSetup in alarmSetups)
            {
                AlarmSetups.Add(alarmSetup);
            }
        }
    }
}