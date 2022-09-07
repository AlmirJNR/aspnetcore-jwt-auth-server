using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Data.Models;

namespace Data.Context
{
    public partial class JWTContext : DbContext
    {
        public JWTContext()
        {
        }

        public JWTContext(DbContextOptions<JWTContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppUser> AppUsers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("app_user");

                entity.HasIndex(e => e.Email, "app_user_email_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.Password).HasColumnName("password");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
