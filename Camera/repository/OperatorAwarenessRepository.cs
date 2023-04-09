using Camera.context;
using Camera.models;
using MonitoringApp.repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.repository
{
    public class OperatorAwarenessRepository
    {
        private static OperatorAwarenessRepository instance;
        private DBContext _dbContext = new DBContext();
        private OperatorAwarenessRepository()
        {

        }
        public static OperatorAwarenessRepository getInstance()
        {
            if (instance == null)
            {
                instance = new OperatorAwarenessRepository();
            }
            return instance;
        }
        public void SaveLogin(string username, long timeTaken)
        {
            OperatorAwareness operatorAwareness = new OperatorAwareness();
            operatorAwareness.user = username;
            operatorAwareness.timeTaken = timeTaken;
            _dbContext.OperatorAwarenesses.Add(operatorAwareness);
            _dbContext.SaveChanges();
            
        }

        public List<OperatorAwareness> Get()
        { 
           return _dbContext.OperatorAwarenesses.ToList();
        }


        public void Delete()
        {
            var list = _dbContext.OperatorAwarenesses.ToList();
            _dbContext.OperatorAwarenesses.RemoveRange(list);
            _dbContext.SaveChanges();
        }

    }
}
