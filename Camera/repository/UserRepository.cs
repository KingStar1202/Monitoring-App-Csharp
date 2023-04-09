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
    public class UserRepository
    {
        private static UserRepository instance;
        public DBContext _dbContext = new DBContext();
        private UserRepository()
        {

        }
        public static UserRepository getInstance()
        {
            if (instance == null)
            {
                instance = new UserRepository();
            }
            return instance;
        }
        public List<User> getUsers()
        {
            var list = _dbContext.Users.ToList();
            return list;
        }


        public void saveUsers(List<User> users)
        {
            foreach (var user in users)
            {
                save(user);
            }
        }

        public void save(User user)
        {
            try
            {
                if (user.id == 0)
                {
                    
                     _dbContext.Users.Add(user);
                     _dbContext.SaveChanges();
                    _dbContext.Entry(user).State = EntityState.Detached;
                    
                }
                else
                {

                    _dbContext.Users.AddOrUpdate(user);
                    _dbContext.SaveChanges();
                    _dbContext.Entry(user).State = EntityState.Detached;

                }


            }
            catch (Exception ex)
            {

            }
        }

        public void delete(User user)
        {
            try
            {
               
                _dbContext.Users.Remove(user);
                 _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
