using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace verifyISBN
{
    class Completo
    {
        private static readonly HttpClient client = new HttpClient();

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
                Console.WriteLine("devi inserire un ISBN (codice identificativo numerico)");
                return false;
            }
        }

        static List<string> GetISBNfromFile()
        {
            return GetISBNfromFile(false);
        }

        static List<string> GetISBNfromFile(bool debug)
        {
            return GetISBNfromFile(GeneratePattern("data/ISBN.txt"), debug);
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

        static void VerifyAllISBN()
        {
            VerifyAllISBN(GetISBNfromFile());
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
            List<string> ISBN = GetISBNfromFile(GeneratePattern("data/ISBN.txt"));
            Console.WriteLine("ISBN già presenti nel file:");
            foreach (string s in ISBN)
            {
                Console.WriteLine(s);
            }
            if (!GetBool("vuoi aggiungere nuovi ISBN?"))
            {
                return;
            }
            string line;
            while ((line = GetString("inserisci il nuovo ISBN, se hai finito premi invio lasciando vuoto")).Length > 0)
            {
                Console.WriteLine($"aggiungo {line} all'elenco");
                ISBN.Add(line);
            }
            Console.WriteLine("esco dalla modalità inserimento\ninizio a salvare i nuovi dati su file");
            ISBN.Sort();
            SetISBNonFile(GeneratePattern("data/ISBN.txt"), ISBN);
        }

        static void RemoveISBN()
        {
            List<string> ISBN = GetISBNfromFile(GeneratePattern("data/ISBN.txt"));
            
            if (ISBN.Count == 0)
            {
                Console.WriteLine("Il file è vuoto, non c'è nulla da rimuovere");
                return;
            }

            Console.WriteLine("ISBN presenti nel file:");
            for (int i = 0; i < ISBN.Count; i++)
            {
                Console.WriteLine($"{i + 1})\t{ISBN[i]}");
            }

            int choice = GetInt("Inserisci il numero dell'ISBN da rimuovere (0 per annullare)");
            
            if (choice == 0)
            {
                Console.WriteLine("Operazione annullata");
                return;
            }

            if (choice < 1 || choice > ISBN.Count)
            {
                Console.WriteLine("Numero non valido");
                return;
            }

            string removedISBN = ISBN[choice - 1];
            ISBN.RemoveAt(choice - 1);
            Console.WriteLine($"ISBN {removedISBN} rimosso");
            
            SetISBNonFile(GeneratePattern("data/ISBN.txt"), ISBN);
            Console.WriteLine("File aggiornato");
        }

        static void ClearFileWithBackup()
        {
            string sourceFile = GeneratePattern("data/ISBN.txt");
            string backupFile = GenerateBackupFileName();

            try
            {
                File.Copy(sourceFile, backupFile, true);
                Console.WriteLine($"Backup creato: {backupFile}");

                SetISBNonFile(sourceFile, new List<string>());
                Console.WriteLine("File svuotato");
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine($"file {sourceFile} non trovato");
            }
            catch (IOException e)
            {
                Console.Error.WriteLine($"errore durante il backup: {e.Message}");
            }
        }

        static void CreateBackup()
        {
            string sourceFile = GeneratePattern("data/ISBN.txt");
            string backupFile = GenerateBackupFileName();

            try
            {
                File.Copy(sourceFile, backupFile, true);
                Console.WriteLine($"Backup salvato: {backupFile}");
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine($"file {sourceFile} non trovato");
            }
            catch (IOException e)
            {
                Console.Error.WriteLine($"errore durante il backup: {e.Message}");
            }
        }

        static string GenerateBackupFileName()
        {
            DateTime now = DateTime.Now;
            string timestamp = now.ToString("yyyy-MM-dd_HH-mm");
            return GeneratePattern($"data/ISBN_{timestamp}.txt");
        }

        static void SetISBNonFile(string filename, List<string> ISBN)
        {
            StreamWriter? writer = null;
            try
            {
                writer = new StreamWriter(filename);
                foreach (string s in ISBN)
                {
                    writer.WriteLine(s);
                }
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine($"file {filename} non trovato");
            }
            catch (IOException)
            {
                Console.Error.WriteLine($"errore di lettura/scrittura su {filename}");
            }
            finally
            {
                writer?.Close();
            }
        }

        static async Task VerifyAndGetBookInfo()
        {
            string isbn = GetString("inserisci l'ISBN da verificare");
            
            if (!VerifyISBN(isbn))
            {
                Console.WriteLine($"{isbn}: ISBN non valido");
                return;
            }

            Console.WriteLine($"{isbn}: ISBN valido");
            Console.WriteLine("\nRecupero informazioni del libro...");

            try
            {
                isbn = isbn.Replace("-", "").Replace(" ", "").Trim();
                string url = $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&format=json&jscmd=data";
                
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                JsonElement root = doc.RootElement;

                string key = $"ISBN:{isbn}";
                if (!root.TryGetProperty(key, out JsonElement bookData))
                {
                    Console.WriteLine("\nImpossibile trovare info libro, potrebbe non essere presente su open library o non esistere");
                    return;
                }

                Console.WriteLine("\n--- INFORMAZIONI LIBRO ---");

                if (bookData.TryGetProperty("title", out JsonElement title))
                {
                    Console.WriteLine($"Titolo: {title.GetString()}");
                }

                if (bookData.TryGetProperty("authors", out JsonElement authors) && authors.GetArrayLength() > 0)
                {
                    var authorNames = new List<string>();
                    foreach (var author in authors.EnumerateArray())
                    {
                        if (author.TryGetProperty("name", out JsonElement authorName))
                        {
                            authorNames.Add(authorName.GetString());
                        }
                    }
                    Console.WriteLine($"Autore/i: {string.Join(", ", authorNames)}");
                }

                if (bookData.TryGetProperty("publishers", out JsonElement publishers) && publishers.GetArrayLength() > 0)
                {
                    if (publishers[0].TryGetProperty("name", out JsonElement publisherName))
                    {
                        Console.WriteLine($"Casa editrice: {publisherName.GetString()}");
                    }
                }

                if (bookData.TryGetProperty("publish_date", out JsonElement publishDate))
                {
                    Console.WriteLine($"Data di pubblicazione: {publishDate.GetString()}");
                }

                string coverUrl = $"https://covers.openlibrary.org/b/isbn/{isbn}-L.jpg";
                Console.WriteLine($"URL copertina: {coverUrl}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"\nErrore connessione API: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"\nErrore parsing risposta: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nErrore: {e.Message}");
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
                Console.Error.WriteLine("devi inserire un valore booleano\t(true per sì e false per no)");
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
                Console.WriteLine("devi inserire un ISBN (codice identificativo numerico)");
                return;
            }
        }

        static async Task Main(string[] args)
        {
            bool running;
            do
            {
                running = true;
                int choice = GetInt("cosa vuoi fare?\n" +
                    "\t1)\taggiungere uno o più ISBN al file\n" +
                    "\t2)\trimuovere un ISBN dal file\n" +
                    "\t3)\tsvuotare il file mantenendo un file di backup\n" +
                    "\t4)\tsalvare una copia del file attuale per avere un backup\n" +
                    "\t5)\tverificare tutti gli ISBN del file\n" +
                    "\t6)\tverificare un singolo ISBN e ottenere tutte le relative informazioni\n" +
                    "\t7)\tcompletare un ISBN con la 13° cifra (cifra di controllo)\n" +
                    "\t0)\tuscire dal programma\n"
                    );

                switch (choice)
                {
                    case 1:
                        AddNewISBN();
                        break;
                    case 2:
                        RemoveISBN();
                        break;
                    case 3:
                        ClearFileWithBackup();
                        break;
                    case 4:
                        CreateBackup();
                        break;
                    case 5:
                        VerifyAllISBN();
                        break;
                    case 6:
                        await VerifyAndGetBookInfo();
                        break;
                    case 7:
                        complete(GetString("inserisci le 12 cifre dell'ISBN da completare"));
                        break;
                    case 0:
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Opzione non valida");
                        break;
                }

                if (running)
                {
                    Console.ReadLine();
                    Console.Clear();
                }
            } while (running);
        }
    }
}