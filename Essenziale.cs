using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

namespace verifyISBN
{
    class Essenziale
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

        static string GetString(string s)
        {
            Console.WriteLine(s);
            return GetString();
        }

        static string GetString()
        {
            return Console.ReadLine();
        }

        static void main(string[] args)
        {
            bool ret = VerifyISBN(GetString("inserisci l'ISBN da verificare"));
            Console.Write("questo ISBN");
            if (!ret) Console.Write("non");
            Console.Write("è valido\n");
        }
    }
}