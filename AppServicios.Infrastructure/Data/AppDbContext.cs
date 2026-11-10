using AppServicios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppServicios.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();

        public DbSet<Profesional> Profesionales => Set<Profesional>();

        public DbSet<Propuesta> Propuestas => Set<Propuesta>();

        public DbSet<ProfesionalPropuesta> Presupuestos
            => Set<ProfesionalPropuesta>();

        public DbSet<Encargo> Encargos => Set<Encargo>();

        public DbSet<Chat> Chats => Set<Chat>();

        public DbSet<Mensaje> Mensajes => Set<Mensaje>();

        public DbSet<Calificacion> Calificaciones
            => Set<Calificacion>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .UseTptMappingStrategy();

            modelBuilder.Entity<Usuario>()
                .ToTable("Usuarios");

            modelBuilder.Entity<Profesional>()
                .ToTable("Profesionales");
            modelBuilder.Entity<Propuesta>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(p => p.IdUsuario)
                .OnDelete(DeleteBehavior.NoAction);
        
            modelBuilder.Entity<ProfesionalPropuesta>()
                .HasOne<Profesional>()
                .WithMany()
                .HasForeignKey(p => p.IdProfesional)
                .OnDelete(DeleteBehavior.NoAction);

         
            modelBuilder.Entity<ProfesionalPropuesta>()
                .HasOne<Propuesta>()
                .WithMany()
                .HasForeignKey(p => p.IdPropuesta)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProfesionalPropuesta>()
                .HasIndex(p => new { p.IdProfesional, p.IdPropuesta })
                .IsUnique();

            modelBuilder.Entity<ProfesionalPropuesta>()
                .Property(p => p.Monto)
                .HasPrecision(18, 2);
        
            modelBuilder.Entity<Encargo>()
                .HasOne<Propuesta>()
                .WithOne()
                .HasForeignKey<Encargo>(e => e.IdPropuesta)
                .OnDelete(DeleteBehavior.NoAction);
           
            modelBuilder.Entity<Encargo>()
                .HasOne<ProfesionalPropuesta>()
                .WithOne()
                .HasForeignKey<Encargo>(e => e.IdProfesionalPropuesta)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Chat>()
                .HasOne<Encargo>()
                .WithMany()
                .HasForeignKey(c => c.IdEncargo)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Chat>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Chat>()
                .HasOne<Profesional>()
                .WithMany()
                .HasForeignKey(c => c.IdProfesional)
                .OnDelete(DeleteBehavior.NoAction);
           
            modelBuilder.Entity<Mensaje>()
                .HasOne<Chat>()
                .WithMany()
                .HasForeignKey(m => m.IdChat)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Mensaje>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(m => m.IdUsuario)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Calificacion>()
                .HasOne<Encargo>()
                .WithOne()
                .HasForeignKey<Calificacion>(c => c.IdEncargo)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Calificacion>()
                .HasOne<Profesional>()
                .WithMany()
                .HasForeignKey(c => c.IdProfesional)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Calificacion>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.NoAction);

        }        

    }
}
