using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace nemetszo
{
    internal class Program
    {
        static Dictionary<string, string[]> szavak = new Dictionary<string, string[]>();
        static void Main(string[] args)
        {
            try
            {
                FileBeolvasas();
                QuizInditas();
            }
            catch (IOException ex)
            {
                Console.WriteLine("szavak.txt fájl nem található!");
                Console.ReadKey();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba történt a program futása során!");
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return;
            }
            
            

        }

        private static void QuizInditas()
        {
            string restart;
            do
            {
                Console.Write("Németet kérdezzem? (Y/N)");
                string be = Console.ReadLine();
                if (be.ToLower().Trim() == "y")
                {
                    NemetMutatMagyarKerdez();
                }
                else
                {
                    MagyarMutatNemetKerdez();
                }
                Console.Write("Újrakezdjem? (Y/N)");
                restart = Console.ReadLine();
            } while (restart.ToLower() == "y");
        }

        private static void FileBeolvasas()
        {
            using (StreamReader reader = new StreamReader("szavak.txt"))
            {
                while (!reader.EndOfStream)
                {
                    string[] darabolt = reader.ReadLine().Split(';');
                    string[] valaszok = darabolt[1].Split(',');
                    for (int i = 0; i < valaszok.Length; i++)
                    {
                        valaszok[i] = valaszok[i].ToLower().Trim().Trim('.');
                    }
                    szavak.Add(darabolt[0].Trim().Trim('.'), valaszok);
                }
            }
        }

        private static void MagyarMutatNemetKerdez()
        {
            Dictionary<string, string> rosszValaszok = new Dictionary<string, string>();
            var clone = szavak.ToDictionary(entry => entry.Key, entry => entry.Value);
            int osszes = szavak.Count;
            int pontszam = 0;
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int kerdesIndex = 1;
            while (clone.Count > 0)
            {
                var valasztott = clone.ElementAt(rnd.Next(clone.Count));
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"{kerdesIndex}. kérdés:");
                Console.WriteLine($"Magyar szó/szavak: {ArrayToString(valasztott.Value)}");
                Console.Write($"Németül: ");
                string valasz = Console.ReadLine();
                if (valasz.ToLower() == valasztott.Key.ToLower())
                {
                    Console.WriteLine("Helyes válasz!");
                    pontszam++;
                }
                else
                {
                    Console.WriteLine($"Helytelen válasz! Helyes: {valasztott.Key}");
                    // kerdesIndex@kerdes helyesValasz@valaszolt
                    rosszValaszok.Add($"{kerdesIndex}@{ArrayToString(valasztott.Value)}",$"{valasztott.Key}@{valasz}");
                }
                Console.WriteLine($"Pontjaid száma: {pontszam}");
                clone.Remove(valasztott.Key);
                Console.WriteLine();
                kerdesIndex++;
            }
            Console.WriteLine($"\nVégeredmény: {pontszam}/{osszes}");

            // kerdes helyesValasz@valaszolt
            if (rosszValaszok.Count != 0)
            {
                RosszValaszokKiirasa(rosszValaszok);
            }
        }

        private static void RosszValaszokKiirasa(Dictionary<string, string> rosszValaszok)
        {
            Console.WriteLine($"Elrontott kérdések:");
            foreach (var item in rosszValaszok)
            {
                string[] daraboltValaszok = item.Value.Split('@');
                string helyes = daraboltValaszok[0];
                string valaszolt = daraboltValaszok[1];

                string[] daraboltKerdes = item.Key.Split('@');
                string kerdesIndex = daraboltKerdes[0];
                string kerdes = daraboltKerdes[1];

                Console.WriteLine($"\t{kerdesIndex}. Kérdés: {kerdes}");
                Console.WriteLine($"\t\tHelyes: {helyes}");
                Console.WriteLine($"\t\tVálaszolt: {valaszolt}");
            }
        }

        private static void NemetMutatMagyarKerdez()
        {
            Dictionary<string,string> rosszValaszok = new Dictionary<string,string>();
            var clone = szavak.ToDictionary(entry => entry.Key, entry => entry.Value);
            int osszes = szavak.Count;
            int pontszam = 0;
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int kerdesIndex = 1;
            while (clone.Count > 0)
            {
                var valasztott = clone.ElementAt(rnd.Next(clone.Count));
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"{kerdesIndex}. kérdés:");
                Console.WriteLine($"Német szó: {valasztott.Key}");
                Console.Write($"Magyarul: ");
                string valasz = Console.ReadLine();
                if (HelyesValaszFromArray(valasz.ToLower(), valasztott.Value))
                {
                    Console.WriteLine("Helyes válasz!");
                    pontszam++;
                }
                else
                {
                    string helyes = ArrayToString(valasztott.Value);
                    Console.WriteLine($"Helytelen válasz! Helyes: {helyes}");
                    rosszValaszok.Add($"{kerdesIndex}@{valasztott.Key}",$"{helyes}@{valasz}");
                }
                Console.WriteLine($"Pontjaid száma: {pontszam}");
                clone.Remove(valasztott.Key);
                Console.WriteLine();
                kerdesIndex++;
            }
            Console.WriteLine($"\nVégeredmény: {pontszam}/{osszes}");
            if (rosszValaszok.Count > 0)
            {
                RosszValaszokKiirasa(rosszValaszok);
            }
        }

        private static string ArrayToString(string[] value)
        {
            StringBuilder builder = new StringBuilder("[");
            for (int i = 0; i < value.Length - 1; i++)
            {
                builder.Append($"{value[i]}, ");
            }
            builder.Append($"{value.Last()}]");
            return builder.ToString();
        }

        private static bool HelyesValaszFromArray(string valasz, string[] valaszok)
        {
            foreach (var item in valaszok)
            {
                if(valasz == item) return true;
            }
            return false;
        }
    }
}
