using Camera.models;
using Camera.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Camera.UserControls
{
    /// <summary>
    /// Interaction logic for PpeIcon.xaml
    /// </summary>
    public partial class PpeIconPanel : RadTransitionControl
    {
        private PpeIconUtil ppeIconUtil = PpeIconUtil.getInstance();
        private PpeIcon ppeIcon { get; set; }
        private bool isEnabled = true;
        private bool isSet = false;
        public PpeIconPanel(PpeIcon ppeIcon)
        {
            InitializeComponent();
            this.ppeIcon = ppeIcon;
            ppeImage.Source = ppeIconUtil.getResource(ppeIcon);
        }

        public bool IsSet()
        {
            return isSet;
        }

        public PpeIconPanel setSet(bool set)
        {
            if (isEnabled)
            {
                isSet = set;

                toggleBorder();
            }
            return this;
        }

        public void setEnabled(bool enabled)
        {
            isEnabled = enabled;
        }

        public PpeIcon getPpeIcon()
        {
            return ppeIcon;
        }
        private void toggleBorder()
        {
            // toggle the border to show the isSet state
            
            if (isSet)
            {
                //ppeButton.BorderBrush = Brushes.Red;
                ppeButton.Background = Brushes.Gray;
                
            }
            else
            {
                //ppeButton.BorderBrush = null;
                ppeButton.Background = null;
            }
            this.Focusable = true;
        }

        private void ppeButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEnabled)
            {
                isSet = !isSet;
                toggleBorder();
            }
        }
    }
}
