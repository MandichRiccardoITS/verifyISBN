using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

namespace verifyISBN
{
    class Completo
    {
        static bool VerifyISBN(string ISBN)
        {
            return VerifyISBN(ISBN, false);
        }
        static bool VerifyISBN(string ISBN, bool debug)
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

        static List<string> GetISBNfromFile(string filename)
        {
            return GetISBNfromFile(filename, false);
        }
        static List<string> GetISBNfromFile(string fileName, bool debug)
        {
            List<string> ret = [];
            StreamReader? reader = null;
            try
            {
                reader = new(fileName);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (debug) Console.WriteLine("Lettura: aggiungo [" + line + "] alla lista");
                    ret.Add(line);
                }
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine($"file {fileName} non trovato");
            }
            catch (IOException)
            {
                Console.Error.WriteLine($"errore apertura file {fileName}");
            }
            finally
            {
                reader?.Close();
            }
            return ret;
        }

        static void VerifyAllISBN(List<string> ISBN)
        {
            foreach (string s in ISBN)
            {
                Console.WriteLine($"{s}:\t{VerifyISBN(s, false)}");
            }
        }

        static void AddNewISBN()
        {
            if(!GetBool("vuoi aggiungere nuovi ISBN?"))
            {
                return;
            }
            List<string> ISBN = GetISBNfromFile(GeneratePattern("data/ISBN.txt"));
            string line;
            while((line = GetString("inserisci il nuovo ISBN, se hai finito premi invio lasciondo vuoto")).Length > 0)
            {
                Console.WriteLine($"aggiungo {line} all'elenco");
                ISBN.Add(line);
            }
            Console.WriteLine("esco dalla modalità inserimento\ninizio a salvare i nuovi dati su file");
            SetISBNonFile(GeneratePattern("data/ISBN.txt"), ISBN);
        }
        static void SetISBNonFile(string filename, List<string> ISBN)
        {
            StreamWriter writer;
            try
            {
                writer = new StreamWriter(filename);
                foreach(string s in ISBN)
                {
                    writer.WriteLine(s);
                }
                writer.Close();
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine($"file {filename} non trovato");
            }
            catch (IOException)
            {
                Console.Error.WriteLine($"errore di lettura/scrittura su {filename}");
            }
        }

        static string GetString(string s)
        {
            Console.WriteLine(s);
            return GetString();
        }

        static string GetString()
        {
            return Console.ReadLine();
        }

        static bool GetBool(string s)
        {
            Console.WriteLine(s);
            return GetBool();
        }

        static bool GetBool()
        {
            try
            {
                return Boolean.Parse(GetString());
            }
            catch (FormatException)
            {
                Console.Error.WriteLine("devi inserire un valore buooleano\t(true per sì e false per no)");
                return GetBool();
            }
        }

        static int GetInt(string s)
        {
            Console.WriteLine(s);
            return GetInt();
        }

        static int GetInt()
        {
            try
            {
                return Int32.Parse(GetString());
            }
            catch (FormatException)
            {
                Console.Error.WriteLine("devi inserire un numero intero");
                return GetInt();
            }
        }

        static string GeneratePattern(string path)
        {
            return "../../../" + path;
        }

        static void verify()
        {
            AddNewISBN();
            VerifyAllISBN(GetISBNfromFile(GeneratePattern("data/ISBN.txt"), false));
        }

        static void complete(string ISBN)
        {
            complete(ISBN, false);
        }
        static void complete(string ISBN, bool debug)
        {
            if (ISBN == null)
            {
                Console.WriteLine("la stringa passata è nulla");
                return;
            }
            if (ISBN.Length != 12)
            {
                Console.WriteLine("la stringa passata non è della dimensione corretta");
                return;
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
                int lastDigit = 0;
                if (total % 10 != 0)
                {
                    lastDigit = 10 - (total % 10);
                }
                Console.WriteLine($"la cifra da aggiungere a {ISBN} per completarlo è {lastDigit}, per cui l'ISBN completo è {ISBN}{lastDigit}");
            }
            catch (FormatException)
            {
                Console.WriteLine("devi iserire un ISBN (codice identificativo numerico)");
                return;
            }
        }

        static void Main(string[] args)
        {
            switch(GetInt("quale modalità vuoi avviare?\n\t1)\tverificare gli ISBN\n\t2)\tcompletare un ISBN"))
            {
                case 1:
                    verify();
                    break;
                case 2:
                    complete(GetString("inserisci le 12 cifre del codice ISBN da completare"));
                    break;
            }
        }
    }
}