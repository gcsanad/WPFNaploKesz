using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace WpfOsztalyzas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string fajlNev = "naplo.txt";
        //Így minden metódus fogja tudni használni.
        ObservableCollection<Osztalyzat> jegyek = new ObservableCollection<Osztalyzat>();

        public MainWindow()
        {
            InitializeComponent();
            // todo Fájlok kitallózásával tegye lehetővé a naplófájl kiválasztását!
            // Ha nem választ ki semmit, akkor "naplo.csv" legyen az állomány neve. A későbbiekben ebbe fog rögzíteni a program.

            // todo A kiválasztott naplót egyből töltse be és a tartalmát jelenítse meg a datagrid-ben!
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog()==true)
            {
                foreach (var elem in File.ReadAllLines(ofd.FileName).ToList())
                {
                    string[] elemek = elem.Split(";");
                    Osztalyzat jegyekBetoltese=new Osztalyzat(elemek[0], elemek[1], elemek[2], Convert.ToInt32(elemek[3]), elemek[4]);
                    jegyek.Add(jegyekBetoltese);
                }
                dgJegyek.ItemsSource = jegyek;
                double jegyekAtlaga = jegyek.Average(x => x.Jegy);
                lblJegyekSzama.Content = dgJegyek.Items.Count;
                lblAtlag.Content = jegyekAtlaga;
                lblFajlHelye.Content = ofd.FileName;
            }
            sliJegy.Value = 3;
            datDatum.Text = DateTime.Now.ToString();
        }

        private void btnRogzit_Click(object sender, RoutedEventArgs e)
        {
            //todo Ne lehessen rögzíteni, ha a következők valamelyike nem teljesül!
            // a) - A név legalább két szóból álljon és szavanként minimum 3 karakterből!
            //      Szó = A szöközökkel határolt karaktersorozat.
            // b) - A beírt dátum újabb, mint a mai dátum

            //todo A rögzítés mindig az aktuálisan megnyitott naplófájlba történjen!


            //A CSV szerkezetű fájlba kerülő sor előállítása
            string[] csaladNev = txtNev.Text.Split(' ');
            string csvSor = $"{txtNev.Text};{datDatum.Text};{cboTantargy.Text};{sliJegy.Value};{csaladNev[0]}";
            //Megnyitás hozzáfűzéses írása (APPEND)
            string[] szetszedes = csvSor.Split(";");
            if (csaladNev.Length<2)
            {
                MessageBox.Show("A név álljon legalább két szóból!");
            }
            else
            {
                if (csaladNev[0].Length < 3 || csaladNev[1].Length<3)
                {
                    MessageBox.Show("A nevek szavanként álljanak legalább 3 betűből!");
                }
                else
                {
                    int idoKivetel = DateTime.Compare((DateTime)datDatum.SelectedDate, DateTime.Now);
                    if (idoKivetel>0)
                    {
                        MessageBox.Show("Nem lehet jövőbeli dátum!");
                    }
                    else
                    {
                        StreamWriter sw = new StreamWriter(fajlNev, append: true);
                        sw.WriteLine(csvSor);
                        sw.Close();

                        if (rbVezetek.IsChecked==true)
                        {
                            Osztalyzat ujJegy = new Osztalyzat(szetszedes[0], szetszedes[1], szetszedes[2], Convert.ToInt32(szetszedes[3]), szetszedes[4]);
                            jegyek.Add(ujJegy);
                            Osztalyzat.ForditottNev(jegyek);
                        }
                        else
                        {
                            Osztalyzat ujJegy = new Osztalyzat(szetszedes[0], szetszedes[1], szetszedes[2], Convert.ToInt32(szetszedes[3]), szetszedes[4]);
                            jegyek.Add(ujJegy);
                            
                        }
                        dgJegyek.ItemsSource = jegyek;
                        double jegyekAtlaga = jegyek.Average(x => x.Jegy);
                        lblJegyekSzama.Content = dgJegyek.Items.Count;
                        lblAtlag.Content = jegyekAtlaga;
                    }
                }
            }


            //todo Az újonnan felvitt jegy is jelenjen meg a datagrid-ben!
        }

        private void btnBetolt_Click(object sender, RoutedEventArgs e)
        {
            jegyek.Clear();  //A lista előző tartalmát töröljük
            StreamReader sr = new StreamReader(fajlNev); //olvasásra nyitja az állományt
            while (!sr.EndOfStream) //amíg nem ér a fájl végére
            {
                string[] mezok = sr.ReadLine().Split(";"); //A beolvasott sort feltördeli mezőkre
                //A mezők értékeit felhasználva létrehoz egy objektumot
                Osztalyzat ujJegy = new Osztalyzat(mezok[0], mezok[1], mezok[2], int.Parse(mezok[3]), mezok[4]); 
                jegyek.Add(ujJegy); //Az objektumot a lista végére helyezi
            }
            sr.Close(); //állomány lezárása

            //A Datagrid adatforrása a jegyek nevű lista lesz.
            //A lista objektumokat tartalmaz. Az objektumok lesznek a rács sorai.
            //Az objektum nyilvános tulajdonságai kerülnek be az oszlopokba.
            dgJegyek.ItemsSource = jegyek;
            double jegyekAtlaga = jegyek.Average(x => x.Jegy);
            lblJegyekSzama.Content = dgJegyek.Items.Count;
            lblAtlag.Content = jegyekAtlaga;
        }

        private void sliJegy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblJegy.Content = sliJegy.Value; //Több alternatíva van e helyett! Legjobb a Data Binding!
        }

        private void rbVezetek_Checked(object sender, RoutedEventArgs e)
        {
            Osztalyzat.ForditottNev(jegyek);
        }

        private void rbKereszt_Checked(object sender, RoutedEventArgs e)
        {
            Osztalyzat.ForditottNev(jegyek);
        }



        //todo Felület bővítése: Az XAML átszerkesztésével biztosítsa, hogy láthatóak legyenek a következők!
        // - A naplófájl neve
        // - A naplóban lévő jegyek száma
        // - Az átlag

        //todo Új elemek frissítése: Figyeljen rá, ha új jegyet rögzít, akkor frissítse a jegyek számát és az átlagot is!

        //todo Helyezzen el alkalmas helyre 2 rádiónyomógombot!
        //Feliratok: [■] Vezetéknév->Keresztnév [O] Keresztnév->Vezetéknév
        //A táblázatban a név azserint szerepeljen, amit a rádiónyomógomb mutat!
        //A feladat megoldásához használja fel a ForditottNev metódust!
        //Módosíthatja az osztályban a Nev property hozzáférhetőségét!
        //Megjegyzés: Felételezzük, hogy csak 2 tagú nevek vannak
    }
}

