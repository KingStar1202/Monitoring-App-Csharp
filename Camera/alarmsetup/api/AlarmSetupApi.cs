using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.alarmsetup.api
{
    public class AlarmSetupApi
    {
        private string ip { get; set; }
        public string CALL_FAILED = "CALL-FAILED";

        public AlarmSetupApi(string ip)
        {
            this.ip = ip;

        }

        public async Task<bool> triggerAlarm()
        {
            var url = "/goform/zForm_webcall?activate=Activate&relayId=Relay1&username=admin&password=alphaadmin";
            try
            {
                var client = new RestClient(ip);
                var request = new RestRequest(url, RestSharp.Method.Get);
                var requestData = await client.ExecuteAsync(request);
                return requestData.IsSuccessful;
            }
            catch
            {

            }
            return false;
        }

        public async Task<bool> disableAlarm()
        {
            var url = "/goform/zForm_webcall?deactivate=Deactivate&relayId=Relay1&username=admin&password=alphaadmin";
            try
            {
                var client = new RestClient(ip);
                var request = new RestRequest(url, RestSharp.Method.Get);
                var requestData = await client.ExecuteAsync(request);
                return requestData.IsSuccessful;
            }
            catch
            {

            }
            return false;

        }


        public async Task<bool> openIntercom()
        {
            var url = "/goform/zForm_webcall?webcall=100&message=Place+call&username=admin&password=alphaadmin";
            try
            {
                var client = new RestClient(ip);
                var request = new RestRequest(url, RestSharp.Method.Get);
                var requestData = await client.ExecuteAsync(request);
                return requestData.IsSuccessful;
            }
            catch
            {

            }
            return false;
        }

        public async Task<bool> closeIntercom()
        {
            var url = "/goform/zForm_webcall?webcall=100&message=Stop&username=admin&password=alphaadmin";
            try
            {
                var client = new RestClient(ip);
                var request = new RestRequest(url, RestSharp.Method.Get);
                var requestData = await client.ExecuteAsync(request);
                return requestData.IsSuccessful;
            }
            catch
            {

            }
            return false;
        }

        public async Task<string> getCallStatus()
        {
            var url = "/goform/zForm_webcall?username=admin&password=alphaadmin";
            try
            {
                var client = new RestClient(ip);
                var request = new RestRequest(url, RestSharp.Method.Post);
                var requestData = await client.ExecuteAsync(request);
                if (requestData.IsSuccessful)
                {
                    return requestData.Content;
                }
            }
            catch
            {

            }
            return CALL_FAILED;
        }
    }
}
