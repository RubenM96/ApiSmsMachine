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
            modelBuilder.Entity<SmsInbound>(e =>
            {
                e.ToTable("SmsInbound");
                e.HasKey(s => s.Id);
                e.Property(s => s.Id).ValueGeneratedOnAdd();

                e.OwnsOne(s => s.Recipient, r =>
                {
                    r.Property(p => p.Value).HasColumnName("Recipient").IsRequired().HasMaxLength(50);
                });

                e.Property(s => s.Text).IsRequired().HasMaxLength(300);
                e.Property(s => s.ReceivedAt).IsRequired();
                e.Property(s => s.Multipart).IsRequired();
                //e.Property(s => s.Notify).IsRequired();
            });
        }
    }
}
