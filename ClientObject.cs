using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace serv_kursach3
{
    public class ClientObject
    {
        public TcpClient client;
        private NetworkStream stream;

        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
            stream = null;
        }


        private bool Entering(string login, string password, bool adm_or_us)
        {
            List<user_admin> users_adm/* = new List<user_admin>()*/;
            if (adm_or_us)
            {
                File_Work file_Work = new File_Work("adm.txt");
                users_adm = file_Work.From_File();
            }
            else
            {
                File_Work file_Work = new File_Work();
                users_adm = file_Work.From_File();
            }
            foreach (var item in users_adm)
            {
                if (item.Get_Login == login && item.Get_Password == password)
                {
                    users_adm.Clear();
                    return true;
                }
            }
            users_adm.Clear();
            return false;
        }

        private bool Authorization()
        {
            bool b = false, adm_or_us = false;
            string login, password, adm_or_us_str, str;

            while (!b)
            {
                try
                {
                    stream = client.GetStream();
                    byte[] buffer = new byte[100];/// буфер для получаемых данных

                    StringBuilder builder = new StringBuilder();/// получаем сообщение
                    int bytes = 0;
                    bytes = stream.Read(buffer, 0, buffer.Length);
                    builder.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
                    str = builder.ToString();
                    login = str.Substring(0, str.IndexOf('\n'));
                    str = str.Remove(0, str.IndexOf('\n') + 1);
                    Console.WriteLine(login);
                    password = str.Substring(0, str.IndexOf('\n'));
                    str = str.Remove(0, str.IndexOf('\n') + 1);
                    Console.WriteLine(password);
                    adm_or_us_str = str;

                    if (adm_or_us_str == "true" || adm_or_us_str == "True")
                    {
                        adm_or_us = true;
                    }
                    else
                    {
                        adm_or_us = false;
                    }

                    b = Entering(login, password, adm_or_us);
                    if (!b)
                    {
                        Console.WriteLine("Клиент не прошел авторизацию");
                    }

                    buffer = Encoding.UTF8.GetBytes(b.ToString());
                    stream.Write(buffer, 0, buffer.Length);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    b = true;
                    throw;
                }
            }
            return adm_or_us;
        }


        public void Process()
        {
            int switch_on = 0;
            bool b = true;
            try
            {
                Console.WriteLine("Клиент подключен");

                try
                {
                    Authorization();
                }
                catch (Exception ex)///TODO:  чтобы гасился поток
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
                Console.WriteLine("Клиент прошел авторизацию");

                while (b)
                {
                    try
                    {
                        byte[] buffer = new byte[1];/// буфер для получаемых данных

                        StringBuilder builder = new StringBuilder();/// получаем сообщение
                        int bytes = 0;
                        bytes = stream.Read(buffer, 0, buffer.Length);
                        builder.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
                        string msg_code = builder.ToString();
                        switch_on = Convert.ToInt32(msg_code);
                        Console.WriteLine($"Клиент выбрал: [ {(Enum)switch_on} ]");
                        Task.Delay(10).Wait();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message); throw;
                    }

                    switch ((Enum)switch_on)
                    {
                        case Enum.accept_users:
                            Work_with_users();
                            break;
                        case Enum.accept_file_save:

                            break;
                        case Enum.accept_file_download:
                            break;
                        case Enum.accept_data_to_calculate:
                            Calculate();
                            break;
                        case Enum.exit:
                            b = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                stream.Dispose();             
                client.Dispose();
            }


        }

        private void Work_with_users()
        {
            try
            {
                byte[] buffer = new byte[256];
                List<user_admin> users /*= new List<user_admin>()*/;
                File_Work file_Work = new File_Work();

                users = file_Work.From_File();
                if (users.Count == 0)
                {
                    buffer = Encoding.UTF8.GetBytes("NULL");
                    stream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    foreach (var item in users)
                    {
                        buffer = Encoding.UTF8.GetBytes(item.NetWork_Send);
                        stream.Write(buffer, 0, buffer.Length);///шлем данные о пользователях
                    }
                }
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                byte[] buf = new byte[1];
                bytes = stream.Read(buf, 0, buf.Length);
                builder.Append(Encoding.UTF8.GetString(buf, 0, bytes));
                string msg_code = builder.ToString();
                if (Convert.ToInt32(msg_code) == 1)
                {                
                    users.Clear();
                    Console.WriteLine("Админ изменил список пользователей ->");
                    string log, pass, str = "";
                    do
                    {
                        builder.Clear();
                        bytes = 0;
                        bytes = stream.Read(buffer, 0, buffer.Length);
                        builder.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
                        str += builder.ToString();
                        builder.Clear();
                    } while (stream.DataAvailable);
                    if (str == "NULL")
                    {
                        Console.WriteLine("Пользователи удалены");
                        users.Clear();
                        file_Work.DelFile();
                    }
                    else
                    {
                        string[] str_arr = str.Split('\n');
                        for (int i = 0, j = 1; i < str_arr.Length - 1; i++, j++)
                        {
                            log = str_arr[i];
                            i++;
                            pass = str_arr[i];
                            users.Add(new user_admin(log, pass));
                            Console.WriteLine($"Ползователь {j}: {log} - {pass}");
                        }
                        file_Work.To_File(users);
                    }
                }
                else
                {
                    Console.WriteLine("Админ не менял данных");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        private void Calculate()
        {
            List<Expert_Talbe> talbe_ex = new List<Expert_Talbe>();
            List<Option_Table> table_op = new List<Option_Table>();
            List<Option> list_op = new List<Option>();
            try
            {
                Console.WriteLine("Пришли данные для расчета ->");
                byte[] buffer = new byte[256];
                string str = "";

                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = 0;
                    bytes = stream.Read(buffer, 0, buffer.Length);
                    builder.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                    str += builder.ToString();
                    builder.Clear();
                } while (stream.DataAvailable);

                string[] str_arr_all = str.Split('@');
                string[] str_arr_ex = null;
                string[] str_arr_op = null;

                for (int i = 0; i < str_arr_all.Length; i++)
                {
                    str_arr_ex = str_arr_all[i].Split('\t');
                    i++;
                    str_arr_op = str_arr_all[i].Split('\t');
                    break;
                }
                if (str_arr_ex[str_arr_ex.Length - 1] == "")
                {

                }

                foreach (var item in str_arr_ex)
                {
                    try
                    {
                        int id; string lname, fname;
                        List<int> list = new List<int>();
                        string[] slocal = item.Split('\n');
                        int i = 0;
                        id = Convert.ToInt32(slocal[i]); i++;
                        fname = slocal[i]; i++;
                        lname = slocal[i]; i++;
                        for (int j = i; j < slocal.Length; j++)
                        {
                            list.Add(Convert.ToInt32(slocal[j]));
                        }
                        talbe_ex.Add(new Expert_Talbe(id, lname, fname, list));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   !!! - {ex.Message}");
                    }
                }
                foreach (var item in talbe_ex)
                {
                    item.View();
                }

                foreach (var item in str_arr_op)
                {
                    try
                    {
                        int id_ex; string lname, fname;
                        list_op = new List<Option>();
                        string[] slocal = item.Split('\n');
                        int i = 0;
                        id_ex = Convert.ToInt32(slocal[i]); i++;
                        fname = slocal[i]; i++;
                        lname = slocal[i]; i++;
                        for (int j = i; j < slocal.Length; j++)
                        {
                            int id_op = Convert.ToInt32(slocal[j]); j++;
                            string op = slocal[j]; j++;
                            string summary = slocal[j]; j++;
                            int mark = Convert.ToInt32(slocal[j]);
                            list_op.Add(new Option(id_op, op, summary, mark));
                        }
                        table_op.Add(new Option_Table(id_ex, lname, fname, list_op));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                foreach (var item in table_op)
                {
                    item.View();
                }
                Console.WriteLine("<-");
            }
            catch (Exception ex)
            {
                if (client == null || stream == null)
                {
                    throw;
                }
                else
                {
                    Console.WriteLine(ex.Message);///TODO error_msg_to_client
                }

            }

            Calculator calculator = new Calculator(client, stream);
            calculator.Characteristics_Expert(talbe_ex);
            calculator.Characteristics_Optins(table_op);
            calculator.Send_Report(table_op);

        }

    }
}
