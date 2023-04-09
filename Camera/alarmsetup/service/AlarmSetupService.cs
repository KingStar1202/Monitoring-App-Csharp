using Camera.alarmsetup.api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Camera.alarmsetup.service
{
    public class AlarmSetupService
    {
        public static string CALL_FAILED = "CALL-FAILED";
        private string outsideAlarmIp { get; set; }
        private string insideAlarmIp { get; set; }

        private System.Threading.Timer alarmRepeatingTimerInside { get; set; }
        private System.Threading.Timer alarmRepeatingTimerOutside { get; set; }

        public AlarmSetupService(string outsideAlarmIp, string insideAlarmIp)
        {
            this.outsideAlarmIp = outsideAlarmIp;
            this.insideAlarmIp = insideAlarmIp;
        }

        public async Task<bool> isOutsideBeingCalled()
        {
            return await isIntercomBeingCalled(outsideAlarmIp);
        }

        public  async Task<bool> isInsideBeingCalled()
        {
            return await isIntercomBeingCalled(insideAlarmIp);
        }

        private async Task<bool> isIntercomBeingCalled(string alarmIp)
        {
            string callStatusHtml = await getCallStatusHtml(alarmIp);

            if (!CALL_FAILED.Equals(callStatusHtml))
            {

                XmlDocument document = new XmlDocument();
                document.LoadXml(callStatusHtml);
                XmlDocument webCallForm = (XmlDocument)document.GetElementsByTagName("form").Item(1);
                XmlElement webCallTable = webCallForm.GetElementById("fullsize");
                XmlNodeList rows = webCallTable.GetElementsByTagName("tr");
                foreach (XmlNode row in rows)
                {
                    XmlNodeList tableDatas = row.OwnerDocument.GetElementsByTagName("td");
                    if (tableDatas.Count == 2 && tableDatas.Item(0).InnerText == "Call status:")
                    {
                        return isCallActive(tableDatas.Item(1).InnerText);
                    }
                }
                
            }

            return false;
        }

        public async void openIntercomOutside()
        {
            // first close any potential open calls on both intercoms
            if (await closeIntercom(outsideAlarmIp) && await closeIntercom(insideAlarmIp))
            {

                // then open the intercom
                await openIntercom(outsideAlarmIp);
            }
        }

        public async void openIntercomInside()
        {
            // first close any potential open calls on both intercoms
            if (await closeIntercom(outsideAlarmIp) && await closeIntercom(insideAlarmIp))
            {

                // then open the intercom
                await openIntercom(insideAlarmIp);
            }
        }

        public void closeIntercomOutside()
        {
            closeIntercom(outsideAlarmIp);
        }

        public void closeIntercomInside()
        {
            closeIntercom(insideAlarmIp);
        }

        public void triggerBothAlarms()
        {
            triggerOutsideAlarm();
            triggerInsideAlarm();
        }

        public void disableBothAlarms()
        {
            disableOutsideAlarm();
            disableInsideAlarm();
        }

        public void triggerOutsideAlarm()
        {
            
            alarmRepeatingTimerOutside = new System.Threading.Timer(alarmRepeatingTimerOutsideEvent, null, 0, 3000);
            
           
            triggerAlarm(outsideAlarmIp);
        }

        private void alarmRepeatingTimerOutsideEvent(object state)
        {
            disableAlarm(outsideAlarmIp);
            triggerAlarm(outsideAlarmIp);
        }

        
        public void triggerInsideAlarm()
        {
            alarmRepeatingTimerInside = new System.Threading.Timer(alarmRepeatingTimerInsideEvent, null, 0, 3000);
            

            triggerAlarm(insideAlarmIp);
        }

        private void alarmRepeatingTimerInsideEvent(object state)
        {
            disableAlarm(insideAlarmIp);
            triggerAlarm(insideAlarmIp);
        }


        public void disableOutsideAlarm()
        {
            //LOGGER.info("Disabling outsisde alarm");
            alarmRepeatingTimerOutside.Dispose();
            alarmRepeatingTimerOutside = null;
            disableAlarm(outsideAlarmIp);
        }

        public void disableInsideAlarm()
        {

            
            alarmRepeatingTimerInside.Dispose();
            alarmRepeatingTimerInside = null;
            disableAlarm(insideAlarmIp);
        }

        private bool isCallActive(string callStatus)
        {

            switch (callStatus)
            {
                case "Idle":
                    return false;
                case "(Outgoing call) Calling":
                case "In call":
                case "(Incoming call) Ringing":
                    return true;

            }

            return false;
        }

        // todo make this non blocking, swingworker maybe? Refactor required
        private async Task<string> getCallStatusHtml(string alarmIp)
        {
            AlarmSetupApi alarmSetupApi = new AlarmSetupApi(alarmIp);
            return await alarmSetupApi.getCallStatus();
        }

        private Task<bool> openIntercom(string alarmIp)
        {
            AlarmSetupApi alarmSetupApi = new AlarmSetupApi(alarmIp);
            return alarmSetupApi.openIntercom();
        }

        private Task<bool> closeIntercom(string alarmIp)
        {
            AlarmSetupApi alarmSetupApi = new AlarmSetupApi(alarmIp);
            return alarmSetupApi.closeIntercom();


        }

        private Task<bool> triggerAlarm(string alarmIp)
        {
            AlarmSetupApi alarmSetupApi = new AlarmSetupApi(alarmIp);
            return alarmSetupApi.triggerAlarm();
        }

        private Task<bool> disableAlarm(string alarmIp)
        {
            AlarmSetupApi alarmSetupApi = new AlarmSetupApi(alarmIp);
            return alarmSetupApi.disableAlarm();

        }

    }
}
