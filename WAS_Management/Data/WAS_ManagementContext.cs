using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using WAS_Management.Models;

namespace WAS_Management.Data;

public partial class WAS_ManagementContext : DbContext
{
    public WAS_ManagementContext()
    {
    }

    public WAS_ManagementContext(DbContextOptions<WAS_ManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contractor> Contractors { get; set; }

    public virtual DbSet<ContractorForm> ContractorForms { get; set; }

    public virtual DbSet<Interaction> Interactions { get; set; }

    public virtual DbSet<InteractionType> InteractionTypes { get; set; }

    public virtual DbSet<Resalenoc> Resalenocs { get; set; }

    public virtual DbSet<StepAction> StepActions { get; set; }

    public virtual DbSet<WAS_Management.Models.Task> Tasks { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserTask> UserTasks { get; set; }

    public virtual DbSet<Workflow> Workflows { get; set; }

    public virtual DbSet<WorkflowDocument> WorkflowDocuments { get; set; }

    public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }

    public virtual DbSet<WorkflowType> WorkflowTypes { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseMySql("name=DefaultConnection", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql"));

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
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Authorization)
                .HasMaxLength(255)
                .HasColumnName("authorization");
            entity.Property(e => e.Bpnumber)
                .HasMaxLength(255)
                .HasColumnName("bpnumber");
            entity.Property(e => e.Color)
                .HasMaxLength(255)
                .HasColumnName("color");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.Companytradelicence)
                .HasMaxLength(255)
                .HasColumnName("companytradelicence");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Emirateid)
                .HasMaxLength(255)
                .HasColumnName("emirateid");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.Landlineno)
                .HasMaxLength(255)
                .HasColumnName("landlineno");
            entity.Property(e => e.Mobileno)
                .HasMaxLength(255)
                .HasColumnName("mobileno");
            entity.Property(e => e.Passportid)
                .HasMaxLength(255)
                .HasColumnName("passportid");
            entity.Property(e => e.Paymentoption)
                .HasMaxLength(255)
                .HasColumnName("paymentoption");
            entity.Property(e => e.Placeofissuance)
                .HasMaxLength(255)
                .HasColumnName("placeofissuance");
            entity.Property(e => e.Plateno)
                .HasMaxLength(255)
                .HasColumnName("plateno");
            entity.Property(e => e.Platetype)
                .HasMaxLength(255)
                .HasColumnName("platetype");
            entity.Property(e => e.Previouspermit)
                .HasMaxLength(255)
                .HasColumnName("previouspermit");
            entity.Property(e => e.RenewalDate)
                .HasColumnType("datetime")
                .HasColumnName("renewal_date");
            entity.Property(e => e.Typeofvehicle)
                .HasMaxLength(255)
                .HasColumnName("typeofvehicle");
            entity.Property(e => e.VehiclePic)
                .HasMaxLength(255)
                .HasColumnName("vehicle_pic");
            entity.Property(e => e.VehicleReg)
                .HasMaxLength(255)
                .HasColumnName("vehicle_reg");
        });

        modelBuilder.Entity<ContractorForm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("contractor_forms");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Company)
                .HasMaxLength(255)
                .HasColumnName("company");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("contact");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.EmiratesId)
                .HasMaxLength(255)
                .HasColumnName("emirates_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.ThirdPartyLiability)
                .HasMaxLength(255)
                .HasColumnName("third_party_liability");
            entity.Property(e => e.TradeLicence)
                .HasMaxLength(255)
                .HasColumnName("trade_licence");
            entity.Property(e => e.VehicleRegistration)
                .HasMaxLength(255)
                .HasColumnName("vehicle_registration");
        });

        modelBuilder.Entity<Interaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("interaction");

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
            entity.Property(e => e.CurrentLayout)
                .HasMaxLength(255)
                .HasColumnName("current_layout");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(255)
                .HasColumnName("customer_name");
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
            entity.Property(e => e.EndDuration)
                .HasColumnType("datetime")
                .HasColumnName("end_duration");
            entity.Property(e => e.FollowUp)
                .HasMaxLength(255)
                .HasColumnName("follow_up");
            entity.Property(e => e.FollowUpDate)
                .HasColumnType("datetime")
                .HasColumnName("follow_up_date");
            entity.Property(e => e.InternalWork)
                .HasColumnType("text")
                .HasColumnName("internal_work");
            entity.Property(e => e.OwnerName)
                .HasMaxLength(255)
                .HasColumnName("owner_name");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(255)
                .HasColumnName("project_name");
            entity.Property(e => e.PropertyType)
                .HasMaxLength(255)
                .HasColumnName("property_type");
            entity.Property(e => e.ProposedLayout)
                .HasMaxLength(255)
                .HasColumnName("proposed_layout");
            entity.Property(e => e.PurposeOfInteraction)
                .HasMaxLength(255)
                .HasColumnName("purpose_of_interaction");
            entity.Property(e => e.Remarks)
                .HasMaxLength(255)
                .HasColumnName("remarks");
            entity.Property(e => e.ScopeOfWork)
                .HasMaxLength(255)
                .HasColumnName("scope_of_work");
            entity.Property(e => e.StartDuration)
                .HasColumnType("datetime")
                .HasColumnName("start_duration");
            entity.Property(e => e.ThirdPartyLiabilityCert)
                .HasMaxLength(255)
                .HasColumnName("third_party_liability_cert");
            entity.Property(e => e.TradeLicence)
                .HasMaxLength(255)
                .HasColumnName("trade_licence");
            entity.Property(e => e.TradeLicenceNo)
                .HasMaxLength(255)
                .HasColumnName("trade_licence_no");
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

        modelBuilder.Entity<InteractionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("interaction_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Resalenoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("resalenoc");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Contactno)
                .HasMaxLength(255)
                .HasColumnName("contactno");
            entity.Property(e => e.Createdby)
                .HasMaxLength(255)
                .HasColumnName("createdby");
            entity.Property(e => e.Createddate)
                .HasMaxLength(255)
                .HasColumnName("createddate");
            entity.Property(e => e.Customername)
                .HasMaxLength(255)
                .HasColumnName("customername");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Intiatorname)
                .HasMaxLength(50)
                .HasColumnName("intiatorname");
            entity.Property(e => e.Mastercomm)
                .HasMaxLength(255)
                .HasColumnName("mastercomm");
            entity.Property(e => e.Projectname)
                .HasMaxLength(255)
                .HasColumnName("projectname");
            entity.Property(e => e.Subprojectname)
                .HasMaxLength(255)
                .HasColumnName("subprojectname");
            entity.Property(e => e.Unitno)
                .HasMaxLength(255)
                .HasColumnName("unitno");
        });

        modelBuilder.Entity<StepAction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("step_actions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActionType)
                .HasColumnType("enum('RFI','Reassign','Return Step','Submit')")
                .HasColumnName("action_type");
            entity.Property(e => e.Answer).HasMaxLength(255);
            entity.Property(e => e.ApprovalEndDate).HasColumnType("datetime");
            entity.Property(e => e.ApprovalStartDate).HasColumnType("datetime");
            entity.Property(e => e.AssignTo).HasColumnName("assignTo");
            entity.Property(e => e.BuiltupExtensionFee).HasPrecision(18, 2);
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.Comments)
                .HasColumnType("text")
                .HasColumnName("comments");
            entity.Property(e => e.ContactNumber).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.InvNuumber).HasMaxLength(255);
            entity.Property(e => e.ModificationFee).HasPrecision(18, 2);
            entity.Property(e => e.ModificationRequest).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PaidBy).HasMaxLength(255);
            entity.Property(e => e.PerformedBy).HasColumnName("performed_by");
            entity.Property(e => e.PerformedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("performed_on");
            entity.Property(e => e.PrevStepId).HasMaxLength(255);
            entity.Property(e => e.StepId).HasColumnName("step_id");
            entity.Property(e => e.SubCat).HasMaxLength(255);
            entity.Property(e => e.SubCategory).HasMaxLength(255);
            entity.Property(e => e.Total).HasPrecision(18, 2);
            entity.Property(e => e.UnlistedContractorFee).HasPrecision(18, 2);
            entity.Property(e => e.VendorName).HasMaxLength(255);
        });

        modelBuilder.Entity<WAS_Management.Models.Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tasks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ageing).HasColumnName("ageing");
            entity.Property(e => e.AssignedTo).HasColumnName("assigned_to");
            entity.Property(e => e.Department)
                .HasMaxLength(255)
                .HasColumnName("department");
            entity.Property(e => e.DueDate)
                .HasColumnType("datetime")
                .HasColumnName("due_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.StepId).HasColumnName("step_id");
            entity.Property(e => e.TaskType)
                .HasColumnType("enum('Workflow','RFI','Reassigned','Return Step')")
                .HasColumnName("task_type");
            entity.Property(e => e.Template)
                .HasMaxLength(255)
                .HasColumnName("template");
            entity.Property(e => e.WorkflowId).HasColumnName("workflow_id");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("units");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Newnumber)
                .HasMaxLength(255)
                .HasColumnName("newnumber");
            entity.Property(e => e.Sapno)
                .HasMaxLength(255)
                .HasColumnName("sapno");
            entity.Property(e => e.Slno)
                .HasMaxLength(255)
                .HasColumnName("slno");
            entity.Property(e => e.Sqft)
                .HasMaxLength(255)
                .HasColumnName("sqft");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
            entity.Property(e => e.TypeP)
                .HasMaxLength(255)
                .HasColumnName("type_p");
            entity.Property(e => e.Unitid)
                .HasMaxLength(255)
                .HasColumnName("unitid");
            entity.Property(e => e.View)
                .HasMaxLength(255)
                .HasColumnName("view");
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
            entity.Property(e => e.Fullname)
                .HasMaxLength(255)
                .HasColumnName("fullname");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_tasks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ageing).HasColumnName("ageing");
            entity.Property(e => e.AssignedTo).HasColumnName("assigned_to");
            entity.Property(e => e.Department)
                .HasMaxLength(255)
                .HasColumnName("department");
            entity.Property(e => e.DueDate)
                .HasColumnType("datetime")
                .HasColumnName("due_date");
            entity.Property(e => e.Isviewed)
                .HasDefaultValueSql("'0'")
                .HasColumnName("isviewed");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.StepId).HasColumnName("step_id");
            entity.Property(e => e.TaskType)
                .HasColumnType("enum('Workflow','RFI','Reassigned','Return Step','CONTRACTOR REGISTRATION','CONTRACTOR RENEWAL','Resale NOC')")
                .HasColumnName("task_type");
            entity.Property(e => e.Template)
                .HasMaxLength(255)
                .HasColumnName("template");
            entity.Property(e => e.WorkflowId).HasColumnName("workflow_id");
        });

        modelBuilder.Entity<Workflow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("workflows");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(8, 2)
                .HasColumnName("amount");
            entity.Property(e => e.ApprovalEndDate)
                .HasColumnType("datetime")
                .HasColumnName("approval_end_date");
            entity.Property(e => e.ApprovalStartDate)
                .HasColumnType("datetime")
                .HasColumnName("approval_start_date");
            entity.Property(e => e.Details)
                .HasColumnType("json")
                .HasColumnName("details");
            entity.Property(e => e.Identifier).HasMaxLength(255);
            entity.Property(e => e.InitiatorId).HasColumnName("initiator_id");
            entity.Property(e => e.InteractionId)
                .HasMaxLength(255)
                .HasColumnName("interaction_id");
            entity.Property(e => e.PaidBy)
                .HasMaxLength(255)
                .HasColumnName("paid_by");
            entity.Property(e => e.ProcessOwner).HasColumnName("process_owner");
            entity.Property(e => e.Progress)
                .HasMaxLength(50)
                .HasColumnName("progress");
            entity.Property(e => e.ReceiptBy)
                .HasMaxLength(255)
                .HasColumnName("receipt_by");
            entity.Property(e => e.ReceiptDate)
                .HasColumnType("datetime")
                .HasColumnName("receipt_date");
            entity.Property(e => e.ReceiptNo)
                .HasMaxLength(255)
                .HasColumnName("receipt_no");
            entity.Property(e => e.StartedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("started_on");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Subject)
                .HasMaxLength(255)
                .HasColumnName("subject");
            entity.Property(e => e.VendorName)
                .HasMaxLength(255)
                .HasColumnName("vendor_name");
            entity.Property(e => e.WorkflowTypeId).HasColumnName("workflow_type_id");
            entity.Property(e => e.Worktype)
                .HasMaxLength(255)
                .HasColumnName("worktype");
        });

        modelBuilder.Entity<WorkflowDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("workflow_documents");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DocumentName)
                .HasMaxLength(255)
                .HasColumnName("document_name");
            entity.Property(e => e.DocumentPath)
                .HasMaxLength(255)
                .HasColumnName("document_path");
            entity.Property(e => e.StepId).HasColumnName("step_id");
            entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");
            entity.Property(e => e.UploadedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("uploaded_on");
            entity.Property(e => e.WorkflowId).HasColumnName("workflow_id");
        });

        modelBuilder.Entity<WorkflowStep>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("workflow_steps");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Actiondetails)
                .HasColumnType("json")
                .HasColumnName("actiondetails");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.AssignedTo)
                .HasColumnType("json")
                .HasColumnName("assigned_to");
            entity.Property(e => e.Details)
                .HasColumnType("json")
                .HasColumnName("details");
            entity.Property(e => e.DueOn)
                .HasColumnType("datetime")
                .HasColumnName("due_on");
            entity.Property(e => e.ExecutedOn)
                .HasColumnType("datetime")
                .HasColumnName("executed_on");
            entity.Property(e => e.ReceivedOn)
                .HasColumnType("datetime")
                .HasColumnName("received_on");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.StepDescription)
                .HasColumnType("text")
                .HasColumnName("step_description");
            entity.Property(e => e.StepName)
                .HasMaxLength(255)
                .HasColumnName("step_name");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.Type).HasColumnType("text");
            entity.Property(e => e.WorkflowId).HasColumnName("workflow_id");
        });

        modelBuilder.Entity<WorkflowType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("workflow_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
