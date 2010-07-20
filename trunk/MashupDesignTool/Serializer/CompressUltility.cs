using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Text;

namespace Serializer
{
    public class CompressUltility
    {
        public static byte[] Compress(string str)
        {
            System.IO.IsolatedStorage.IsolatedStorageFile store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
            if (!store.DirectoryExists("Temp"))
                store.CreateDirectory("Temp");
            if (store.FileExists("Temp/temp.tmp"))
                store.DeleteFile("Temp/temp.tmp");
            Stream stream = store.CreateFile("Temp/temp.tmp");

            if (stream == null) 
                return new byte[0];

            byte[] bytes = Encoding.UTF8.GetBytes(str);
            ZipOutputStream zos = new ZipOutputStream(stream);
            zos.PutNextEntry(new ZipEntry("a"));
            zos.Write(bytes, 0, bytes.Length);
            zos.Close();
            stream.Close();

            long count = 0;
            Stream stream1 = store.OpenFile("Temp/temp.tmp", FileMode.Open);
            int size = 0;
            do
            {
                size = stream1.Read(bytes, 0, bytes.Length);
                count += size;
            } while (size > 0);
            stream1.Close();

            byte[] buffer = new byte[count];
            Stream stream2 = store.OpenFile("Temp/temp.tmp", FileMode.Open);
            count = 0;
            do
            {
                size = stream2.Read(bytes, 0, bytes.Length);
                for (int i = 0; i < size; i++)
                {
                    buffer[count] = bytes[i];
                    count++;
                }
            } while (size > 0);
            stream2.Close();
            return buffer;
        }

        public static string Decompress(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            ZipInputStream zis = new ZipInputStream(ms);
            System.IO.IsolatedStorage.IsolatedStorageFile store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
            if (!store.DirectoryExists("Temp"))
                store.CreateDirectory("Temp");
            if (store.FileExists("Temp/temp.tmp"))
                store.DeleteFile("Temp/temp.tmp");
            Stream stream1 = store.CreateFile("Temp/temp.tmp");
            zis.GetNextEntry();
            long count = 0;
            int size;
            do
            {
                size = zis.Read(bytes, 0, bytes.Length);
                stream1.Write(bytes, 0, size);
                count += size;
            } while (size > 0);
            stream1.Close();

            Stream stream2 = store.OpenFile("Temp/temp.tmp", FileMode.Open);
            byte[] buffer = new byte[count];
            count = 0;
            do
            {
                size = stream2.Read(bytes, 0, bytes.Length);
                for (int i = 0; i < size; i++)
                {
                    buffer[count] = bytes[i];
                    count++;
                }
            } while (size > 0);
            stream2.Close();

            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }
    }
}
