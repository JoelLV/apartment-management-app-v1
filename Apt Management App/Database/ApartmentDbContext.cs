using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Apt_Management_App.Database
{
    public partial class ApartmentDbContext : DbContext
    {
        public ApartmentDbContext()
        {
        }

        public ApartmentDbContext(DbContextOptions<ApartmentDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Apartment> Apartments { get; set; } = null!;
        public virtual DbSet<Contract> Contracts { get; set; } = null!;
        public virtual DbSet<ElectricityContract> ElectricityContracts { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Renter> Renters { get; set; } = null!;
        public virtual DbSet<WaterContract> WaterContracts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("DataSource=.\\Database\\ApartmentDb.db;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apartment>(entity =>
            {
                entity.HasKey(e => e.AptId);

                entity.ToTable("Apartment");

                entity.HasIndex(e => e.AptNum, "IX_Apartment_apt_num")
                    .IsUnique();

                entity.Property(e => e.AptId)
                    .HasColumnType("varchar(8)")
                    .HasColumnName("apt_id");

                entity.Property(e => e.AptNum)
                    .HasColumnType("varchar(10)")
                    .HasColumnName("apt_num");

                entity.Property(e => e.Bathrooms)
                    .HasColumnType("integer")
                    .HasColumnName("bathrooms")
                    .HasConversion<byte>();

                entity.Property(e => e.Bedrooms)
                    .HasColumnType("integer")
                    .HasColumnName("bedrooms")
                    .HasConversion<byte>();

                entity.Property(e => e.Capacity)
                    .HasColumnType("integer")
                    .HasColumnName("capacity")
                    .HasConversion<byte>();

                entity.Property(e => e.HasKitchen)
                    .HasColumnType("boolean")
                    .HasColumnName("has_kitchen")
                    .HasConversion<bool>();

                entity.Property(e => e.MonthlyCost)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("monthly_cost")
                    .HasConversion<float>();
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("Contract");

                entity.Property(e => e.ContractId)
                    .HasColumnType("varchar(8)")
                    .HasColumnName("contract_id");

                entity.Property(e => e.AptId)
                    .HasColumnType("varchar(8)")
                    .HasColumnName("apt_id");

                entity.Property(e => e.DateSigned)
                    .HasColumnType("date")
                    .HasColumnName("date_signed")
                    .HasConversion<string>();

                entity.Property(e => e.Deposit)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("deposit")
                    .HasConversion<float>();

                entity.Property(e => e.ExpDate)
                    .HasColumnType("date")
                    .HasColumnName("exp_date")
                    .HasConversion<string>();

                entity.Property(e => e.NumResidents)
                    .HasColumnType("integer")
                    .HasColumnName("num_residents")
                    .HasConversion<byte>();

                entity.Property(e => e.PaymentDay)
                    .HasColumnType("integer")
                    .HasColumnName("payment_day")
                    .HasConversion<byte>();

                entity.Property(e => e.RenterId)
                    .HasColumnType("varchar(8)")
                    .HasColumnName("renter_id");

                entity.HasOne(d => d.Apt)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.AptId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Renter)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.RenterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ElectricityContract>(entity =>
            {
                entity.HasKey(e => e.ElecContractId);

                entity.ToTable("Electricity_Contract");

                entity.Property(e => e.ElecContractId)
                    .HasColumnType("varchar(8)")
                    .HasColumnName("elec_contract_id");

                entity.Property(e => e.AptId)
                    .HasColumnType("varchar(8)")
                    .HasColumnName("apt_id");

                entity.Property(e => e.MeasurerNum)
                    .HasColumnType("varchar(8)")
                    .HasColumnName("measurer_num");

                entity.Property(e => e.PaymentDue)
                    .HasColumnType("date")
                    .HasColumnName("payment_due")
                    .HasConversion<string>();

                entity.Property(e => e.Rmu)
                    .HasColumnType("text")
                    .HasColumnName("RMU");

                entity.Property(e => e.ServiceNum)
                    .HasColumnType("varchar(15)")
                    .HasColumnName("service_num");

                entity.Property(e => e.ShutOffDate)
                    .HasColumnType("date")
                    .HasColumnName("shut_off_date")
                    .HasConversion<string>();

                entity.HasOne(d => d.Apt)
                    .WithMany(p => p.ElectricityContracts)
                    .HasForeignKey(d => d.AptId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PayId);

                entity.ToTable("Payment");

                entity.Property(e => e.PayId)
                    .HasColumnType("varchar(12)")
                    .HasColumnName("pay_id");

                entity.Property(e => e.AmountPayed)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("amount_payed")
                    .HasConversion<float>();

                entity.Property(e => e.ContractId)
                    .HasColumnType("varchar(8)")
                    .HasColumnName("contract_id");

                entity.Property(e => e.DatePayed)
                    .HasColumnType("date")
                    .HasColumnName("date_payed")
                    .HasConversion<string>();

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.ContractId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Renter>(entity =>
            {
                entity.ToTable("Renter");

                entity.Property(e => e.RenterId)
                    .HasColumnType("varchar(8)")
                    .HasColumnName("renter_id");

                entity.Property(e => e.Email)
                    .HasColumnType("text")
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .HasColumnType("text")
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasColumnType("text")
                    .HasColumnName("phone");
            });

            modelBuilder.Entity<WaterContract>(entity =>
            {
                entity.ToTable("Water_Contract");

                entity.Property(e => e.WaterContractId)
                    .HasColumnType("varchar(8)")
                    .HasColumnName("water_contract_id");

                entity.Property(e => e.AccountNum)
                    .HasColumnType("varchar(10)")
                    .HasColumnName("account_num");

                entity.Property(e => e.AptId)
                    .HasColumnType("varchar(8)")
                    .HasColumnName("apt_id");

                entity.Property(e => e.ConsumeEndDate)
                    .HasColumnType("date")
                    .HasColumnName("consume_end_date")
                    .HasConversion<string>();

                entity.Property(e => e.ConsumeStartDate)
                    .HasColumnType("date")
                    .HasColumnName("consume_start_date")
                    .HasConversion<string>();

                entity.Property(e => e.ExpDate)
                    .HasColumnType("date")
                    .HasColumnName("exp_date")
                    .HasConversion<string>();

                entity.HasOne(d => d.Apt)
                    .WithMany(p => p.WaterContracts)
                    .HasForeignKey(d => d.AptId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
