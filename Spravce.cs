using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace pojisteni
{
    internal abstract class Spravce<T>
    {
        // Deklarace databáze záznamů
        protected List<T> databaze;

        // Deklaruje jméno souboru .csv, kde budou záznamy uloženy
        protected string soubor;

        // Deklarace instance nastavení
        protected Nastaveni nastaveni;

        // Ryze lingvistické pole o délce 2
        // index 0 = záznam v 2. pádě jednotného čísla
        // index 1 = záznam v 2. pádě množného čísla
        public string[] OznaceniZaznamu { get { return oznaceniZaznamu; } }
        protected string[] oznaceniZaznamu;

        // Pole příkazů a funkčních kláves pro generickou třídu Aplikace.PrehledZaznamu
        public string[] PrikazyPrehladZaznamu { get { return prikazyPrehledZaznamu; } }
        public char[] KlavesyPrehledZaznamu { get { return klavesyPrehledZaznamu; } }
        protected string[] prikazyPrehledZaznamu;
        protected char[] klavesyPrehledZaznamu;
       
        // Počítadlo záznamů, zvýší se o jedna s každým novým záznamem
        protected int counter;
        public int Counter {
            get
            {
                return counter;
            }
            protected set
            {
                counter = value;
            }
        }
        // Deklarace proměnné, kam bude správce ukládat zprávy o vykonaných metodách
        // Bude-li aplikace chtít, může zprávy číst a použít
        public string Zprava { get
            {
                return zprava; ;
            } protected set
            {
                zprava = value;
            }
        }
        protected string zprava = "";

        /// <summary>
        /// Konstruktor třídy Spravce
        /// Vytvoří instanci databáze spravovaných záznamů a nastaví coutner na nula
        /// </summary>
        /// <param name="nastaveni"></param>
        public Spravce(Nastaveni nastaveni)
        {
            databaze = new List<T>();
            counter = 0;
            this.nastaveni = nastaveni;
        }
        /// <summary>
        /// Upraví a zvaliduje hodnoty předím, než jsou poslány na metodu Pridej
        /// </summary>
        /// <param name="hodnoty"></param>
        /// <param name="doplnkoveHodnoty"></param>
        /// <returns></returns>
        public abstract string[] UpravHodnoty(string[] hodnoty, string[] doplnkoveHodnoty);
        /// <summary>
        /// Zavliduje zadané parametry a jsou-li ok, vytvoří novou instanci záznamu a přidá ji do databáze
        /// </summary>
        /// <param name="hodnoty"></param> pole hodnot potřebné pro vytvoření instance záznamu
        /// <param name="novyZaznam"></param> true = nový záznam a číslování dle couteru; false = vytváření záznamů dle uložených dat v csv
        /// <returns></returns>
        public abstract bool Pridej(string[] hodnoty, bool novyZaznam = true);
        /// <summary>
        /// Smaže záznam. Předtím ověří, zda na záznam nejsou navázány jiné záznamy, pak by nebylo smazání možné
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Smaz(string id)
        {
            T zaznam = VratZaznamDleId(id);
            string nazevZaznamu = (zaznam as IZaznam).NazevZaznamu;
            bool smazano = databaze.Remove(zaznam);
            Zprava = String.Format("Záznam {0} úspěšně smazán.", nazevZaznamu);
            if (!smazano)
                Zprava = String.Format("Záznam {0} se nepodařilo smazat.", nazevZaznamu);
            return smazano;
        }
        /// <summary>
        /// Editace jediné vlastnosti záznamu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vlastnost"></param>
        /// <param name="hodnotaVstup"></param>
        public void Edituj(string id, string vlastnost, string hodnotaVstup)
        {
            // Nalezení záznamu dle jeho ID a načtení editavané vlastnosti
            T zaznam = VratZaznamDleId(id);
            PropertyInfo atribut = typeof(T).GetProperty(vlastnost);

            // Nalezení správného datového typu vlasnosti a vložení hodnoty ve správném datovém typu
            if (atribut.PropertyType.Name == "DateTime")
            {
                DateTime hodnota = DateTime.Parse(hodnotaVstup);
                atribut.SetValue(zaznam, hodnota);
            }
            else if (atribut.PropertyType.Name == "Int32")
            {
                int hodnota = int.Parse(hodnotaVstup);
                atribut.SetValue(zaznam, hodnota);
            }
            else
            {
                atribut.SetValue(zaznam, hodnotaVstup);
            }
        }
        /// <summary>
        /// Zkontroluje, zda vstup odpovídá typu vlastnosti
        /// </summary>
        /// <param name="vlastnost"></param>
        /// <param name="hodnota"></param>
        /// <returns></returns>
        public bool ZkontrolujValidituVstupu(string vlastnost, string hodnota)
        {
            // Načtení kontrolované vlastnnosti
            PropertyInfo atribut = typeof(T).GetProperty(vlastnost);

            // Validace, zda vstup odpovídá datovému typu vlastnosti
            if (atribut.PropertyType.Name == "DateTime")
                return DateTime.TryParse(hodnota, out DateTime aaa);
            else if (atribut.PropertyType.Name == "Int32")
                return int.TryParse(hodnota, out int aaa);
            else
                return true;
        }
        /// <summary>
        /// Vrátí množinu logických operátorů pro filtraci, které odpovídají datovému typu zvolené vlastnosti
        /// </summary>
        /// <param name="vlastnost"></param>
        /// <returns></returns>
        public string[] VratMnozinuOperatoruProDanouVlastnost(string vlastnost)
        {
            // Načtení zvolené vlastnnosti
            PropertyInfo atribut = typeof(T).GetProperty(vlastnost);

            // Nalezení odpovídajícího datového typu a vrácení množiny odpovídajících vlastností
            if (atribut.PropertyType.Name == "DateTime")
                return new string[] { ">", ">=", "<=", "<", "=", "!%", "" };
            else if (atribut.PropertyType.Name == "int")
                return new string[] { ">", ">=", "<=", "<", "=", "!%", "" };
            else
                return new string[] { "=", "!=", "%", "!%", "" };
        }
        /// <summary>
        /// Vrátí ID posledního záznamu v databázi
        /// </summary>
        /// <returns></returns>
        public string VratIdPoslednihoZaznamu()
        {
            IZaznam posledniZaznam = databaze.Last() as IZaznam;
            return posledniZaznam.Id;
        }
        /// <summary>
        /// Vyhledá záznam dle zadaného ID a vrátí hodnotu zvolené vlastnosti jako string
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vlastnost"></param>
        /// <returns></returns>
        public string VyhledejHodnotuDleIdZaznamu(string id, string vlastnost)
        {
            // Nalezení záznamu s odpovídajícím ID a požadované vlastnosti
            T zaznam = VratZaznamDleId(id);
            PropertyInfo atribut = typeof(T).GetProperty(vlastnost);

            // Vrácení stringu
            if (atribut.PropertyType.Name == "DateTime")
            {
                // Převod na krátký string datumu. Trochu krkolomně, ale účel to splňuje
                string s = typeof(T).GetProperty(vlastnost).GetValue(zaznam).ToString();
                DateTime datum = DateTime.Parse(s);
                return datum.ToShortDateString();
            }    
            else
                return typeof(T).GetProperty(vlastnost).GetValue(zaznam).ToString();
        }
        /// <summary>
        /// Vrátí pole všech ID záznamů, které odpovidají zadaným parametrům
        /// </summary>
        /// <param name="parametry"></param>
        /// <param name="pozice"></param>
        /// <returns></returns>
        public string[] VyhledejID(string[] parametry)
        {
            List<T> vyfiltrovaneZaznamy = NajdiDleFiltru(parametry);
            List<string> seznamId = new List<string>();
            foreach (T zaznam in vyfiltrovaneZaznamy)
                seznamId.Add(((IZaznam)zaznam).Id);
            return seznamId.ToArray();
        }
        protected List<T> NajdiDleFiltru(string[] parametry)
        {
            List<T> vyfiltrovaneZaznamy = new List<T>();

            for (int j = 0; j < databaze.Count(); j++)
            {
                T o = databaze[j];
                bool vyhovuje = true;
                for (int i = 0; i < parametry.Length; i += nastaveni.delkaParametru)
                {
                    PropertyInfo atribut = typeof(T).GetProperty(parametry[i + nastaveni.indexAtribut]);
                    if (parametry[i + nastaveni.indexFiltr] != "")
                    {
                        if (atribut.PropertyType.Name == "DateTime")
                        {
                            DateTime hodnota = DateTime.Parse(atribut.GetValue(o).ToString());
                            if (DateTime.TryParse(parametry[i + nastaveni.indexFiltr], out DateTime filtr))
                            {
                                vyhovuje = parametry[i + nastaveni.indexPorovnani] switch
                                {
                                    ">" => hodnota > filtr,
                                    ">=" => hodnota >= filtr,
                                    "<" => hodnota < filtr,
                                    "<=" => hodnota <= filtr,
                                    "=" => hodnota == filtr,
                                    "!=" => hodnota != filtr,
                                    _ => true
                                };
                            }
                            else
                                vyhovuje = false;
                        }
                        else if (atribut.PropertyType.Name == "int")
                        {
                            int hodnota = int.Parse(atribut.GetValue(o).ToString());
                            int filtr = int.Parse(parametry[i + nastaveni.indexFiltr]);
                            vyhovuje = parametry[i + nastaveni.indexPorovnani] switch
                            {
                                ">" => hodnota > filtr,
                                ">=" => hodnota >= filtr,
                                "<" => hodnota < filtr,
                                "<=" => hodnota <= filtr,
                                "=" => hodnota == filtr,
                                "!=" => hodnota != filtr,
                                _ => true
                            };
                        }
                        else
                        {
                            string hodnota = atribut.GetValue(o).ToString().ToLower();
                            string filtr = parametry[i + nastaveni.indexFiltr].ToLower();
                            vyhovuje = parametry[i + nastaveni.indexPorovnani] switch
                            {
                                "=" => hodnota == filtr,
                                "!=" => hodnota != filtr,
                                "!%" => !hodnota.Contains(filtr),
                                "%" => hodnota.Contains(filtr),
                                _ => true
                            };
                        }
                    }
                    if (!vyhovuje)
                        break;
                }
                if (vyhovuje)
                {
                    vyfiltrovaneZaznamy.Add(o);
                }
                

            }
            return vyfiltrovaneZaznamy;
        }


        /// <summary>
        /// Vrátí záznam dle zadaného ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected T VratZaznamDleId(string id)
        {
            List<IZaznam> databazeID = databaze.Cast<IZaznam>().ToList();
            var zaznam = (from z in databazeID
                         where z.Id == id
                         select z).First();
            return (T)zaznam;
        }
        /// <summary>
        /// Vrátí pole povinných vlastností, které je třeba zadat při vytváření nové instance záznamu
        /// </summary>
        /// <returns></returns>
        public abstract string[] VratZakladniVlastnost();

        /// <summary>
        /// Vrátí pole editovatelných vlastností na záznamu
        /// </summary>
        /// <returns></returns>
        public abstract string[] VratEditovatelneVlasnosti();

        /// <summary>
        /// Převede vlastnoti záznamu na jednu řádku CSV
        /// </summary>
        /// <returns></returns>
        public abstract string[] PrevedDatabaziNaCSV();
        /// <summary>
        /// Uloží záznamy jako CSV
        /// První řádka je hodnota counteru
        /// Od druhé řádky dál jsou uloženy hodnoty záznamů
        /// </summary>
        /// <param name="radky"></param>
        public void Uloz(string[] radky)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(soubor, false, Encoding.UTF8))
                {
                    // zapsání hlavičky
                    sw.WriteLine(counter.ToString());

                    // zapsání jednotlivých záznamů
                    foreach (String radek in radky)
                        sw.WriteLine(radek);

                    sw.Flush();
                    Zprava = String.Format("Databáze {0} je úspěšně uložena.", OznaceniZaznamu[1]);
                }
            }
            catch
            {
                Zprava = String.Format("Databázi {0} se nepodařilo uložit.", OznaceniZaznamu[1]);
            }
            
        }
        /// <summary>
        /// Načtení uložených dat v CSV do databáze
        /// </summary>
        public void Nacti()
        {
            databaze.Clear();
            int nactenyCounter = 0;
            try
            {
                using (StreamReader sr = new StreamReader(soubor))
                {
                    // Načtení a kontrola hlavičky
                    if (int.TryParse(sr.ReadLine(), out nactenyCounter))
                    {
                        // Načítání záznamů
                        string radek = "";
                        int nenacteneRadky = 0;
                        while ((radek = sr.ReadLine()) != null)
                        {
                            string[] hodnoty = radek.Split(';');
                            bool zapisOK = Pridej(hodnoty, false);
                            if (!zapisOK)
                                nenacteneRadky++;
                        }
                        counter = nactenyCounter;

                        // Info o průběhu načítání
                        Zprava = String.Format("Načtení záznamů {0} proběhlo úspěšně.", OznaceniZaznamu[1]);
                        if (nenacteneRadky > 0)
                            Zprava = String.Format("Nepodařilo se načíst {0} řadek.", nenacteneRadky);
                    }
                    else
                        Zprava = "Nepodařilo se načíst hlavičku souboru.";
                }            
            }
            catch
            {
                Zprava = String.Format("Soubor {0} nenalezen, načtení záznamů {1} se nezdařilo.", soubor, OznaceniZaznamu[1]);
            }
            
        }
    }
}
