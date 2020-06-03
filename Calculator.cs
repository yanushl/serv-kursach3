using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace serv_kursach3
{
    public class Calculator
    {
        public TcpClient client;
        private NetworkStream stream;


        private List<double> competence_experts;///
        private List<double> RijSum;///rj
        private double sum_ex;///

        private List<double> DRj;



        private List<double> Ck;///
        private List<double> Cij_rj_sum;///Ck
        private double sum_op;///

        private List<double> DCj;

        public Calculator(TcpClient client, NetworkStream stream)
        {
            this.client = client;
            this.stream = stream;

            competence_experts = new List<double>();
            RijSum = new List<double>();

            DRj = new List<double>();


            Ck = new List<double>();
            Cij_rj_sum = new List<double>();

            DCj = new List<double>();
        }

        public void Characteristics_Expert(List<Expert_Talbe> list)
        {
            foreach (var item in list)
            {
                double sum = item.Mark_Sum;
                RijSum.Add(sum);
            }
            sum_ex = RijSum.Sum();
            foreach (var item in RijSum)
            {
                double res = item / sum_ex;
                res = Math.Round(res, 3);
                competence_experts.Add(res);
                Console.WriteLine($"Оценка компетентности(rj): {res}");
            }
            Dispersion_Ex(list);
        }


        public void Dispersion_Ex(List<Expert_Talbe> list)
        {
            int i = 0;
            foreach (var item in list)
            {

                double _Rj = item.Mark_Sum / (list.Count - 1);
                int j = 0;
                foreach (var el in item.Marks_Exp)
                {

                    if (i == 0)
                    {
                        double obj = (el + _Rj) * (el + _Rj);
                        DRj.Add(obj);
                    }
                    else
                    {
                        DRj[j] += (el + _Rj) * (el + _Rj);
                    }
                    j++;
                }
                i++;
            }

            MyCounter myCounter = new MyCounter(DRj, list.Count);//тут свой итератор
            foreach (var item in myCounter)
            {
                Console.WriteLine($"Дисперсия эксперта(DRj): {item}");
            }

        }


        public void Characteristics_Optins(List<Option_Table> list)
        {
            List<double> Cij_rj = new List<double>();

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].list.Count; j++)
                {
                    if (i == 0)
                    {
                        Cij_rj_sum.Add(competence_experts[i] * list[i].list[j].mark);
                    }
                    else
                    {
                        Cij_rj_sum[j] += competence_experts[i] * list[i].list[j].mark;
                    }
                }

            }
            sum_op = Cij_rj_sum.Sum();
            foreach (var item in Cij_rj_sum)
            {
                double res = item / sum_op;
                res = Math.Round(res, 3);
                Ck.Add(res);
                Console.WriteLine($"Коэф. предпочтительности(Ck): {res}");
            }
            Dispersion_Op(list);
        }


        public void Dispersion_Op(List<Option_Table> list)
        {
            List<double> _Cj = new List<double>();
            try
            {
                int e = 0;
                foreach (var item in list)
                {
                    int f = 0;
                    foreach (var el in item.list)
                    {

                        if (e == 0)
                        {
                            double obj = (el.mark);
                            _Cj.Add(obj);
                        }
                        else
                        {
                            _Cj[f] += (el.mark);
                        }
                        f++;
                    }
                    e++;
                }

                for (int l = 0; l < _Cj.Count; l++)
                {
                    _Cj[l] = _Cj[l] / (list.Count - 1);
                }

                int i = 0;
                foreach (var item in list)
                {
                    //double _Cj = item.Sum_Mark / (item.list.Count - 1);
                    int j = 0;
                    foreach (var el in item.list)
                    {

                        if (i == 0)
                        {
                            double obj = (el.mark + _Cj[j]) * (el.mark + _Cj[j]);
                            DCj.Add(obj);
                        }
                        else
                        {
                            DCj[j] += (el.mark + _Cj[j]) * (el.mark + _Cj[j]);
                        }
                        j++;
                    }
                    i++;
                }



                MyCounter myCounter = new MyCounter(DCj, list.Count);///////тут свой итератор
                foreach (var item in myCounter)
                {
                    Console.WriteLine($"Дисперсия варианта(DCj): {item}");
                }


                //for (i = 0; i < DCj.Count; i++)
                //{
                //    DCj[i] = Math.Round(DCj[i], 3);
                //    if (list.Count - 2 != 0)
                //    {
                //        DCj[i] /= (list.Count - 2);
                //    }

                //    Console.WriteLine($"Дисперсия варианта(DCj): {DCj[i]}");
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!!{ex.Message}");
            }

        }





        public void Send_Report(List<Option_Table> list)
        {
            if (stream != null || client != null)
            {

                byte[] buffer = new byte[256];

                var iter = competence_experts.GetEnumerator();
                foreach (var item in competence_experts)
                {
                    buffer = Encoding.Unicode.GetBytes(item.ToString() + "\n");
                    stream.Write(buffer, 0, buffer.Length);
                }
                buffer = Encoding.Unicode.GetBytes("@");
                stream.Write(buffer, 0, buffer.Length);
                foreach (var item in DRj)
                {
                    buffer = Encoding.Unicode.GetBytes(item.ToString() + "\n");
                    stream.Write(buffer, 0, buffer.Length);
                }
                buffer = Encoding.Unicode.GetBytes("@");
                stream.Write(buffer, 0, buffer.Length);
                foreach (var item in Ck)
                {
                    buffer = Encoding.Unicode.GetBytes(item.ToString() + "\n");
                    stream.Write(buffer, 0, buffer.Length);
                }
                buffer = Encoding.Unicode.GetBytes("@");
                stream.Write(buffer, 0, buffer.Length);
                foreach (var item in DCj)
                {
                    buffer = Encoding.Unicode.GetBytes(item.ToString() + "\n");
                    stream.Write(buffer, 0, buffer.Length);
                }
                buffer = Encoding.Unicode.GetBytes("@");
                stream.Write(buffer, 0, buffer.Length);
                buffer = Encoding.Unicode.GetBytes(Report(list).ToString());
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        public string Report(List<Option_Table> list)
        {
            string str = " "; int i = 0, count = 0, j = 0;
            for (i = 0; i < Ck.Count; i++)
            {
                if (Ck.Max() == Ck[i])
                {
                    j = i;
                    count++;
                    //break;
                }
            }           
            if (count == 1)
            {
                str = $"Система пришла к выводу, что на наилучшим вариантом системы является №{ j + 1}: { list[0].list[j].Rep}.";
            }
            else
            {
                str = $"Система пришла к выводу, что существует несколько наиболее предпочтительных вариантов системы:";
                for (i = 0; i < Ck.Count; i++)
                {
                    if (Ck.Max() == Ck[i])
                    {
                        str += $"\n№{ i + 1}: {list[0].list[i].Rep}";
                    }
                }
            }
            return str;
        }

    }
}
