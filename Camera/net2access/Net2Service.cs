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
using System.Windows;
using System.Windows.Forms;

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
            string location = propertyUtil.GetNet2AccessDataFileLocation();
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
            Dictionary<string, List<Net2RoomData>> mapOfRooms = new Dictionary<string, List<Net2RoomData>>();
            try
            {
                string doc = File.ReadAllText(location);
                XmlDocument document = new XmlDocument();
                document.LoadXml(doc);
                XmlNodeList tables = document.GetElementsByTagName("table");

                foreach (XmlNode table in tables)
                {
                    XmlNodeList rows = table.SelectNodes("tr");
                    foreach (XmlNode row in rows)
                    {
                        XmlNodeList tableData = row.SelectNodes("td");
                        if (tableData.Count == 3)
                        {
                            string user = tableData.Item(0).InnerText.ToString();
                            string roomName = tableData.Item(1).InnerText.ToString();
                            string lastTime = tableData.Item(2).InnerText.ToString();

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
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Please check the htlm parser");
            }
            
            

            return mapOfRooms;

        }
            

        private void SaveRoomDataToDatabase(Dictionary<String, List<Net2RoomData>> mapOfRooms)
        {
            roomDataRepository.save(mapOfRooms);
        }
    }
}
