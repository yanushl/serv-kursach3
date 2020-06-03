using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace serv_kursach3
{
    public abstract class base1
    {
        virtual  public List<user_admin> From_File() { return null; }
    }
    public abstract class base2:base1
    {
         virtual  public void To_File(List<user_admin> users) { return; }
    }
    public abstract class base3:base2
    {
        virtual public void DelFile() { return; }
    }
    public abstract class base4:base3
    {
        protected string path;
    }

    public class File_Work:base4
    {
        //private string path;

        public File_Work(string way = "users.txt")
        {
            path = way;
        }


        public  override List<user_admin> From_File()
        {
            List<user_admin> users = new List<user_admin>();
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        users.Add(new user_admin(reader.ReadLine(), reader.ReadLine()));
                    }
                }

                return users;
            }
            else
            {
                return users;
            }
        }

        public override void To_File(List<user_admin> users)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (var item in users)
                {
                    writer.WriteLine(item.Get_Login);
                    writer.WriteLine(item.Get_Password);
                }
            }
        }

        public override void DelFile()
        {
            File.Delete(path);
        }
    }
}
