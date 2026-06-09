using Microsoft.EntityFrameworkCore;
using QuickPass.Domain.Entities;
using System.Net.Sockets;

namespace QuickPass.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Ticket> tickets { get; set; }
    public DbSet<Role> roles { get; set; }
    public DbSet<Account> account { get; set; }
    public DbSet<User> users { get; set; }
    public DbSet<TicketHistory> ticketHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Ticket>(entity => //Tickets
        {
            entity.ToTable("tickets");
            entity.HasKey(e => e.TicketsId);
            entity.Property(e => e.TicketsId).HasColumnName("id_ticket").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasColumnType("TEXT");
            entity.Property(e => e.Status).HasColumnName("status").HasColumnType("ENUM('Abierto', 'Asignado','En proceso','Resuelto','Cerrado')")
            .HasConversion(v => v == TicketStatus.Enproceso ? "En proceso" : v.ToString(), v => v == "En proceso" ? TicketStatus.Enproceso : Enum.Parse<TicketStatus>(v));
            entity.Property(e => e.CustomerId).HasColumnName("customer_id").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(e => e.TechId).HasColumnName("tech_id").HasColumnType("BINARY(16)");
            entity.Property(e => e.Priority).HasColumnName("priority").HasColumnType("ENUM('Baja', 'Media', 'Alta', 'Critica')").HasConversion(v => v.ToString(), v => Enum.Parse<TicketPriority>(v));
            entity.Property(e => e.Category).HasColumnName("category").HasColumnType("ENUM('General', 'Hardware', 'Software', 'Redes', 'Acceso')").HasConversion(v => v.ToString(), v => Enum.Parse<TicketCategory>(v));
        });
        modelBuilder.Entity<Role>(entity => // Roles
        {
            entity.ToTable("roles");
            entity.HasKey(r => r.IdRol);
            entity.Property(r => r.IdRol).HasColumnName("id_rol").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(r => r.NameRol).HasColumnName("name_rol").HasColumnType("ENUM('Administrador', 'Tecnico', 'Usuario')")
            .HasConversion(v => v.ToString(), v => Enum.Parse<RoleNames>(v));
        });
        modelBuilder.Entity<Account>(entity => // Account
        {
            entity.ToTable("accounts");
            entity.HasKey(a => a.accId);
            entity.Property(a => a.accId).HasColumnName("id_acc").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(a => a.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
            entity.Property(a => a.Pass).HasColumnName("pass").HasMaxLength(256);
            entity.Property(a => a.RolId).HasColumnName("rol_id").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(a => a.OtherMed).HasColumnName("other_med").HasColumnType("BOOLEAN").HasDefaultValue(false);
            entity.HasOne(d => d.Role).WithMany().HasForeignKey(d => d.RolId).OnDelete(DeleteBehavior.Restrict);
            entity.Property(a => a.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").ValueGeneratedOnAdd();
            entity.Property(a => a.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        });
        modelBuilder.Entity<User>(entity => // Users
        {
            entity.ToTable("users");
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.UserId).HasColumnName("id_user").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(u => u.NameUser).HasColumnName("name_user").HasMaxLength(69).IsRequired();
            entity.Property(u => u.Description).HasColumnName("description").HasMaxLength(255);
            entity.Property(u => u.UrlPic).HasColumnName("profile_pic").HasColumnType("TEXT");
            entity.Property(u => u.AccId).HasColumnName("account_id").HasColumnType("BINARY(16)").IsRequired();
            entity.HasOne(d => d.Account).WithOne().HasForeignKey<User>(d => d.AccId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TicketHistory>(entity => // TicketHistory
        {
            entity.ToTable("ticket_histories");
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Id).HasColumnName("id_history").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(h => h.TicketId).HasColumnName("ticket_id").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(h => h.ModifiedBy).HasColumnName("modified_by").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(h => h.PrevStatus).HasColumnName("prev_status").HasColumnType("ENUM('Abierto', 'Asignado', 'En proceso', 'Resuelto', 'Cerrado')")
                .HasConversion(v => v == null ? null : (v == TicketStatus.Enproceso ? "En proceso" : v.ToString()), v => string.IsNullOrEmpty(v) ? null : (v == "En proceso" ? TicketStatus.Enproceso : Enum.Parse<TicketStatus>(v)));
            entity.Property(h => h.NewStatus).HasColumnName("new_status").HasColumnType("ENUM('Abierto', 'Asignado', 'En proceso', 'Resuelto', 'Cerrado')")
                .HasConversion(v => v == TicketStatus.Enproceso ? "En proceso" : v.ToString(), v => v == "En proceso" ? TicketStatus.Enproceso : Enum.Parse<TicketStatus>(v));
            entity.Property(h => h.Comment).HasColumnName("comment").HasColumnType("TEXT");
            entity.Property(h => h.ChangedAt).HasColumnName("changed_at").HasColumnType("TIMESTAMP").ValueGeneratedOnAdd();

            entity.HasOne(h => h.Ticket)
                .WithMany(t => t.Histories)
                .HasForeignKey(h => h.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(h => h.Modifier)
                .WithMany()
                .HasForeignKey(h => h.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Semilla para Roles por defecto
        modelBuilder.Entity<Role>().HasData(
            new Role { IdRol = Guid.Parse("a817cfd4-00ee-4d1e-8bbe-8d4c0e071a2e"), NameRol = RoleNames.Administrador },
            new Role { IdRol = Guid.Parse("b817cfd4-00ee-4d1e-8bbe-8d4c0e071a2e"), NameRol = RoleNames.Tecnico },
            new Role { IdRol = Guid.Parse("c817cfd4-00ee-4d1e-8bbe-8d4c0e071a2e"), NameRol = RoleNames.Usuario }
        );
    }
}
