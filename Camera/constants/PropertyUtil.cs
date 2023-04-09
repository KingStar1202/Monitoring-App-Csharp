using Aspose.Cells;
using Camera.camera.types;
using Camera.repository;
using Camera.sensors.dto;
using MediaFoundation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Xml.Linq;

namespace Camera.constants
{
    public class PropertyUtil
    {
        private static PropertyUtil instance;
        public static PropertyUtil getInstance()
        {
            if (instance == null)
            {
                instance = new PropertyUtil();
            }
            return instance;
        }

        private PropertyUtil()
        {
            //try (FileInputStream fileInputStream = new FileInputStream(SERVER_CONFIGURATION_FILE)){
                
            //    properties.load(fileInputStream);
            //} catch (IOException e)
            //{
               
            //}
        }

        public string GetPropertyAsstring(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        public int GetPropertyAsInt(string name)
        {

            try
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings[name]);
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public bool GetPropertyAsBoolean(string name)
        {
            string value = ConfigurationManager.AppSettings[name];
            if (null == value || "".Equals(value))
            {
                return false;
            }

            return "true".Equals(ConfigurationManager.AppSettings[name]);
        }

        /**
         * get camera type from: server.properties
         * @return
         */
        public string GetCameraType()
        {
            string value = ConfigurationManager.AppSettings[ZGroupContstant.CAMERA_TYPE];
            if (null == value || "".Equals(value))
            {
                return "RUGICAM";
            }

            return value;
            
        }

        public string GetNet2AccessDataFileLocation()
        {
            return ConfigurationManager.AppSettings[ZGroupContstant.WHOSIN_FILE_LOCATION];
        }

        public string GetTransportMode()
        {
            return ConfigurationManager.AppSettings[ZGroupContstant.TRANSPORT_MODE];
        }

        public string GetOperatorAwarenessLgo()
        {
            return ConfigurationManager.AppSettings[ZGroupContstant.OPERATOR_AWARENESS_LOGO];
        }

        

        public bool IsDemoMode()
        {
            return "true".Equals(ConfigurationManager.AppSettings[ZGroupContstant.DEMO_MODE]);
        }

        public int GetScanRangeStart()
        {
            return GetPropertyAsInt(ZGroupContstant.SCANRANGE_START) > 0 ? GetPropertyAsInt(ZGroupContstant.SCANRANGE_START) : 1;
        }

        public int GetScanRangeEnd()
        {
            return GetPropertyAsInt(ZGroupContstant.SCANRANGE_END) > 0 ? GetPropertyAsInt(ZGroupContstant.SCANRANGE_END) : 255;
        }



        public void SetScanRangeStart(int start)
        {
            SaveConfig(ZGroupContstant.SCANRANGE_START, start.ToString());
        }

        public void SetScanRangeEnd(int start)
        {
            SaveConfig(ZGroupContstant.SCANRANGE_START, start.ToString());
        }

        public void SaveConfig(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            //ConfigurationManager.RefreshSection("appSettings");
        }

        public int GetOperatorAwarenessTiming()
        {
            return GetPropertyAsInt(ZGroupContstant.OPERATOR_AWARENESS_TIMING) > 0 ? GetPropertyAsInt(ZGroupContstant.OPERATOR_AWARENESS_TIMING) : 60;
        }

        public int GetINetLiveInterval()
        {
            return GetPropertyAsInt(ZGroupContstant.INETLIVE_QUERY_INTERVAL) > 0 ? GetPropertyAsInt(ZGroupContstant.INETLIVE_QUERY_INTERVAL) : 10000; // delay is in ms
        }

        public int GetIntercomMonitoringInterval()
        {
            return GetPropertyAsInt(ZGroupContstant.INTERCOM_MONITORING_INTERVAL) > 0 ? GetPropertyAsInt(ZGroupContstant.INTERCOM_MONITORING_INTERVAL) : 2000; // delay is in ms
        }

        public bool IsIntercomEnabled()
        {
            return GetPropertyAsBoolean(ZGroupContstant.INTERCOM_ENABLED);
        }

        public INetLiveAuth GetINetLiveAuth()
        {
            INetLiveAuth auth = new INetLiveAuth { 
                clientId = GetPropertyAsstring(ZGroupContstant.INETLIVE_CLIENT_ID),
                clientSecret = GetPropertyAsstring(ZGroupContstant.INETLIVE_CLIENT_SECRET),
                user = GetPropertyAsstring(ZGroupContstant.INETLIVE_USER),
                pw = GetPropertyAsstring(ZGroupContstant.INETLIVE_PW),
                accountId = GetPropertyAsstring(ZGroupContstant.INETLIVE_ACCOUNT_ID)
            };
            return auth;
        }
    }
}
