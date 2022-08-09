using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pojisteni
{
    internal class Smlouva : IZaznam
    {
        public string TypPojistky { get; private set; }
        public DateTime Zacatek { get; private set; }
        public DateTime Konec { get; private set; }
        public int Vyse { get; private set; }
        public string IdPojistence { get; private set; }
        private string id = "";
        public string Id { get
            {
                return id;
            } set
            {
                // dovolí nastavit Id jen při inicializaci
                if (id == "")
                    id = value;
            } }
        public string NazevZaznamu
        {
            get
            {
                return Id + " - " + TypPojistky;
            }
        }
        private static string tag = "SML";
        public static string[] EditovatelneVlastnosti
        {
            get
            {
                return new string[] { "Zacatek", "Začátek",
                                      "Konec", "Konec",
                                      "Vyse", "Výše"};
            }
        }
        public static string[] ZakladniVlastnosti
        {
            get
            {
                return new string[] { "TypPojistky", "Typ pojistky",
                                      "Zacatek", "Začátek",
                                      "Konec", "Konec",
                                      "Vyse", "Výše"};
            }
        }
        public Smlouva(int counter, string typPojistky, DateTime zacatek, DateTime konec, int vyse, string idPojistence)
        {
            Id = tag + counter.ToString().PadLeft(3, '0');
            TypPojistky = typPojistky;
            Zacatek = zacatek;
            Konec = konec;
            Vyse = vyse;
            IdPojistence = idPojistence;
        }
        public override string ToString()
        {
            return Id;
        }
    }
}
