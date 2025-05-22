using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BazaPracownikowAPI.Data;
using BazaPracownikowAPI.Models;

namespace BazaPracownikowAPI.Controllers
{
    // Ustawiamy ścieżkę dla kontrolera
    [Route("api/[controller]")]
    [ApiController]
    public class PracownicyController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Konstruktor - Dodaje AppDbContext z bazą danych
        public PracownicyController(AppDbContext context)
        {
            _context = context;
        }



        // GET - Zwraca listę wszystkich pracowników w formacie JSON
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pracownik>>> GetPracownicy()
        {
            var pracownicy = await _context.TabelaPracownikow.ToListAsync(); // Pobieramy wszystkich pracowników
            return Ok(pracownicy);  // Zwraca dane w formacie JSON.
        }

        // GET - wraca pojedynczego pracownika na podstawie ID w formacie JSON
        [HttpGet("{id}")]
        public async Task<ActionResult<Pracownik>> GetPracownik(int id)
        {
            // Szukamy pracownika w bazie po ID.
            var pracownik = await _context.TabelaPracownikow.FindAsync(id);

            if (pracownik == null)
            {
                // Obsługa błędu w przypadku, jeśli pracownik nie został znaleziony
                return NotFound();
            }

            // Zwracamy znalezionego pracownika w formacie JSON
            return Ok(pracownik);
        }

        // POST - Dodaje nowego pracownika (dane w formacie JSON)
        [HttpPost]
        public async Task<ActionResult<Pracownik>> PostPracownik(Pracownik pracownik)
        {
            // Dodajemy nowego pracownika do bazy.
            _context.TabelaPracownikow.Add(pracownik);
            await _context.SaveChangesAsync();

            // Zwracamy status 201 (Created) i link do nowo utworzonego zasobu (z danymi pracownika).
            return CreatedAtAction(nameof(GetPracownik), new { id = pracownik.Id }, pracownik);
        }


        // DELETE - Usuwa pracownika na podstawie ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePracownik(int id)
        {
            // Szukamy pracownika w bazie po id
            var pracownik = await _context.TabelaPracownikow.FindAsync(id);
            if (pracownik == null)
            {
                // Jeśli pracownik nie istnieje, zwracamy błąd nie znaleziono
                return NotFound();
            }

            // Usuwamy pracownika z bazy
            _context.TabelaPracownikow.Remove(pracownik);
            await _context.SaveChangesAsync();

            // Gdy usunięcie się powiedzie zwracamy brak zawartości
            return NoContent();
        }

        //GET - Wyszukiwanie pracownika
        [HttpGet("search")]
        public IActionResult SearchPracownicy(string query)
        {
            var wyniki = _context.TabelaPracownikow
                .Where(p => p.Imie.Contains(query) || p.Nazwisko.Contains(query))
                .ToList();
            return Ok(wyniki);
        }

        //PUT - Edycja pracownikaa
        [HttpPut("{id}")]
        public IActionResult UpdatePracownik(int id, [FromBody] Pracownik updatedPracownik)
        {
            if (id != updatedPracownik.Id)
            {
                return BadRequest("ID w URL różni się od ID w danych.");
            }

            try
            {
                var existingPracownik = _context.TabelaPracownikow.FirstOrDefault(p => p.Id == id);

                if (existingPracownik == null)
                {
                    return NotFound($"Pracownik z ID {id} nie został znaleziony.");
                }

                // Przypisanie nowych wartości, jeśli są dostępne
                if (!string.IsNullOrEmpty(updatedPracownik.Imie)) existingPracownik.Imie = updatedPracownik.Imie;
                if (!string.IsNullOrEmpty(updatedPracownik.Nazwisko)) existingPracownik.Nazwisko = updatedPracownik.Nazwisko;
                if (updatedPracownik.DataUrodzenia != default(DateTime))
                {
                    existingPracownik.DataUrodzenia = updatedPracownik.DataUrodzenia;
                }
                if (!string.IsNullOrEmpty(updatedPracownik.Stanowisko)) existingPracownik.Stanowisko = updatedPracownik.Stanowisko;
                if (updatedPracownik.DataZatrudnienia != default(DateTime))
                {
                    existingPracownik.DataZatrudnienia = updatedPracownik.DataZatrudnienia;
                }
                if (updatedPracownik.WynagrodzenieBrutto > 0)
                {
                    existingPracownik.WynagrodzenieBrutto = updatedPracownik.WynagrodzenieBrutto;
                }
                if (!string.IsNullOrEmpty(updatedPracownik.RodzajZatrudnienia)) existingPracownik.RodzajZatrudnienia = updatedPracownik.RodzajZatrudnienia;

                // Zapisanie zmian do bazy
                _context.SaveChanges();

                return Ok(existingPracownik);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas aktualizacji: {ex.Message}");
                return StatusCode(500, "Wystąpił błąd podczas aktualizacji.");
            }
        }

        // Metoda sprawdzająca, czy pracownik istnieje w bazie, pomocnicza
        private bool PracownikExists(int id)
        {
            return _context.TabelaPracownikow.Any(e => e.Id == id);
        }
    }
}
