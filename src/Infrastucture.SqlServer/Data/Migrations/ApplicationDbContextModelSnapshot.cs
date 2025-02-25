﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MoneyGroup.Infrastucture.Data;

#nullable disable

namespace MoneyGroup.Infrastucture.SqlServer.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MoneyGroup.Core.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IssuerId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("IssuerId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("MoneyGroup.Core.Entities.OrderConsumer", b =>
                {
                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("ConsumerId")
                        .HasColumnType("int");

                    b.HasKey("OrderId", "ConsumerId");

                    b.HasIndex("ConsumerId");

                    b.ToTable("OrderConsumers");
                });

            modelBuilder.Entity("MoneyGroup.Core.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MoneyGroup.Core.Entities.Order", b =>
                {
                    b.HasOne("MoneyGroup.Core.Entities.User", "Issuer")
                        .WithMany()
                        .HasForeignKey("IssuerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Issuer");
                });

            modelBuilder.Entity("MoneyGroup.Core.Entities.OrderConsumer", b =>
                {
                    b.HasOne("MoneyGroup.Core.Entities.User", "Consumer")
                        .WithMany()
                        .HasForeignKey("ConsumerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MoneyGroup.Core.Entities.Order", "Order")
                        .WithMany("Consumers")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Consumer");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("MoneyGroup.Core.Entities.Order", b =>
                {
                    b.Navigation("Consumers");
                });
#pragma warning restore 612, 618
        }
    }
}
