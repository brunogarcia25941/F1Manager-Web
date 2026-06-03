using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Models;

namespace F1Manager.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Propriedades DbSet que representam as tabelas na base de dados
        public DbSet<Campeonato> Campeonatos { get; set; }
        public DbSet<Equipa> Equipas { get; set; }
        public DbSet<Piloto> Pilotos { get; set; }
        public DbSet<Corrida> Corridas { get; set; }
        public DbSet<ResultadoCorrida> ResultadosCorridas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Como a tabela ResultadoCorrida é Muitos-para-Muitos, ela precisa de uma chave primária composta (PilotoId + CorridaId).
            modelBuilder.Entity<ResultadoCorrida>()
                .HasKey(rc => new { rc.PilotoId, rc.CorridaId });
        }
    }
}