using Camera.context;
using Camera.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.repository
{
    public class GasSensorReadingRepository
    {
        private static GasSensorReadingRepository instance;
        private DBContext _dbContext = new DBContext();
        private GasSensorReadingRepository()
        {

        }

        public static GasSensorReadingRepository getInstance()
        {
            if (instance == null)
            {
                instance = new GasSensorReadingRepository();
            }
            return instance;
        }

        public void save(GasSensorReading gasSensorReading)
        {
            try
            {
                _dbContext.GasSensorReadings.Add(gasSensorReading);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void writeValuesToDb(Room room, string type, string state, double value, string measurementType)
        {
            //GasSensorReading gasSensorReading = new GasSensorReading{
            //    room = room,
            //    type = type,
            //    value = value,
            //    state = state,
            //    measurementType = measurementType,
            //    createdAt = new DateTime()
            //};
            //_dbContext.GasSensorReadings.Add(gasSensorReading);
            //_dbContext.SaveChanges();
        }
    }
}
