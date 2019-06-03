﻿// <auto-generated />
using HomeLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HomeLib.Migrations
{
    [DbContext(typeof(LibraryContext))]
    [Migration("20190603131827_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HomeLib.Models.Authtor", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("");

                    b.Property<string>("FullName");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("");

                    b.Property<string>("MiddleName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("");

                    b.HasKey("Id");

                    b.HasIndex("FullName");

                    b.ToTable("Authtors");
                });

            modelBuilder.Entity("HomeLib.Models.Book", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Annotation");

                    b.Property<string>("AuthtorFirstName");

                    b.Property<string>("AuthtorLastName");

                    b.Property<string>("AuthtorMiddleName");

                    b.Property<string>("Cover");

                    b.Property<string>("Isbn");

                    b.Property<string>("PathArchive");

                    b.Property<string>("PathBook");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<string>("Year");

                    b.HasKey("Id");

                    b.HasIndex("Title");

                    b.HasIndex("AuthtorFirstName", "AuthtorMiddleName", "AuthtorLastName");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("HomeLib.Models.Book", b =>
                {
                    b.HasOne("HomeLib.Models.Authtor", "Authtor")
                        .WithMany("Books")
                        .HasForeignKey("AuthtorFirstName", "AuthtorMiddleName", "AuthtorLastName")
                        .HasPrincipalKey("FirstName", "MiddleName", "LastName");
                });
#pragma warning restore 612, 618
        }
    }
}