﻿using Checkr.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Checkr.Data
{
    public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
    {
        public DbSet<Board> Boards { get; set; }

        public DbSet<Box> Boxes { get; set; }

        public DbSet<Card> Cards { get; set; }

        public DbSet<ToDoItem> ToDoItems { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Invitation> Invitations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Boards)
                .WithMany(b => b.Users)
                .UsingEntity(j => j.ToTable("UserBoards"));

            modelBuilder.Entity<Box>()
                .HasMany(b => b.Tags)
                .WithMany(t => t.Boxes)
                .UsingEntity<Dictionary<string, object>>(
                    "BoxTag",
                    j => j
                        .HasOne<Tag>()
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Restrict),
                    j => j
                        .HasOne<Box>()
                        .WithMany()
                        .HasForeignKey("BoxesId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.ToTable("BoxTags"));

            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.Sender)
                .WithMany(u => u.SentInvitations)
                .HasForeignKey(i => i.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.Recipient)
                .WithMany(u => u.ReceivedInvitations)
                .HasForeignKey(i => i.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invitation>()
                .Property(i => i.Status)
                .HasConversion<int>();
        }
    }
}
