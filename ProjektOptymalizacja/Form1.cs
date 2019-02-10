using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace ProjektOptymalizacja
{
    public partial class Form1 : Form
    {

        //blad, jaki mozemy przyjac miedzy kolejnymi krokami 
        //(czyli utknelismy na plaszczyznie)
        const double epsilon = 0.1;
        
        //wspolczynniki - potegi jako liczby calkowite
        double A;
        int B;
        double C;
        int D;
        double E;
        int F;
        double G;
        int H;

        Random rnd;
        //sprawdza, czy program wykonywany jest pierwszy raz
        bool first;

        public Form1()
        {
            //pierwsze otwarcie programu
            first = true;
            InitializeComponent();

            //w menu mozemy wybrak krok staly lub wyliczany
            //domyslny jest staly v = 1
            //z listy mozna wybierac jeden z itemow, nie mozna nic wpisac
            toolStripComboBox1.Text = "Krok staly";
            toolStripComboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        //zwraca wartosc funkcji
        double f(double x1, double x2)
        {
            return A * Math.Pow(x1, B) + C * Math.Pow(x2, D) + E * Math.Pow(x1, F) + G * Math.Pow(x2, H);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rnd = new Random();
        }

        //przycisk "licz"
        private void button2_Click(object sender, EventArgs e)
        {
            //n = liczba iteracji
            //jesli krok jest staly, to mozna zwiekszyc n, bo nie rosnie to tak szybko
            //jesli krok jest wyliczany, to bywa bardzo duzy
            //i po kilku iteracjach wartosc funkcji moze wykraczac poza wartosc double
            //(jezeli min lub max dazy do nieskonczonosci)
            //dla stalego dalem max 500 iteracji, dla wyliczanego 20
            int n;
            if (toolStripComboBox1.SelectedItem.ToString() == "Krok staly")
                n = 500;
            else n = 20;

            try
            {
                A = int.Parse(textBox1.Text);
                B = int.Parse(textBox2.Text);
                C = int.Parse(textBox3.Text);
                D = int.Parse(textBox4.Text);
                E = int.Parse(textBox5.Text);
                F = int.Parse(textBox6.Text);
                G = int.Parse(textBox7.Text);
                H = int.Parse(textBox8.Text);

                //jezeli krok jest wyliczany to ograniczamy potegi zmiennych do <0,2>
                //bo dla kazdej trzeba rozpisywac inne rownanie, dla kroku stalego nie
                //ma to znaczenia wiec pomijamy
                if (toolStripComboBox1.SelectedItem.ToString() == "Krok wyliczany")
                {
                    if (B > 2 || B < 0 || D > 2 || D < 0 || F > 2 || F < 0 || H > 2 || H < 0)
                    {
                        MessageBox.Show("potegi musza byc z zakresu <0, 2>");
                        return;
                    }
                }

                label1.Text = "f(x1, x2) = " + A + "x1" + "^" + B + " + " + C + "x2" + "^" + D + " + " + E + "x1" + "^" + F + " + " + G + "x2" + "^" + H;
                
            }
            catch (FormatException)
            {
                MessageBox.Show("Blednie wprowadzone dane.");
                return;
            }

            //poczatkowe x1,x2 i krok v
            double x1, x2;
            double v = 0;

            //losowanie x1,x2 z zakresu <-5,5> (mozna zmieniac do woli)
            x1 = rnd.Next(-5, 5);
            x2 = rnd.Next(-5, 5);

            label1.Text += "\n punkt startujacy X = (" + x1 + ", " + x2 + ").";

            //wartosc funkcji f0
            double f0 = 0;
            //wartosc nowej funkcji f1 po dodaniu do jednej ze zmiennych v
            //potem sprawdzamy czy f1<>f0 w celu wybrania lepszej
            double f1 = 0;
            //ostatni pomiar f0 - jesli jest taki sam jak poprzedni
            //(z dokladnoscia do epsilon) to konczymy, bo utknelismy
            //na plaszczyznie
            double lastF = 999;
            //flaga - dla szukania min = 0, dla max = 1
            int minmax = 0;

            //te zmienne zostaly mi z wczesniej, chyba niepotrzebne
            //ale zostawilem zeby nic sie nie rozdupcylo
            double minx1, minx2;
            double maxx1, maxx2;

            //tablice przechowujace x1, x2 dla szukania min i max
            //potrzebne do narysowania potem wykresow
            double[,] tabminx = new double[n, 2];
            double[,] tabmaxx = new double[n, 2];
            double[] tabfminx = new double[n];
            double[] tabfmaxx = new double[n];


            //petla dla n iteracji (lub jesli utkniemy na plaszczyznie)
            for (int i = 0; (i < n && (Math.Abs(f0 - lastF) > epsilon)); i++) 
            {
                //jesli szukamy minimum
                if (minmax == 0)
                {
                    //wrzucamy w tablice x1, x2, f(x1,x2)
                    tabminx[i, 0] = x1; tabminx[i, 1] = x2;
                    tabfminx[i] = f(x1, x2);
                }
                //jesli szukamy maximum
                if (minmax == 1)
                {
                    //wrzucamy w tablice x1, x2, f(x1,x2)
                    tabmaxx[i, 0] = x1; tabmaxx[i, 1] = x2;
                    tabfmaxx[i] = f(x1, x2);
                }

                //najpierw szukamy minimum dla n iteracji, jesli dojdziemy do konca
                //musimy sie przerzucic na szukanie max rowniez dla n iteracji -
                //zeby nie robic nowej petli polegajacej na tym samym cofamy i do -1
                //(przy przejsciu przez petle bedzie inkrementowana, wiec wyjdzie i=0
                //i tak jakbysmy zaczeli od poczatku)

                //wchodzimy tu jesli szukany jest min i jestesmy na przedostatnim przejsciu
                //petli
                if (minmax == 0 && i == n-1)
                {
                    //dopisujemy ostatnie wartosci do tablicy min
                    minx1 = x1;
                    minx2 = x2;

                    //losujemy nowy punkt startowy
                    x1 = rnd.Next(-5, 5);
                    x2 = rnd.Next(-5, 5);

                    //przelaczamy na szukanie max
                    minmax = 1;

                    //i dajemy na -1
                    i = -1;
                }

                //wybor kroku - albo staly rowny 1, albo obiczany przez optimize()
                if(toolStripComboBox1.SelectedItem.ToString() == "Krok staly")
                    v = 1;
                else v = optimize(A, B, E, F, C, D, G, H, x1, x2, 1);

                //obliczamy f0 i f1 (z krokiem v)
                f0 = f(x1, x2);
                f1 = f(x1 + v, x2);
                //jezeli szukamy max
                if (minmax == 1)
                {
                    //jezeli f1 jest lepsze to podmieniamy zmienna
                    if (f1 > f0)
                    {
                        x1 = x1 + v;
                        f0 = f1;
                    }

                    //jesli nie jest lepsze to sprawdzamy z odjetym krokiem
                    else
                    {
                        f1 = f(x1 - v, x2);
                        if (f1 > f0)
                        {
                            x1 = x1 - v;
                            f0 = f1;
                        }
                    }
                    //jesli nie znaleziono lepszego to nic nie robimy
                }

                //wchodzimy jesli szukamy minimum, reszta analogicznie do szukania max
                if (minmax == 0)
                {
                    if (f1 < f0)
                    {
                        x1 = x1 + v;
                        f0 = f1;
                    }
                    else
                    {
                        f1 = f(x1 - v, x2);
                        if (f1 < f0)
                        {
                            x1 = x1 - v;
                            f0 = f1;
                        }
                    }
                }
                

                //kroki analogiczne do powyzszych, tutaj szukamy dla drugiej zmiennej - x2
                if (toolStripComboBox1.SelectedItem.ToString() == "Krok staly")
                    v = 1;
                else v = optimize(C, D, G, H, A, B, E, F, x1, x2, 2);

                f0 = f(x1, x2);
                f1 = f(x1, x2 + v);
                if (minmax == 1)
                {
                    if (f1 > f0)
                    {
                        x2 = x2 + v;
                        f0 = f1;
                    }
                    else
                    {
                        f1 = f(x1, x2 - v);
                        if (f1 > f0)
                        {
                            x2 = x2 - v;
                            f0 = f1;
                        }
                    }
                }
                if(minmax == 0)
                {
                    if (f1 < f0)
                    {
                        x2 = x2 + v;
                        f0 = f1;
                    }
                    else
                    {
                        f1 = f(x1, x2 - v);
                        if (f1 < f0)
                        {
                            x2 = x2 - v;
                            f0 = f1;
                        }
                    }
                }
            }

            //za kazdym razem czyscimy punkty, w przeciwnym wypadku kazde klikniecie
            //buttona bedzie nakladalo na siebie kolejne wykresy
            foreach (var series in chart1.Series)
                series.Points.Clear();

            foreach (var series in chart2.Series)
                series.Points.Clear();

            foreach (var series in chart3.Series)
                series.Points.Clear();

            //jesli odpalamy program pierwszy raz, do wykresow zostaja dodane tytuly
            if (first)
            {
                first = false;
                chart1.Titles.Add("Wykres x1, x2 przy szukaniu minimum");
                chart2.Titles.Add("Wykres x1, x2 przy szukaniu maksimum");
                chart3.Titles.Add("Wykres maksymalnej i minimalnej wartosci funkcji");
            }

            //rysowanie wykresow za pomoca danych min, max z tablic
            for (int i = 0; i < n; i++)
            {
                //zatrzymuje wykresy na wysokich wartosciach, bo zdarzalo sie, 
                //ze przekraczaly max wartosc dla double
                if (tabfmaxx[i] > 999999) tabfmaxx[i] = 999999;
                if (tabfminx[i] < -999999) tabfminx[i] = -999999;
                if (tabmaxx[i,0] > 999999) tabmaxx[i,0] = 999999;
                if (tabmaxx[i,1] > 999999) tabmaxx[i,1] = 999999;
                if (tabminx[i,0] < -999999) tabminx[i,0] = -999999;
                if (tabminx[i,1] < -999999) tabminx[i,1] = -999999;


                chart1.Series["x1"].Points.AddXY(i + 1, tabminx[i, 0]);
                chart1.Series["x2"].Points.AddXY(i + 1, tabminx[i, 1]);
                chart2.Series["x1"].Points.AddXY(i + 1, tabmaxx[i, 0]);
                chart2.Series["x2"].Points.AddXY(i + 1, tabmaxx[i, 1]);
                chart3.Series["f_min"].Points.AddXY(i + 1, tabfminx[i]);
                chart3.Series["f_max"].Points.AddXY(i + 1, tabfmaxx[i]);
            }
            
            //wartosci minimalne i maksymalne dla x1, x2
            label10.Text = "x1_max = " + tabmaxx[n-1, 0] + "         x2_max = " + tabmaxx[n-1, 1] + "\nx1_min = " + tabminx[n-1, 0] + "         x2_min = " + tabminx[n-1, 1];
        }

        //funkcja wyliczajaca krok
        //factor1 - wspolczynnik przy pierwszej potedze
        //factor2 - wspolczynnik przy drugiej potedze
        //pow1 - pierwsza potega
        //pow2 - druga potega
        //a,b,c,d - kolejne wspolczynniki drugiej zmiennej
        //x1, x2 - wartosci zmiennych
        //variable - ktora zmienna jest liczona (1 - x1, 2 - x2)
        double optimize(double factor1, int pow1, double factor2, int pow2, double a, int b, double c, int d, double x1, double x2, int variable)
        {
            double choosenX = 0;
            double delta = 0;

            //wybiranie zmiennej
            if (variable == 1) choosenX = x1;
            if (variable == 2) choosenX = x2;

            //jezeli zmienne sa mnozone przez 0 lub do potegi 0, to krok = 0
            if((factor1 == 0 && factor2 == 0) || (pow1 == 0 && pow2 == 0))
                return 0;

            //jezeli pierwszy czlon zmiennej ^1, a drugi ^0
            if (pow1 == 1 && pow2 == 0)
            {
                return ((a * Math.Pow(x1, b) + c * Math.Pow(x2, d)) / (factor1)) - choosenX;
            }

            //jezeli pierwszy czlon zmiennej ^0, a drugi ^1
            if (pow1 == 0 && pow2 == 1)
            {
                return ((a * Math.Pow(x1, b) + c * Math.Pow(x2, d)) / (factor2)) - choosenX;
            }

            //jezeli pierwszy czlon zmiennej ^1 i drugi ^1
            if (pow1 == 1 && pow2 == 1)
            {
                return ((a * Math.Pow(x1, b) + c * Math.Pow(x2, d)) / (factor1 + factor2)) - choosenX;
            }

            //jezeli pierwszy czlon zmiennej ^2, a drugi ^0
            if (pow1 == 2 && pow2 == 0)
            {
                delta = Math.Pow(2 * factor1 * choosenX, 2) - 4 * (factor1 * (factor1 * Math.Pow(choosenX, 2) + (a * Math.Pow(x1, b) + c * Math.Pow(x2, d))));
                //return -delta / 4 * factor1;
                return -(2 * factor1 * choosenX) / 2 * (factor1);
            }

            //jezeli pierwszy czlon zmiennej ^0, a drugi ^2
            if (pow1 == 0 && pow2 == 2)
            {
                delta = Math.Pow(2 * factor2 * choosenX, 2) - 4 * (factor2 * (factor2 * Math.Pow(choosenX, 2) + (a * Math.Pow(x1, b) + c * Math.Pow(x2, d))));
                //return -delta / 4 * factor2;
                return -(2 * factor2 * choosenX) / 2 * (factor2);

            }

            //jezeli pierwszy czlon zmiennej ^2, a drugi ^1
            if (pow1 == 2 && pow2 == 1)
            {
                delta = Math.Pow(2 * factor1 * choosenX + factor2, 2) - 4 * (factor1 * (factor1 * Math.Pow(choosenX, 2) + factor2 * choosenX + (a * Math.Pow(x1, b) + c * Math.Pow(x2, d))));
                //return -delta / 4 * factor1;
                return -(2 * factor1 * choosenX+factor2) / 2 * (factor1);
            }

            //jezeli pierwszy czlon zmiennej ^1, a drugi ^2
            if (pow1 == 1 && pow2 == 2)
            {
                delta = Math.Pow(2 * factor2 * choosenX + factor1, 2) - 4 * (factor2 * (factor2 * Math.Pow(choosenX, 2) + factor1 * choosenX + (a * Math.Pow(x1, b) + c * Math.Pow(x2, d))));
                //return -delta / 4 * factor2;
                return -(2 * factor2 * choosenX+factor1) / 2 * (factor2);
            }

            //jezeli oba czlony zmiennej ^2
            if (pow1 == 2 && pow2 == 2)
            {
                delta = Math.Pow(2 * (factor1+factor2) * choosenX, 2) - 4 * ((factor1+factor2) * (Math.Pow(choosenX, 2) + a * Math.Pow(x1, b) + c * Math.Pow(x2, d)));
                //return -delta / 4 * (factor1+factor2);
                return -(2 * (factor2+factor1) * choosenX) / 2 * (factor1+factor2);
            }

            //te delty sa niepotrzebnie liczone, ale zostawilem gdyby jeszcze
            //kiedys mogly sie przydac
            return 0;
        }
    }
}
