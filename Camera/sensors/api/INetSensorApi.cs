using RestSharp;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.sensors.api
{
    public class INetSensorApi
    {
        private string base_url { get; set; }
        private string token { get; set; }
        private string accountId { get; set; }
        private RestClient client { get; set; }
        public INetSensorApi(string base_url, string token, string accountId) 
        { 
            this.base_url = base_url; 
            this.token = token;
            this.accountId = accountId;
            client = new RestClient(this.base_url);
           
        }
        public RestResponse getLiveData(string updateTime)
        {
            var request = new RestRequest("query/live", Method.Post);

            request.AddHeader("Content-Type", "application/json");
           
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("X-AccountId", accountId);
            request.AddJsonBody(updateTime);
            var requestData = client.Execute(request);
            return requestData;

        }
    }
}
