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
        public DbSet<Producto> productos { get; set; }

        public DbSet<Venta> ventas { get; set; }

        public DbSet<DetalleVenta> detalle { get; set; }
        public DbSet<Inventario> inventario { get; set; }
        public DbSet<Pedido> pedidos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(tb =>
            {
                tb.HasKey(col => col.IdUsuario);
                tb.Property(col => col.IdUsuario).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombre).HasMaxLength(50);
                tb.Property(col => col.Apellido).HasMaxLength(50);
                tb.Property(col => col.Correo).HasMaxLength(50);
                tb.Property(col => col.Contraseña).HasMaxLength(250);
            });
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario {IdUsuario=1, Nombre = "Abed", Apellido = "Llovera", Correo = "admin@gmail.com", Contraseña = "$2a$11$ftbwkevIm.kVrZCJtWwVFOnz8EICF6isvJeKp0ZIC.dCNM0t6ZHti", IdRol = 1 }
            );

            modelBuilder.Entity<Libro>(tb =>
            {
                tb.HasKey(col => col.IdLibro);
                tb.Property(col => col.IdLibro).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Autor).HasMaxLength(50);
                tb.Property(col => col.Titulo).HasMaxLength(100);                
                tb.Property(col => col.Genero).HasMaxLength(50);
                tb.Property(col => col.Precio).IsRequired();
                tb.Property(col => col.Stock).IsRequired();
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

            modelBuilder.Entity<Venta>(tb =>
            {
                tb.HasKey(col => col.IdVenta);
                tb.Property(col => col.IdVenta).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.FechaVenta).IsRequired().HasDefaultValueSql("GETDATE()"); 
                tb.Property(col => col.Total).IsRequired().HasColumnType("decimal(18,2)");
                tb.Property(col => col.MetodoPago).IsRequired().HasMaxLength(50);
                tb.Property(col => col.Estado).IsRequired().HasMaxLength(20).HasDefaultValue("Completada");
                tb.Property(col => col.IdUsuario).IsRequired();

                tb.HasOne(v => v.Usuario)
                    .WithMany()
                    .HasForeignKey(v => v.IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);

                tb.HasMany(v => v.Detalles)
                    .WithOne(d => d.Venta)
                    .HasForeignKey(d => d.IdVenta)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Venta>().ToTable("Ventas");

            modelBuilder.Entity<DetalleVenta>(tb =>
            {
                tb.HasKey(col => col.IdDetalle);
                tb.Property(col => col.IdDetalle).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Cantidad).IsRequired();
                tb.Property(col => col.PrecioUnitario).IsRequired().HasColumnType("decimal(18,2)");
                tb.Property(col => col.Subtotal).IsRequired().HasColumnType("decimal(18,2)");
                tb.Property(col => col.IdLibro).IsRequired(false);
                tb.Property(col => col.IdProducto).IsRequired(false);
                tb.Property(col => col.IdVenta).IsRequired();

                tb.HasOne(d => d.Venta)
                    .WithMany(v => v.Detalles)
                    .HasForeignKey(d => d.IdVenta)
                    .OnDelete(DeleteBehavior.Cascade);

                tb.HasOne(d => d.Libro)
                    .WithMany()
                    .HasForeignKey(d => d.IdLibro)
                    .OnDelete(DeleteBehavior.SetNull);

                tb.HasOne(d => d.Producto)
                    .WithMany()
                    .HasForeignKey(d => d.IdProducto)
                    .OnDelete(DeleteBehavior.SetNull);

            });
            modelBuilder.Entity<DetalleVenta>().ToTable("DetalleVenta");

            modelBuilder.Entity<Inventario>(tb =>
            {
                tb.HasKey(col => col.IdInventario);
                tb.Property(col => col.IdInventario).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Stock).IsRequired();
                tb.Property(col => col.FechaActualizacion).IsRequired().HasDefaultValueSql("GETDATE()");
                tb.Property(col => col.IdLibro).IsRequired();
                tb.Property(col => col.IdProducto).IsRequired();

                tb.HasOne(i => i.Libro)
                    .WithMany()
                    .HasForeignKey(i => i.IdLibro)
                    .OnDelete(DeleteBehavior.Cascade);

                tb.HasOne(i => i.Producto)
                    .WithMany()
                    .HasForeignKey(i => i.IdProducto)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Inventario>().ToTable("Inventario");

            modelBuilder.Entity<Pedido>(tb =>
            {
                tb.HasKey(col => col.IdReserva);
                tb.Property(col => col.IdReserva).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Cantidad).IsRequired();
                tb.Property(col => col.FechaReserva).IsRequired().HasDefaultValueSql("GETDATE()");
                tb.Property(col => col.FechaEntrega).IsRequired(false);
                tb.Property(col => col.Estado).IsRequired().HasMaxLength(50).HasDefaultValue("Pendiente");
                tb.Property(col => col.IdUsuario).IsRequired();
                tb.Property(col => col.IdLibro).IsRequired();
                tb.Property(col => col.IdProducto).IsRequired(false);

                tb.HasOne(p => p.Usuario)
                    .WithMany()
                    .HasForeignKey(p => p.IdUsuario)
                    .OnDelete(DeleteBehavior.Cascade);

                tb.HasOne(p => p.Libro)
                    .WithMany()
                    .HasForeignKey(p => p.IdLibro)
                    .OnDelete(DeleteBehavior.Cascade);

                tb.HasOne(p => p.Producto)
                    .WithMany()
                    .HasForeignKey(p => p.IdProducto)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<Pedido>().ToTable("Pedido");

            modelBuilder.Entity<Producto>(tb =>
            {
                tb.HasKey(col => col.IdProducto);
                tb.Property(col => col.IdProducto).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombre).IsRequired().HasMaxLength(100);
                tb.Property(col => col.Descripcion).IsRequired(false).HasMaxLength(500);
                tb.Property(col => col.Precio).IsRequired().HasColumnType("decimal(18,2)");
                tb.Property(col => col.Categoria).IsRequired().HasMaxLength(50);
                tb.Property(col => col.Stock).IsRequired();
                tb.Property(col => col.Activo).IsRequired().HasDefaultValue(true);

                tb.HasIndex(p => p.Nombre).IsUnique();
                tb.HasIndex(p => p.Categoria);
                tb.HasIndex(p => p.Activo);
            });

            modelBuilder.Entity<Producto>().ToTable("Producto");

            base.OnModelCreating(modelBuilder);
        }
    }
}
