using System;

public class Input
{
    public static string GetString(string s)
    {
        Console.WriteLine(s);
        return GetString();
    }

    public static string GetString()
    {
        return Console.ReadLine();
    }

    public static bool GetBool(string s)
    {
        Console.WriteLine(s);
        return GetBool();
    }

    public static bool GetBool()
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

    public static int GetInt(string s)
    {
        Console.WriteLine(s);
        return GetInt();
    }

    public static int GetInt()
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
        catch (OverflowException)
        {
            Console.Error.WriteLine("devi inserire un numero intero di massimo 9 cifre");
            return GetInt();
        }
    }
}
