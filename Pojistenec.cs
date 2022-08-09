using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pojisteni
{
    internal class Pojistenec : IZaznam
    {
        public string Jmeno { get; private set; }
        public string Prijmeni { get; private set; }
        public string CeleJmeno { get
            {
                return Prijmeni + " " + Jmeno;
            } }
        public string Telefon { get; private set; }
        public int Vek { get; private set; }
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
        public string NazevZaznamu { get
            {
                return Id + " - " + CeleJmeno;
            } }
        private static string tag = "POJ";
        public static string[] EditovatelneVlastnosti { get {
                return new string[] { "Jmeno", "Jméno",
                                      "Prijmeni", "Příjmení",
                                      "Vek", "Věk",
                                      "Telefon", "Telefon" };
            }}
        public static string[] ZakladniVlastnosti
        {
            get
            {
                return new string[] { "Jmeno", "Jméno",
                                      "Prijmeni", "Příjmení",
                                      "Vek", "Věk"};
            }
        }
        public Pojistenec(int counter, string jmeno, string prijmeni, int vek, string telefon)
        {
            Id = tag + counter.ToString().PadLeft(3, '0');
            Jmeno = jmeno;
            Prijmeni = prijmeni;
            Vek = vek;
            Telefon = telefon;
        }
        public override string ToString()
        {
            return CeleJmeno;
        }
    }
}
