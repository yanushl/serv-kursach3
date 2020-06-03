using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace serv_kursach3
{
    public class MyCounter
    {
        List<double> ts;
        int count;
        public MyCounter(List<double> ts, int count)
        {
            this.ts = ts;
            this.count = count;
        }

        public System.Collections.Generic.IEnumerator<double> GetEnumerator()
        {
            for (int i = 0; i < ts.Count; i++)
            {             
                if (count - 2 != 0)
                {
                    ts[i] /= (count - 2);               
                }
                ts[i] = Math.Round(ts[i], 3);
                yield return ts[i];
            }
        }

    }
}
