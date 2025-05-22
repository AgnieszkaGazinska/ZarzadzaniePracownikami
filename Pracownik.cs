namespace BazaPracownikowAPI.Models
{
    //klasa odpowiadająca danym w tabeli mssql TabelaPracownicy
    public class Pracownik
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public DateTime DataUrodzenia { get; set; }
        public string Stanowisko { get; set; }
        public DateTime DataZatrudnienia { get; set; }
        public decimal WynagrodzenieBrutto { get; set; }
        public string RodzajZatrudnienia { get; set; }
    }
}
