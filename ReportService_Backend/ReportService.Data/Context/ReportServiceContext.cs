using Microsoft.EntityFrameworkCore;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;
using System.Globalization;
using System.Threading;

namespace ReportService.Data.Context
{
    public class ReportServiceContext : DbContext
    {
        public ReportServiceContext(DbContextOptions<ReportServiceContext> options)
            : base(options)
        {
            // Use the configured culture settings
            var cultureInfo = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChartConfiguration> ChartConfigurations { get; set; }
        public DbSet<SessionWorkflow> SessionWorkflows { get; set; }
        public DbSet<MeasuredValue> MeasuredValues { get; set; }
        public DbSet<ChartType> ChartTypes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure all string properties to use invariant culture
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string))
                    {
                        property.SetCollation("SQL_Latin1_General_CP1_CI_AS");
                    }
                }
            }

            modelBuilder.Entity<ChatSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.ReportName).HasMaxLength(450).IsRequired(false);

                entity.HasMany(e => e.ChatMessages)
                    .WithOne(e => e.ChatSession)
                    .HasForeignKey(e => e.ChatSessionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ChatSessionId).IsRequired();

                entity.HasCheckConstraint("CK_ChatMessages_Role", "[Role] IN ('user', 'assistant')");
            });

            modelBuilder.Entity<ChartConfiguration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasConversion<int>();
                entity.Property(e => e.ChatSessionId).IsRequired();
                entity.Property(e => e.XAxisField).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.YAxisField).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.SeriesField).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.SizeField).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.ColorField).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.OptionsJson).IsRequired(false);
                entity.Property(e => e.FiltersJson).IsRequired(false);

                // Removed invalid references to Options and Filters
                // Removed invalid reference to ChatSession navigation property
            });

            modelBuilder.Entity<SessionWorkflow>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.ChatSessionId).IsRequired();
                entity.Property(e => e.HasReportName).HasDefaultValue(false);
                entity.Property(e => e.HasMessageQuery).HasDefaultValue(false);
                entity.Property(e => e.HasChartConfigured).HasDefaultValue(false);
                entity.Property(e => e.IsPublished).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.ChatSession)
                    .WithMany()
                    .HasForeignKey(e => e.ChatSessionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MeasuredValue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Query).IsRequired();
                entity.HasOne(e => e.ChatSession)
                    .WithMany(c => c.MeasuredValues)
                    .HasForeignKey(e => e.ChatSessionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChartType>(entity =>
            {
                entity.ToTable("ChartTypes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
}
