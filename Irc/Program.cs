using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Irc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //first wee se if we have all data we want.
            if (!Directory.Exists("Script/Database/Server.txt"))
                CreateServerDatabase();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void CreateServerDatabase()
        {
            StreamWriter writer;
            if (!Directory.Exists("Db"))
            {
                Directory.CreateDirectory("Db");
            }

            if (!File.Exists("script.txt"))
                File.Create("script.txt").Close();

            if (!Directory.Exists("Script"))
            {
                Directory.CreateDirectory("Script");
            }

            if (!File.Exists("Script/benencode.txt"))
            {
                writer = File.CreateText("Script/benencode.txt");
                writer.Write(@"global type = include('System.Type');
global count = include('System.Array.Count');
                global strlen = include('System.String.Length');
                global alert = include('System.Alert');

                function benencode(data)
                {
                    t = type(data);
                    if (t == 'namedArray')
                    {
                        d = 'd';
                        foreach (data as key : value){
                    d = d + benencode(key) + benencode(value);
                }
                return d + 'e';
            }
            elseif(t == 'array'){
                l = 'l';
                for (i = 0; i < count(data); i++)
                {
                    l = l + benencode(data[i]);
                }
                return l + 'e';
            }
            elseif(t == 'number'){
                return 'i' + data + 'e';
            }
            elseif(t == 'string'){
                return strlen(data) + ':' + data;
            }else{
                alert('Unknown type detected in benencode: ' + t);
            }
        }

return benencode;");
                writer.Close();
            }

            if (!File.Exists("Script/bendecode.txt"))
            {
                writer = File.CreateText("Script/bendecode.txt");
                writer.Write(@"global toCharArray = include('System.String.CharArray');
global alert = include('System.Alert');
                global is_numeric = include('System.Type.Numric');
                global toNumber = include('System.Type.ToNumber');

                function current(data)
                {
                    return data['str'][data['pointer']];
                }

                function next(data)
                {
                    data['pointer']++;
                }

                function rawdecode(data)
                {
                    if (current(data) == 'l')
                    {
                        next(data);
                        l = [];
                        while (current(data) != 'e')
                        {
                            l[] = rawdecode(data);
                        }
                        next(data);
                        return l;
                    }
                    elseif(current(data) == 'd'){
                        next(data);
                        d = { };
                        while (current(data) != 'e')
                        {
                            d[rawdecode(data)] = rawdecode(data);
                        }
                        next(data);
                        return d;
                    }
                    elseif(is_numeric(current(data))){
                        num = current(data);
                        next(data);
                        while (current(data) != ':')
                        {
                            num = num + current(data);
                            next(data);
                        }
                        next(data);
                        str = '';
                        num = toNumber(num);
                        for (i = 0; i < num; i++)
                        {
                            str = str + current(data);
                            next(data);
                        }
                        return str;
                    }
                    elseif(current(data) == 'i'){
                        next(data);
                        num = '';
                        while (current(data) != 'e')
                        {
                            num = num + current(data);
                            next(data);
                        }
                        next(data);
                        return toNumber(num);
                    }
                    alert('Unknown prefix detected in bendecode: ' + current(data));
                }

                function bendecode(string)
                {
                    return rawdecode({ 'str' : toCharArray(string), 'pointer' : 0});
        }

return bendecode;");
                writer.Close();
            }

            if (!Directory.Exists("Script/Database"))
            {
                Directory.CreateDirectory("Script/Database");
            }

            if (!File.Exists("Script/Database/Server.txt"))
            {
                writer = File.CreateText("Script/Database/Server.txt");
                writer.Write(@"bendecode = include('bendecode.txt');
global benencode = include('benencode.txt');

                file_exists = include('System.IO.File.Exists');

                global array_count = include('System.Array.Count');

                global db_name = 'Db/Server.ben';

                if (!file_exists(db_name))
                {
                    file_create = include('System.IO.File.Create');
                    file_create(db_name);
                }

                get_file_contents = include('System.IO.File.GetContents');
                global put_file_contents = include('System.IO.File.PutContents');
                file = get_file_contents(db_name);
                global data = null;
                if (file)
                    data = bendecode(file);
                else
                    data = [];


                function getListCount()
                {
                    return array_count(data);
                }

                function saveServer(identify, host, port, nick, channels)
                {
                    data[] = {
     'identify' : identify,
     'host'     : host,
     'port'     : port,
     'nick'     : nick,
     'channels' : channels
   };
            put_file_contents(db_name, benencode(data));
        }

        function getServer(i)
        {
            return data[i];
        }

        function in_array(array, expect)
        {
            for (i = 0; i < array_count(array); i++)
            {
                if (array[i] == expect)
                    return true;
            }
            return false;
        }

        function saveChannel(identify, channel)
        {
            for (i = 0; i < array_count(data); i++)
            {
                if (data[i]['identify'] == identify)
                {
                    if (!in_array(data[i]['channels'], channel))
                    {
                        data[i]['channels'][] = channel;
                    }
                }
            }

            put_file_contents(db_name, benencode(data));
        }

        function deleteServer(identify)
        {
            for (i = 0; i < array_count(data); i++)
            {
                if (data[i]['identify'] == identify)
                {
                    delete data[i];
                    put_file_contents(db_name, benencode(data));
                    return;
                }
            }
        }

        function deleteChannel(identify, channel)
        {
            for (i = 0; i < array_count(data); i++)
            {
                if (data[i]['identify'] == identify)
                {
                    for (c = 0; c < array_count(data[i]['channels']); c++)
                    {
                        if (data[i]['channels'][c] == channel)
                        {
                            delete data[i]['channels'] [c];
          put_file_contents(db_name, benencode(data));
          return;
        }
}
      return;
    }
  }
}");
                writer.Close();
            }
        }
    }
}
