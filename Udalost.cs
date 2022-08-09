using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pojisteni
{
    internal class Udalost : IZaznam
    {
        public string Nazev { get; private set; }
        public DateTime Datum { get; private set; }
        public int Vyse { get; private set; }
        public string Popis { get; private set; }
        public string IdPojistence { get; private set; }
        public string IdSmlouvy { get; private set; }
        private string id = "";
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                // dovolí nastavit Id jen při inicializaci
                if (id == "")
                    id = value;
            }
        }
        public string NazevZaznamu
        {
            get
            {
                return Id + " - " + Nazev;
            }
        }
        private static string tag = "UDL";
        public static string[] EditovatelneVlastnosti
        {
            get
            {
                return new string[] { "Nazev", "Název",
                                      "Datum", "Datum",
                                      "Vyse", "Výše",
                                      "Popis", "Popis"};
            }
        }
        public static string[] ZakladniVlastnosti
        {
            get
            {
                return new string[] { "Nazev", "Název",
                                      "Datum", "Datum",
                                      "Vyse", "Výše",
                                      "Popis", "Popis" };
            }
        }
        public Udalost(int counter, string nazev, DateTime datum, int vyse, string popis, string idPojistence, string idSmlouvy)
        {
            Id = tag + counter.ToString().PadLeft(3, '0');
            Nazev = nazev;
            Datum = datum;
            Vyse = vyse;
            Popis = popis;
            IdPojistence = idPojistence;
            IdSmlouvy = idSmlouvy;
        }
        public override string ToString()
        {
            return Id + " - " + Nazev;
        }
    }
}
