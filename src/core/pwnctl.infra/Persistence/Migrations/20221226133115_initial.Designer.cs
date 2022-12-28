﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using pwnctl.infra.Persistence;

#nullable disable

namespace pwnctl.infra.Migrations
{
    [DbContext(typeof(PwnctlDbContext))]
    [Migration("20221226133115_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("pwnctl.app.Assets.Aggregates.AssetRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FoundBy")
                        .HasColumnType("text");

                    b.Property<bool>("InScope")
                        .HasColumnType("boolean");

                    b.Property<int?>("OwningProgramId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("OwningProgramId");

                    b.ToTable("AssetRecords");
                });

            modelBuilder.Entity("pwnctl.app.Notifications.Entities.NotificationRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Filter")
                        .HasColumnType("text");

                    b.Property<string>("ShortName")
                        .HasColumnType("text");

                    b.Property<string>("Topic")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("NotificationRules");
                });

            modelBuilder.Entity("pwnctl.app.Scope.Entities.OperationalPolicy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("AllowActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Blacklist")
                        .HasColumnType("text");

                    b.Property<long?>("MaxAggressiveness")
                        .HasColumnType("bigint");

                    b.Property<string>("Whitelist")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("OperationalPolicies");
                });

            modelBuilder.Entity("pwnctl.app.Scope.Entities.Program", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Platform")
                        .HasColumnType("text");

                    b.Property<int?>("PolicyId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PolicyId")
                        .IsUnique();

                    b.ToTable("Programs");
                });

            modelBuilder.Entity("pwnctl.app.Scope.Entities.ScopeDefinition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Pattern")
                        .HasColumnType("text");

                    b.Property<int?>("ProgramId")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProgramId");

                    b.ToTable("ScopeDefinitions");
                });

            modelBuilder.Entity("pwnctl.app.Tagging.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AssetRecordId")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("RecordId")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AssetRecordId");

                    b.HasIndex("RecordId", "Name")
                        .IsUnique();

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("pwnctl.app.Tasks.Entities.TaskDefinition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Aggressiveness")
                        .HasColumnType("integer");

                    b.Property<string>("CommandTemplate")
                        .HasColumnType("text");

                    b.Property<string>("Filter")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("ShortName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TaskDefinitions");
                });

            modelBuilder.Entity("pwnctl.app.Tasks.Entities.TaskEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AssetRecordId")
                        .HasColumnType("text");

                    b.Property<int>("DefinitionId")
                        .HasColumnType("integer");

                    b.Property<int?>("ExitCode")
                        .HasColumnType("integer");

                    b.Property<DateTime>("FinishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("QueuedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RecordId")
                        .HasColumnType("text");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AssetRecordId");

                    b.HasIndex("DefinitionId");

                    b.HasIndex("RecordId");

                    b.ToTable("TaskEntries");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.CloudService", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DomainId")
                        .HasColumnType("text");

                    b.Property<string>("Hostname")
                        .HasColumnType("text");

                    b.Property<string>("Provider")
                        .HasColumnType("text");

                    b.Property<string>("Service")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.HasIndex("Hostname")
                        .IsUnique();

                    b.ToTable("CloudService");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.DNSRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DomainId")
                        .HasColumnType("text");

                    b.Property<string>("HostId")
                        .HasColumnType("text");

                    b.Property<string>("Key")
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.HasIndex("HostId");

                    b.HasIndex("Type", "Key")
                        .IsUnique();

                    b.ToTable("DNSRecords");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Domain", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<bool>("IsRegistrationDomain")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("ParentDomainId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParentDomainId");

                    b.ToTable("Domains");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Email", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("DomainId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Address")
                        .IsUnique();

                    b.HasIndex("DomainId");

                    b.ToTable("Emails");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Endpoint", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ParentEndpointId")
                        .HasColumnType("text");

                    b.Property<string>("Path")
                        .HasColumnType("text");

                    b.Property<string>("Scheme")
                        .HasColumnType("text");

                    b.Property<string>("ServiceId")
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ParentEndpointId");

                    b.HasIndex("ServiceId");

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("Endpoints");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Host", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("IP")
                        .HasColumnType("text");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("IP")
                        .IsUnique();

                    b.ToTable("Hosts");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Keyword", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DomainId")
                        .HasColumnType("text");

                    b.Property<string>("Word")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DomainId")
                        .IsUnique();

                    b.HasIndex("Word")
                        .IsUnique();

                    b.ToTable("Keywords");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.NetRange", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("FirstAddress")
                        .HasColumnType("text");

                    b.Property<int>("NetPrefixBits")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FirstAddress", "NetPrefixBits")
                        .IsUnique();

                    b.ToTable("NetRanges");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Parameter", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("EndpointId")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.Property<string>("UrlEncodedCsValues")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EndpointId");

                    b.HasIndex("Url", "Name", "Type")
                        .IsUnique();

                    b.ToTable("Parameters");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Service", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ApplicationProtocol")
                        .HasColumnType("text");

                    b.Property<string>("DomainId")
                        .HasColumnType("text");

                    b.Property<string>("HostId")
                        .HasColumnType("text");

                    b.Property<string>("Origin")
                        .HasColumnType("text");

                    b.Property<int>("Port")
                        .HasColumnType("integer");

                    b.Property<int>("TransportProtocol")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.HasIndex("HostId");

                    b.HasIndex("Origin")
                        .IsUnique();

                    b.ToTable("Services");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.VirtualHost", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("ServiceId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.ToTable("VirtualHosts");
                });

            modelBuilder.Entity("pwnctl.app.Assets.Aggregates.AssetRecord", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.CloudService", "CloudService")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.domain.Entities.DNSRecord", "DNSRecord")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.domain.Entities.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.domain.Entities.Email", "Email")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.domain.Entities.Endpoint", "Endpoint")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.domain.Entities.Host", "Host")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.domain.Entities.Keyword", "Keyword")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.domain.Entities.NetRange", "NetRange")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.domain.Entities.Parameter", "Parameter")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.domain.Entities.Service", "Service")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.domain.Entities.VirtualHost", "VirtualHost")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.app.Scope.Entities.Program", "OwningProgram")
                        .WithMany()
                        .HasForeignKey("OwningProgramId");

                    b.OwnsOne("pwnctl.domain.ValueObjects.AssetClass", "SubjectClass", b1 =>
                        {
                            b1.Property<string>("AssetRecordId")
                                .HasColumnType("text");

                            b1.Property<string>("Class")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("AssetRecordId");

                            b1.ToTable("AssetRecords");

                            b1.WithOwner()
                                .HasForeignKey("AssetRecordId");
                        });

                    b.Navigation("CloudService");

                    b.Navigation("DNSRecord");

                    b.Navigation("Domain");

                    b.Navigation("Email");

                    b.Navigation("Endpoint");

                    b.Navigation("Host");

                    b.Navigation("Keyword");

                    b.Navigation("NetRange");

                    b.Navigation("OwningProgram");

                    b.Navigation("Parameter");

                    b.Navigation("Service");

                    b.Navigation("SubjectClass");

                    b.Navigation("VirtualHost");
                });

            modelBuilder.Entity("pwnctl.app.Notifications.Entities.NotificationRule", b =>
                {
                    b.OwnsOne("pwnctl.domain.ValueObjects.AssetClass", "SubjectClass", b1 =>
                        {
                            b1.Property<int>("NotificationRuleId")
                                .HasColumnType("integer");

                            b1.Property<string>("Class")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("NotificationRuleId");

                            b1.ToTable("NotificationRules");

                            b1.WithOwner()
                                .HasForeignKey("NotificationRuleId");
                        });

                    b.Navigation("SubjectClass");
                });

            modelBuilder.Entity("pwnctl.app.Scope.Entities.Program", b =>
                {
                    b.HasOne("pwnctl.app.Scope.Entities.OperationalPolicy", "Policy")
                        .WithOne()
                        .HasForeignKey("pwnctl.app.Scope.Entities.Program", "PolicyId");

                    b.Navigation("Policy");
                });

            modelBuilder.Entity("pwnctl.app.Scope.Entities.ScopeDefinition", b =>
                {
                    b.HasOne("pwnctl.app.Scope.Entities.Program", "Program")
                        .WithMany("Scope")
                        .HasForeignKey("ProgramId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Program");
                });

            modelBuilder.Entity("pwnctl.app.Tagging.Entities.Tag", b =>
                {
                    b.HasOne("pwnctl.app.Assets.Aggregates.AssetRecord", null)
                        .WithMany("Tags")
                        .HasForeignKey("AssetRecordId");

                    b.HasOne("pwnctl.app.Assets.Aggregates.AssetRecord", "Record")
                        .WithMany()
                        .HasForeignKey("RecordId");

                    b.Navigation("Record");
                });

            modelBuilder.Entity("pwnctl.app.Tasks.Entities.TaskDefinition", b =>
                {
                    b.OwnsOne("pwnctl.domain.ValueObjects.AssetClass", "SubjectClass", b1 =>
                        {
                            b1.Property<int>("TaskDefinitionId")
                                .HasColumnType("integer");

                            b1.Property<string>("Class")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("TaskDefinitionId");

                            b1.ToTable("TaskDefinitions");

                            b1.WithOwner()
                                .HasForeignKey("TaskDefinitionId");
                        });

                    b.Navigation("SubjectClass");
                });

            modelBuilder.Entity("pwnctl.app.Tasks.Entities.TaskEntry", b =>
                {
                    b.HasOne("pwnctl.app.Assets.Aggregates.AssetRecord", null)
                        .WithMany("Tasks")
                        .HasForeignKey("AssetRecordId");

                    b.HasOne("pwnctl.app.Tasks.Entities.TaskDefinition", "Definition")
                        .WithMany()
                        .HasForeignKey("DefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.app.Assets.Aggregates.AssetRecord", "Record")
                        .WithMany()
                        .HasForeignKey("RecordId");

                    b.Navigation("Definition");

                    b.Navigation("Record");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.CloudService", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.Navigation("Domain");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.DNSRecord", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.Domain", "Domain")
                        .WithMany("DNSRecords")
                        .HasForeignKey("DomainId");

                    b.HasOne("pwnctl.domain.Entities.Host", "Host")
                        .WithMany("AARecords")
                        .HasForeignKey("HostId");

                    b.Navigation("Domain");

                    b.Navigation("Host");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Domain", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.Domain", "ParentDomain")
                        .WithMany()
                        .HasForeignKey("ParentDomainId");

                    b.Navigation("ParentDomain");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Email", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.Navigation("Domain");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Endpoint", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.Endpoint", "ParentEndpoint")
                        .WithMany()
                        .HasForeignKey("ParentEndpointId");

                    b.HasOne("pwnctl.domain.Entities.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId");

                    b.Navigation("ParentEndpoint");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Keyword", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.Domain", "Domain")
                        .WithOne()
                        .HasForeignKey("pwnctl.domain.Entities.Keyword", "DomainId");

                    b.Navigation("Domain");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Parameter", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.Endpoint", "Endpoint")
                        .WithMany("Parameters")
                        .HasForeignKey("EndpointId");

                    b.Navigation("Endpoint");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Service", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.HasOne("pwnctl.domain.Entities.Host", "Host")
                        .WithMany()
                        .HasForeignKey("HostId");

                    b.Navigation("Domain");

                    b.Navigation("Host");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.VirtualHost", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("pwnctl.app.Assets.Aggregates.AssetRecord", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnctl.app.Scope.Entities.Program", b =>
                {
                    b.Navigation("Scope");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Domain", b =>
                {
                    b.Navigation("DNSRecords");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Endpoint", b =>
                {
                    b.Navigation("Parameters");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Host", b =>
                {
                    b.Navigation("AARecords");
                });
#pragma warning restore 612, 618
        }
    }
}
