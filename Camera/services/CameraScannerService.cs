using Camera.models;
using Camera.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using Org.BouncyCastle.Bcpg;

namespace Camera.services
{
    public class CameraScannerService
    {
        private static string CAMERA_OUTISDE_RANGE = "10.10.1.";
        private static string CAMERA_INSIDE_RANGE = "10.10.2.";
        private static int RTSP_PORT = 554;
        private static int TIMEOUT = 1000;
        private static CameraUtils cameraUtils = new CameraUtils();
        private List<Room> rooms = new List<Room>();
        private  List<CameraCombo> cameraCombos = new List<CameraCombo>();

        private int start;
        private int end;
        public CameraScannerService(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public CameraScannerService() { }

        public List<CameraCombo> doInBackground()
        {
            cameraCombos = new List<CameraCombo>();
            for (int i = start; i <= end; i++)
            {
                string outsideHost = CAMERA_OUTISDE_RANGE + i;
                string insideHost = CAMERA_INSIDE_RANGE + i;

                // for testing
                //            CameraCombo newTestCameraCombo = new CameraCombo();
                //            newTestCameraCombo.setOutsideIp(outsideHost);
                //            newTestCameraCombo.setInsideIp(insideHost);
                //            newTestCameraCombo.setName(outsideHost);
                //            cameraCombos.add(newTestCameraCombo);

                if (isActiveCamera(outsideHost))
                {
                    CameraCombo newCameraCombo = new CameraCombo();
                    newCameraCombo.outsideIp = (outsideHost);
                    newCameraCombo.name = (outsideHost);

                    // leave this commented for now
                    //                if (isReachableAndCamera(insideHost)) {
                    newCameraCombo.insideIp = (insideHost);
                    //                }

                    cameraCombos.Add(newCameraCombo);
                }
            }
            return cameraCombos;
        }



        public bool isActiveCamera(string host)
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IAsyncResult result = socket.BeginConnect(host, RTSP_PORT, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(TIMEOUT, true);
                if (socket.Connected)
                {
                    socket.EndConnect(result); 
                    socket.Close();
                    return true;
                }
                else
                {
                    socket.Close();
                    return false;
                }
               
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
