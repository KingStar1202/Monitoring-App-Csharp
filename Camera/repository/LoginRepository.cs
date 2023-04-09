using Camera.context;
using Camera.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.repository
{
    public class LoginRepository
    {
        private static LoginRepository instance;
        public DBContext _dbContext = new DBContext();
        private LoginRepository()
        {

        }
        public static LoginRepository getInstance()
        {
            if (instance == null)
            {
                instance = new LoginRepository();
            }
            return instance;
        }

        public bool isUserAndPasswordCorrect(String username, String password)
        {
            try
            {
                User user = _dbContext.Users.SingleOrDefault(m => m.username == username && m.password == password);
                if (user == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string getUserRole(string username)
        {
            User user = _dbContext.Users.SingleOrDefault(m => m.username == username);
            return user.role;
        }

        public void saveLogin(string username)
        {
            Camera.models.Login login = new Camera.models.Login
            {
                username = username,
                createAt = DateTime.Now,
            };
            _dbContext.Logins.Add(login);
            _dbContext.SaveChanges();
        }
    }
}
