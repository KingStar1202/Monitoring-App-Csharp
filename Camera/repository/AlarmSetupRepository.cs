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
    public class AlarmSetupRepository
    {

        private static AlarmSetupRepository instance;
        public DBContext _dbContext = new DBContext();
        public static AlarmSetupRepository getInstance()
        {
            if (instance == null)
            {
                instance = new AlarmSetupRepository();
            }
            return instance;
        }
        public List<AlarmSetup> Get()
        {
            return _dbContext.AlarmSetups.ToList();
        }
        public AlarmSetup Find(int id) { 
            return _dbContext.AlarmSetups.Find(id);
        }

        public void deleteAlarmSetup(AlarmSetup alarmSetup)
        {
            try
            {
                _dbContext.Entry(alarmSetup).State = EntityState.Deleted;
                _dbContext.AlarmSetups.Remove(alarmSetup);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void saveAlarmSetup(AlarmSetup alarmSetup)
        {
            try
            {
                if (alarmSetup.id == 0)
                {
                    _dbContext.AlarmSetups.Add(alarmSetup);
                    _dbContext.SaveChanges();
                    _dbContext.Entry(alarmSetup).State = EntityState.Detached;
                }
                else
                {
                    _dbContext.AlarmSetups.AddOrUpdate(alarmSetup);
                    _dbContext.SaveChanges();
                    _dbContext.Entry(alarmSetup).State = EntityState.Detached;
                }


            }
            catch (Exception ex)
            {

            }
        }



        public void saveAlarmSetups(List<AlarmSetup> alarmSetups)
        {
            foreach (var alarmsetup in alarmSetups)
            {
                saveAlarmSetup(alarmsetup);
            }
        }

    }

}
