using System;
using System.Text.RegularExpressions;

class Magazyn
{
    //// Definicja magazynu
    static int liczbaRegalow = 3;
    static int liczbaPolekNaRegale = 4;
    static int[] obciazalnoscRegalow = { 120, 150, 180 };

    //// Ciąg palet
    static List<(string Id, int Masa)> palety = new List<(string Id, int Masa)>
{
    ("A", 90),
    ("B", 60),
    ("C", 50),
    ("D", 40),
    ("E", 70),
    ("F", 80),
    ("G", 30),
    ("H", 20),
    ("I", 100),
    ("J", 110),
    ("K", 25),
    ("L", 15)
};
    // Tablica do przechowywania palet na regałach
    static (string Id, int Masa)[,] magazyn;
    static void Main()
    {

        //sortowanie palet przez klucz p bedacy masa tej palety
        var posortowanePalety = palety.OrderByDescending(p => p.Masa).ToList();

        //Tworze magazyn i ustalam jaki ma rozmiar
        magazyn = new (string Id, int Masa)[liczbaRegalow, liczbaPolekNaRegale];

        //szukanie najbardziej optymalnego regału / ostatnie_regaly przechowuje najbardziej optymalny regal dla kazdej iteracji
        List<List<(string Id, int Masa)>> ostatnie_regaly = new List<List<(string Id, int Masa)>>();
        //iterowanie po wszystkich regałach
        for (int indexregalu = 0; indexregalu < liczbaRegalow; indexregalu++)
        {
            //do listy ostatnie_regaly dodaj najbardziej optymalny regal dla kazdej iteracji
            for (int indexpalety = 0; indexpalety < posortowanePalety.Count; indexpalety++)
            {

                ostatnie_regaly.Add(znajdz_najlepszy_regal_dla_palety(posortowanePalety, obciazalnoscRegalow[indexregalu], indexpalety));
            }
            //Sortowanie regałów na podstawie masy i ilości zajetych półek
            var posortowane_regaly = ostatnie_regaly.OrderBy(regal => regal.Sum(paleta => paleta.Masa)).ThenBy(regal => regal.Count).ToList();

            //Wśród zmiennej posortowane_regały znajdz najbardziej optymalny
            var optymalny_regal = posortowane_regaly.Last();

            //Rozmieszczenie palet na wszystkich regałach w magazynie
            for (int i = optymalny_regal.Count - 1; i >= 0; i--)
            {
                magazyn[indexregalu, i] = optymalny_regal[optymalny_regal.Count - 1 - i];
            }

            //Usuwanie palet po umieszczeniu na regale
            foreach (var paleta in optymalny_regal)
            {
                posortowanePalety.RemoveAll(moja_paleta => moja_paleta.Id == paleta.Id);
            }
            ostatnie_regaly.Clear();
        }
        WyswietlenieMagazynu();
    }
    private static void WyswietlenieMagazynu()
    {
        for (int indexregalu = 0; indexregalu < liczbaRegalow; indexregalu++)
        {
            Console.WriteLine($"Regał {indexregalu + 1}: ");
            for (int indexpolki = 0; indexpolki < liczbaPolekNaRegale; indexpolki++)
            {
                Console.WriteLine($"\tPółka {indexpolki + 1}: {magazyn[indexregalu, indexpolki].Id} ({magazyn[indexregalu, indexpolki].Masa} kg)");
            }
        }
    }


    static List<(string Id, int Masa)> znajdz_najlepszy_regal_dla_palety(List<(string Id, int Masa)> posortowane_palety, int obciazalnoscRegalu, int index_poczatkowy)
    {

        // Dodanie listy przechowującej wszystkie najlepsze regały dla każdej iteracji.
        List<List<(string Id, int Masa)>> wszystkie_regaly = new List<List<(string Id, int Masa)>>();
        // Tworzenie nowej listy reprezentującej jeden regał i przypisane do niego palety.
        List<(string Id, int Masa)> regal = new List<(string Id, int Masa)>();
        int index_ostatni_usunietej_palety = index_poczatkowy;
        //iterowanie po wszystkich dostepnych paletach
        for (int indexpalety = index_poczatkowy; indexpalety < posortowane_palety.Count; indexpalety++)
        {
            //iterujemy po wszystkich paletach zaczynajac od indexu ostatniej usunientej palety
            for (int i = index_ostatni_usunietej_palety; i < posortowane_palety.Count; i++)
            {
                //liczenie obciażenia obecnego regału
                int obciazenie_regalu = 0;
                foreach (var paleta in regal)
                {
                    obciazenie_regalu += paleta.Masa;
                }

                //dodaj palete, jeżeli dana paleta może zostać dodana do regału bez przekroczenia maksymalnego obciążenia i liczby półek.
                if (obciazenie_regalu + posortowane_palety[i].Masa <= obciazalnoscRegalu && regal.Count < liczbaPolekNaRegale)
                {
                    regal.Add(posortowane_palety[i]);
                }

                //W przeciwnym wypadku, wrzuc obecny regał do wszysktich regałow
                else
                {
                    List<(string Id, int Masa)> nowy_regal = new List<(string Id, int Masa)>(regal);
                    wszystkie_regaly.Add(nowy_regal);
                }
                //jeżeli nie osiagneliśmy limitu, a skończyliśmy iterować, dodaj obecny regał do wszysktich regałow
                if (i == posortowane_palety.Count - 1)
                {
                    List<(string Id, int Masa)> nowy_regal = new List<(string Id, int Masa)>(regal);
                    wszystkie_regaly.Add(nowy_regal);

                }
            }
            //Po każdej iteracji, znajdź index ostatniej dodanej palety i ustawiamy jako "index_ostatni_usunietej_palety"
            index_ostatni_usunietej_palety = posortowane_palety.IndexOf(regal[regal.Count - 1]) + 1;
            //Usuwamy ostatnia dodana palete, że sprawdzic pozostałe iteracje
            regal.RemoveAt(regal.Count - 1);
        }
        //sortujemy znalezione najlepsze regaly
        var posortowane_regaly = wszystkie_regaly.OrderBy(regal => regal.Sum(paleta => paleta.Masa)).ToList();

        //zwróc najlepszy znaloeziony regał
        return posortowane_regaly.Last();


    }
}