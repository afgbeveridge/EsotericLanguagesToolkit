﻿// <auto-generated />
using System;
using Eso.API.Editor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eso.API.Editor.Migrations
{
    [DbContext(typeof(EditorDBContext))]
    partial class EditorDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.3");

            modelBuilder.Entity("Eso.API.Editor.Models.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime");

                    b.Property<string>("Hash")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("IsNativelySupported")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("Eso.API.Editor.Models.LanguageCommand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Concept")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("varchar(60)");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime");

                    b.Property<string>("Keyword")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("varchar(1)");

                    b.Property<int?>("LanguageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LanguageId");

                    b.ToTable("LanguageCommands");
                });

            modelBuilder.Entity("Eso.API.Editor.Models.LanguageCommand", b =>
                {
                    b.HasOne("Eso.API.Editor.Models.Language", "Language")
                        .WithMany("Commands")
                        .HasForeignKey("LanguageId");

                    b.Navigation("Language");
                });

            modelBuilder.Entity("Eso.API.Editor.Models.Language", b =>
                {
                    b.Navigation("Commands");
                });
#pragma warning restore 612, 618
        }
    }
}
