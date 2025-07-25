﻿// <auto-generated />
using System;
using FootballAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FootballAPI.Migrations
{
    [DbContext(typeof(FootballDbContext))]
    [Migration("20250722062220_AddIsEnabledToPlayer")]
    partial class AddIsEnabledToPlayer
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FootballAPI.Models.Match", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("MatchDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TeamAGoals")
                        .HasColumnType("int");

                    b.Property<int>("TeamAId")
                        .HasColumnType("int");

                    b.Property<int>("TeamBGoals")
                        .HasColumnType("int");

                    b.Property<int>("TeamBId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MatchDate");

                    b.HasIndex("TeamAId");

                    b.HasIndex("TeamBId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("FootballAPI.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CurrentTeamId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("Rating")
                        .HasColumnType("decimal(3,2)");

                    b.HasKey("Id");

                    b.HasIndex("CurrentTeamId");

                    b.HasIndex("FirstName", "LastName");

                    b.ToTable("Players");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CurrentTeamId = 1,
                            FirstName = "Ion",
                            IsAvailable = true,
                            IsEnabled = true,
                            LastName = "Popescu",
                            Rating = 8.5m
                        },
                        new
                        {
                            Id = 2,
                            CurrentTeamId = 1,
                            FirstName = "Marius",
                            IsAvailable = true,
                            IsEnabled = true,
                            LastName = "Ionescu",
                            Rating = 7.8m
                        });
                });

            modelBuilder.Entity("FootballAPI.Models.PlayerMatchHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("MatchId")
                        .HasColumnType("int");

                    b.Property<decimal>("PerformanceRating")
                        .HasColumnType("decimal(3,2)");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("RecordDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TeamId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("PlayerId");

                    b.HasIndex("TeamId");

                    b.ToTable("PlayerMatchHistory");
                });

            modelBuilder.Entity("FootballAPI.Models.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Teams");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "FC Brasov"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Steaua Bucuresti"
                        });
                });

            modelBuilder.Entity("FootballAPI.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Password = "parola123",
                            Role = "Admin",
                            Username = "admin"
                        });
                });

            modelBuilder.Entity("FootballAPI.Models.Match", b =>
                {
                    b.HasOne("FootballAPI.Models.Team", "TeamA")
                        .WithMany("HomeMatches")
                        .HasForeignKey("TeamAId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FootballAPI.Models.Team", "TeamB")
                        .WithMany("AwayMatches")
                        .HasForeignKey("TeamBId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("TeamA");

                    b.Navigation("TeamB");
                });

            modelBuilder.Entity("FootballAPI.Models.Player", b =>
                {
                    b.HasOne("FootballAPI.Models.Team", "CurrentTeam")
                        .WithMany("CurrentPlayers")
                        .HasForeignKey("CurrentTeamId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("CurrentTeam");
                });

            modelBuilder.Entity("FootballAPI.Models.PlayerMatchHistory", b =>
                {
                    b.HasOne("FootballAPI.Models.Match", "Match")
                        .WithMany("PlayerHistory")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FootballAPI.Models.Player", "Player")
                        .WithMany("MatchHistory")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FootballAPI.Models.Team", "Team")
                        .WithMany("PlayerHistory")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Match");

                    b.Navigation("Player");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("FootballAPI.Models.Match", b =>
                {
                    b.Navigation("PlayerHistory");
                });

            modelBuilder.Entity("FootballAPI.Models.Player", b =>
                {
                    b.Navigation("MatchHistory");
                });

            modelBuilder.Entity("FootballAPI.Models.Team", b =>
                {
                    b.Navigation("AwayMatches");

                    b.Navigation("CurrentPlayers");

                    b.Navigation("HomeMatches");

                    b.Navigation("PlayerHistory");
                });
#pragma warning restore 612, 618
        }
    }
}
