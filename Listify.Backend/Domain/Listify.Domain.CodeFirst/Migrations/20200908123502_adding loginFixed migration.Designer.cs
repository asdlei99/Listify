﻿// <auto-generated />
using System;
using Listify.Domain.CodeFirst;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Listify.Domain.CodeFirst.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200908123502_adding loginFixed migration")]
    partial class addingloginFixedmigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Listify.Domain.Lib.Entities.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("AspNetUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("PlaylistCountMax")
                        .HasColumnType("int");

                    b.Property<int>("PlaylistSongCount")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("AspNetUserId")
                        .IsUnique()
                        .HasFilter("[AspNetUserId] IS NOT NULL");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasFilter("[Username] IS NOT NULL");

                    b.ToTable("ApplicationUsers","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.ApplicationUserRoom", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("bit");

                    b.Property<bool>("IsOwner")
                        .HasColumnType("bit");

                    b.Property<Guid>("RoomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("RoomId");

                    b.ToTable("ApplicationUsersRooms","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.ApplicationUserRoomConnection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid>("ApplicationUserRoomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConnectionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("HasPingBeenSent")
                        .HasColumnType("bit");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("bit");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserRoomId");

                    b.HasIndex("ConnectionId")
                        .IsUnique()
                        .HasFilter("[ConnectionId] IS NOT NULL");

                    b.ToTable("ApplicationUsersRoomsConnections","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.ApplicationUserRoomCurrency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid>("ApplicationUserRoomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserRoomId");

                    b.HasIndex("CurrencyId");

                    b.ToTable("ApplicationUsersRoomsCurrencies","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.ChatMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid?>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApplicationUserRoomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("ApplicationUserRoomId");

                    b.ToTable("ChatMessages","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.Currency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("CurrencyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuantityIncreasePerTick")
                        .HasColumnType("int");

                    b.Property<int>("TimeSecBetweenTick")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("TimestampLastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int>("Weight")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Currencies","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.LogAPI", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("EndPointType")
                        .HasColumnType("int");

                    b.Property<string>("IPAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ResponseCode")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("LogsAPI","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.LogError", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Exception")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IPAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("LogErrors","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.Playlist", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsSelected")
                        .HasColumnType("bit");

                    b.Property<string>("PlaylistName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Playlists","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.PurchasableItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<float>("DiscountApplied")
                        .HasColumnType("real");

                    b.Property<string>("ImageUri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PurchasableItemName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PurchasableItemType")
                        .HasColumnType("int");

                    b.Property<float>("Quantity")
                        .HasColumnType("real");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<float>("UnitCost")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("PurchasableItems","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.Purchase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<float>("AmountCharged")
                        .HasColumnType("real");

                    b.Property<Guid>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("PurchaseMethod")
                        .HasColumnType("int");

                    b.Property<float>("Subtotal")
                        .HasColumnType("real");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Purchases","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.PurchasePurchasableItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid>("PurchasableItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PurchaseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("PurchasableItemId");

                    b.HasIndex("PurchaseId");

                    b.ToTable("PurchasePurchasableItems","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.Room", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsRoomOnline")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRoomPublic")
                        .HasColumnType("bit");

                    b.Property<string>("RoomCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId")
                        .IsUnique();

                    b.HasIndex("RoomCode")
                        .IsUnique()
                        .HasFilter("[RoomCode] IS NOT NULL");

                    b.ToTable("Rooms","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.Song", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int>("SongLengthSeconds")
                        .HasColumnType("int");

                    b.Property<string>("SongName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("YoutubeId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Songs","Listify");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.SongRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SongId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("SongRequestType")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("SongId");

                    b.ToTable("SongRequests","Listify");

                    b.HasDiscriminator<string>("Discriminator").HasValue("SongRequest");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid>("ApplicationUserRoomCurrencyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuantityChange")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserRoomCurrencyId");

                    b.ToTable("Transactions","Listify");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Transaction");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.SongPlaylist", b =>
                {
                    b.HasBaseType("Listify.Domain.Lib.Entities.SongRequest");

                    b.Property<int>("PlayCount")
                        .HasColumnType("int");

                    b.Property<Guid>("PlaylistId")
                        .HasColumnType("uniqueidentifier");

                    b.HasIndex("PlaylistId");

                    b.ToTable("SongRequests","Listify");

                    b.HasDiscriminator().HasValue("SongPlaylist");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.SongQueued", b =>
                {
                    b.HasBaseType("Listify.Domain.Lib.Entities.SongRequest");

                    b.Property<Guid>("ApplicationUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("HasBeenPlayed")
                        .HasColumnType("bit");

                    b.Property<Guid>("RoomId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("TimestampPlayed")
                        .HasColumnType("datetime2");

                    b.Property<int>("WeightedValue")
                        .HasColumnType("int");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("RoomId");

                    b.ToTable("SongRequests","Listify");

                    b.HasDiscriminator().HasValue("SongQueued");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.TransactionSongQueued", b =>
                {
                    b.HasBaseType("Listify.Domain.Lib.Entities.Transaction");

                    b.Property<Guid>("SongQueuedId")
                        .HasColumnType("uniqueidentifier");

                    b.HasIndex("SongQueuedId");

                    b.ToTable("Transactions","Listify");

                    b.HasDiscriminator().HasValue("TransactionSongQueued");
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.ApplicationUserRoom", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.ApplicationUser", "ApplicationUser")
                        .WithMany("ApplicationUsersRooms")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Listify.Domain.Lib.Entities.Room", "Room")
                        .WithMany("ApplicationUsersRooms")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.ApplicationUserRoomConnection", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.ApplicationUserRoom", "ApplicationUserRoom")
                        .WithMany("ApplicationUsersRoomsConnections")
                        .HasForeignKey("ApplicationUserRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.ApplicationUserRoomCurrency", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.ApplicationUserRoom", "ApplicationUserRoom")
                        .WithMany("ApplicationUsersRoomsCurrencies")
                        .HasForeignKey("ApplicationUserRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Listify.Domain.Lib.Entities.Currency", "Currency")
                        .WithMany("ApplicationUsersRoomsCurrencies")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.ChatMessage", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.ApplicationUser", null)
                        .WithMany("ChatMessages")
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("Listify.Domain.Lib.Entities.ApplicationUserRoom", "ApplicationUserRoom")
                        .WithMany("ChatMessages")
                        .HasForeignKey("ApplicationUserRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.LogAPI", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.ApplicationUser", "ApplicationUser")
                        .WithMany("LogsAPI")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.LogError", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.ApplicationUser", "ApplicationUser")
                        .WithMany("LogsErrors")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.Playlist", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.ApplicationUser", "ApplicationUser")
                        .WithMany("Playlists")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.Purchase", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.ApplicationUser", "ApplicationUser")
                        .WithMany("Purchases")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.PurchasePurchasableItem", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.PurchasableItem", "PurchasableItem")
                        .WithMany("PurchasePurchasableItems")
                        .HasForeignKey("PurchasableItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Listify.Domain.Lib.Entities.Purchase", "Purchase")
                        .WithMany("PurchasePurchasableItems")
                        .HasForeignKey("PurchaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.Room", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.ApplicationUser", "ApplicationUser")
                        .WithOne("Room")
                        .HasForeignKey("Listify.Domain.Lib.Entities.Room", "ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.SongRequest", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.Song", "Song")
                        .WithMany("SongRequests")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.Transaction", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.ApplicationUserRoomCurrency", "ApplicationUserRoomCurrency")
                        .WithMany("Transactions")
                        .HasForeignKey("ApplicationUserRoomCurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.SongPlaylist", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.Playlist", "Playlist")
                        .WithMany("SongsPlaylist")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.SongQueued", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.ApplicationUser", "ApplicationUser")
                        .WithMany("SongsQueued")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Listify.Domain.Lib.Entities.Room", "Room")
                        .WithMany("SongsQueued")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Listify.Domain.Lib.Entities.TransactionSongQueued", b =>
                {
                    b.HasOne("Listify.Domain.Lib.Entities.SongQueued", "SongQueued")
                        .WithMany("TransactionsSongQueued")
                        .HasForeignKey("SongQueuedId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
