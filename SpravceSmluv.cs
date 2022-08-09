using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace pojisteni
{
    internal class SpravceSmluv<T> : Spravce<Smlouva>
    {
        public SpravceSmluv(Nastaveni nastaveni) : base(nastaveni)
        {
            soubor = "smlouvy.csv";

            oznaceniZaznamu = new string[] { "smlouvy", "smluv" };
            prikazyPrehledZaznamu = new string[] { "Hlavní menu", "Filtrovat", "Otevřít" };
            klavesyPrehledZaznamu = new char[] { 'H', 'F', 'O', 'P', 'D' };
        }

        public override string[] UpravHodnoty(string[] hodnoty, string[] doplnkoveHodnoty)
        {
            string[] uplneHodnoty = new string[6];
            // index 0 = id
            uplneHodnoty[1] = hodnoty[0]; // Nazev
            uplneHodnoty[2] = hodnoty[1]; // Zacatek
            uplneHodnoty[3] = hodnoty[2]; // Konec
            uplneHodnoty[4] = hodnoty[3]; // Vyse
            uplneHodnoty[5] = doplnkoveHodnoty[0]; // idPojistence
            return uplneHodnoty;
        }

        public override bool Pridej(string[] hodnoty, bool novyZaznam = true)
        {
            int spravnaDelka = 6;
            int counterZaznam = 0;
            string nazev = "";
            DateTime zacatek = new DateTime(2000, 1, 1);
            DateTime konec = new DateTime(2000, 1, 1);
            int vyse = 0;
            string idPojistence = "";

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

                // Nazev
                nazev = hodnoty[1];

                // Zacatek
                if (!DateTime.TryParse(hodnoty[2], out zacatek))
                    spravneHodnoty = false;

                // Konec
                if (!DateTime.TryParse(hodnoty[3], out konec))
                    spravneHodnoty = false;

                // Vyse
                if (!int.TryParse(hodnoty[4], out vyse))
                    spravneHodnoty = false;

                // IdPojistence
                idPojistence = hodnoty[5];
                if (idPojistence.Substring(0, 3) != "POJ")
                    spravneHodnoty = false;
            }

            if (spravneHodnoty)
            {
                Smlouva smlouva = new Smlouva(counterZaznam, nazev, zacatek, konec, vyse, idPojistence);
                databaze.Add(smlouva);
                counter++;
                Zprava = String.Format("Nová smlouva \"{0}\" byla úspěšně přidána.", smlouva);
            }
            else
            {
                Zprava = "Záznam se nepodařilo založit.";
            }

            return spravneHodnoty;
        }
        //public override bool Smaz(string id)
        //{
        //    return true;
        //}
        public override string[] VratEditovatelneVlasnosti()
        {
            return Smlouva.EditovatelneVlastnosti;
        }
        public override string[] VratZakladniVlastnost()
        {
            return Smlouva.ZakladniVlastnosti;
        }
        public override string[] PrevedDatabaziNaCSV()
        {
            List<string> radky = new List<string>();
            foreach(Smlouva s in databaze)
            {
                List<string> radka = new List<string>();
                radka.Add(s.Id);
                radka.Add(s.TypPojistky);
                radka.Add(s.Zacatek.ToShortDateString());
                radka.Add(s.Konec.ToShortDateString());
                radka.Add(s.Vyse.ToString());
                radka.Add(s.IdPojistence);
                string spojenaRadka = String.Join(";", radka.ToArray());
                radky.Add(spojenaRadka);
            }
            return radky.ToArray();
        }
        public int VratCelkovouHodnotuSmluvDleIdPojistence(string id)
        {
            var a = (from z in databaze
                     where z.IdPojistence == id
                     select z.Vyse).Sum();
            return a;
        }
    }
}
