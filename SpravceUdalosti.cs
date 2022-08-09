using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pojisteni
{
    internal class SpravceUdalosti<T> : Spravce<Udalost>
    {
        public SpravceUdalosti(Nastaveni nastaveni) : base(nastaveni)
        {
            soubor = "udalosti.csv";

            oznaceniZaznamu = new string[] { "události", "událostí" };
            prikazyPrehledZaznamu = new string[] { "Hlavní menu", "Filtrovat", "Otevřít" };
            klavesyPrehledZaznamu = new char[] { 'H', 'F', 'O', 'P', 'D' };
        }
        public override string[] UpravHodnoty(string[] hodnoty, string[] doplnkoveHodnoty)
        {
            string[] uplneHodnoty = new string[7];
            // index 0 = id
            uplneHodnoty[1] = hodnoty[0]; // název
            uplneHodnoty[2] = hodnoty[1]; // datum
            uplneHodnoty[3] = hodnoty[2]; // výše
            uplneHodnoty[4] = hodnoty[3]; // popis
            uplneHodnoty[5] = doplnkoveHodnoty[0]; // idPojistence
            uplneHodnoty[6] = doplnkoveHodnoty[1]; // idSmlouvy
            return uplneHodnoty;
        }
        public override bool Pridej(string[] hodnoty, bool novyZaznam = true)
        {
            int spravnaDelka = 7;
            int counterZaznam = 0;
            string nazev = "";
            DateTime datum = new DateTime(2000, 1, 1);
            int vyse = 0;
            string popis = "";
            string idPojistence = "";
            string idSmlouvy = "";

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

                // Datum
                if (!DateTime.TryParse(hodnoty[2], out datum))
                    spravneHodnoty = false;

                // Vyse
                if (!int.TryParse(hodnoty[3], out vyse))
                    spravneHodnoty = false;

                // Popis
                popis = hodnoty[4];

                // IdPojistence
                idPojistence = hodnoty[5];
                if (idPojistence.Substring(0, 3) != "POJ")
                    spravneHodnoty = false;

                // IdSmlouvy
                idSmlouvy = hodnoty[6];
                if (idSmlouvy.Substring(0, 3) != "SML")
                    spravneHodnoty = false;
            }

            if (spravneHodnoty)
            {
                Udalost udalost = new Udalost(counterZaznam, nazev, datum, vyse, popis, idPojistence, idSmlouvy);
                databaze.Add(udalost);
                counter++;
                Zprava = String.Format("Nová událost \"{0}\" byla úspěšně přidána.", udalost);
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
            return Udalost.EditovatelneVlastnosti;
        }
        public override string[] VratZakladniVlastnost()
        {
            return Udalost.ZakladniVlastnosti;
        }
        public override string[] PrevedDatabaziNaCSV()
        {
            List<string> radky = new List<string>();
            foreach (Udalost u in databaze)
            {
                List<string> radka = new List<string>();
                radka.Add(u.Id);
                radka.Add(u.Nazev);
                radka.Add(u.Datum.ToShortDateString());
                radka.Add(u.Vyse.ToString());
                radka.Add(u.Popis);
                radka.Add(u.IdPojistence);
                radka.Add(u.IdSmlouvy);
                string spojenaRadka = String.Join(";", radka.ToArray());
                radky.Add(spojenaRadka);
            }
            return radky.ToArray();
        }
        public int VratCelkovouVysiUdalostiDleIdsSmluv(string[] ids)
        {
            var a = (from z in databaze
                     where ids.Contains(z.IdSmlouvy)
                     select z.Vyse).Sum();
            //var a = (from id in ids
            //         join z in databaze on id equals z.IdSmlouvy
            //         select z.Vyse).Sum();
            return a;
        }
    }

    
}
