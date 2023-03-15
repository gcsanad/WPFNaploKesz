using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfOsztalyzas
{
    public class Osztalyzat
    {
        public String nev;
        String datum;
        String tantargy;
        public String csaladiNev;
        int jegy;

        public Osztalyzat(string nev, string datum, string tantargy, int jegy, string csaladiNev)
        {
            this.nev = nev;
            this.datum = datum;
            this.tantargy = tantargy;
            this.jegy = jegy;
            this.csaladiNev = csaladiNev;
        }

        public string Nev { get => nev;  }
        public string Datum { get => datum;  }
        public string Tantargy { get => tantargy; }
        public string CsaladiNev { get => csaladiNev;}
        public int Jegy { get => jegy; }

        public static ObservableCollection<Osztalyzat> ForditottNev(ObservableCollection<Osztalyzat> lista)
        {
            foreach (var elem in lista)
            {
                string[] tarolas = elem.nev.Split(" ");
                string[] forditott = tarolas.Reverse().ToArray();
                elem.nev= forditott[0]+" " + forditott[1];
            }
            return lista;
        }
    }
        //todo Bővítse az osztályt! Készítsen CsaladiNev néven property-t, ami a névből a családi nevet adja vissza. Feltételezve, hogy a névnek csak az első tagja az.

        //todo Készítsen metódust ForditottNev néven, ami a két tagból álló nevek esetén megfordítja a névtagokat. Pld. Kiss Ádám => Ádám Kiss
}
