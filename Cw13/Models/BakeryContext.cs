using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Cw13.Models
{
    public class BakeryContext : DbContext
    {
        public DbSet<Klient> Klienci { get; set; }
        public DbSet<Pracownik> Pracownicy { get; set; }
        public DbSet<Zamowienie> Zamowienia { get; set; }
        public DbSet<WyrobCukierniczy> wyrobCukiernicze { get; set; }
        public DbSet<Zamowienia_WyrobCukierniczy> zamowienia_WyrobCukiernicze { get; set; }

        public BakeryContext(DbContextOptions options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Zamowienie>()
                .HasOne<Klient>(z => z.klient)
                .WithMany(k => k.zamowienia)
                .HasForeignKey(zi => zi.IdKlient);

            modelBuilder.Entity<Zamowienie>()
                .HasOne<Pracownik>(z => z.pracownik)
                .WithMany(p => p.zamowienia)
                .HasForeignKey(zi => zi.IdPracownik);
            
            modelBuilder.Entity<Zamowienia_WyrobCukierniczy>()
                .HasKey(zwc => new { zwc.IdWyrobuCukierniczego, zwc.IdZamowienia });

            
            modelBuilder.Entity<Zamowienia_WyrobCukierniczy>()
                .HasOne<WyrobCukierniczy>(z => z.wyrobCukierniczy)
                .WithMany(p => p.zamowienia_WyrobCukiernicze)
                .HasForeignKey(zi => zi.IdWyrobuCukierniczego);

            modelBuilder.Entity<Zamowienia_WyrobCukierniczy>()
                .HasOne<Zamowienie>(z => z.zamowienie)
                .WithMany(p => p.zamowienia_WyrobCukiernicze)
                .HasForeignKey(zi => zi.IdZamowienia);



            modelBuilder.Entity<Klient>().HasData(

            new Klient
            {
                IdKlient = 1,
                Imie = "Yaroslav",
                Nazwisko = "Chuiev",
            }
            );

            modelBuilder.Entity<Pracownik>().HasData(

            new Pracownik
            {
                IdPracownik = 1,
                Imie = "Jan",
                Nazwisko = "Kowalski",
            }
            );

            modelBuilder.Entity<WyrobCukierniczy>().HasData(

            new WyrobCukierniczy
            {
                IdWyrobuCukierniczego = 1,
                Nazwa = "Wyrob",
                CenaZaSzt = 3,
                Typ = "Typ",
            }
            );

        }
    }
}
