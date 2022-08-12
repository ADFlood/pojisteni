using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace pojisteni
{
    internal class SpravcePojistencu<T> : Spravce<Pojistenec>
    {
        public SpravcePojistencu(Nastaveni nastaveni) : base (nastaveni)
        {
            soubor = "pojistenci.csv";

            oznaceniZaznamu = new string[] { "pojištěnce", "pojištěnců" };
            prikazyPrehledZaznamu = new string[] { "Hlavní menu", "Filtrovat", "Otevřít", "Nový" };
            klavesyPrehledZaznamu = new char[] { 'H', 'F', 'O', 'N', 'P', 'D' };
        }
        public override string[] UpravHodnoty(string[] hodnoty, string[] doplnkoveHodnoty)
        {
            string[] uplneHodnoty = new string[5];
            // index 0 = id
            uplneHodnoty[1] = hodnoty[0]; // Jmeno
            uplneHodnoty[2] = hodnoty[1]; // Prijmeni
            uplneHodnoty[3] = hodnoty[2]; // Větk
            uplneHodnoty[4] = ""; // nepovinná položka telefon
            return uplneHodnoty;
        }

        public override bool Pridej(string[] hodnoty, bool novyZaznam = true)
        {
            int spravnaDelka = 5;
            int counterZaznam = 0;
            string jmeno = "";
            string prijmeni = "";
            int vek = 0;
            string telefon = "";

            // Kontrola úplnosti a správnosti zadaných hodnot
            bool spravneHodnoty = true;
            if (hodnoty.Length != spravnaDelka)
                spravneHodnoty = false;
            else
            {
                // Id
                if (novyZaznam)
                    counterZaznam = counter + 1;
                else
                {
                    string s = hodnoty[0].Substring(3, 3);
                    if (!int.TryParse(s, out counterZaznam))
                        spravneHodnoty = false;
                }

                // Jmeno
                jmeno = hodnoty[1];

                // Prijmeni
                prijmeni = hodnoty[2];

                // Věk
                vek = int.Parse(hodnoty[3]);

                // Telefon
                telefon = hodnoty[4];
            }

            if (spravneHodnoty)
            {
                Pojistenec pojistenec = new Pojistenec(counterZaznam, jmeno, prijmeni, vek, telefon);
                databaze.Add(pojistenec);
                counter++;
                Zprava = String.Format("Nový pojištěnec \"{0}\" byl úspěšně přidán.", pojistenec);
            }
            else
            {
                Zprava = "Záznam se nepodařilo založit.";
            }

            return spravneHodnoty;
        }


        public override string[] VratEditovatelneVlasnosti()
        {
            return Pojistenec.EditovatelneVlastnosti;
        }
        public override string[] VratZakladniVlastnost()
        {
            return Pojistenec.ZakladniVlastnosti;
        }
        public override string[] PrevedDatabaziNaCSV()
        {
            List<string> radky = new List<string>();
            foreach (Pojistenec p in databaze)
            {
                List<string> radka = new List<string>();
                radka.Add(p.Id);
                radka.Add(p.Jmeno);
                radka.Add(p.Prijmeni);
                radka.Add(p.Vek.ToString());
                radka.Add(p.Telefon);
                string spojenaRadka = String.Join(";", radka.ToArray());
                radky.Add(spojenaRadka);
            }
            return radky.ToArray();
        }
    }
}
