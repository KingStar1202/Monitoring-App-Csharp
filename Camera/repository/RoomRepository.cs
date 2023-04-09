using Camera.context;
using Camera.models;
using Camera.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringApp.repository
{
    public class RoomRepository
    {
        private static RoomRepository instance;
        public DBContext _dbContext = new DBContext();
        private RoomRepository()
        {

        }
        public static RoomRepository getInstance()
        {
            if (instance == null)
            {
                instance = new RoomRepository();
            }
            return instance;
        }

        public List<Room> Get()
        {
            CameraUtils cameraUtils = new CameraUtils();
            var rooms = _dbContext.Rooms.OrderBy(m => m.x).OrderBy(m => m.y).ToList();
            foreach (var room in rooms)
            {
                room.insideCameraType = cameraUtils.getCameraType(room.insideCamera);
                room.outsideCameraType = cameraUtils.getCameraType(room.outsideCamera);
            }
            return rooms;
        }

        public int getMaxColumnForGrid()
        {
            int result = _dbContext.Rooms.Max(m => m.x);
            return result;
        }

        public int getMaxColumnsForRow(int row)
        {
            var list = _dbContext.Rooms.Where(m => m.y == row).ToList();
            return list.Count;
        }

        public int getMaxRowsForGrid()
        {
            int result = _dbContext.Rooms.Max(m => m.y);
            return result;
        }

        public Room getRoomWithName(string roomName)
        {
            return _dbContext.Rooms.Where(m => m.name == roomName).FirstOrDefault();
        }

        public bool roomWithNameExists(string roomName)
        {
            var rooms = _dbContext.Rooms.Where(m => m.name == roomName);
            bool roomExists = rooms.Count() == 1 ? true : false;
            return roomExists;
        }
        public void SaveRooms(List<Room> rooms)
        {

            try
            {
                foreach (var room in rooms)
                {
                    Save(room);
                }

            }
            catch (Exception ex)
            {

            }
        }

        public void Save(Room room)
        {


            try
            {
                if (room.id == 0)
                {
                    _dbContext.Rooms.Add(room);
                    _dbContext.SaveChanges();
                    _dbContext.Entry(room).State = EntityState.Detached;

                }
                else
                {
                    _dbContext.Entry(room).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    _dbContext.Entry(room).State = EntityState.Detached;

                }


            }
            catch (Exception ex)
            {

            }

        }

        public void Delete(Room room)
        {
            if (room.id == null)
            {
                return;
            }

            // first delete all occurrences of the id in other tables to satisfy foreign key constraints
            int roomId = room.id;
            try
            {
                var gasSensorReadings = _dbContext.GasSensorReadings.Where(mbox => mbox.room.id == roomId).DeleteFromQuery();
                
                
                //_dbContext.GasSensorReadings.RemoveRange(gasSensorReadings);
        
                var item = _dbContext.Rooms.FirstOrDefault(mbox => mbox.id == roomId);
                item.ppeIcons.Clear();
                _dbContext.Rooms.Remove(item);
                _dbContext.SaveChanges();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
