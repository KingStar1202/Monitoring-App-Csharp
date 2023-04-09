using Camera.context;
using Camera.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.repository
{
    public class InetGasCodeRepository
    {
        private static InetGasCodeRepository instance;
        private DBContext _dbContext = new DBContext();

        private InetGasCodeRepository()
        {

        }

        public static InetGasCodeRepository getInstance()
        {
            if (instance == null)
            {
                instance = new InetGasCodeRepository();
            }
            return instance;
        }

        public InetGasCode getINetGasCodeForGasCode(string gasCode)
        {
            var item = _dbContext.InetGasCodes.Where(m => m.gasCode == gasCode).FirstOrDefault();
            return item;
        }

        public List<InetGasCode> getAllInetGasCodes()
        {
            var list = _dbContext.InetGasCodes.ToList();
            return list;
        }
    }

}
