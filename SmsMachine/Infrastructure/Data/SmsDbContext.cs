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
            modelBuilder.Entity<SmsOutbound>(e =>
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
                e.Property(s => s.Index);
                e.Property(s => s.CampaignId);
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
                e.Property(n => n.Text).HasColumnName("TextReport").IsRequired().HasMaxLength(20);
                e.Property(n => n.DateTime).IsRequired();
                e.Property(n => n.IndexSms).HasColumnName("SmsIndex").IsRequired();
                e.Property(n => n.Status).IsRequired().HasMaxLength(10);
                e.Property(n => n.SmsOutbounsId);
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

            //Tabella CampaignSms
            modelBuilder.Entity<CampaignSms>(e =>
            {
                e.ToTable("CampaignSms");
                e.HasKey(c => c.Id);
                e.Property(c => c.Id).ValueGeneratedOnAdd();
                e.Property(c => c.Title).IsRequired().HasMaxLength(100);
                e.Property(c => c.Text).IsRequired().HasMaxLength(160);
                e.Property(c => c.RecipientList).IsRequired();
                e.Property(c => c.CampaignNotify).IsRequired();
                e.Property(c => c.CreatedAt).IsRequired();
                e.Property(c => c.Status).IsRequired();
                e.Property(c => c.Description).HasMaxLength(500);
                e.Property(c => c.TotalRecipients).IsRequired();
                e.Property(c => c.DeliveredCount).IsRequired();
                e.Property(c => c.FailedCount).IsRequired();
            });

            //Tabella SmsQueue
            modelBuilder.Entity<SmsQueue>(e =>
            {
                e.ToTable("SmsQueue");
                e.HasKey(s => s.Id);
                e.Property(t => t.Id).ValueGeneratedOnAdd();
                e.OwnsOne(s => s.Recipient, r =>
                {
                    r.Property(p => p.Value).HasColumnName("Recipient").IsRequired().HasMaxLength(50);
                });
                e.Property(s => s.Text).IsRequired().HasMaxLength(300);
                e.Property(s => s.Multipart).IsRequired();
                e.Property(s => s.Notify).IsRequired();
                e.Property(s => s.CampaignId);
            });
        }
    }
}
