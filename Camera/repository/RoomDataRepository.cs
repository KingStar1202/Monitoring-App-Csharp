using Camera.context;
using Camera.models;
using Camera.net2access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.repository
{
    public class RoomDataRepository
    {
        private static RoomDataRepository instance;
        private DBContext _dbContext = new DBContext();
        public RoomDataRepository()
        {

        }
        public static RoomDataRepository getInstance()
        {
            if (instance == null)
            {
                instance = new RoomDataRepository();
            }
            return instance;
        }
        public  void save(Dictionary<string, List<Net2RoomData>> mapOfRooms)
        {
            try
            {
                string query = "TRUNCATE TABLE room_data";
                _dbContext.Database.ExecuteSqlCommand(query);
                var list = _dbContext.Net2RoomDatas.ToList();
                _dbContext.Net2RoomDatas.RemoveRange(list);
                foreach (var entry in mapOfRooms)
                {
                    foreach (var room in entry.Value)
                    {
                        _dbContext.Net2RoomDatas.Add(room);
                    }
                }
                _dbContext.SaveChanges();
            }
            catch(Exception e)
            {

            }
            
        }

        public int getAmountOfPeopleInRoomWithName(string roomName)
        {
            int result = 0;
            var countQry = _dbContext.Net2RoomDatas.Where(m => m.roomName == roomName).ToList();
            if (countQry != null)
            {
                result = countQry.Count;
            }
            return result;

        }
        public List<Attendee> getListOfAttendeesForRoom(string roomName)
        {
            var list = _dbContext.Net2RoomDatas.Where(m => m.roomName == roomName).ToList();
            List<Attendee> attendeeList = new List<Attendee>();
            foreach (Net2RoomData item in list)
            {
                attendeeList.Add(new Attendee
                {
                    name = item.person,
                    checkinTime = item.lastLogin
                });
            }

            return attendeeList;
        }
    }


}
