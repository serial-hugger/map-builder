﻿// <auto-generated />
using System;
using MapBuilder.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MapBuilder.Data.Migrations
{
    [DbContext(typeof(DbContext))]
    [Migration("20241028194748_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("MapBuilder.Data.CellModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CellToken")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Cells");
                });

            modelBuilder.Entity("MapBuilder.Data.NodeModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CellModelId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Latitude")
                        .HasColumnType("REAL");

                    b.Property<double>("Longitude")
                        .HasColumnType("REAL");

                    b.Property<long>("NodeId")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("WayId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CellModelId");

                    b.ToTable("NodeModel");
                });

            modelBuilder.Entity("MapBuilder.Data.NodeModel", b =>
                {
                    b.HasOne("MapBuilder.Data.CellModel", null)
                        .WithMany("Nodes")
                        .HasForeignKey("CellModelId");
                });

            modelBuilder.Entity("MapBuilder.Data.CellModel", b =>
                {
                    b.Navigation("Nodes");
                });
#pragma warning restore 612, 618
        }
    }
}
