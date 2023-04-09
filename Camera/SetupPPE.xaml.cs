using Camera.models;
using Camera.repository;
using Camera.UserControls;
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
    /// Interaction logic for SetupPPE.xaml
    /// </summary>
    public partial class SetupPPE : RadRibbonWindow
    {
        private PpeIconRepository ppeIconRepository = PpeIconRepository.getInstance();
        private List<PpeIconPanel> ppeIconPanels = new List<PpeIconPanel>();
        private Room room { get; set; }
        static SetupPPE()
        {
            SetupPPE.IsWindowsThemeEnabled = false;
        }

        public SetupPPE(Room room)
        {
            InitializeComponent();
            this.room = room;
            setupPpeIcons();
        }

        private void setupPpeIcons()
        {
            List<PpeIcon> ppeIcons = ppeIconRepository.Get();
            if(ppeIcons != null && ppeIcons.Count > 0)
            {
                int index = 0;
                foreach(PpeIcon icon in ppeIcons)
                {
                    
                    PpeIconPanel ppeIconPanel = new PpeIconPanel(icon);
                    if (index < 5)
                    {
                        Grid.SetRow(ppeIconPanel, 1);
                        int column = index % 5;
                        Grid.SetColumn(ppeIconPanel, column);
                    }
                    else
                    {
                        Grid.SetRow(ppeIconPanel, 2);
                        int column = index % 5 + 1 ;
                        Grid.SetColumn(ppeIconPanel, column);
                    }
                    this.topSect.Children.Add(ppeIconPanel);
                    index++;
                    ppeIconPanels.Add(ppeIconPanel);
                }
            }
            if (room != null)
            {
                if (room.ppeIcons != null && room.ppeIcons.Count != 0)
                {
                    foreach (PpeIcon roomPpeIcon in room.ppeIcons)
                    {
                        var optionalPpeIconPanel = ppeIconPanels.FirstOrDefault(e => e.getPpeIcon().code == roomPpeIcon.code);


                        if (optionalPpeIconPanel != null)
                        {
                            optionalPpeIconPanel.setSet(true);
                        }
                    }
                }
            }
            else
            {
                // disable buttons when no room is loaded
                foreach (var ppeIconPanel in ppeIconPanels)
                {
                    ppeIconPanel.setEnabled(false);
                }

            }
        }

        private void SavePpeIcons_Click(object sender, RoutedEventArgs e)
        {
            room.ppeIcons = null;
            List<PpeIcon> newList = new List<PpeIcon>();
            ppeIconPanels.Where(iconPanel => iconPanel.IsSet()).ToList().ForEach(iconPanel => newList.Add(iconPanel.getPpeIcon()));
            room.ppeIcons = newList;
            this.IsEnabled = true;
            this.Close();
        }
    }
}
