using Camera.models;
using Camera.net2access;
using Camera.repository;
using Camera.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using Telerik.Windows.Controls;

namespace Camera
{
    /// <summary>
    /// Interaction logic for Entrants.xaml
    /// </summary>
    public partial class Entrants : RadRibbonWindow
    {
        private string roomName { get;set; }
        private CameraPanel panel { get; set; }
        private RoomDataRepository roomDataRepository = RoomDataRepository.getInstance();
        private List<Attendee> list { get; set; }
        static Entrants()
        {
            Entrants.IsWindowsThemeEnabled = false;
        }

        public Entrants(string roomName, CameraPanel panel)
        {
            InitializeComponent();
            this.roomName = roomName;
            this.panel = panel;
            list = roomDataRepository.getListOfAttendeesForRoom(roomName);
            foreach(Attendee item in list)
            {
                radGridViewEntrants.Items.Add(item);
            }
        }

        
        private void RadRibbonWindow_Closed(object sender, EventArgs e)
        {
            this.panel.clearEntrantsPopup();
        }
    }

    
}
