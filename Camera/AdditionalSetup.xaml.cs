using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using OfficeOpenXml;

using Telerik.Windows.Controls;
using Camera.context;
using Camera.models;
using Camera.repository;
using System.Collections.ObjectModel;
using MediaFoundation;
using Camera.UserControls;
using MonitoringApp.repository;
using Camera.services;
using System.Threading;
using Camera.constants;
using LibVLCSharp.Shared;
using Org.BouncyCastle.Utilities;
using Microsoft.Win32;

namespace Camera
{
    /// <summary>
    /// Interaction logic for AdditionalSetup.xaml
    /// </summary>
    public partial class AdditionalSetup : RadRibbonWindow
    {
        static AdditionalSetup()
        {
            AdditionalSetup.IsWindowsThemeEnabled = false;
            //Load currently saved setup from database.
        }


        private GasDetectionRepository gasDetectionRepository = GasDetectionRepository.getInstance();
        private AlarmSetupRepository alarmsetupRepository = AlarmSetupRepository.getInstance();
        private UserRepository userRepository = UserRepository.getInstance();
        private RoomRepository roomRepository = RoomRepository.getInstance();
        private CameraComboRepository cameraComboRepository = CameraComboRepository.getInstance();
        private OperatorAwarenessRepository operatorAwarenessRepository = OperatorAwarenessRepository.getInstance();
        private AlarmSetup newAlarmSetup;
        private User newUser;

        private PropertyUtil property = PropertyUtil.getInstance();

        private List<SettingCameraRoomPanel> cameraRoomPanels = new List<SettingCameraRoomPanel>();
        private List<GasDetection> gasDetections = new List<GasDetection>();
        private List<AlarmSetup> alarmSetups = new List<AlarmSetup>();
        private List<Room> rooms = new List<Room>();
        private List<CameraCombo> cameraCombos = new List<CameraCombo>();
        private Dictionary<int, string> cameraResolutionDictionary = new Dictionary<int, string>();
        private Dictionary<int, string> cameraType = new Dictionary<int, string>();

        private GasDetection unsetGasDetection;
        private AlarmSetup unsetAlarmSetup;
        private CameraCombo unsetCameraCombo;

        private int initialSetCount = 12;
        private bool roomSetupIsSaved = false;
        private string loginedUser;
        public AdditionalSetup(string username)
        {
            InitializeComponent();
            loginedUser = username;
            LoadInitial();
            LogdConfigTab();
        }

        private void LogdConfigTab()
        {
            string net2_path = property.GetPropertyAsstring(ZGroupContstant.WHOSIN_FILE_LOCATION);
            var auth = property.GetINetLiveAuth();
            int interval = property.GetINetLiveInterval();
            int operatorInteval = property.GetOperatorAwarenessTiming();
            string resolution = property.GetPropertyAsstring(ZGroupContstant.CAMERA_AXIS_RESOLUTION);
            TxtNet2.Text = net2_path;
            ClientID.Text = auth.clientId;
            ClientSecret.Text = auth.clientSecret;
            PW.Text = auth.pw;
            AccountID.Text = auth.accountId;
            User.Text = auth.user;
            Interval.Text = interval.ToString();
            OperatorAwarenessInterval.Text = operatorInteval.ToString();

            cameraResolutionDictionary.Add(0, "640x480");
            cameraResolutionDictionary.Add(1, "800x600");
            cameraResolutionDictionary.Add(2, "1024x768");
            cameraResolutionDictionary.Add(3, "1280x960");
            cameraResolutionDictionary.Add(4, "640x360");
            cameraResolutionDictionary.Add(5, "1024x576");
            cameraResolutionDictionary.Add(6, "1280x720");
            cameraResolutionDictionary.Add(7, "1920x1080");
            cameraResolutionDictionary.Add(8, "2304x1296");

            cameraType.Add(0, "AXIS");
            cameraType.Add(1, "REGCAM");

            foreach (var pair in cameraResolutionDictionary)
            {
                if (pair.Value == resolution)
                {
                    CmbCamResolution.SelectedIndex = pair.Key;
                    break;
                }
            }

            string logo = property.GetOperatorAwarenessLgo();
            ShowLogo(logo);

        }

