using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Camera.camera.types
{
    public interface  CameraType
    {
        /**
   * Get type of camera
   * @return enum cameratype
   */
        EnumCameraType GetType();

        /**
         * IP-address of a camera
         * @return
         */
        string GetCameraIp();

        /**
         * creates a snapshot
         * @param file
         */
        void GetSnapshotStreamAndSaveToFile(String file);

        /**
         * Gets the URL which is able to take a snapshot via a BufferedInputStream
         * @return URL
         * @throws MalformedURLException
         */
        string GetSnapshotUrl();
    }
}
