using Camera.context;
using Camera.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;

namespace Camera.repository
{
    public class PpeIconRepository
    {
        private static PpeIconRepository instance;
        public DBContext _dbContext = new DBContext();
        private PpeIconRepository() { }
        public static PpeIconRepository getInstance()
        {
            if (instance == null)
            {
                instance = new PpeIconRepository();
            }
            return instance;
        }

        public List<PpeIcon> Get()
        {
            var list = _dbContext.PpeIcons.OrderBy(e=>e.id).ToList();
            return list;
        }
    }
}
