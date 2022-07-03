using Escola.Models;
using Microsoft.EntityFrameworkCore;

namespace Escola.Context
{
    public class EscolaContext : DbContext
    {
        public EscolaContext(DbContextOptions<EscolaContext> options) : base(options)
        {
        }

        public DbSet<Aluno> Aluno { get; set; }
        public DbSet<Turma> Turma { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Turma>().ToTable("turma")
                .HasMany(c => c.Alunos)
                .WithOne(e => e.Turma);

            modelBuilder.Entity<Turma>()
                .Navigation(c => c.Alunos)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Aluno>().ToTable("aluno")
                .HasOne(e => e.Turma)
                .WithMany(c => c.Alunos)
                .HasForeignKey(e => e.TurmaId);

            modelBuilder.Entity<Aluno>()
                .Navigation(e => e.Turma)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}
