using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.camera.types
{
    public class AxisCameraType : CameraType
    {
        private string cameraIp;
        public AxisCameraType(String cameraIp)
        {
            this.cameraIp = cameraIp;
        }
        public string GetCameraIp()
        {
            return this.cameraIp;
        }

        public void GetSnapshotStreamAndSaveToFile(string file)
        {
            string url = "";
            var req = System.Net.WebRequest.Create(url);
            using (Stream stream = req.GetResponse().GetResponseStream())
            {

            }
        }

        public string GetSnapshotUrl()
        {
            try
            {
                return string.Format("http://{0}/snap.jpg", GetCameraIp());
            }
            catch (Exception ex)
            {
                // LOGGER.error("An exception occurred while requesting snapshot URL from camera with ip: {}", getCameraIp(), e);
                throw new Exception();
            }
        }

        EnumCameraType CameraType.GetType()
        {
            return EnumCameraType.AXIS;
        }
    }
}
