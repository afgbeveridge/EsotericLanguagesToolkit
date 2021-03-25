using Eso.API.Editor.Models;
using Microsoft.EntityFrameworkCore;

namespace Eso.API.Editor {
        public class EditorDBContext : DbContext {

                public DbSet<Language> Languages { get; set; }
                public DbSet<LanguageCommand> LanguageCommands { get; set; }

                public EditorDBContext(DbContextOptions<EditorDBContext> options) : base(options) {
                }

                protected override void OnModelCreating(ModelBuilder builder) {
                        ForLanguage(builder).
                        ForLanguageCommand(builder);
                }

                private EditorDBContext ForLanguage(ModelBuilder builder) {
                        builder.Entity<Language>()
                                .ToTable("Languages")
                                .HasKey(l => l.Id);
                        builder.Entity<Language>()
                                .Property(l => l.Id).ValueGeneratedOnAdd().UseMySqlIdentityColumn().HasColumnType("int");
                        builder.Entity<Language>()
                                .Property(l => l.Name).IsRequired().HasMaxLength(100).HasColumnType("varchar(100)");
                        builder.Entity<Language>()
                                .Property(l => l.DateCreated).IsRequired().ValueGeneratedOnAdd().HasColumnType("datetime");
                        builder.Entity<Language>()
                                .Property(l => l.IsNativelySupported).IsRequired().HasColumnType("boolean").HasDefaultValue(false);
                        return this;
                }

                private EditorDBContext ForLanguageCommand(ModelBuilder builder) {
                        builder.Entity<LanguageCommand>()
                                                        .ToTable("LanguageCommands")
                                                        .HasKey(l => l.Id);
                        builder.Entity<LanguageCommand>()
                                .Property(l => l.Id).ValueGeneratedOnAdd().UseMySqlIdentityColumn().HasColumnType("int");
                        builder.Entity<LanguageCommand>()
                                .Property(l => l.Concept).IsRequired().HasMaxLength(60).HasColumnType("varchar(60)");
                        builder.Entity<LanguageCommand>()
                                .Property(l => l.Keyword).IsRequired().HasMaxLength(1).HasColumnType("varchar(1)");
                        builder.Entity<LanguageCommand>()
                                .Property(l => l.DateCreated).IsRequired().ValueGeneratedOnAdd().HasColumnType("datetime");
                        builder.Entity<LanguageCommand>()
                                .Ignore(l => l.Nature);
                        return this;
                }
        }
}
