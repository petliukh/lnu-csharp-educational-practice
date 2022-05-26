﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RestApi.DataAccess.Contracts;

#nullable disable

namespace RestApi.Migrations.Contracts
{
    [DbContext(typeof(ContractPostgreSqlContext))]
    [Migration("20220508141023_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("RestApi.Models.Contracts.Contract", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ContractorEmail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ContractorFirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ContractorLastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ContractorPhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Contracts");
                });
#pragma warning restore 612, 618
        }
    }
}
