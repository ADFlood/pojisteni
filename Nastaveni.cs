using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pojisteni
{
    internal class Nastaveni
    {
        public readonly int vyska = 30;
        public readonly int sirka = 120;
        public readonly int radekZahlavi = 0;
        public readonly int radekPrikazy = 2;

        public readonly int odsazeni = 2;
        public readonly int odsazeniOknaOznameniOdSpodniHrany = 3;

        public readonly int radekVypisuPrehledu = 4;
        public readonly int maxDelkaVypisuPrehledu = 10;

        public readonly int xSloupec1 = 2;
        public readonly int xSloupec2 = 20;
        public readonly int xSloupec3 = 23;
        public readonly int odsazeniPolozky = 15;

        public readonly int delkaParametru = 5;
        public readonly int indexNazev = 0;
        public readonly int indexSirka = 1;
        public readonly int indexAtribut = 2;
        public readonly int indexPorovnani = 3;
        public readonly int indexFiltr = 4;

        public readonly string[] parametryPrehleduPojistencu = new string[]
        {
            "ID", "8", "Id", "", "",
            "Příjmení", "20", "Prijmeni", "", "",
            "Jméno", "20", "Jmeno", "", ""
        };
        public readonly string[] parametryPrehleduSmluv = new string[]
        {
            "ID", "8", "Id", "", "",
            "Typ pojistky", "30", "TypPojistky", "", "",
            "Začátek", "12", "Zacatek", "", "",
            "Konec", "12", "Konec", "", "",
            "Výše", "-12", "Vyse", "", ""
        };
        public readonly string[] parametryPrehleduUdalosti = new string[]
        {
            "ID", "8", "Id", "", "",
            "Datum", "12", "Datum", "", "",
            "Typ události", "30", "Nazev", "", "",
            "Popis", "30", "Popis", "", "",
            "Výše", "-12", "Vyse", "", ""
        };
    }
}
