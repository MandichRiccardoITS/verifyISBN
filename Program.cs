using System;
using System.Collections.Generic;
using System.IO;

namespace HelloWorld
{
    class Program
    {
        static bool verifyISBN(string ISBN, bool debug)
        {
            if (ISBN == null)
            {
                Console.WriteLine("la stringa passata è nulla");
                return false;
            }
            if (ISBN.Length != 13)
            {
                Console.WriteLine("la stringa passata non è della dimensione corretta");
                return false;
            }
            int pari = 0;
            int dispari = 0;
            try
            {
                for (int i = 0; i < ISBN.Length; i++)
                {
                    int j = Int32.Parse(ISBN[i].ToString());
                    if (debug) Console.WriteLine($"ISBN alla cifra {i + 1}: {j}");
                    if (i % 2 == 0)
                    {
                        dispari += j;
                    }
                    else
                    {
                        pari += j;
                    }
                }
                int total = dispari + (pari * 3);
                if (total % 10 == 0)
                {
                    if (debug) Console.WriteLine($"il risultato dell'equazione di controllo è multiplo di 10 ({total})");
                    return true;
                }
                else
                {
                    if (debug) Console.WriteLine($"il risultato dell'equazione di controllo non è multiplo di 10 ({total})");
                    return false;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("devi iserire un ISBN (codice identificativo numerico)");
                return false;
            }
        }

        static List<string> getISBNfromFile(string fileName, bool debug)
        {
            List<string> ret = new List<string>();
            try
            {
                StreamReader reader = new StreamReader(fileName);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (debug) Console.WriteLine("Lettura: aggiungo [" + line + "] alla lista");
                    ret.Add(line);
                }
                reader.Close();
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine($"file {fileName} non trovato");
            }
            catch (IOException)
            {
                Console.Error.WriteLine($"errore apertura file {fileName}");
            }
            return ret;
        }

        static void Main(string[] args)
        {
            //Console.WriteLine("inserire ISBN da verificare");
            //string ISBN = Console.ReadLine();
            List<string> ISBN = getISBNfromFile("ISBN.txt", false);
            foreach (string s in ISBN)
            {
                Console.WriteLine($"{s}:\t{verifyISBN(s, false)}");
            }
        }
    }
}