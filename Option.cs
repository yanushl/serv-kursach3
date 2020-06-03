using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serv_kursach3
{
    public class Option
    {
        public int Id;
        private string option;
        private string summary;
        public double mark;

        public Option(int id, string op, string summary, int mark)
        {
            Id = id;
            option = op;
            this.summary = summary;
            this.mark = mark;
        }

        public void View()
        {
            Console.WriteLine($"ID варинта: {Id} - Вариант: {option} - Описание: {summary} - Оценка: {mark}");
        }

        public string Rep { get { return option; } }

    }
}
