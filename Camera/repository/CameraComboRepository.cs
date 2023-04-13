using Camera.context;
using Camera.models;
using Camera.repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.repository
{
    public class CameraComboRepository
    {
        private static CameraComboRepository instance;
        public DBContext _dbContext = new DBContext();
        private CameraComboRepository()
        {

        }
        public static CameraComboRepository getInstance()
        {
            if (instance == null)
            {
                instance = new CameraComboRepository();
            }
            return instance;
        }
        public List<CameraCombo> getConfiguredCamreaCombos()
        {
            var list = _dbContext.CameraCombos.ToList();
            return list;
        }

        public void SaveCombos(List<CameraCombo> cameraCombos)
        {
            foreach (var cameraCombo in cameraCombos)
            {
                Save(cameraCombo);
            }
            
        }

        public void Save(CameraCombo cameraCombo)
        {
            try
            {
                if (cameraCombo.id == 0)
                {
                    if(cameraCombo.name == "<unset>")
                    {
                        return;
                    }
                    _dbContext.CameraCombos.Add(cameraCombo);
                    _dbContext.SaveChanges();
                }
                else
                {
                    _dbContext.CameraCombos.AddOrUpdate(cameraCombo);
                    _dbContext.Entry(cameraCombo).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                }


            }
            catch (Exception ex)
            {

            }
        }
    }
}
