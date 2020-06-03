using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serv_kursach3
{
    public class Expert_Talbe
    {
        private int Id;
        private string LastName;
        private string FirstName;

        List<int> marks_exp;

        public Expert_Talbe()
        {
            marks_exp = new List<int>();
        }
        public Expert_Talbe(int id, string lname, string fname, List<int> list)
        {
            Id = id;
            LastName = lname;
            FirstName = fname;
            marks_exp = list;
        }

        public void View()
        {
            Console.WriteLine($"ID: {Id} - Фамилия: {LastName} - Имя: {FirstName} =>");
            foreach (var item in marks_exp)
            {
                Console.Write($"{item} ");
            }
            Console.WriteLine("<=");
        }

        public double Mark_Sum { get { return marks_exp.Sum(); } }

        public List<int> Marks_Exp { get { return marks_exp; } }


    }
}
