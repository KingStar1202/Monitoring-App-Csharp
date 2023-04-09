using Camera.models;
using Camera.repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using System.Xml.Linq;
using Xamarin.Forms;
using System.Xml;
using Camera.constants;

namespace Camera.net2access
{
    public class Net2Service
    {
        private RoomDataRepository roomDataRepository = RoomDataRepository.getInstance();
        private PropertyUtil propertyUtil = PropertyUtil.getInstance();
        private CameraOverView cameraOverView { get;set; }
        public Net2Service(CameraOverView cameraOverView)
        {
            this.cameraOverView = cameraOverView;
        }

        public void ReadFileAndSaveToDatabase()
        {
            string location = ConfigurationManager.AppSettings["WHOSIN_FILE_LOCATION"].ToString();
            try
            {
                SaveRoomDataToDatabase(GetListOfRoomDataFromFile(location));
                cameraOverView.UpdatePeopleInRooms();
            }
            catch (IOException e)
            {
                
            }

        }

        private Dictionary<String, List<Net2RoomData>> GetListOfRoomDataFromFile(String location)
        {
            string doc = File.ReadAllText(location);
            Dictionary<string, List<Net2RoomData>> mapOfRooms = new Dictionary<string, List<Net2RoomData>>();
            XmlDocument document = new XmlDocument();
            document.LoadXml(doc);
            XmlNodeList table = document.GetElementsByTagName("table");
           
          
            foreach (XmlNode element in table)
            {
                XmlNodeList rows = element.OwnerDocument.GetElementsByTagName("tr");
                foreach (XmlNode row in rows)
                {
                    XmlNodeList tableData = row.OwnerDocument.GetElementsByTagName("td");
                    if (tableData.Count == 3)
                    {
                        String user = tableData.Item(0).Value.ToString();
                        String roomName = tableData.Item(1).Value.ToString();
                        String lastTime = tableData.Item(2).Value.ToString();

                        Net2RoomData roomData = new Net2RoomData
                        {
                            person = user,
                            roomName = roomName,
                            lastLogin = lastTime
                        };

                        if (mapOfRooms.ContainsKey(roomName))
                        {
                            mapOfRooms[roomName].Add(roomData);

                        }
                        else
                        {
                            List<Net2RoomData> roomList = new List<Net2RoomData>();
                            roomList.Add(roomData);
                            mapOfRooms.Add(roomName, roomList);
                        }
                    }
                }
                
            }
            return mapOfRooms;

        }
            

        private void SaveRoomDataToDatabase(Dictionary<String, List<Net2RoomData>> mapOfRooms)
        {
            roomDataRepository.save(mapOfRooms);
        }
    }
}
