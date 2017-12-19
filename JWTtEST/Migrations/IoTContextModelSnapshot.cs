﻿// <auto-generated />
using JWTtEST;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace JWTtEST.Migrations
{
    [DbContext(typeof(IoTContext))]
    partial class IoTContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("JWTtEST.Device", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActivationId");

                    b.Property<string>("ActivationSecret");

                    b.Property<string>("DeviceEndpointId");

                    b.Property<string>("RSAKeyXML");

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("JWTtEST.Registration", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Created");

                    b.Property<string>("CreatedAsString");

                    b.Property<string>("Description");

                    b.Property<string>("DeviceEnpointId");

                    b.Property<long>("DeviceId");

                    b.Property<bool>("Enabled");

                    b.Property<string>("HardwareId");

                    b.Property<string>("HardwareRevision");

                    b.Property<string>("Manufacturer");

                    b.Property<string>("ModelNumber");

                    b.Property<string>("Name");

                    b.Property<string>("Request");

                    b.Property<string>("Response");

                    b.Property<string>("SerialNumber");

                    b.Property<string>("SharedSecret");

                    b.Property<string>("SharedSecretEncoded");

                    b.Property<string>("SoftwareRevision");

                    b.Property<string>("SoftwareVersion");

                    b.Property<string>("State");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId")
                        .IsUnique();

                    b.ToTable("Registrations");
                });

            modelBuilder.Entity("JWTtEST.Registration", b =>
                {
                    b.HasOne("JWTtEST.Device", "Device")
                        .WithOne("Registration")
                        .HasForeignKey("JWTtEST.Registration", "DeviceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
