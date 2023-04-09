using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Camera.models;
using MySql.Data.EntityFramework;
using SharpDX;

namespace Camera.context
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class DBContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Camera.models.Login> Logins { get; set; }
        public DbSet<GasDetection> GasDetections { get; set; }
        public DbSet<AlarmSetup> AlarmSetups { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<GasSensorReading> GasSensorReadings { get; set; }
        public DbSet<CameraCombo> CameraCombos { get; set; }
        public DbSet<PpeIcon> PpeIcons { get; set; }
        public DbSet<InetGasCode> InetGasCodes { get; set; }
        public DbSet<OperatorAwareness> OperatorAwarenesses { get; set; }
        public DbSet<Net2RoomData> Net2RoomDatas { get; set; }
        //public DbSet<PpeIconRoom> PpeIconRooms { get; set; }
        public DBContext() : base("DefaultConnection")
        {

        }
    }
}
