using Camera.context;
using Camera.models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.repository
{
    public class GasDetectionRepository
    {
        private static GasDetectionRepository instance;
        public DBContext _dbContext = new DBContext();
        private GasDetectionRepository()
        {

        }
        public static GasDetectionRepository getInstance()
        {
            if (instance == null)
            {
                instance = new GasDetectionRepository();
            }
            return instance;
        }
        public List<GasDetection> GetGasDetections()
        {
            var list = _dbContext.GasDetections.ToList();
            return list;
        }

        public void SaveGasDetections(List<GasDetection> gasDetections)
        {
            foreach (var gasDetection in gasDetections)
            {
                Save(gasDetection);
            }

        }

        public void Save(GasDetection gasDetection)
        {
            try
            {
                if (gasDetection.id == 0)
                {
                    _dbContext.GasDetections.Add(gasDetection);
                    _dbContext.SaveChanges();
                    _dbContext.Entry(gasDetection).State = EntityState.Detached;
                }
                else
                {
                    _dbContext.GasDetections.AddOrUpdate(gasDetection);
                    _dbContext.SaveChanges();
                    _dbContext.Entry(gasDetection).State = EntityState.Detached;
                }


            }
            catch (Exception ex)
            {

            }
        }

        public void Delete(GasDetection gasDetection)
        {
            try
            {
               if(gasDetection == null)
                {
                    return;
                }
                else
                {
                    _dbContext.GasDetections.Remove(gasDetection);
                    _dbContext.SaveChanges();
                }


            }
            catch (Exception ex)
            {

            }
        }

    }
}
