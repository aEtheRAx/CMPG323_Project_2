﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
//using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleImageGallery.Data;

namespace SimpleImageGallery.Data.Migrations
{
    [DbContext(typeof(SimpleImageGalleryDbContext))]
    [Migration("20211030102135_Initial Migration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SimpleImageGallery.Data.Models.GalleryImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<string>("Title");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("GalleryImages");
                });

            modelBuilder.Entity("SimpleImageGallery.Data.Models.ImageTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<int?>("GalleryImageId");

                    b.HasKey("Id");

                    b.HasIndex("GalleryImageId");

                    b.ToTable("ImageTags");
                });

            modelBuilder.Entity("SimpleImageGallery.Data.Models.ImageTag", b =>
                {
                    b.HasOne("SimpleImageGallery.Data.Models.GalleryImage")
                        .WithMany("Tags")
                        .HasForeignKey("GalleryImageId");
                });
#pragma warning restore 612, 618
        }
    }
}
