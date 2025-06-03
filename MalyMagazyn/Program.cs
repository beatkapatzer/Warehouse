using System;
using System.Text.RegularExpressions;

class MalyMagazyn
{
    // Definicja magazynu
    static int liczbaRegalow = 2;
    static int liczbaPolekNaRegale = 4;
    static int[] obciazalnoscRegalow = { 100, 100 }; // Maksymalne obciążenie dla każdego regału

    // Ciąg palet
    static List<(string Id, int Masa)> palety = new List<(string Id, int Masa)>
{
    ("A", 40),
    ("B", 30),
    ("C", 20),
    ("D", 50),
    ("E", 25),
    ("F", 35),
    ("G", 10),
    ("H", 15),
    ("I", 45),
    ("J", 60)
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

        WyswietlenieMagazynu(posortowanePalety);
    }

    private static void WyswietlenieMagazynu(List<(string Id, int Masa)> nieDodanePalety)
    {
        Console.WriteLine("Magazyn:");
        for (int indexregalu = 0; indexregalu < liczbaRegalow; indexregalu++)
        {
            int sumaObciazenia = 0;
            Console.WriteLine($"Regał {indexregalu + 1}: ");
            for (int indexpolki = 0; indexpolki < liczbaPolekNaRegale; indexpolki++)
            {
                var paleta = magazyn[indexregalu, indexpolki];
                if (paleta.Id != null)
                {
                    Console.WriteLine($"\tPółka {indexpolki + 1}: {paleta.Id} ({paleta.Masa} kg)");
                    sumaObciazenia += paleta.Masa;
                }
                else
                {
                    Console.WriteLine($"\tPółka {indexpolki + 1}: pusta");
                }
            }
            Console.WriteLine($"\tSuma obciążenia regału: {sumaObciazenia} kg\n");
        }

        Console.WriteLine("Palety, które nie zostały dodane:");
        foreach (var paleta in nieDodanePalety)
        {
            Console.WriteLine($"\tPaleta {paleta.Id}: {paleta.Masa} kg");
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
                //jeżeli nie osiagnelismy limitu, a skończyliśmy iterować, dodaj obecny regał do wszysktich regałow
                if (i == posortowane_palety.Count - 1)
                {
                    List<(string Id, int Masa)> nowy_regal = new List<(string Id, int Masa)>(regal);
                    wszystkie_regaly.Add(nowy_regal);
                }
            }
            //Po każdej iteracji, znajdż index ostatniej dodaj palety i ustaw jako "index_ostatni_usunientej_palety"
            index_ostatni_usunietej_palety = posortowane_palety.IndexOf(regal[regal.Count - 1]) + 1;
            //Usuwamy ostatnia dodaTNIA PALETE, żeby sprawdzić pozostałe iteracje
            regal.RemoveAt(regal.Count - 1);
        }
        //sortujemy znalezione najlepsze regaly
        var posortowanie_regaly = wszystkie_regaly.OrderBy(regal => regal.Sum(paleta => paleta.Masa)).ToList();

        //zwróc najlepszy znaleziony regal
        return posortowanie_regaly.Last();

    }
}