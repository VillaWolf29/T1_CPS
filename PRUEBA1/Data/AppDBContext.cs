using Microsoft.EntityFrameworkCore;
using PRUEBA1.Models;

namespace PRUEBA1.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Reseña> Reseñas { get; set; }
        public DbSet<Autor> Autores { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(tb =>
            {
                tb.HasKey(col => col.IdUsuario);
                tb.Property(col => col.IdUsuario).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombre).HasMaxLength(50);
                tb.Property(col => col.Apellido).HasMaxLength(50);
                tb.Property(col => col.Correo).HasMaxLength(50);
                tb.Property(col => col.Contraseña).HasMaxLength(50);
            });
            modelBuilder.Entity<Usuario>().ToTable("Usuario");

            modelBuilder.Entity<Libro>(tb =>
            {
                tb.HasKey(col => col.IdLibro);
                tb.Property(col => col.IdLibro).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Autor).HasMaxLength(50);
                tb.Property(col => col.Titulo).HasMaxLength(100);                
                tb.Property(col => col.Genero).HasMaxLength(50);
            });
            modelBuilder.Entity<Libro>().ToTable("Libro");

            modelBuilder.Entity<Rol>(tb =>
            {
                tb.HasKey(col => col.IdRol);
                tb.Property(col => col.IdRol).UseIdentityColumn().ValueGeneratedOnAdd();                
                tb.Property(col => col.Nombre).HasMaxLength(50);
                tb.Property(col => col.Descripcion).HasMaxLength(200);
            });
            modelBuilder.Entity<Rol>().ToTable("Rol");

            modelBuilder.Entity<Reseña>(tb =>
            {
                tb.HasKey(col => col.IdReseña);
                tb.Property(col => col.IdReseña).UseIdentityColumn().ValueGeneratedOnAdd();                
                tb.Property(col => col.Contenido).HasMaxLength(500);
                tb.Property(col => col.Calificacion).HasMaxLength(10);
            });
            modelBuilder.Entity<Reseña>().ToTable("Reseña");

            modelBuilder.Entity<Autor>(tb =>
            {
                tb.HasKey(col => col.IdAutor);
                tb.Property(col => col.IdAutor).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombre).HasMaxLength(50);
                tb.Property(col => col.Apellido).HasMaxLength(50);
                tb.Property(col => col.Nacionalidad).HasMaxLength(50);
            });
            modelBuilder.Entity<Autor>().ToTable("Autor");

            modelBuilder.Entity<Rol>().HasData(
                new Rol { IdRol = 1, Nombre = "Administrador", Descripcion = "Administrador" },
                new Rol { IdRol = 2, Nombre = "Usuario", Descripcion = "Usuario" }
            );

            

            base.OnModelCreating(modelBuilder);
        }
    }
}
