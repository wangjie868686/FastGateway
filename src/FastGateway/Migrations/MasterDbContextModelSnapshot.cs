﻿// <auto-generated />
using FastGateway.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FastGateway.Migrations
{
    [DbContext(typeof(MasterDbContext))]
    partial class MasterDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("FastGateway.Domain.Location", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("AddHeader")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("LoadType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProxyPass")
                        .HasColumnType("TEXT");

                    b.Property<string>("Root")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServiceId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TryFiles")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpStreams")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.ToTable("location", (string)null);
                });

            modelBuilder.Entity("FastGateway.Domain.Service", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Enable")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EnableFlowMonitoring")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EnableHttp3")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EnableRequestSource")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EnableTunnel")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHttps")
                        .HasColumnType("INTEGER");

                    b.Property<ushort>("Listen")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ServiceNames")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SslCertificate")
                        .HasColumnType("TEXT");

                    b.Property<string>("SslCertificatePassword")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("service", (string)null);
                });

            modelBuilder.Entity("FastGateway.Domain.Location", b =>
                {
                    b.HasOne("FastGateway.Domain.Service", "Service")
                        .WithMany("Locations")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");
                });

            modelBuilder.Entity("FastGateway.Domain.Service", b =>
                {
                    b.Navigation("Locations");
                });
#pragma warning restore 612, 618
        }
    }
}