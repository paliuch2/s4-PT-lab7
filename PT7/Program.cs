// KAMIL PALUSZEWSKI 180194

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PT7
{
    static class Program
    {
        static void Main(string[] args)
        {
            string sciezka;
            if (args.Length > 0)
            {
                sciezka = args[0];
            }
            else
            {
                Console.WriteLine("BLAD: Nie podano sciezki");
                return;
            }

            // do zadania 1. -> wypisanie zawartości katalogu bez rekurencji
            int state = NonRecursivePrint(sciezka);
            Console.WriteLine();
  
            if (state != 0) // nie znaleziono sciezki - zamykanie programu.
            {
                return;
            }

            // do zadania 2. oraz 4. -> wypisanie zawartości rekurencyjnie, z wcieciami, uwzglednione takze rozmiar oraz atrybuty 'rahs'
            RecursivePrint(sciezka);
            Console.WriteLine();
    
            //do zadania 3. -> rekurencyjnego wyszukania najstarszego pliku
            DirectoryInfo info = (new DirectoryInfo(sciezka));
            DateTime najstarszy = Oldest(info);
            Console.WriteLine("Najstarszy plik: " + najstarszy.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine();

            //do zadania 5. -> zaladowanie wszystkich bezpośrednich elementów katalogu do kolekcji
            // funkcja LoadToCollection wywoluje wewnatrz siebie funkcje serializacji i deserializacji.
            LoadToCollection(sciezka);
        }

        private static int NonRecursivePrint(string sciezka)
        {     
            Console.WriteLine("NIEREKURENCYJNE WYPISANIE ZAWARTOSCI ");               
            
            if (Directory.Exists(sciezka))
            {                        
                string[] katalogi = Directory.GetDirectories(sciezka);

                foreach (var k in katalogi)
                {
                    string[] czesciSciezki = k.Split("\\");
                    string nazwaKatalogu = czesciSciezki[czesciSciezki.Length - 1];
                    Console.WriteLine(nazwaKatalogu);
                }
                string[] pliki = Directory.GetFiles(sciezka);
                foreach (var p in pliki)
                {
                    string[] czesciSciezki = p.Split("\\");
                    string nazwaPliku = czesciSciezki[czesciSciezki.Length - 1];
                    Console.WriteLine(nazwaPliku);
                }
            }
            else if (File.Exists(sciezka))
            {
                string[] czesciSciezki = sciezka.Split("\\");
                string nazwaPliku = czesciSciezki[czesciSciezki.Length - 1];
                Console.WriteLine(nazwaPliku);
            }
            else
            {
                Console.WriteLine("Nie znaleziono sciezki!");
                return 1;
            }
            return 0;
        }

        public static int RecursivePrint(string sciezka)
        {
            Console.WriteLine("REKURENCYJNE WYPISANIE ZAWARTOSCI dla sciezki:");
            Console.WriteLine(sciezka); 
            Console.WriteLine();

            if (File.Exists(sciezka))
            {
                FileRead(sciezka, 0);
            }
            else if (Directory.Exists(sciezka))
            {
                DirectoryInfo info = new DirectoryInfo(sciezka);

                DirRead(sciezka, 0);
            }
            else
            {
                Console.WriteLine("Nie znaleziono sciezki!");
                return 1;
            }
            return 0;
        }

        public static void FileRead(string sciezka, int poziom)
        {
            string[] czesciSciezki = sciezka.Split("\\");
            string nazwaPliku = czesciSciezki[czesciSciezki.Length - 1];

            string toPrint = "";
            for (int i = 0; i < poziom; i++)
            {
                toPrint += "\t";
            }
            FileInfo info = new FileInfo(sciezka);
            toPrint += nazwaPliku + " " + info.Length + " bajtow  " + info.RAHS();
            Console.WriteLine(toPrint);
        }

        public static void DirRead(string sciezka, int poziom)
        {
            string toPrint = ""; 
            
            string[] katalogi = Directory.GetDirectories(sciezka);
            string[] czesciSciezki = sciezka.Split("\\");
            string nazwaKatalogu = czesciSciezki[czesciSciezki.Length - 1];
     
            for (int i = 0; i < poziom; i++)
            {
                toPrint += "\t";
            }

            DirectoryInfo info = new DirectoryInfo(sciezka);
            toPrint += nazwaKatalogu + " (";
            int plikow_w_katalogu = info.GetFiles().Length + info.GetDirectories().Length;
            toPrint += plikow_w_katalogu + ") " + info.RAHS();
            Console.WriteLine(toPrint);

            foreach (var k in katalogi)
            {

                DirRead(k, poziom + 1);
            }

            string[] pliki = Directory.GetFiles(sciezka);
            foreach (var p in pliki)
            {
                FileRead(p, poziom + 1);
            }

        }

        public static DateTime Oldest(this DirectoryInfo name)
        {
            DateTime wynik = new DateTime(9999, 12, 31, 23, 59, 59);

            if (Directory.Exists(name.FullName))
            {
                foreach (var katInfo in name.GetDirectories())
                {
                    DateTime nowyWynik = katInfo.Oldest();
                    if (nowyWynik < wynik)
                    {
                        wynik = nowyWynik;
                    }
                }

                foreach (var plikInfo in name.GetFiles())
                {
                    DateTime data = plikInfo.CreationTime;
                    if (data < wynik)
                    {
                        wynik = data;
                    }
                }
            }
            else if (File.Exists(name.FullName)) // jezeli podana sciezka to nie katalog, a plik.
            {
                wynik = name.CreationTime;
      
            }
                return wynik;
        }

        public static string RAHS(this FileSystemInfo fsi)
        {
            char[] letters = new char[4] { 'r', 'a', 'h', 's' };
            bool[] attrs = new bool[4] { false, false, false, false };

            FileAttributes fileAttrs = fsi.Attributes;

            if ((fileAttrs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                attrs[0] = true;
            }
            if ((fileAttrs & FileAttributes.Archive) == FileAttributes.Archive)
            {
                attrs[1] = true;
            }
            if ((fileAttrs & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                attrs[2] = true;
            }
            if ((fileAttrs & FileAttributes.System) == FileAttributes.System)
            {
                attrs[3] = true;
            }

            string rahs = ""; // ciag znakow rahs do wypisania w konsoli

            for (int i = 0; i < 4; i++)
            {
                if (attrs[i])
                {
                    rahs += letters[i];
                }
                else
                {
                    rahs += '-';
                }
            }
            return rahs;
        }

        public static void LoadToCollection(string sciezka)
        {
            SortedDictionary<string, long> collect = new SortedDictionary<string, long>(new CollectionComparer());

            if (Directory.Exists(sciezka))
            {
                string[] katalogi = Directory.GetDirectories(sciezka);
                foreach (var k in katalogi)
                {
                    string[] czesciSciezki = k.Split("\\");
                    string nazwaKatalogu = czesciSciezki[czesciSciezki.Length - 1];
                    DirectoryInfo info = new DirectoryInfo(k);
                    collect.Add(nazwaKatalogu, info.GetFiles().Length + info.GetDirectories().Length);

                }
                string[] pliki = Directory.GetFiles(sciezka);
                foreach (var p in pliki)
                {
                    string[] czesciSciezki = p.Split("\\");
                    string nazwaPliku = czesciSciezki[czesciSciezki.Length - 1];
                    FileInfo info = new FileInfo(p);
                    collect.Add(nazwaPliku, info.Length);
                }

                Serialize(collect);
                Deserialize();
            }

        }

        public static void Serialize(SortedDictionary<string, long> collection)
        {
            FileStream fs = new FileStream("Serializacja.dat", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                bf.Serialize(fs, collection);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

        }

        static void Deserialize()
        {     
            SortedDictionary<string, long> deserializedCollection = new SortedDictionary<string, long>(new CollectionComparer());

            FileStream fs = new FileStream("Serializacja.dat", FileMode.Open);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();

                deserializedCollection = (SortedDictionary<string, long>)bf.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
            foreach (var para in deserializedCollection) // wypisanie zdeserializowanej kolekcji
            {
                Console.WriteLine(para.Key + " -> " + para.Value);
            }
        }
    }
}

[Serializable]
public class CollectionComparer : IComparer<string>
{
    int IComparer<string>.Compare(string x, string y)
    {
        int lengthDiff = x.Length - y.Length;
        if (lengthDiff != 0)
        {
            return lengthDiff;
        }
        else
        {
            return x.CompareTo(y);
        }
    }
}
