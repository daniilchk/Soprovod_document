using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    internal class Program
    {
        static string _filesFolder = ".\\Storage\\";
        static IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        static int port = 8888;
        static TcpListener server = new TcpListener(localAddr, port);
        //static DataBase dataBase = new DataBase();
        static ServerManager _mngr = new ServerManager();
        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    server.Start();
                    Console.WriteLine("Ожидание подключений... ");


                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Подключен клиент. Выполнение запроса...");

                    NetworkStream stream = client.GetStream();

                    List<byte> allBytes = new List<byte>();
                    byte[] data = new byte[256];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    bool first = true;
                    string name = "";
                    int operType = 0;
                    do
                    {
                        if (first)
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            operType = data[0];
                            byte[] onlyName = data.Skip(1).ToArray();
                            name = Encoding.UTF8.GetString(onlyName).Replace("\0", String.Empty);
                            first = false;
                        }
                        else
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            allBytes.AddRange(data);
                        }


                    }
                    while (stream.DataAvailable);
                    int id = 0;
                    switch (operType)
                    {
                        case 1:
                            Console.WriteLine("Получено файл " + name);

                            string relPath = CreateFile(allBytes.ToArray(), name);
                            int fileSize = (int)new System.IO.FileInfo(relPath).Length;
                            //dataBase.add_file(name, fileSize, relPath);
                            break;
                        case 2:
                            id = 0;
                            foreach (char sym in name.ToCharArray())
                            {
                                id += (int)sym;
                            }

                            //_mngr.ClearPath(id);
                            string nameFile2 = _mngr.GetFileName(id);
                            DeleteFile(_filesFolder + nameFile2);
                            Console.WriteLine("Удаление файла "+nameFile2);
                            break;
                        case 3:
                            id = 0;
                            foreach (char sym in name.ToCharArray())
                            {
                                id += (int)sym;
                            }


                            string nameFile = _mngr.GetFileName(id);
                            byte[] nameBuffer = new byte[256];
                            //nameBytes= Encoding.UTF8.GetBytes(_name);
                            for (int i = 0; i < nameBuffer.Length; i++)
                            {
                                nameBuffer[i] = (byte)'\0';
                            }
                            byte[] bytesName = Encoding.UTF8.GetBytes(nameFile);
                            for (int i = 0; i < bytesName.Length; i++)
                            {
                                nameBuffer[i] = bytesName[i];
                            }
                            byte[] fileBytes = GetFileBytes(nameFile);

                            stream.Write(nameBuffer.Concat(fileBytes).ToArray(), 0, fileBytes.Length);

                            Console.WriteLine("Скачивание файла "+nameFile);
                            break;
                    }

                    // закрываем поток
                    stream.Close();
                    // закрываем подключение
                    client.Close();
                }
            }
            catch (Exception error)
            {

            }


            Console.ReadKey();
            //CreateFile(bytes,"ggg.docx");
        }

        static string CreateFile(byte[] bytes, string name)
        {
            File.WriteAllBytes(_filesFolder + name, bytes);
            return _filesFolder + name;
        }

        static byte[] GetFileBytes(string name)
        {
            return File.ReadAllBytes(_filesFolder + name);
        }

        static void DeleteFile(string name)
        {
            if (File.Exists(name))
                File.Delete(name);
        }


    }
}
