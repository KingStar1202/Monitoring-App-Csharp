using Camera.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Camera.utils
{
    public class PpeIconUtil
    {
        private static PpeIconUtil instance;
        public static PpeIconUtil getInstance()
        {
            if (instance == null)
            {
                instance = new PpeIconUtil();
            }
            return instance;
        }

        public BitmapImage getResource(PpeIcon ppeIcon)
        {
            BitmapImage bi3 = new BitmapImage(new Uri("pack://application:,,,/resources/" + ppeIcon.imageFile));
            return bi3;
        }

    }
}
