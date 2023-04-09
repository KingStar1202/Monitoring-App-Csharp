using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Camera.sensors.api
{
    public class INetSensorAuthApi
    {
        RestClient client;
        public INetSensorAuthApi(string ip) {
            client = new RestClient(ip);
        }
        public RestResponse getAuthToken(string conten_type, string accountId, string clientId, string clientSecret, string user, string pw)
        {

            var request = new RestRequest("oauth2/endpoint/iNet/token?grant_type=password", Method.Post)
                              .AddHeader("content_type", conten_type)
                              .AddParameter("client_id", clientId)
                              .AddParameter("client_secret",clientSecret)
                              .AddParameter("username",user)
                              .AddParameter("password", pw);
            var requestData =  client.Execute(request);
            return requestData;
            
        }
    }
}
