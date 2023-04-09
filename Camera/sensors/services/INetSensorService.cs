using Camera.sensors.api;
using Camera.sensors.dto;
using Camera.sensors.response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Json;
using Camera.constants;

namespace Camera.sensors.services
{
    public class INetSensorService
    {
        private PropertyUtil properties = PropertyUtil.getInstance();
        private static string CONTENT_TYPE = "application/x-www-form-urlencoded;charset=UTF-8";
        //public string base_ip = //ConfigurationManager.AppSettings["inetapi_domain"].ToString();

        public INetLiveResponseDto getINetLiveData(List<String> equipmentSerialNumbers)
        {
            string base_ip = properties.GetPropertyAsstring(ZGroupContstant.INetAPI_Domain);
            INetSensorAuthApi iNetSensorAuthApi = new INetSensorAuthApi(base_ip);
            string clientId = properties.GetINetLiveAuth().clientId;//ConfigurationManager.AppSettings["inetlive.auth.client_id"].ToString();
            string clientSecret = properties.GetINetLiveAuth().clientSecret;//ConfigurationManager.AppSettings["inetlive.auth.client_secret"].ToString();
            string user = properties.GetINetLiveAuth().user;//ConfigurationManager.AppSettings["inetlive.auth.user"].ToString();
            string pw = properties.GetINetLiveAuth().pw;//ConfigurationManager.AppSettings["inetlive.auth.pw"].ToString();
            string accountId = properties.GetINetLiveAuth().accountId;//ConfigurationManager.AppSettings["inetlive.auth.account_id"].ToString();
            INetLiveAuth iNetLiveAuth = new INetLiveAuth
            {
                accountId = accountId,
                clientId = clientId,
                clientSecret = clientSecret,
                user = user,
                pw = pw
            };

            var response = iNetSensorAuthApi.getAuthToken(CONTENT_TYPE, iNetLiveAuth.accountId, iNetLiveAuth.clientId, iNetLiveAuth.clientSecret, iNetLiveAuth.user, iNetLiveAuth.pw);
            if (response.IsSuccessful)
            {
                var authResponse = JsonConvert.DeserializeObject<INetAuthResponseDto>(response.Content.ToString());
                string accessToken = authResponse.access_token;


                INetSensorApi liveApi = new INetSensorApi("https://inetapi.indsci.com/iNetAPI/v1/", accessToken, iNetLiveAuth.accountId);

                

                string date = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");

                INetSensorModel model = new INetSensorModel
                {
                    historyTime = date,
                    updateTime = date,
                    sn = equipmentSerialNumbers
                };

                INetSensorQuery query = new INetSensorQuery
                {
                    query = model
                };

                var body = JsonConvert.SerializeObject(query);
                var liveSensorDataResponse =  liveApi.getLiveData(body);

                if (liveSensorDataResponse.IsSuccessful)
                {
                    var data = JsonConvert.DeserializeObject<INetLiveResponseDto>(liveSensorDataResponse.Content.ToString());
                    return data;
                }
                else
                {
                    return null;
                    //LOGGER.error("Could not retrieve live sensor data. \n{}", liveSensorDataResponse.errorBody());
                }
            }
            return null;
        }
    }
}
