using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace BdvCovid19GenomeStudy
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> Translator = new Dictionary<string, string>();
            string[] Proteins;
            string AminoAcids = "";

            string CovidRNA = File.ReadAllText("covid19.txt");
            string InverseTable = File.ReadAllText("inverse_table_genetic_code.txt");

            CovidRNA = Regex.Replace(CovidRNA, @"[\d-]", string.Empty);
            CovidRNA = Regex.Replace(CovidRNA, @" ", "", RegexOptions.Multiline);
            CovidRNA = Regex.Replace(CovidRNA, @"\n", "", RegexOptions.Multiline);

            InverseTable = Regex.Replace(InverseTable, @"^[^\/]*\/", "", RegexOptions.Multiline);
            InverseTable = Regex.Replace(InverseTable, @",", " ", RegexOptions.Multiline);
            InverseTable = Regex.Replace(InverseTable, @";", " ", RegexOptions.Multiline);

            string[] ParsedTable = InverseTable.Split("\n", CovidRNA.Length, StringSplitOptions.RemoveEmptyEntries);

            foreach (var element in ParsedTable)
            {
                string NoStop = Regex.Replace(element, "STOP", "*");
                string[] KeyVal = NoStop.Split("\t", CovidRNA.Length, StringSplitOptions.RemoveEmptyEntries);
                string[] Keys = KeyVal[1].ToLower().Split(" ", CovidRNA.Length, StringSplitOptions.RemoveEmptyEntries);
                foreach (var k in Keys)
                {
                    if (!Translator.ContainsKey(Regex.Replace(k, @"u", "t", RegexOptions.Multiline)))
                    {
                        Translator.Add(Regex.Replace(k, @"u", "t", RegexOptions.Multiline), KeyVal[0]);
                    }
                }
            }

            for (int i = 1; i < 3; i++)
            {
                for (int j = i; j < CovidRNA.Length - 2; j += 3)
                {
                    string CodonToTest = CovidRNA.Substring(j, 3);
                    if (Translator.ContainsKey(CodonToTest))
                    {
                        AminoAcids += Translator[CodonToTest];
                    }
                }
            }


            Proteins = AminoAcids.Split("*", CovidRNA.Length, StringSplitOptions.RemoveEmptyEntries);
            File.WriteAllText(@"translation.txt", AminoAcids);
        }
    }
}