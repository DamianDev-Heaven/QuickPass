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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("tickets");
            entity.HasKey(e => e.TicketsId);
            entity.Property(e => e.TicketsId).HasColumnName("id_ticket").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(e => e.title).HasColumnName("title").HasMaxLength(200).IsRequired();
            entity.Property(e => e.description).HasColumnName("description").HasColumnType("TEXT");
            entity.Property(e => e.status).HasColumnName("status").HasColumnType("ENUM('Abierto', 'Asignado','En proceso','Resuelto','Cerrado')")
            .HasConversion(v => v == TicketStatus.Enproceso ? "En proceso" : v.ToString(),v => v == "En proceso" ? TicketStatus.Enproceso : Enum.Parse<TicketStatus>(v));
            entity.Property(e => e.CustomerId).HasColumnName("customer_id").HasColumnType("BINARY(16)").IsRequired();
            entity.Property(e => e.TechId).HasColumnName("tech_id").HasColumnType("BINARY(16)");
        });
    }
}