        public void LoadInitial()
        {
            rooms = roomRepository.Get();
            if (rooms.Count > initialSetCount)
            {
                if (rooms.Count % 2 == 0)
                {
                    initialSetCount = rooms.Count;
                }
                else
                {
                    initialSetCount = rooms.Count + 1;
                }
            }
            LoadGrid();
            LoadSets();
            LoadGasSerialNumbers();
            LoadCameraCombos();
            LoadAlarmSetups();
            LoadConfigAndPopulate();
        }

        public void RefreshData()
        {
            cameraRoomPanels.Clear();
            RemoveGrid();
            LoadInitial();
        }


        private void setupCameraPanelSet()
        {
            cameraRoomPanels.ForEach(e => e.setSettingsForm(this));
        }
        private void clearCameraPanels()
        {
            //cameraRoomPanels.ForEach(e => e.Controls.Clear());
            cameraRoomPanels.ForEach(e => e.setRoom(null));

        }
        private void LoadCameraCombos()
        {
            cameraCombos = cameraComboRepository.getConfiguredCamreaCombos();
            if (unsetCameraCombo == null)
            {
                unsetCameraCombo = new CameraCombo();
                unsetCameraCombo.name = ("<unset>");
                unsetCameraCombo.outsideIp = ("<unset>");
            }
            cameraCombos.Add(unsetCameraCombo);
        }
        public void LoadConfigAndPopulate()
        {

            if (rooms.Count > 0)
            {
                roomSetupIsSaved = true;
                SetupRoomsInGrid();
                cameraRoomPanels.ForEach(e => e.IsEnabled = true);
                //btnContinue.Enabled = (true);
            }
            else
            {
                btnContinue.IsEnabled = false;
            }
        }

        private void SetupRoomsInGrid()
        {

            foreach (Room room in rooms)
            {
                int cameraPanelNumber = room.x + (initialSetCount / 2) * (room.y - 1);
                cameraRoomPanels.ElementAt(cameraPanelNumber - 1).setRooms(rooms);
                cameraRoomPanels.ElementAt(cameraPanelNumber - 1).setRoom(room);
                cameraRoomPanels.ElementAt(cameraPanelNumber - 1).setGasSerialNumbers();
                cameraRoomPanels.ElementAt(cameraPanelNumber - 1).setAlarmSetups();
                cameraRoomPanels.ElementAt(cameraPanelNumber - 1).setCameraCombos();
                cameraRoomPanels.ElementAt(cameraPanelNumber - 1).setSettingsForm(this);
                cameraRoomPanels.ElementAt(cameraPanelNumber - 1).alarmSetupCount = initialSetCount;
            }

            foreach (var settingsCameraRoomPanel in cameraRoomPanels)
            {
                if (settingsCameraRoomPanel.getRoom() == null)
                {
                    settingsCameraRoomPanel.setRooms(rooms);
                    //settingsCameraRoomPanel.setRoom(new Room());
                    settingsCameraRoomPanel.setGasSerialNumbers();
                    settingsCameraRoomPanel.setAlarmSetups();
                    settingsCameraRoomPanel.setCameraCombos();
                    settingsCameraRoomPanel.setSettingsForm(this);
                    settingsCameraRoomPanel.alarmSetupCount = initialSetCount;
                }
            }
            setSetupIsDoneFlag(true);
        }
        private void LoadGasSerialNumbers()
        {
            gasDetections = gasDetectionRepository.GetGasDetections();

            if (unsetGasDetection == null)
            {
                unsetGasDetection = new GasDetection();
                unsetGasDetection.serialNumber = ("<unset>");
            }
            gasDetections.Add(unsetGasDetection);
        }

        private void LoadAlarmSetups()
        {
            alarmSetups = alarmsetupRepository.Get();

            if (unsetAlarmSetup == null)
            {
                unsetAlarmSetup = new AlarmSetup();
                unsetAlarmSetup.name = ("<unset>");
            }
            alarmSetups.Add(unsetAlarmSetup);
        }

