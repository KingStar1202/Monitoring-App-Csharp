using Camera.camera.types;
using Camera.constants;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.Utils
{
    public class CameraUtils
    {
        private PropertyUtil propertyUtil = PropertyUtil.getInstance();
        public void SaveSnapshotCamera(string roomName, CameraType cameraType)
        {
            String fileName = roomName + "-" + DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + ".png";
            cameraType.GetSnapshotStreamAndSaveToFile(fileName);
        }

        /**
         * Gets cameratype based on server.properties
         * @param ip ip of the camera to create new object
         * @return Interface CameraType
         */
        public CameraType getCameraType(string ip)
        {
            string camera_type = propertyUtil.GetCameraType();//ConfigurationManager.AppSettings["CameraType"].ToUpper();
            switch (camera_type)
            {
                case "RUGICAM":
                    return new RugiCameraType(ip);
                case "AXIS":
                    return new AxisCameraType(ip);
                default:
                    return new RugiCameraType(ip);
            }
        }
    }
}
