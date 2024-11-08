﻿// <auto-generated />
using System;
using MapBuilder.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MapBuilder.Data.Migrations
{
    [DbContext(typeof(CellContext))]
    partial class CellContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("MapBuilder.Shared.Cell", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CellToken")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("GenerationTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("GenerationVersion")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Cells");
                });

            modelBuilder.Entity("MapBuilder.Shared.Node", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CellId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Lat")
                        .HasColumnType("REAL");

                    b.Property<double>("Lng")
                        .HasColumnType("REAL");

                    b.Property<long>("NodeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NodeOrder")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("WayId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CellId");

                    b.ToTable("Node");
                });

            modelBuilder.Entity("MapBuilder.Shared.Way", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CellId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Closed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Filled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RetrievedData")
                        .HasColumnType("TEXT");

                    b.Property<int>("TotalNodes")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("WayId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CellId");

                    b.ToTable("Way");
                });

            modelBuilder.Entity("MapBuilder.Shared.Node", b =>
                {
                    b.HasOne("MapBuilder.Shared.Cell", null)
                        .WithMany("Nodes")
                        .HasForeignKey("CellId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MapBuilder.Shared.Way", b =>
                {
                    b.HasOne("MapBuilder.Shared.Cell", null)
                        .WithMany("Ways")
                        .HasForeignKey("CellId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MapBuilder.Shared.Cell", b =>
                {
                    b.Navigation("Nodes");

                    b.Navigation("Ways");
                });
#pragma warning restore 612, 618
        }
    }
}
