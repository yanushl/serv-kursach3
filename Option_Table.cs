using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serv_kursach3
{
    public class Option_Table
    {
        private int Id;
        private string LastName;
        private string FirstName;

        public List<Option> list;


        public Option_Table()
        {
            list = new List<Option>();
        }

        public Option_Table(int id, string lname, string fname, List<Option> list_other)
        {
            Id = id;
            LastName = lname;
            FirstName = fname;
            list = list_other;
        }

        public void View()
        {
            Console.WriteLine($"ID: {Id} - Фамилия: {LastName} - Имя: {FirstName} =>");
            foreach (var item in list)
            {
                item.View();
            }
            Console.WriteLine("<=");
        }


        public double Sum_Mark
        {
            get
            {
                double s = 0;
                foreach (var item in list)
                {
                    s += item.mark;
                }
                return s;
            }
        }

        
    }
}
