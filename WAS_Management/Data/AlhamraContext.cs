using System;
using System.Collections.Generic;
using Alhamra.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Alhamra.Data;

public partial class AlhamraContext : DbContext
{
    public AlhamraContext()
    {
    }

    public AlhamraContext(DbContextOptions<AlhamraContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contractor> Contractors { get; set; }

    public virtual DbSet<Interaction> Interactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("name=DefaultConnection", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.39-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Contractor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("contractors");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("contact");
            entity.Property(e => e.ContractorTradeLicence)
                .HasMaxLength(255)
                .HasColumnName("contractor_trade_licence");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.CurrentProposedLayout)
                .HasMaxLength(255)
                .HasColumnName("current_proposed_layout");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.EmiratesId)
                .HasMaxLength(255)
                .HasColumnName("emirates_id");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PaymentOptions)
                .HasMaxLength(255)
                .HasColumnName("payment_options");
            entity.Property(e => e.ScopeOfWork)
                .HasMaxLength(255)
                .HasColumnName("scope_of_work");
            entity.Property(e => e.ThirdPartyLiability)
                .HasMaxLength(255)
                .HasColumnName("third_party_liability");
        });

        modelBuilder.Entity<Interaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("interactions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContactNo)
                .HasMaxLength(255)
                .HasColumnName("contact_no");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("contact_number");
            entity.Property(e => e.ContractAgreement)
                .HasMaxLength(255)
                .HasColumnName("contract_agreement");
            entity.Property(e => e.ContractorComp)
                .HasMaxLength(255)
                .HasColumnName("contractor_comp");
            entity.Property(e => e.ContractorCompName)
                .HasMaxLength(255)
                .HasColumnName("contractor_comp_name");
            entity.Property(e => e.ContractorContact)
                .HasMaxLength(255)
                .HasColumnName("contractor_contact");
            entity.Property(e => e.ContractorEmail)
                .HasMaxLength(255)
                .HasColumnName("contractor_email");
            entity.Property(e => e.ContractorName)
                .HasMaxLength(255)
                .HasColumnName("contractor_name");
            entity.Property(e => e.ContractorRepName)
                .HasMaxLength(255)
                .HasColumnName("contractor_rep_name");
            entity.Property(e => e.CurrentProposedLayout)
                .HasPrecision(10)
                .HasColumnName("current_proposed_layout");
            entity.Property(e => e.CustomerType)
                .HasMaxLength(255)
                .HasColumnName("customer_type");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(255)
                .HasColumnName("email_address");
            entity.Property(e => e.EmirateId)
                .HasMaxLength(255)
                .HasColumnName("emirate_id");
            entity.Property(e => e.EndDuration).HasColumnName("end_duration");
            entity.Property(e => e.FollowUp)
                .HasMaxLength(255)
                .HasColumnName("follow_up");
            entity.Property(e => e.FollowUpDate)
                .HasColumnType("datetime")
                .HasColumnName("follow_up_date");
            entity.Property(e => e.InternalWork)
                .HasMaxLength(255)
                .HasColumnName("internal_work");
            entity.Property(e => e.OwnerName)
                .HasColumnType("datetime")
                .HasColumnName("owner_name");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(255)
                .HasColumnName("project_name");
            entity.Property(e => e.PropertyType)
                .HasMaxLength(255)
                .HasColumnName("property_type");
            entity.Property(e => e.PurposeOfInteraction)
                .HasMaxLength(255)
                .HasColumnName("purpose_of_interaction");
            entity.Property(e => e.Remarks).HasMaxLength(255);
            entity.Property(e => e.ScopeOfWork)
                .HasMaxLength(255)
                .HasColumnName("scope_of_work");
            entity.Property(e => e.StartDuration).HasColumnName("start_duration");
            entity.Property(e => e.ThirdPartyLiabilityCert)
                .HasMaxLength(255)
                .HasColumnName("third_party_liability_cert");
            entity.Property(e => e.TradeLicence)
                .HasMaxLength(255)
                .HasColumnName("trade_licence");
            entity.Property(e => e.TypeOfInteraction)
                .HasMaxLength(255)
                .HasColumnName("type_of_interaction");
            entity.Property(e => e.TypeSel)
                .HasMaxLength(255)
                .HasColumnName("type_sel");
            entity.Property(e => e.UnitNumber)
                .HasMaxLength(255)
                .HasColumnName("unit_number");
            entity.Property(e => e.WorkType)
                .HasMaxLength(255)
                .HasColumnName("work_type");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Department)
                .HasMaxLength(255)
                .HasColumnName("department");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
