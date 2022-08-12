using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace pojisteni
{
    internal class Aplikace
    {
        SpravcePojistencu<Pojistenec> spravcePojistencu;
        SpravceSmluv<Smlouva> spravceSmluv;
        SpravceUdalosti<Udalost> spravceUdalosti;

        Nastaveni nastaveni;

        public Aplikace()
        {
            nastaveni = new Nastaveni();
            NastaveniKonzole();

            spravcePojistencu = new SpravcePojistencu<Pojistenec>(nastaveni);
            spravceSmluv = new SpravceSmluv<Smlouva>(nastaveni);
            spravceUdalosti = new SpravceUdalosti<Udalost>(nastaveni);
        }
        private void NastaveniKonzole()
        {
            Console.WindowWidth = nastaveni.sirka;
            Console.WindowHeight = nastaveni.vyska;
            BarvyStandard();
            Console.Clear();
        }
        public void Start()
        {
            string[] texty = NactiZaznamy();
            VytvorOknoOznameni(texty);

            HlavniMenu();
        }

        /// <summary>
        /// Delegát pro nastavení barvy pozadí a popředí
        /// </summary>
        private delegate void NastavBarvu();
        private void BarvyStandard()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
        private void BarvyStandardSlabe()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
        }
        private void BarvyOznameni()
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
        }
        private void BarvyRevers()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
        }
        private void BarvyReversZvyrazneni()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkRed;
        }
        private void BarvyReversEdit()
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkRed;
        }
        private void BarvyReversEditSlabe()
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }
        private void BarvyReversSlabe()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        private void BarvyReversZahlavi()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
        }

        /// <summary>
        /// Smaže 1 řádek obrazovky tak, že ho vyplní zadaným znak o zadané barvě
        /// </summary>
        /// <param name="radek"></param> číslo řádku
        /// <param name="barva"></param> barevné schéma
        /// <param name="znak"></param> znak, kterým bude řádek vyplněn
        private void SmazRadek(int radek, NastavBarvu barva, char znak = ' ')
        {
            //string text = "";
            //for (int i = 0; i < nastaveni.sirka; i++)
            //    text += znak;
            VypisText(radek, 0, barva, VytvorRetezecZnaku(znak, nastaveni.sirka));
        }
        /// <summary>
        /// Smaže obrazovku, resetuje barvy, vypíše záhlaví obrazovky a příkazový řádek
        /// </summary>
        /// <param name="nadpis"></param>
        private void VytvorNovouObrazovku(string zahlavi, string[] nabidka)
        {
            // Smaže celé okno a nastaví barvy na standard
            BarvyStandard();
            Console.Clear();

            // Vykreslí první dva řádky podkladovou barvou záhlaví
            ushort znak = 0x2584;
            SmazRadek(nastaveni.radekZahlavi, BarvyRevers);
            SmazRadek(nastaveni.radekZahlavi + 1, BarvyRevers, Convert.ToChar(znak));

            // Vytvoří text záhlaví a vypíše ho s mezerami mezi znaky
            string mezitext = "*** " + zahlavi.ToUpper() + " ***";
            string text = "";
            for (int i = 0; i < mezitext.Length; i++)
                text += mezitext[i] + " ";
            int x = (nastaveni.sirka - text.Length) / 2;
            VypisText(0, x, BarvyReversZahlavi, text);

            // Vypíše řádek s příkazy okna
            VypisNabidku(nabidka);
        }
        private void VypisNabidku(string[] nabidka)
        {
            // Vykreslí prázdný řádek podkladovou barvou příkazů
            SmazRadek(nastaveni.radekPrikazy, BarvyRevers);

            // Vypíše příkazy, klávesové zkratky zvýrazěnou barvou
            NastavKurzor(nastaveni.radekPrikazy, 0);
            foreach (string prikaz in nabidka)
                VypisPrikaz(prikaz + "   ");
        }
        private int VypisText(int y, int x, NastavBarvu barva, string text)
        {
            NastavKurzor(y, x);
            barva();
            Console.Write(text);
            return text.Length;
        }
        private void VypisPrikaz(string prikaz)
        {
            foreach (char znak in prikaz)
            {
                if ((int)znak <= (char)'Z' && (int)znak >= (char)'A')
                    BarvyReversZvyrazneni();
                else
                    BarvyRevers();
                Console.Write(znak);
            }
        }
        private char VyzadatKlavesu(char[] validniZnaky)
        {
            char klavesa = 'q';  // vybráno jako nejméně pravděpodobný znak, který nebude na 99% potřeba
            while (!validniZnaky.Contains(klavesa))
                klavesa = Char.ToUpper(Console.ReadKey(true).KeyChar);
            return klavesa;
        }
       
        private void VytvorOknoOznameni(string text)
        {
            VytvorOknoOznameni(new string[] { text });
        }
        private void VytvorOknoOznameni(string[] texty)
        {
            int sirka = 0;
            foreach (string s in texty)
                sirka = Math.Max(sirka, s.Length);
            sirka = sirka + nastaveni.odsazeni * 2;

            int vyska = texty.Length + nastaveni.odsazeni;
            int x = (nastaveni.sirka - sirka) / 2;
            int y = nastaveni.vyska - vyska - nastaveni.odsazeniOknaOznameniOdSpodniHrany;

            VykresliPodokno(y, x, vyska, sirka, BarvyOznameni);
            for (int i = 0; i < texty.Length; i++)
                VypisText(y + i + 1, x + nastaveni.odsazeni, BarvyOznameni, texty[i]);
            Console.ReadKey();
        }
        private void VykresliPodokno(int y, int x, int vyska, int sirka, NastavBarvu barva)
        {
            for (int yy = y; yy < y + vyska; yy++)
            {
                for (int xx = x; xx < x + sirka; xx++)
                    VypisText(yy, xx, barva, " ");
            }
        }
        /// <summary>
        /// Vytvoří podokno v prostřed obrazovky
        /// </summary>
        /// <param name="sirka"></param>
        /// <param name="vyska"></param>
        /// <param name="nadpis"></param>
        /// <param name="text1"></param>
        /// <param name="text2"></param>
        /// <returns></returns> Navrátí pozici levého horního rohu
        private (int, int) VytvorPodokno(int sirka, int vyska, string nadpis, string text1, string text2 = "")
        {
            int x = (nastaveni.sirka - sirka) / 2;
            int y = (nastaveni.vyska - vyska) / 2;

            // Vykresli podokno
            VykresliPodokno(y, x, vyska, sirka, BarvyRevers);

            // Vypiš záhlaví a první dva řádky
            VypisText(y, x + (sirka - nadpis.Length) / 2, BarvyReversZahlavi, nadpis.ToUpper());
            VypisText(y + nastaveni.odsazeni, x + nastaveni.odsazeni, BarvyRevers, text1);
            if (text2 != "")
                VypisText(y + nastaveni.odsazeni + 1, x + nastaveni.odsazeni, BarvyRevers, text2);

            return (y, x);
        }
        /// <summary>
        /// Vyžádá a vrátí int v rozmezí zadaných limitů. Prázdná hodnota vrátí int.Maxvalue
        /// </summary>
        /// <param name="minHodnota"></param>
        /// <param name="maxHodnota"></param>
        /// <param name="nadpis"></param>
        /// <returns></returns>
        private int VyzadejVolbuZaznamu(int minHodnota, int maxHodnota, string nadpis)
        {
            string text1 = String.Format("Zadejte # záznamu {0}-{1}: ", minHodnota, maxHodnota);
            string text2 = "(prázdná hodnota = zpět)";

            int sirka = text1.Length + 2 + nastaveni.odsazeni * 2;
            int vyska = 2 + nastaveni.odsazeni * 2;

            int volba = -1;
            if (maxHodnota >= minHodnota && maxHodnota > 0)
            {
                while ((volba < minHodnota || volba > maxHodnota) && volba != int.MaxValue)
                {
                    (int y, int x) pozice = VytvorPodokno(sirka, vyska, nadpis, text1, text2);
                    string input = VyzadejText(pozice.y + 2, pozice.x + text1.Length + 2, BarvyRevers);
                    if (input != "")
                    {
                        if (int.TryParse(input, out int a))
                            volba = a;
                    }
                    else
                        volba = int.MaxValue;
                }
            }
            else
                volba = int.MaxValue;

            return volba;
        }
        /// <summary>
        /// Spustí hlavní menu aplikace
        /// </summary>
        private void HlavniMenu()
        {
            string zahlavi = "HLAVNÍ MENU";
            string[] nabidka = new string[] { "Pojištěnci", "Smlouvy", "Události", "uLož", "O aplikaci", "Konec" };
            char[] povoleneKlavesy = new char[] { 'P', 'S', 'U', 'L', 'O', 'K' };
            char klavesa = 'Q';
            while (klavesa != 'K')
            {
                VytvorNovouObrazovku(zahlavi, nabidka);
                klavesa = VyzadatKlavesu(povoleneKlavesy);
                if (klavesa == 'P')
                    PrehledZaznamu(spravcePojistencu, nastaveni.parametryPrehleduPojistencu);
                else if (klavesa == 'S')
                    PrehledZaznamu(spravceSmluv, nastaveni.parametryPrehleduSmluv);
                else if (klavesa == 'U')
                    PrehledZaznamu(spravceUdalosti, nastaveni.parametryPrehleduUdalosti);
                else if (klavesa == 'O')
                    OAplikaci();
                else if (klavesa == 'L')
                {
                    string[] texty = UlozZaznamy();
                    VytvorOknoOznameni(texty);
                }
            }
        }
        private void OAplikaci()
        {
            // Vykreslení obrazovky a všech elementů
            string zahlavi = "o aplikaci";
            string[] nabidka = new string[] { "Zpět" };
            char[] povoleneKlavesy = new char[] { 'Z' };
            VytvorNovouObrazovku(zahlavi, nabidka);

            char klavesa = 'Q';
            while (klavesa != 'Z')
            {
                NastavKurzor(nastaveni.radekPrikazy + 3, 0);
                BarvyStandard();
                Console.WriteLine("Jednoduchá aplikace na evidenci pojištěnců, pojistných smluv a pojistných událostí.");
                Console.WriteLine();
                Console.WriteLine("Struktura databáze");
                Console.WriteLine("Smlouva je vázána na pojištěnce. Novou smlouvu lze založit na kartě konkrétního pojištěnce.");
                Console.WriteLine("Událost je vázání na smlouvu. Novou událost lze založit na kartě konkrétní smlouvy.");
                Console.WriteLine("Jakýkoli záznam lze smazat, jen když na něj nejsou navázány další záznamy.");
                Console.WriteLine("Každý typ záznamů má svou vlastní databázi. Každá databáze má svého správce.");
                Console.WriteLine("Samotná aplikace komunikuje jen vůči uživateli na jedné straně a konkrétnímu správci na druhé straně.");
                Console.WriteLine("Aplikace žádá u správce o data a ten jí je po zpracování pošle zpět formou stringu.");
                Console.WriteLine();
                Console.WriteLine("Filtrování v přehledu záznamů");
                Console.WriteLine("Vypsané záznamy lze filtrovat s užitím logických operátorů:");
                Console.WriteLine("= rovná se, != nerovná se, % obsahuje, !% neobsahuje, >, >=, <, <=");
                Console.WriteLine();
                Console.WriteLine("Uložení databáze");
                Console.WriteLine("Data jsou uložena formou CSV, pro jednoduchost v kořenovém adresáři aplikace.");
                Console.WriteLine("Při otevření aplikace dojde automaticky k načtení uložených dat");
                Console.WriteLine();
                Console.WriteLine("O vývoji");
                Console.WriteLine("Vytvořeno během 4 týdnů po večerech mezi prací a spánkem, v tramvajích a vlacích.");
                Console.WriteLine("Co zprvu vypadalo nad Starbucks kafem jednoduché jak facka nakonec tak jednoduché nebylo, ale díky za to.");
                Console.WriteLine("100x jsem přepisoval a upravoval, zobecňoval a zjednodušoval. Stále cítím mezery, ale potřebuju to už dokončit");
                Console.WriteLine("a posunout se dál. Nicméně byla to skvělá zkušenost.");
                klavesa = VyzadatKlavesu(povoleneKlavesy);
            }

        }
        private string[] NactiZaznamy()
        {
            List<string> zpravy = new List<string>();

            spravcePojistencu.Nacti();
            zpravy.Add(spravcePojistencu.Zprava);

            spravceSmluv.Nacti();
            zpravy.Add(spravceSmluv.Zprava);

            spravceUdalosti.Nacti();
            zpravy.Add(spravceUdalosti.Zprava);

            return zpravy.ToArray();
        }
        private string[] UlozZaznamy()
        {
            List<string> zpravy = new List<string>();

            spravcePojistencu.Uloz(spravcePojistencu.PrevedDatabaziNaCSV());
            zpravy.Add(spravcePojistencu.Zprava);

            spravceSmluv.Uloz(spravceSmluv.PrevedDatabaziNaCSV());
            zpravy.Add(spravceSmluv.Zprava);

            spravceUdalosti.Uloz(spravceUdalosti.PrevedDatabaziNaCSV());
            zpravy.Add(spravceUdalosti.Zprava);

            return zpravy.ToArray();
        }
        /// <summary>
        /// Vytvoří obrazovku s přehledem záznamů, sloupce dle parametru v konstruktoru
        /// </summary>
        /// <typeparam name="T"></typeparam> třída záznamu
        /// <param name="spravce"></param> správce třídy
        /// <param name="parametry"></param> parametry určující zobrazné sloupce a nastavené filtry
        private void PrehledZaznamu<T>(Spravce<T> spravce, string[] parametry)
        {
            // Definice záhlaví a příkazového řádku, deklarace seznamu záznamů pro později
            string zahlavi = ("přehled " + spravce.OznaceniZaznamu[1]).ToUpper();
            string[] nabidka = spravce.PrikazyPrehladZaznamu;
            char[] povoleneKlavesy = spravce.KlavesyPrehledZaznamu;
            string[] nactenaID;

            // Definice pozice prvního vypsaného záznamu a max. délka výpisu
            int prvniVypsanyZaznam = 1;
            int maxDelkaVypisu = nastaveni.maxDelkaVypisuPrehledu;

            char klavesa = 'Q';
            while (klavesa != 'H')
            {
                // Načtení hodnot záznamů dané třídy dle zadaných paramterů
                nactenaID = spravce.VyhledejID(parametry);

                // Vykreslení obrazovky a všech elementů
                VytvorNovouObrazovku(zahlavi, nabidka);
                VypisSeznam(spravce, nactenaID, parametry, prvniVypsanyZaznam, maxDelkaVypisu, "Předchozí", "Další", nastaveni.radekVypisuPrehledu, 0, true);

                klavesa = VyzadatKlavesu(povoleneKlavesy);
                if (klavesa == 'F')
                    parametry = EditujFiltr(spravce, parametry);
                else if (klavesa == 'O')
                {
                    int volba = VyzadejVolbuZaznamu(prvniVypsanyZaznam,
                                    Math.Min(prvniVypsanyZaznam + maxDelkaVypisu - 1, nactenaID.Length), ("volba " + spravce.OznaceniZaznamu[0]).ToUpper());
                    if (volba != int.MaxValue)
                    {
                        // Hledání Id vybraného záznamu ze seznamu načtených id
                        string id = nactenaID[volba - 1];
                        string tag = id.Substring(0, 3);
                        if (tag == "POJ")
                            klavesa = KartaPojistence(id);
                        else if (tag == "SML")
                            klavesa = KartaSmlouvy(id);
                        else if (tag == "UDL")
                            klavesa = KartaUdalosti(id);
                    }
                }
                else if (klavesa == 'N')
                {
                    bool vytvoreno = PridejNovyZaznam(spravce, new string[] {""});
                    if (vytvoreno)
                        KartaPojistence(spravce.VratIdPoslednihoZaznamu());
                }
                    
                else if (klavesa == 'P')
                    prvniVypsanyZaznam = SpoctiVychoziPoziciPredchozi(prvniVypsanyZaznam, maxDelkaVypisu);
                else if (klavesa == 'D')
                    prvniVypsanyZaznam = SpoctiVychoziPoziciDalsi(prvniVypsanyZaznam, maxDelkaVypisu, nactenaID.Length);
            }
        }
        private int SpoctiVychoziPoziciPredchozi(int prvniVypsanyZaznam, int maxDelkaVypisu)
        {
            return Math.Max(prvniVypsanyZaznam - maxDelkaVypisu, 1);
        }
        private int SpoctiVychoziPoziciDalsi(int prvniVypsanyZaznam, int maxDelkaVypisu, int delkaVsechZaznamu)
        {
            return Math.Min(prvniVypsanyZaznam + maxDelkaVypisu, delkaVsechZaznamu);
        }
       
        private char KartaPojistence(string id)
        {
            string zahlavi = "KARTA POJIŠTĚNCE";
            string[] nabidka = new string[] { "Hlavní menu", "Editovat pojištěnce", "Vymazat pojištěnce", "Nová smlouva",
                "otevřít Smlouvu", "otevřít Událost", "Zpět"};
            char[] povoleneKlavesy = new char[] { 'H', 'E', 'V', 'N', 'S', 'U', 'Z', 'R', 'A', 'P', 'D' };

            string[] parametryPojistencu = new string[]
            {
                "ID", "8", "Id", "=", id,  // nutné pro vyfiltrování hodnot pro právě tento jediný záznam
                "Příjmení", "20", "Prijmeni", "", "",
                "Jméno", "20", "Jmeno", "", "",
                "Telefon", "11", "Telefon", "", ""
            };
            string[] parametrySmluv = new string[]
            {
                "ID", "8", "Id", "", "",
                "", "0", "IdPojistence", "", "",
                "Typ Pojistky", "30", "TypPojistky", "", "",
                "Začátek", "12", "Zacatek", "", "",
                "Konec", "12", "Konec", "", "",
                "Výše", "-12", "Vyse", "", ""
            };
            string[] parametryUdalosti = new string[]
            {
                "ID", "8", "Id", "", "",
                "", "0", "IdPojistence", "", "",
                "Název", "30", "Nazev", "", "",
                "Smlouva", "12", "IdSmlouvy", "", "",
                "Datum", "12", "Datum", "", "",
                "Výše", "-12", "Vyse", "", ""
            };
            int poziceSmluv = 1;
            int maxDelkaSmluv = 5;
            int poziceUdalosti = 1;
            int maxDelkaUdalosti = 5;
            int radekSmlouvy = nastaveni.radekPrikazy + 7;
            int radekUdalosti = radekSmlouvy + maxDelkaSmluv + 5;

            char klavesa = 'Q';
            while (klavesa != 'H' && klavesa != 'Z')
            {
                // Načíst příslušného pojištěnce
                string idPojistence = id; //spravcePojistencu.VyhledejHodnotuDleIdZaznamu(id, "Id");
                string prijmeni = spravcePojistencu.VyhledejHodnotuDleIdZaznamu(id, "Prijmeni");
                string jmeno = spravcePojistencu.VyhledejHodnotuDleIdZaznamu(id, "Jmeno");
                string telefon = spravcePojistencu.VyhledejHodnotuDleIdZaznamu(id, "Telefon");

                // Načíst smlouvy vybraného pojištěnce
                parametrySmluv = NastavFiltr(parametrySmluv, "IdPojistence", idPojistence);
                string[] nactenaIdSmluv = spravceSmluv.VyhledejID(parametrySmluv);

                // Načíst události vybraného pojištěnce
                parametryUdalosti = NastavFiltr(parametryUdalosti, "IdPojistence", idPojistence);
                string[] nactenaIdUdalosti = spravceUdalosti.VyhledejID(parametryUdalosti);

                VytvorNovouObrazovku(zahlavi, nabidka);
                Console.CursorTop = nastaveni.radekPrikazy + 2;
                Console.CursorLeft = 0;
                BarvyStandard();
                Console.WriteLine("ID:       {0}", idPojistence);
                Console.WriteLine("Příjmení: {0}", prijmeni);
                Console.WriteLine("Jméno:    {0}", jmeno);
                Console.WriteLine("Telefon:  {0}", telefon);

                int celkovaHodnotaSmluv = spravceSmluv.VratCelkovouHodnotuSmluvDleIdPojistence(idPojistence);
                int celkovaVyseUdalosti = spravceUdalosti.VratCelkovouVysiUdalostiDleIdsSmluv(spravceSmluv.VyhledejID(parametrySmluv));
                NastavKurzor(nastaveni.radekPrikazy + 4, nastaveni.sirka / 2);
                Console.Write("Celková hodnota uzavřených smluv [Kč]: {0}", celkovaHodnotaSmluv);
                NastavKurzor(nastaveni.radekPrikazy + 5, nastaveni.sirka / 2);
                Console.Write("Celková výše pojistných událostí [Kč]: {0}", celkovaVyseUdalosti);

                Console.CursorTop = radekSmlouvy;
                int y = nastaveni.radekPrikazy + 7;
                int x = 0;
                VypisText(y, x, BarvyStandard, "Uzavřené smlouvy:");
                VypisSeznam(spravceSmluv, nactenaIdSmluv, parametrySmluv, poziceSmluv, maxDelkaSmluv, "pRedchozí", "dAlší", y + 1, x);

                y = radekUdalosti;
                VypisText(y, x, BarvyStandard, "Evidované pojistné události:");
                VypisSeznam(spravceUdalosti, nactenaIdUdalosti, parametryUdalosti, poziceUdalosti, maxDelkaUdalosti, "Předchozí", "Další", y + 1, x);

                klavesa = VyzadatKlavesu(povoleneKlavesy);
                if (klavesa == 'E')
                    EditujZaznam(spravcePojistencu, idPojistence);
                else if (klavesa == 'V')
                {
                    bool smazano = VymazZaznam(spravcePojistencu, idPojistence);
                    if (smazano)
                        klavesa = 'Z';
                }
                else if (klavesa == 'N')
                {
                    bool vytvoreno = PridejNovyZaznam(spravceSmluv, new string[] {idPojistence});
                    if (vytvoreno)
                        KartaSmlouvy(spravceSmluv.VratIdPoslednihoZaznamu());
                }
                else if (klavesa == 'S')
                {
                    int volba = VyzadejVolbuZaznamu(poziceSmluv, Math.Min(poziceSmluv + maxDelkaSmluv - 1, nactenaIdSmluv.Length), "volba smlouvy");
                    if (volba != int.MaxValue)
                        klavesa = KartaSmlouvy(nactenaIdSmluv[volba - 1]);
                }

                else if (klavesa == 'U')
                {
                    int volba = VyzadejVolbuZaznamu(poziceUdalosti, Math.Min(poziceUdalosti + maxDelkaUdalosti - 1, nactenaIdUdalosti.Length), "volba smlouvy");
                    if (volba != int.MaxValue)
                        klavesa = KartaUdalosti(nactenaIdUdalosti[volba - 1]);
                }
                else if (klavesa == 'R')
                    poziceSmluv = SpoctiVychoziPoziciPredchozi(poziceSmluv, maxDelkaSmluv);
                else if (klavesa == 'A')
                    poziceSmluv = SpoctiVychoziPoziciDalsi(poziceSmluv, maxDelkaSmluv, nactenaIdSmluv.Length);
                else if (klavesa == 'P')
                    poziceUdalosti = SpoctiVychoziPoziciPredchozi(poziceUdalosti, maxDelkaUdalosti);
                else if (klavesa == 'D')
                    poziceUdalosti = SpoctiVychoziPoziciDalsi(poziceUdalosti, maxDelkaUdalosti, nactenaIdUdalosti.Length);
            }
            return klavesa;
        }

        private char KartaSmlouvy(string id)
        {
            string zahlavi = "KARTA SMLOUVY";
            string[] nabidka = new string[] { "Hlavní menu", "Editovat smlouvu", "Vymazat smlouvu",
                "otevřít Pojištěnce", "Nová událost", "otevřít Událost", "Zpět"};
            char[] povoleneKlavesy = new char[] { 'H', 'E', 'V', 'P', 'N', 'U', 'P', 'D', 'Z' };
            string[] parametrySmluv = new string[]
            {
                "ID", "8", "Id", "=", id,  // nutné pro vyfiltrování hodnot pro právě tento jediný záznam
                "Typ pojistky", "30", "TypPojistky", "", "",
                "Začátek", "12", "Zacatek", "", "",
                "Konec", "12", "Konec", "", "",
                "Výše", "-12", "Vyse", "", "",
                "", "0", "IdPojistence", "", ""
            };
            string[] parametryUdalosti = new string[]
            {
                "ID", "8", "Id", "", "",
                "Název", "30", "Nazev", "", "",
                "Datum", "12", "Datum", "", "",
                "Výše", "-12", "Vyse", "", "",
                "", "0", "IdSmlouvy", "", ""
            };
            int poziceUdalosti = 1;
            int maxDelkaUdalosti = 10;
            int radekUdalosti = 11;

            char klavesa = 'Q';
            while (klavesa != 'H' && klavesa != 'Z')
            {
                // Načíst příslušnou smlouvu
                string[] nactenaIdSmluv = spravceSmluv.VyhledejID(parametrySmluv);
                string idSmlouvy = spravceSmluv.VyhledejHodnotuDleIdZaznamu(id, "Id");
                string typPojistky = spravceSmluv.VyhledejHodnotuDleIdZaznamu(id, "TypPojistky");
                string zacatek = spravceSmluv.VyhledejHodnotuDleIdZaznamu(id, "Zacatek");
                string konec = spravceSmluv.VyhledejHodnotuDleIdZaznamu(id, "Konec");
                string vyse = spravceSmluv.VyhledejHodnotuDleIdZaznamu(id, "Vyse");
                string idPojistence = spravceSmluv.VyhledejHodnotuDleIdZaznamu(id, "IdPojistence");
                string jmenoPojistence = spravcePojistencu.VyhledejHodnotuDleIdZaznamu(idPojistence, "CeleJmeno");

                // Načíst události vybrané smlouvy
                parametryUdalosti = NastavFiltr(parametryUdalosti, "IdSmlouvy", id);
                string[] nactenaIdUdalosti = spravceUdalosti.VyhledejID(parametryUdalosti);

                VytvorNovouObrazovku(zahlavi, nabidka);
                NastavKurzor(nastaveni.radekPrikazy + 2, 0);
                BarvyStandard();
                Console.WriteLine("ID:                   {0}", id);
                Console.WriteLine("Typ pojistné smlouvy: {0}", typPojistky);
                Console.WriteLine("Jméno pojištěnce:     {0}", jmenoPojistence);
                Console.WriteLine("Začátek smlouvy:      {0}", zacatek);
                Console.WriteLine("Konec smlouvy:        {0}", konec);
                Console.WriteLine("Výše pojistky [Kč]:   {0}", vyse);

                int y = radekUdalosti;
                int x = 0;
                VypisText(y, x, BarvyStandard, "Evidované pojistné události:");
                VypisSeznam(spravceUdalosti, nactenaIdUdalosti, parametryUdalosti, poziceUdalosti, maxDelkaUdalosti, "Předchozí", "Další", y + 1, x);

                klavesa = VyzadatKlavesu(povoleneKlavesy);
                if (klavesa == 'E')
                    EditujZaznam(spravceSmluv, id);
                else if (klavesa == 'V')
                {
                    bool smazano = VymazZaznam(spravceSmluv, id);
                    if (smazano)
                        klavesa = 'Z';
                }
                else if (klavesa == 'P')
                    klavesa = KartaPojistence(idPojistence);
                else if (klavesa == 'N')
                {
                    bool vytvoreno = PridejNovyZaznam(spravceUdalosti, new string[] { idPojistence, id });
                    if (vytvoreno)
                        KartaUdalosti(spravceUdalosti.VratIdPoslednihoZaznamu());
                }
                else if (klavesa == 'U')
                {
                    int volba = VyzadejVolbuZaznamu(poziceUdalosti, Math.Min(poziceUdalosti + maxDelkaUdalosti - 1, nactenaIdUdalosti.Length), "volba smlouvy");
                    if (volba != int.MaxValue)
                        klavesa = KartaUdalosti(nactenaIdUdalosti[volba - 1]);
                }
                else if (klavesa == 'P')
                    poziceUdalosti = SpoctiVychoziPoziciPredchozi(poziceUdalosti, maxDelkaUdalosti);
                else if (klavesa == 'D')
                    poziceUdalosti = SpoctiVychoziPoziciDalsi(poziceUdalosti, maxDelkaUdalosti, nactenaIdUdalosti.Length);
            }
            return klavesa;
        }

        private char KartaUdalosti(string id)
        {
            string zahlavi = "KARTA UDÁLOSTI";
            string[] nabidka = new string[] { "Hlavní menu", "Editovat událost", "Vymazat událost", "otevřít Pojištěnce",
                    "otevřít Smlouvu", "Zpět"};
            char[] povoleneKlavesy = new char[] { 'H', 'E', 'V', 'P', 'S', 'Z' };
            string[] parametryUdalosti = new string[]
            {
                "ID", "8", "Id", "", id,  // nutné pro vyfiltrování hodnot pro právě tento jediný záznam
                "Název", "30", "Nazev", "", "",
                "Datum", "12", "Datum", "", "",
                "Výše", "-12", "Vyse", "", "",
                "", "0", "IdSmlouvy", "", "",
                "Popis", "30", "Popis","", ""
            };

            char klavesa = 'Q';
            while (klavesa != 'H' && klavesa != 'Z')
            {
                // Načíst příslušnou událost
                string[] nactenaIdUdalosti = spravceUdalosti.VyhledejID(parametryUdalosti);
                string idUdalosti = spravceUdalosti.VyhledejHodnotuDleIdZaznamu(id, "Id");
                string nazev = spravceUdalosti.VyhledejHodnotuDleIdZaznamu(id, "Nazev");
                string datum = spravceUdalosti.VyhledejHodnotuDleIdZaznamu(id, "Datum");
                string vyse = spravceUdalosti.VyhledejHodnotuDleIdZaznamu(id, "Vyse");
                string popis = spravceUdalosti.VyhledejHodnotuDleIdZaznamu(id, "Popis");

                string idSmlouvy = spravceUdalosti.VyhledejHodnotuDleIdZaznamu(id, "IdSmlouvy");
                string typ = spravceSmluv.VyhledejHodnotuDleIdZaznamu(idSmlouvy, "TypPojistky");

                string idPojistence = spravceSmluv.VyhledejHodnotuDleIdZaznamu(idSmlouvy, "IdPojistence");
                string jmenoPojistence = spravcePojistencu.VyhledejHodnotuDleIdZaznamu(idPojistence, "CeleJmeno");

                VytvorNovouObrazovku(zahlavi, nabidka);
                NastavKurzor(nastaveni.radekPrikazy + 2, 0);
                BarvyStandard();
                Console.WriteLine("ID:                          {0}", id);
                Console.WriteLine("Název události:              {0}", nazev);
                Console.WriteLine("Jméno pojištěnce:            {0}", jmenoPojistence);
                Console.WriteLine();
                Console.WriteLine("Číslo pojistné smlouvy:      {0}", idSmlouvy);
                Console.WriteLine("Typ pojistné události:       {0}", typ);
                Console.WriteLine();
                Console.WriteLine("Datum pojistené události:    {0}", datum);
                Console.WriteLine("Výše pojistné události [Kč]: {0}", vyse);
                Console.WriteLine();
                Console.WriteLine("Popis pojistné události:     {0}", popis);

                klavesa = VyzadatKlavesu(povoleneKlavesy);
                if (klavesa == 'E')
                    EditujZaznam(spravceUdalosti, id);
                else if (klavesa == 'V')
                {
                    bool smazano = VymazZaznam(spravceUdalosti, id);
                    if (smazano)
                        klavesa = 'Z';
                }
                else if (klavesa == 'S')
                    klavesa = KartaSmlouvy(idSmlouvy);
                else if (klavesa == 'P')
                    klavesa = KartaPojistence(idPojistence); 
            }
            return klavesa;
        }

        private string[] VratKazdouXtouPolozkuZpole(string[] pole, int sirkaSady, int index)
        {
            List<string> novePole = new List<string>();
            for (int i = index; i < pole.Length; i += sirkaSady)
                novePole.Add(pole[i]);
            return novePole.ToArray();
        }
        private int VyzadejHodnotuVRozmezi(int y, int x, NastavBarvu barva, int min, int max)
        {
            int volba = -1;
            while ((volba < min || volba > max) && volba != int.MaxValue)
            {
                string input = VyzadejText(y, x, barva, true);
                if (input != "")
                {
                    if(int.TryParse(input, out int a))
                        volba = a;
                }
                else
                    volba = int.MaxValue;
            }
            return volba;
        }
        private void OznacVybranouPolozku(int volba, string[] nazvyPoli, int y, int x)
        {
            int index = volba - 1;
            string cislovanyNazevPole = String.Format("{0}. {1}: ", volba, nazvyPoli[index]);
            VypisText(y, x, BarvyReversZvyrazneni, cislovanyNazevPole);
        }
        private string EditujVybranouPolozku(int volba, int y, int x)
        {
            return VyzadejText(y, x, BarvyReversEdit);   
        }

        private void NastavKurzor(int y, int x)
        {
            Console.CursorTop = y;
            Console.CursorLeft = x;
        }
        private string VyzadejText(int y, int x, NastavBarvu barva, bool predtimSmazat = false)
        {
            if (predtimSmazat)
                VypisText(y, x, barva, "   ");
            NastavKurzor(y, x);
            barva();
            string input = Console.ReadLine();

            // Odstraň před návratem případné středníky
            return input.Replace(";", "").Trim();
        }
        
        private void VypisSloupecPodokna(int y, int x, int maxSirka, NastavBarvu barva, string[] texty, bool cislovany = false)
        {
            for (int i = 0; i < texty.Length; i++)
            {
                string text = texty[i].PadRight(maxSirka).Substring(0, maxSirka);
                if (cislovany)
                    text = String.Format("{0}. {1}: ", i + 1, texty[i]);
                VypisText(y + i, x, barva, text);
            }
        }

        private string[] EditujFiltr<T>(Spravce<T> spravce, string[] parametry)
        {
            string nadpis = "NASTAVENÍ FILTRU";
            string text1 = String.Format("Zadejte # pole k úpravě filtru: ", 1, parametry.Length / nastaveni.delkaParametru);
            string text2 = "(prázdná hodnota/ENTER = zpět)";            

            // Sestavení polí se statickými texty jednotlivých sloupců
            string[] nazvyPoli = VratKazdouXtouPolozkuZpole(parametry, nastaveni.delkaParametru, nastaveni.indexNazev);

            // Definice šířky okna a sloupců
            int sirkaSloupce1 = 0;
            foreach (string s in nazvyPoli)
                sirkaSloupce1 = Math.Max(sirkaSloupce1, s.Length);
            sirkaSloupce1 += 3; // kvůli číslování

            int sirkaSloupce2 = 2;

            int sirkaSloupce3 = 0;
            string[] sirkyPoli = VratKazdouXtouPolozkuZpole(parametry, nastaveni.delkaParametru, nastaveni.indexSirka);
            foreach (string s in sirkyPoli)
                sirkaSloupce3 = Math.Max(sirkaSloupce3, Math.Abs(int.Parse(s)));

            // Nastavení podokna
            int x = 0;
            int y = 0;
            int sirka = nastaveni.odsazeni * 4 + sirkaSloupce1 + sirkaSloupce2 + sirkaSloupce3;
            int vyska = parametry.Length / nastaveni.delkaParametru + nastaveni.odsazeni * 2 + 2;

            int volba = -1;
            while (volba != int.MaxValue)
            {
                // Sestavení polí s proměnnými texty jednotlivých sloupců
                // Musí být až zde ve smyčce, aby se vykreslovaly vždy aktuální hodnoty
                string[] hodnotyOperatoru = VratKazdouXtouPolozkuZpole(parametry, nastaveni.delkaParametru, nastaveni.indexPorovnani);
                string[] hodnotyFiltru = VratKazdouXtouPolozkuZpole(parametry, nastaveni.delkaParametru, nastaveni.indexFiltr);

                (int y, int x) souradnice = VytvorPodokno(sirka, vyska, nadpis, text1, text2);
                x = souradnice.x;
                y = souradnice.y;

                int x1 = x + nastaveni.odsazeni;
                int x2 = x1 + sirkaSloupce1 + nastaveni.odsazeni;
                int x3 = x2 + sirkaSloupce2 + nastaveni.odsazeni;
                int y1 = y + nastaveni.odsazeni + 2;

                VypisSloupecPodokna(y1, x1, sirkaSloupce1, BarvyRevers, nazvyPoli, true);
                VypisSloupecPodokna(y1, x2, sirkaSloupce2, BarvyReversEditSlabe, hodnotyOperatoru);
                VypisSloupecPodokna(y1, x3, sirkaSloupce3, BarvyReversEditSlabe, hodnotyFiltru);

                VypisText(y + vyska - 2, x1, BarvyReversSlabe, "(operátory: >, >=, <=, <, =, !=, %, !%)");

                // Dotaz na volbu, kterou vlastnost chce uživatel editovat
                volba = VyzadejHodnotuVRozmezi(y + nastaveni.odsazeni, x + nastaveni.odsazeni + text1.Length, BarvyRevers, 1, nazvyPoli.Length);

                // Editace správně zvolené vlastnosti
                if (volba != int.MaxValue)
                {
                    OznacVybranouPolozku(volba, nazvyPoli, y1 + volba - 1, x1);

                    // Editace operátorů a kontrola na správnost zadání
                    int index = (volba - 1) * nastaveni.delkaParametru;
                    string[] validniOperatory = spravce.VratMnozinuOperatoruProDanouVlastnost(parametry[index + nastaveni.indexAtribut]);
                    string novaHodnotaOperatoru = "q";
                    while (!validniOperatory.Contains(novaHodnotaOperatoru))
                        novaHodnotaOperatoru = EditujVybranouPolozku(volba, y1 + volba - 1, x2);

                    // Editace hodnot a kontrola na správnost zadání
                    string novaHodnotaFiltru = "";
                    bool spravnostZadani = false;
                    while (!spravnostZadani)
                    {
                        novaHodnotaFiltru = EditujVybranouPolozku(volba, y1 + volba - 1, x3);
                        if (novaHodnotaFiltru == "")
                            spravnostZadani = true;
                        else
                            spravnostZadani = spravce.ZkontrolujValidituVstupu(parametry[index + nastaveni.indexAtribut], novaHodnotaFiltru);
                    }

                    // Vložení upravených hodnot do sady parametrů
                    parametry[(volba - 1) * nastaveni.delkaParametru + nastaveni.indexPorovnani] = novaHodnotaOperatoru;
                    parametry[(volba - 1) * nastaveni.delkaParametru + nastaveni.indexFiltr] = novaHodnotaFiltru;

                    // Reset volby při návratu do podokna, aby to se hned zavřelo
                    volba = -1;
                }
            }
            BarvyStandard();
            return parametry;
        }

        private string[] NastavFiltr(string[] parametry, string atribut, string id, string porovani = "=")
        {
            for (int i = 0; i < parametry.Length; i += nastaveni.delkaParametru)
            {
                if (parametry[i + nastaveni.indexAtribut] == atribut)
                {
                    parametry[i + nastaveni.indexFiltr] = id;
                    parametry[i + nastaveni.indexPorovnani] = porovani;
                    break;
                }
            }
            return parametry;
        }

        private bool PridejNovyZaznam<T>(Spravce<T> spravce, string[] doplnkoveHodnoty)
        {
            string[] hodnoty = EditujZaznam(spravce, "bez ID", true);
            hodnoty = spravce.UpravHodnoty(hodnoty, doplnkoveHodnoty);
            bool vytvoreno = spravce.Pridej(hodnoty);
            VytvorOknoOznameni(spravce.Zprava);
            return vytvoreno;     
        }
       
        private string[] EditujZaznam<T>(Spravce<T> spravce, string id, bool novyZaznam = false)
        {
            string[] vlastnosti = spravce.VratEditovatelneVlasnosti();
            if (novyZaznam)
                vlastnosti = spravce.VratZakladniVlastnost();
            int pocetPoliProNovyZaznam = vlastnosti.Length / 2;
            int automatickaVolba = 1;
            string[] noveHodnoty = new string[vlastnosti.Length / 2];

            string nadpis = "EDITACE ZÁZNAMU";
            if (novyZaznam)
                nadpis = "PŘIDAT NOVÝ ZÁZNAM";
            string text1 = String.Format("Zadejte # pole k editaci: ", 1, vlastnosti.Length);
            string text2 = "(prázdná hodnota/ENTER = zpět)";
            if (novyZaznam)
            {
                text1 = "Vyplňte povinná pole pro";
                text2 = "přidání nového záznamu.";
            }

            // Sestavení polí se statickými texty jednotlivých sloupců
            string[] nazvyPoli = VratKazdouXtouPolozkuZpole(vlastnosti, 2, 1);

            // Definice šířky okna a sloupců
            int maxSirkaSloupec1 = 0;
            foreach (string s in nazvyPoli)
                maxSirkaSloupec1 = Math.Max(maxSirkaSloupec1, s.Length);
            maxSirkaSloupec1 += 3; // kvůli číslování
            int maxSirkaSloupec2 = 35;
            int sirka = nastaveni.odsazeni * 3 + maxSirkaSloupec1 + maxSirkaSloupec2;
            int vyska = vlastnosti.Length / 2 + 6;

            // Definice šířky okna a sloupců
            int x = 0;
            int y = 0;

            int volba = -1;
            while (volba != int.MaxValue)
            {
                if (!novyZaznam || (novyZaznam && automatickaVolba == 1))
                {
                    (int yy, int xx) souradnice = VytvorPodokno(sirka, vyska, nadpis, text1, text2);
                    y = souradnice.yy;
                    x = souradnice.xx;
                }
                int xSloupec1 = x + nastaveni.odsazeni;
                int xSloupec2 = xSloupec1 + maxSirkaSloupec1 + nastaveni.odsazeni;

                if (!novyZaznam || (novyZaznam && automatickaVolba == 1))
                {
                    VypisSloupecPodokna(y + 4, xSloupec1, maxSirkaSloupec1, BarvyRevers, nazvyPoli, true);
                    string[] prazdnaPole = VytvorPoleStringuZadaneDelky(vlastnosti.Length / 2, maxSirkaSloupec2);
                    VypisSloupecPodokna(y + 4, xSloupec2, maxSirkaSloupec2, BarvyReversEditSlabe, prazdnaPole);
                }

                if (!novyZaznam)
                // Sestavení polí s proměnnými texty jednotlivých sloupců
                // Musí být až zde ve smyčce, aby se vykreslovaly vždy aktuální hodnoty
                {
                    List<string> seznam = new List<string>();
                    for (int i = 0; i < vlastnosti.Length; i += 2)
                        seznam.Add(spravce.VyhledejHodnotuDleIdZaznamu(id, vlastnosti[i]));
                    string[] hodnotyPoli = seznam.ToArray();
                    VypisSloupecPodokna(y + 4, xSloupec2, maxSirkaSloupec2, BarvyReversSlabe, hodnotyPoli);
                }

                // Dotaz na volbu, kterou vlastnost chce uživatel editovat
                if (!novyZaznam)
                    volba = VyzadejHodnotuVRozmezi(y + 2, x + 2 + text1.Length, BarvyRevers, 1, nazvyPoli.Length);
                else
                    volba = automatickaVolba;

                // Editace správně zvolené vlastnosti
                if (volba != int.MaxValue)
                {
                    OznacVybranouPolozku(volba, nazvyPoli, y + 4 + volba - 1, xSloupec1);
                    string novaHodnota = EditujVybranouPolozku(volba, y + 4 + volba - 1, xSloupec2);

                    // Zaslání správci, aby ověřil správný datový typ a případně hodnotu do vlastnosti dosadil
                    if (novyZaznam)
                    {
                        bool spravnostVstupu = spravce.ZkontrolujValidituVstupu(vlastnosti[(volba - 1) * 2], novaHodnota);
                        if (spravnostVstupu)
                        {
                            noveHodnoty[automatickaVolba - 1] = novaHodnota;
                            automatickaVolba++;
                        }
                    }
                    else
                        spravce.Edituj(id, vlastnosti[(volba - 1) * 2], novaHodnota);
                        

                    // Reset volby při návratu do podokna
                    // V případě naplnění všech povinných hodnot u 
                    //volba = 0;
                    if (novyZaznam && automatickaVolba > vlastnosti.Length / 2)
                        volba = int.MaxValue;
                }
            }

            BarvyStandard();
            return noveHodnoty;
        }

        private string[] VytvorPoleStringuZadaneDelky(int pocet, int delka)
        {
            string[] pole = new string[pocet];
            string rezezec = VytvorRetezecZnaku(' ', delka);
            for (int i = 0; i < pole.Length; i++)
                pole[i] = rezezec;
            return pole;
        }
        private bool VymazZaznam<T>(Spravce<T> spravce, string id)
        {
            bool smazano = false;

            // Nastavení podokna

            string nadpis = "SMAZÁNÍ ZÁZNAMU";
            string text1 = String.Format("Opravdu smazat záznam \"{0}\"?", spravce.VyhledejHodnotuDleIdZaznamu(id, "NazevZaznamu").Substring(0, 20));
            string text2 = "A-ano: ";
            int sirka = text1.Length + 4;
            int vyska = 6;

            string[] parametrySmluv = new string[]
            {
                "IdPojistence", "", "IdPojistence", "=", id
            };
            string[] parametryUdalosti = new string[]
            {
                "IdSmlouvy", "", "IdSmlouvy", "=", id
            };

            (int y, int x) pozice = VytvorPodokno(sirka, vyska, nadpis, text1, text2);

            string input = VyzadejText(pozice.y + 3, pozice.x + 2 + text2.Length, BarvyReversZvyrazneni).ToUpper();
            if (input == "A")
            {
                bool navazanePodzaznamy = false;
                if (id.Substring(0, 3) == "POJ")
                {
                    if (spravceSmluv.VyhledejID(parametrySmluv).Length > 0)
                        navazanePodzaznamy = true;
                }
                else if (id.Substring(0,3) == "SML")
                {
                    if (spravceUdalosti.VyhledejID(parametryUdalosti).Length > 0)
                        navazanePodzaznamy = true;
                }

                string text = "";
                if (navazanePodzaznamy)
                {
                    string nazevZaznamu = spravce.VyhledejHodnotuDleIdZaznamu(id, "NazevZaznamu");
                    text = String.Format("Záznam {0} se nepodařilo smazat. Smažte nejprve jiné záznamy, které na něj odkazují.", nazevZaznamu);
                }
                else
                {
                    smazano = spravce.Smaz(id);
                    text = spravce.Zprava;
                }
                VytvorOknoOznameni(text);
            }

            return smazano;
        }

        private int VypisSeznam<T>(Spravce<T> spravce, string[] poleId, string[] parametry, int pocatek, int delka, string text2, string text4, int y, int x, bool pridatRadekFiltru = false)
        {
            // Vypíše záhlaví a případně řádek filtrů a zároveň zjistí šířku výpisu
            int sirka = VypisText(y, x, BarvyStandard, VypisZahlaviSeznamu(parametry));
            if (pridatRadekFiltru)
            {
                y++;
                VypisText(y, x, BarvyStandardSlabe, VypisZahlaviSeznamu(parametry, true));
            }
            VypisText(y + 1, x, BarvyStandard, VytvorRetezecZnaku('-', sirka));

            // Sestavování jednotlivých řádek výpisu
            string radka = "";
            string id = "";
            int pocetRadek = Math.Min(delka, poleId.Length - pocatek + 1);
            for (int i = pocatek - 1; i < pocatek + pocetRadek - 1; i++)
            {
                id = poleId[i];
                radka = ((i + 1) + ".").PadRight(5);
                for (int j = 0; j < parametry.Length; j += nastaveni.delkaParametru)
                {
                    string hodnota = spravce.VyhledejHodnotuDleIdZaznamu(id, parametry[j + nastaveni.indexAtribut]);
                    int zarovnani = int.Parse(parametry[j + nastaveni.indexSirka]);
                    hodnota = hodnota.Substring(0, Math.Min(Math.Abs(zarovnani), hodnota.Length));
                    if (zarovnani > 0)
                        radka += hodnota.PadRight(zarovnani);
                    else if (zarovnani < 0)
                        radka += hodnota.PadLeft(zarovnani * -1);
                }
                VypisText(y + 2 + i - pocatek + 1, x, BarvyStandard, radka);
            }

            // Sestavení patičky seznamu
            string text1 = String.Format("     zobrazeno {0}/{1} záznam(ů)", pocetRadek, poleId.Length);
            string text3 = "   ";
            if (pocatek == 1)
                text2 = "";
            if (pocatek + pocetRadek - 1 >= poleId.Length)
                text4 = "";
            if (text4 == "")
                text3 = "";
            text1 = text1.PadRight(sirka - text2.Length - text3.Length - text4.Length);
            VypisText(y + 2 + pocetRadek, x, BarvyStandard, text1);
            VypisPrikaz(text2);
            VypisText(y + 2 + pocetRadek, x + text1.Length + text2.Length, BarvyStandard, text3);
            VypisPrikaz(text4);

            int delkaVypisu = 2 + pocetRadek + 1;
            if (pridatRadekFiltru)
                delkaVypisu++;
            return delkaVypisu;
        }

        private string VypisZahlaviSeznamu(string[] parametry, bool radekFiltru = false)
        {
            string radka = "#";
            int index = nastaveni.indexNazev;
            if (radekFiltru)
            {
                radka = " ";
                index = nastaveni.indexFiltr;
            }
            radka = radka.PadRight(5);

            for (int i = 0; i < parametry.Length; i += nastaveni.delkaParametru)
            {
                int zarovnani = int.Parse(parametry[i + nastaveni.indexSirka]);
                string text = parametry[i + index];
                if (radekFiltru)
                    text = parametry[i + index - 1] + parametry[i + index];
                if (zarovnani > 0)
                    radka += text.PadRight(zarovnani);
                else if (zarovnani < 0)
                    radka += text.PadLeft(zarovnani * -1);
            }

            return radka;
        }

        private string VytvorRetezecZnaku(char znak, int delka)
        {
            string retezec = "";
            for (int i = 0; i < delka; i++)
                retezec += znak;
            return retezec;
        }
    }
}
