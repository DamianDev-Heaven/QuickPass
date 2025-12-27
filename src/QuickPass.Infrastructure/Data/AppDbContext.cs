using Microsoft.EntityFrameworkCore;
using QuickPass.Domain.Entities;
using System.Net.Sockets;

namespace QuickPass.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Ticket> tickets {  get; set; }
    public DbSet<Roles> roles { get; set; }
    public DbSet<Account> account { get; set; }
    public DbSet<Users> users { get; set; }

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
            .HasConversion(v => v == TicketStatus.Enproceso ? "En proceso" : v.ToString(),v => v == "En proceso" ? TicketStatus.Enproceso : Enum.Parse<TicketStatus>(v));
            entity.Property(e => e.CustomerId).HasColumnName("customer_id").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(e => e.TechId).HasColumnName("tech_id").HasColumnType("BINARY(16)");
        });        
        modelBuilder.Entity<Roles>(entity => // Roles
        {
            entity.ToTable("roles");
            entity.HasKey(r => r.idRol);
            entity.Property(r => r.idRol).HasColumnName("id_rol").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(r => r.NameRol).HasColumnName("name_rol").HasColumnType("ENUM('Administrador', 'Tecnico', 'Usuario')")
            .HasConversion(v => v.ToString(), v => Enum.Parse<RoleNames>(v));
        });
        modelBuilder.Entity<Account>(entity => // Account
        {
            entity.ToTable("accounts");
            entity.HasKey(a => a.accId);
            entity.Property(a => a.accId).HasColumnName("id_acc").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(a => a.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
            entity.Property(a => a.Pass).HasColumnName("pass").HasMaxLength(256).IsRequired();
            entity.Property(a => a.RolId).HasColumnName("rol_id").HasColumnType("BINARY(16)");
            entity.HasOne(d => d.Role).WithMany().HasForeignKey(d => d.RolId).OnDelete(DeleteBehavior.Restrict);
            entity.Property(a => a.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").ValueGeneratedOnAdd();
            entity.Property(a => a.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        });
        modelBuilder.Entity<Users>(entity => // Users
        {
            entity.ToTable("users");
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.UserId).HasColumnName("id_user").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(u => u.NameUser).HasColumnName("name_user").HasMaxLength(69).IsRequired();
            entity.Property(u => u.Description).HasColumnName("description").HasMaxLength(255);
            entity.Property(u => u.UrlPic).HasColumnName("profile_pic");
            entity.Property(u => u.AccId).HasColumnName("account_id").HasColumnType("BINARY(16)").IsRequired();
            entity.HasOne(d => d.account).WithOne().HasForeignKey<Users>(d => d.AccId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
