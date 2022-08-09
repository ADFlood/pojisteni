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
            string[] uplneHodnoty = new string[4];
            // index 0 = id
            uplneHodnoty[1] = hodnoty[0]; // Jmeno
            uplneHodnoty[2] = hodnoty[1]; // Prijmeni
            uplneHodnoty[3] = ""; // nepovinná položka telefon
            return uplneHodnoty;
        }

        public override bool Pridej(string[] hodnoty, bool novyZaznam = true)
        {
            int spravnaDelka = 4;
            int counterZaznam = 0;
            string jmeno = "";
            string prijmeni = "";
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

                // Telefon
                telefon = hodnoty[3];
            }

            if (spravneHodnoty)
            {
                Pojistenec pojistenec = new Pojistenec(counterZaznam, jmeno, prijmeni, telefon);
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
        //public override bool Smaz(string id)
        //{
        //    T zaznam = VratZaznamDleId(id);
        //    bool smazano = databaze.Remove(zaznam);
        //    if (!smazano)
        //        Zprava = "Záznam se nepodařilo smazat.";
        //    return smazano;
        //}


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
                radka.Add(p.Telefon);
                string spojenaRadka = String.Join(";", radka.ToArray());
                radky.Add(spojenaRadka);
            }
            return radky.ToArray();
        }
    }
}
