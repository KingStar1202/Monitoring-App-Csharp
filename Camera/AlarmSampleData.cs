using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera
{
    public class AlarmSampleData
    {
        public AlarmSampleData()
        {
            Alarm = new ObservableCollection<Alarm>();
            Alarm.Add(new Alarm(1,"cam","cam1", "done"));
            Alarm.Add(new Alarm(1, "cam", "cam1", "done"));
            Alarm.Add(new Alarm(1, "cam", "cam1", "done"));
            Alarm.Add(new Alarm(1, "cam", "cam1", "done"));
            Alarm.Add(new Alarm(1, "cam", "cam1", "done"));
            Alarm.Add(new Alarm(1, "cam", "cam1", "done"));
            Alarm.Add(new Alarm(1, "cam", "cam1", "done"));
        }

        public ObservableCollection<Alarm> Alarm
        {
            get;
            set;
        }
    }
}