        private void LoadGrid()
        {
            for (int i = 0; i < 2; i++)
            {
                this.topSect.RowDefinitions.Add(new RowDefinition() { Height = new GridLength() });
            }

            for (int i = 0; i < initialSetCount / 2; i++)
            {
                this.topSect.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
        }

        private void RemoveGrid()
        {
            this.topSect.RowDefinitions.RemoveRange(0, this.topSect.RowDefinitions.Count);
            this.topSect.ColumnDefinitions.RemoveRange(0, this.topSect.ColumnDefinitions.Count);
        }

        private void setSetupIsDoneFlag(bool v)
        {
            cameraRoomPanels.ForEach(e => e.setSetupIsDone(v));
        }
        private void LoadSets()
        {
            for (int i = 1; i <= initialSetCount; i++)
            {
                SettingCameraRoomPanel settingPanel = new SettingCameraRoomPanel();
                settingPanel.setPanelId(i);
                if (i <= initialSetCount / 2)
                {
                    Grid.SetRow(settingPanel, 0);

                }
                else
                {
                    Grid.SetRow(settingPanel, 2);
                }
                int columnNumber = (i - 1) % (initialSetCount / 2);
                Grid.SetColumn(settingPanel, columnNumber);
                this.topSect.Children.Add(settingPanel);
                cameraRoomPanels.Add(settingPanel);
            }
        }

        private void addRowUserControls(int column)
        {
            SettingCameraRoomPanel settingPanel1 = new SettingCameraRoomPanel();
            settingPanel1.setPanelId(column * 2);
            Grid.SetRow(settingPanel1, 0);
            Grid.SetColumn(settingPanel1, column - 1);
            this.topSect.Children.Add(settingPanel1);
            cameraRoomPanels.Add(settingPanel1);
            SettingCameraRoomPanel settingPanel2 = new SettingCameraRoomPanel();
            settingPanel2.setPanelId(column * 2 + 1);
            Grid.SetRow(settingPanel2, 2);
            Grid.SetColumn(settingPanel2, column - 1);
            this.topSect.Children.Add(settingPanel2);
            cameraRoomPanels.Add(settingPanel2);
            setupCameraPanelSet();
            initialSetCount = initialSetCount + 2;
        }
        ColumnDefinition rowDefinition = new ColumnDefinition();
        private void Rt14_Checked(object sender, RoutedEventArgs e)
        {
            //---------------------------------------------------------

            //rowDefinition.Width = new GridLength(1, GridUnitType.Star);
            //rowDefinition.Name = "xname";
            //topSect.ColumnDefinitions.Add(rowDefinition);
            //addRowUserControls(7);
            this.topSect.Children.RemoveRange(0, topSect.Children.Count);
            cameraRoomPanels.Clear();

            RemoveGrid();
            initialSetCount = 14;
            LoadInitial();
            //---------------------------------------------------------
        }



        private void Rt14_Unchecked(object sender, RoutedEventArgs e)
        {
            //topSect.ColumnDefinitions.Remove(rowDefinition);
            if (Rt16.IsChecked == true)
                Rt16.IsChecked = false;

            //if (rowDefinition1.IsEnabled)
            //{
            //    topSect.ColumnDefinitions.Remove(rowDefinition1);
            //}
            //cameraRoomPanels.RemoveRange(11, cameraRoomPanels.Count - initialSetCount);
            //topSect.Children.RemoveRange(11, cameraRoomPanels.Count - initialSetCount);
            //setupCameraPanelSet();
            this.topSect.Children.RemoveRange(0, topSect.Children.Count);
            cameraRoomPanels.Clear();
            RemoveGrid();
            initialSetCount = 12;
            LoadInitial();

        }
        ColumnDefinition rowDefinition1 = new ColumnDefinition();
        private void Rt16_Checked(object sender, RoutedEventArgs e)
        {

            //---------------------------------------------------------
            //if (Rt14.IsChecked == false)
            //{
            //    rowDefinition.Width = new GridLength(1, GridUnitType.Star);
            //    rowDefinition.Name = "xname";
            //    topSect.ColumnDefinitions.Add(rowDefinition);
            //    addRowUserControls(7);
            //}
            //rowDefinition1.Width = new GridLength(1, GridUnitType.Star);
            //topSect.ColumnDefinitions.Add(rowDefinition1);
            //rowDefinition1.Name = "xname1";
            //addRowUserControls(8);
            //setupCameraPanelSet();
            this.topSect.Children.RemoveRange(0, topSect.Children.Count);
            cameraRoomPanels.Clear();
            RemoveGrid();
            initialSetCount = 16;
            LoadInitial();
            //initialSetCount = 16;
            //---------------------------------------------------------
        }

        private void Rt16_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Rt14.IsChecked == true)
                Rt14.IsChecked = false;
            //topSect.ColumnDefinitions.Remove(rowDefinition);
            //topSect.ColumnDefinitions.Remove(rowDefinition1);
            //cameraRoomPanels.RemoveRange(11, cameraRoomPanels.Count - initialSetCount);
            //topSect.Children.RemoveRange(11, cameraRoomPanels.Count - initialSetCount);
            //setupCameraPanelSet();
            this.topSect.Children.RemoveRange(0, topSect.Children.Count);
            cameraRoomPanels.Clear();
            RemoveGrid();
            initialSetCount = 12;
            LoadInitial();
        }

        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image files (*.bmp;*.jpg;*.jpeg,*.png)|*.bmp;*.jpg;*.jpeg;*.png|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                // Create a new BitmapImage and set its source to the selected image file
                ShowLogo(openFileDialog.FileName);
                property.SaveConfig(ZGroupContstant.OPERATOR_AWARENESS_LOGO, openFileDialog.FileName);
            }


        }

        private void ShowLogo(string path)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();


                // Set the source of the ImgAwarnessLogo Image control to the selected image file
                ImgAwarnessLogo.Source = bitmap;
            }
            catch
            {

            }

        }

        private void BtnContinue_Click(object sender, RoutedEventArgs e)
        {


            new CameraOverView(loginedUser).ShowDialog();
            //this.Close();

        }

        private void BtnNet2Select_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "HTML files (*.html)|*.html|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                TxtNet2.Text = openFileDialog.FileName;
            }
        }

        //private void BtnExportAwarnessLog_Click(object sender, RoutedEventArgs e)
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //    // MySQL database connection string
        //    string connString = "server=localhost;database=tgroup;uid=root;pwd=;";

        //    // Query to select the required columns from the operator_awareness table
        //    string query = "SELECT time_taken, user, created_at FROM operator_awareness";

        //    // Create a new connection to the MySQL database
        //    using (MySqlConnection conn = new MySqlConnection(connString))
        //    {
        //        conn.Open();

        //        // Create a new command with the query and connection
        //        using (MySqlCommand cmd = new MySqlCommand(query, conn))
        //        {
        //            // Execute the query and create a MySqlDataReader
        //            using (MySqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                // Create a new ExcelPackage
        //                ExcelPackage excel = new ExcelPackage();
        //                var worksheet = excel.Workbook.Worksheets.Add("Operator Awareness");

        //                // Write the column headers to the first row of the worksheet
        //                worksheet.Cells[1, 1].Value = "Time Taken";
        //                worksheet.Cells[1, 2].Value = "User";
        //                worksheet.Cells[1, 3].Value = "Created At";

        //                int row = 2;

        //                // Read each row from the MySQL data reader and write it to the Excel worksheet
        //                while (reader.Read())
        //                {
        //                    worksheet.Cells[row, 1].Value = reader["time_taken"];
        //                    worksheet.Cells[row, 2].Value = reader["user"];
        //                    worksheet.Cells[row, 3].Value = reader["created_at"];
        //                    row++;
        //                }

        //                // Save the Excel file to a MemoryStream
        //                MemoryStream stream = new MemoryStream();
        //                excel.SaveAs(stream);

        //                // Save the MemoryStream to a file
        //                string fileName = "OperatorAwarenessLog.xlsx";
        //                using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
        //                {
        //                    stream.WriteTo(file);
        //                }
        //            }
        //        }
        //    }
        //}
        private void BtnExportAwarnessLog_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var list = operatorAwarenessRepository.Get();
            ExcelPackage excel = new ExcelPackage();
            var worksheet = excel.Workbook.Worksheets.Add("Operator Awareness");

            // Write the column headers to the first row of the worksheet
            worksheet.Cells[1, 1].Value = "Time Taken";
            worksheet.Cells[1, 2].Value = "User";
            worksheet.Cells[1, 3].Value = "Created At";

            int row = 2;
            foreach (var item in list)
            {
                worksheet.Cells[row, 1].Value = item.timeTaken;
                worksheet.Cells[row, 2].Value = item.user;
                worksheet.Cells[row, 3].Value = item.createAt;
                row++;
            }


            // Save the Excel file to a MemoryStream
            MemoryStream stream = new MemoryStream();
            excel.SaveAs(stream);

            // Create a SaveFileDialog and set the file name filter
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";

            // Show the save dialog and check if the user clicked the "OK" button
            bool? result = saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK;
            if (result == true)
            {
                // Save the Excel file to the selected file path
                string fileName = saveFileDialog.FileName;
                File.WriteAllBytes(fileName, excel.GetAsByteArray());

                // Show a message box to confirm the file was saved successfully
                System.Windows.MessageBox.Show("Exported data to " + fileName, "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }




        }

        private void BtnClearDatabaseAwareness_Click(object sender, RoutedEventArgs e)
        {

            operatorAwarenessRepository.Delete();

            System.Windows.MessageBox.Show("Cleared operator_awareness table", "Clear Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RefreshSerialNumber()
        {
            SerialNumberGridView.ItemsSource = null;
            SerialNumberGridView.ItemsSource = new GasDetectionData().GasDetections;
        }
        private void btnSerialNumberSave_Click(object sender, RoutedEventArgs e)
        {
            var list = SerialNumberGridView.Items;
            foreach (GasDetection item in list)
            {
                gasDetectionRepository.Save(item);
            }
            RefreshSerialNumber();
        }

        private void btnSerialNumberReload_Click(object sender, RoutedEventArgs e)
        {
            RefreshSerialNumber();
        }

        private void RefreshAlarmSetup()
        {
            AlarmSetupGridView.ItemsSource = null;
            AlarmSetupGridView.ItemsSource = new AlarmSetupData().AlarmSetups;

        }

        private void ClearAlarmSetup()
        {
            OIP.Text = "";
            InIP.Text = "";
            alarmSetupName.Text = "";
            newAlarmSetup = null;
        }

        private bool ValidateAlarmSetup()
        {
            return OIP.Text.Length > 0 && InIP.Text.Length > 0 && alarmSetupName.Text.Length > 0;
        }

        private void AlarmSetupGridView_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            var item = (AlarmSetup)e.Row.Item;

            if (item == null)
            {
                System.Windows.MessageBox.Show("Please select the AlarmSetup Rows");
                return;
            }
            OIP.Text = item.outsideAlamIp;
            InIP.Text = item.insideAlarmIp;
            alarmSetupName.Text = item.name;
            newAlarmSetup = item;
        }

        private void btnNewAlarmSetupSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAlarmSetup())
                return;
            var item = new AlarmSetup
            {
                outsideAlamIp = OIP.Text,
                insideAlarmIp = InIP.Text,
                name = alarmSetupName.Text
            };
            alarmsetupRepository.saveAlarmSetup(item);
            RefreshAlarmSetup();
            ClearAlarmSetup();
        }

        private void btnAlarmSetupDelete_Click(object sender, RoutedEventArgs e)
        {
            alarmsetupRepository.deleteAlarmSetup(newAlarmSetup);
            RefreshAlarmSetup();
            ClearAlarmSetup();
        }

        private void btnAlarmSetupSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAlarmSetup())
                return;
            var item = new AlarmSetup
            {
                outsideAlamIp = OIP.Text,
                insideAlarmIp = InIP.Text,
                name = alarmSetupName.Text
            };
            alarmsetupRepository.saveAlarmSetup(item);
            RefreshAlarmSetup();
            ClearAlarmSetup();
        }

        private bool ValidateUser()
        {
            return userName.Text.Length > 0 && Password.Text.Length > 0 && fullName.Text.Length > 0;
        }

        private void ClearUser()
        {
            userName.Text = "";
            Password.Text = "";
            fullName.Text = "";
            Role.Text = "";
            newUser = null;
        }
        private void RefreshUser()
        {
            UserGridView.ItemsSource = null;
            UserGridView.ItemsSource = new UserData().Users;
        }
        private void btnSaveModifiedUser_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateUser())
                return;
            var item = new User
            {
                id = newUser.id,
                username = userName.Text,
                fullName = fullName.Text,
                role = Role.Text,
                password = Password.Text,
                createAt = DateTime.Now
            };
            userRepository.save(item);
            RefreshUser();
            ClearUser();
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            userRepository.delete(newUser);
            RefreshUser();
            ClearUser();
        }

        private void btnSaveNewUser_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateUser())
                return;
            var item = new User
            {
                username = userName.Text,
                fullName = fullName.Text,
                role = Role.Text,
                password = Password.Text,
                createAt = DateTime.Now
            };
            userRepository.save(item);
            RefreshUser();
            ClearUser();
        }

        private void UserGridView_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            var user = (User)e.Row.Item;

            if (user == null)
            {
                System.Windows.MessageBox.Show("Please select the AlarmSetup Rows");
                return;
            }
            userName.Text = user.username;
            Password.Text = user.password;
            fullName.Text = user.fullName;
            Role.Text = user.role;
            newUser = user;
        }

        private void btnReloadDb_Click(object sender, RoutedEventArgs e)
        {
            setSetupIsDoneFlag(false);
            clearCameraPanels();
            LoadConfigAndPopulate();
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            rooms.Clear();
            rooms = roomRepository.Get();
            setSetupIsDoneFlag(false);
            Thread thread = new Thread(new ThreadStart(OnCameraScanThread));
            thread.Start();
        }

        private void OnCameraScanThread()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                CameraScannerService cameraScannerService = new CameraScannerService(Convert.ToInt32(TxtFrom.Value), Convert.ToInt32(TxtTo.Value));
                try
                {

                    List<CameraCombo> scannedCameras = cameraScannerService.doInBackground();
                    List<CameraCombo> scannedCamerasToKeep = new List<CameraCombo>(scannedCameras);
                    List<Room> scannedRooms = new List<Room>();
                    foreach (CameraCombo scannedCameraCombo in scannedCameras)
                    {

                        var optionalExistingCameraCombo = cameraCombos.Where(m => m.outsideIp == scannedCameraCombo.outsideIp).FirstOrDefault();
                        if (rooms.Count > 0)
                        {
                            if (optionalExistingCameraCombo != null)
                            {
                                scannedCamerasToKeep.Remove(scannedCameraCombo);
                            }
                            else

                            {
                                // it's a new cameracombo so make a room for it
                                Room newRoom = new Room
                                {
                                    outsideCamera = scannedCameraCombo.outsideIp,
                                    insideCamera = scannedCameraCombo.insideIp,
                                    name = scannedCameraCombo.name
                                };
                                scannedRooms.Add(newRoom);
                            }
                        }
                        else
                        {
                            Room newRoom = new Room
                            {
                                outsideCamera = scannedCameraCombo.outsideIp,
                                insideCamera = scannedCameraCombo.insideIp,
                                name = scannedCameraCombo.name
                            };
                            scannedRooms.Add(newRoom);
                        }

                    }

                    RefreshCameraCombosWithScanned(scannedCamerasToKeep);
                    cameraComboRepository.SaveCombos(cameraCombos);

                    // check if we have existing rooms in our new rooms and remove them
                    foreach (Room existingRoom in rooms)
                    {
                        var optionalRoom = scannedRooms.Where(item => item.outsideCamera == existingRoom.outsideCamera).FirstOrDefault();

                        if (optionalRoom != null)
                        {
                            scannedRooms.Remove(optionalRoom);
                        }
                    }

                    // loop the remaining new rooms and assing x & y coords
                    // get the existing room size so we can continue setting x,y coords on new rooms
                    int currentNumberOfRooms = rooms.Count();

                    foreach (Room potentialNewRoom in scannedRooms)
                    {
                        currentNumberOfRooms++;
                        potentialNewRoom.roomId = (currentNumberOfRooms);

                        // calculate the x,y coords in the room setup screen
                        int x = currentNumberOfRooms > (initialSetCount / 2) ? currentNumberOfRooms - (initialSetCount / 2) : currentNumberOfRooms;
                        int y = currentNumberOfRooms > (initialSetCount / 2) ? 2 : 1;
                        potentialNewRoom.x = (x);
                        potentialNewRoom.y = (y);
                    }

                    rooms.AddRange(scannedRooms);
                    if (rooms.Count == 0)
                    {
                        System.Windows.MessageBox.Show("Couldn't find any cameras!");
                        AlarmMessage.Content = "Not Found the Cameras";
                        //lblNoConfigFound.Text = ("Couldn't find any cameras!");
                    }
                    else
                    {
                        //lblNoConfigFound.Visible = (false);
                        //GetRoomIpList();
                        AlarmMessage.Content = "Found " + scannedCameras.Count + " Cameras";
                        SetupRoomsInGrid();
                        //btnSaveRoomConfig.Enabled = (true);
                    }

                    SaveConfig();
                }
                catch (Exception interruptedException)
                {
                    //LOGGER.error(interruptedException.getMessage());

                    //lblNoConfigFound.Text = ("An error has occured while scanning for camera's, please check the logs and retry");
                    //Thread.currentThread().interrupt();
                }
            }));

        }

        private void RefreshCameraCombosWithScanned(List<CameraCombo> scannedCamerasToKeep)
        {
            cameraCombos.Remove(unsetCameraCombo);
            cameraCombos.AddRange(scannedCamerasToKeep);
            cameraCombos.Add(unsetCameraCombo);
        }

        //private void GetRoomIpList()
        //{
        //    roomIps = new List<string>();
        //    if (rooms.Count > 0)
        //    {
        //        rooms.ForEach(e => roomIps.Add(e.insideCamera));
        //    }
        //}
        private void TxtFrom_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int fromIP = Convert.ToInt32(TxtFrom.Value);
            int toIP = fromIP + initialSetCount;
            if (toIP > 255)
            {
                System.Windows.MessageBox.Show("Range IP Over!!!");
                return;
            }


        }

        private void TxtTo_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();

            btnContinue.IsEnabled = (true);
        }

        private void SaveConfig()
        {
            List<Room> updatedRoomList = new List<Room>();
            cameraRoomPanels.Where(m => m.SaveAndGetRoom() != null).ToList().ForEach(m => updatedRoomList.Add(m.SaveAndGetRoom()));

            List<Room> deletedRooms = new List<Room>();
            rooms.Where(m => !updatedRoomList.Contains(m)).ToList().ForEach(d => deletedRooms.Add(d));
            if (deletedRooms.Count != 0)
            {
                StringBuilder stringBuilder = new StringBuilder();

                deletedRooms.Where(d => d.id != null).ToList().ForEach(d => stringBuilder.Append(d.name + ", "));


                if (stringBuilder.ToString() != "")
                {


                    string deletedRoomNames = stringBuilder.ToString().Substring(0, stringBuilder.ToString().Length - 2);

                    string warnBeforeDeletionMessage = "Warning, the following rooms have been unset:\n" + deletedRoomNames + "\n"
                            + "If you continue this will unrevertable delete all references to this room including PPE Setup\n"
                            + "and any saved gas readings. Are you sure?";

                    // we'll count clicking the X button as a NO


                    MessageBoxResult answer = System.Windows.MessageBox.Show(warnBeforeDeletionMessage, "Warning! About to delete data", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);

                    if (answer == 0)
                    { // if user clicked yes
                        foreach (Room room in deletedRooms)
                        {
                            rooms.Remove(room);
                        }
                        deletedRooms.ForEach(d => roomRepository.Delete(d));
                    } // todo fix for when user clicks X
                }
                else
                {
                    foreach (Room room in deletedRooms)
                    {
                        rooms.Remove(room);
                    }
                }

            }

            // create a linked hash set from rooms then addAll() updatedRoomsList to only add uniques
            HashSet<Room> combinedSet = new HashSet<Room>(rooms);
            updatedRoomList.ForEach(a => combinedSet.Add(a));
            rooms = new List<Room>(combinedSet);

            roomRepository.SaveRooms(rooms);
            roomSetupIsSaved = true;
            btnContinue.IsEnabled = true;
        }

        private void SetupRoom_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RefreshData();
        }

        private void btnSerialNumberDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = SerialNumberGridView.SelectedItem as GasDetection;
            if (item != null) { return; }
            gasDetectionRepository.Delete(item);
            RefreshSerialNumber();

        }

        private void CmbCamResolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = CmbCamResolution.SelectedIndex;

        }

        private void CmbCameraType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbCameraType.SelectedIndex == 1)
            {
                ResulutionPixelPanel.Visibility = Visibility.Visible;
            }
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirm = System.Windows.MessageBox.Show("Are you sure?", "Save Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (confirm == MessageBoxResult.Yes)
            {
                string net2_path = TxtNet2.Text;
                string clientId = ClientID.Text;
                string clientSecret = ClientSecret.Text;
                string pw = PW.Text;
                string accountId = AccountID.Text;
                string user = User.Text;
                string interval = Interval.Text;
                string operatorInteval = OperatorAwarenessInterval.Text;
                string resolution = cameraResolutionDictionary[CmbCamResolution.SelectedIndex];
                string camera = cameraType[CmbCameraType.SelectedIndex];

                property.SaveConfig(ZGroupContstant.WHOSIN_FILE_LOCATION, net2_path);
                property.SaveConfig(ZGroupContstant.INETLIVE_CLIENT_ID, clientId);
                property.SaveConfig(ZGroupContstant.INETLIVE_CLIENT_SECRET, clientSecret);
                property.SaveConfig(ZGroupContstant.INETLIVE_PW, pw);
                property.SaveConfig(ZGroupContstant.INETLIVE_ACCOUNT_ID, accountId);
                property.SaveConfig(ZGroupContstant.INETLIVE_USER, user);

                property.SaveConfig(ZGroupContstant.INETLIVE_QUERY_INTERVAL, interval);
                property.SaveConfig(ZGroupContstant.OPERATOR_AWARENESS_TIMING, operatorInteval);

                property.SaveConfig(ZGroupContstant.CAMERA_TYPE, resolution);
                property.SaveConfig(ZGroupContstant.CAMERA_AXIS_RESOLUTION, resolution);
            }

        }
    }

    public class GasDetectionData
    {
        private GasDetectionRepository gasDetectionRepository = GasDetectionRepository.getInstance();
        public ObservableCollection<GasDetection> GasDetections
        {
            get;
            set;
        }
        public GasDetectionData()
        {
            GasDetections = new ObservableCollection<GasDetection>();

            var gasDetections = gasDetectionRepository.GetGasDetections();

            foreach (var gasDetection in gasDetections)
            {
                GasDetections.Add(gasDetection);
            }
        }
    }

    public class UserData
    {
        private UserRepository userRepository = UserRepository.getInstance();
        public ObservableCollection<User> Users
        {
            get;
            set;
        }
        public UserData()
        {
            Users = new ObservableCollection<User>();
            var users = userRepository.getUsers();
            foreach (var user in users)
            {
                Users.Add(user);
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
            foreach (var alarmSetup in alarmSetups)
            {
                AlarmSetups.Add(alarmSetup);
            }
        }
    }
}


