using Microsoft.EntityFrameworkCore;
using SmsMachine.Models;

namespace SmsMachine.Infrastructure.Data
{
    public class SmsDbContext : DbContext
    {
        public SmsDbContext(DbContextOptions<SmsDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sms>(e =>
            {
                e.ToTable("Sms");
                e.HasKey(s => s.Id);
                e.Property(t => t.Id).ValueGeneratedOnAdd();
                e.OwnsOne(s => s.Recipient, r =>
                {
                    r.Property(p => p.Value).HasColumnName("Recipient").IsRequired().HasMaxLength(50);
                });
                e.Property(s => s.Text).IsRequired().HasMaxLength(300);
                e.Property(s => s.SentAt).IsRequired();
                e.Property(s => s.Multipart).IsRequired();  
                e.Property(s => s.Notify).IsRequired();
            });

            //Tabella Notify
            modelBuilder.Entity<Notify>(e =>
            {
                e.ToTable("Notify");
                e.HasKey(n => n.Id);
                e.OwnsOne(s => s.Recipient, r =>
                {
                    r.Property(p => p.Value).HasColumnName("Recipient").IsRequired().HasMaxLength(50);
                });
                e.Property(n => n.Text).IsRequired().HasMaxLength(20);
                e.Property(n => n.DateTime).IsRequired();
            });

        }
    }
}
