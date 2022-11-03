﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using pwnwrk.infra.Persistence;

#nullable disable

namespace pwnwrk.infra.Persistence.Migrations
{
    [DbContext(typeof(PwnctlDbContext))]
    [Migration("20221103083704_exot_code_rename")]
    partial class exot_code_rename
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.CloudService", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DomainId")
                        .HasColumnType("text");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FoundBy")
                        .HasColumnType("text");

                    b.Property<string>("Hostname")
                        .HasColumnType("text");

                    b.Property<bool>("InScope")
                        .HasColumnType("boolean");

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

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.DNSRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DomainId")
                        .HasColumnType("text");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FoundBy")
                        .HasColumnType("text");

                    b.Property<string>("HostId")
                        .HasColumnType("text");

                    b.Property<bool>("InScope")
                        .HasColumnType("boolean");

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

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Domain", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FoundBy")
                        .HasColumnType("text");

                    b.Property<bool>("InScope")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRegistrationDomain")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("RegistrationDomainId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("RegistrationDomainId");

                    b.ToTable("Domains");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Email", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("DomainId")
                        .HasColumnType("text");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FoundBy")
                        .HasColumnType("text");

                    b.Property<bool>("InScope")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("Address")
                        .IsUnique();

                    b.HasIndex("DomainId");

                    b.ToTable("Emails");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Endpoint", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FoundBy")
                        .HasColumnType("text");

                    b.Property<bool>("InScope")
                        .HasColumnType("boolean");

                    b.Property<string>("Path")
                        .HasColumnType("text");

                    b.Property<string>("Scheme")
                        .HasColumnType("text");

                    b.Property<string>("ServiceId")
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("Endpoints");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Host", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FoundBy")
                        .HasColumnType("text");

                    b.Property<string>("IP")
                        .HasColumnType("text");

                    b.Property<bool>("InScope")
                        .HasColumnType("boolean");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("IP")
                        .IsUnique();

                    b.ToTable("Hosts");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Keyword", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("DomainId")
                        .HasColumnType("text");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FoundBy")
                        .HasColumnType("text");

                    b.Property<bool>("InScope")
                        .HasColumnType("boolean");

                    b.Property<string>("Word")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.HasIndex("Word")
                        .IsUnique();

                    b.ToTable("Keywords");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.NetRange", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("FirstAddress")
                        .HasColumnType("text");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FoundBy")
                        .HasColumnType("text");

                    b.Property<bool>("InScope")
                        .HasColumnType("boolean");

                    b.Property<int>("NetPrefixBits")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FirstAddress", "NetPrefixBits")
                        .IsUnique();

                    b.ToTable("NetRanges");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Parameter", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("EndpointId")
                        .HasColumnType("text");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FoundBy")
                        .HasColumnType("text");

                    b.Property<bool>("InScope")
                        .HasColumnType("boolean");

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

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Service", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ApplicationProtocol")
                        .HasColumnType("text");

                    b.Property<string>("DomainId")
                        .HasColumnType("text");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FoundBy")
                        .HasColumnType("text");

                    b.Property<string>("HostId")
                        .HasColumnType("text");

                    b.Property<bool>("InScope")
                        .HasColumnType("boolean");

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

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.VirtualHost", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FoundBy")
                        .HasColumnType("text");

                    b.Property<bool>("InScope")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("ServiceId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.ToTable("VirtualHosts");
                });

            modelBuilder.Entity("pwnwrk.domain.Common.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CloudServiceId")
                        .HasColumnType("text");

                    b.Property<string>("DNSRecordId")
                        .HasColumnType("text");

                    b.Property<string>("DomainId")
                        .HasColumnType("text");

                    b.Property<string>("EmailId")
                        .HasColumnType("text");

                    b.Property<string>("EndpointId")
                        .HasColumnType("text");

                    b.Property<string>("HostId")
                        .HasColumnType("text");

                    b.Property<string>("KeywordId")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("NetRangeId")
                        .HasColumnType("text");

                    b.Property<string>("ParameterId")
                        .HasColumnType("text");

                    b.Property<string>("ServiceId")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.Property<string>("VirtualHostId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CloudServiceId");

                    b.HasIndex("EmailId");

                    b.HasIndex("KeywordId");

                    b.HasIndex("ParameterId");

                    b.HasIndex("VirtualHostId");

                    b.HasIndex("DNSRecordId", "Name")
                        .IsUnique();

                    b.HasIndex("DomainId", "Name")
                        .IsUnique();

                    b.HasIndex("EndpointId", "Name")
                        .IsUnique();

                    b.HasIndex("HostId", "Name")
                        .IsUnique();

                    b.HasIndex("NetRangeId", "Name")
                        .IsUnique();

                    b.HasIndex("ServiceId", "Name")
                        .IsUnique();

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("pwnwrk.domain.Notifications.Entities.NotificationRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Filter")
                        .HasColumnType("text");

                    b.Property<string>("Severity")
                        .HasColumnType("text");

                    b.Property<string>("ShortName")
                        .HasColumnType("text");

                    b.Property<string>("Subject")
                        .HasColumnType("text");

                    b.Property<string>("Topic")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("NotificationRules");
                });

            modelBuilder.Entity("pwnwrk.domain.Targets.Entities.OperationalPolicy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("AllowActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Blacklist")
                        .HasColumnType("text");

                    b.Property<int?>("MaxAggressiveness")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Whitelist")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("OperationalPolicies");
                });

            modelBuilder.Entity("pwnwrk.domain.Targets.Entities.Program", b =>
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

            modelBuilder.Entity("pwnwrk.domain.Targets.Entities.ScopeDefinition", b =>
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

            modelBuilder.Entity("pwnwrk.domain.Tasks.Entities.TaskDefinition", b =>
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

                    b.Property<string>("Subject")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TaskDefinitions");
                });

            modelBuilder.Entity("pwnwrk.domain.Tasks.Entities.TaskRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Arguments")
                        .HasColumnType("text");

                    b.Property<string>("CloudServiceId")
                        .HasColumnType("text");

                    b.Property<string>("DNSRecordId")
                        .HasColumnType("text");

                    b.Property<int>("DefinitionId")
                        .HasColumnType("integer");

                    b.Property<string>("DomainId")
                        .HasColumnType("text");

                    b.Property<string>("EmailId")
                        .HasColumnType("text");

                    b.Property<string>("EndpointId")
                        .HasColumnType("text");

                    b.Property<int?>("ExitCode")
                        .HasColumnType("integer");

                    b.Property<DateTime>("FinishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("HostId")
                        .HasColumnType("text");

                    b.Property<string>("KeywordId")
                        .HasColumnType("text");

                    b.Property<string>("NetRangeId")
                        .HasColumnType("text");

                    b.Property<string>("ParameterId")
                        .HasColumnType("text");

                    b.Property<DateTime>("QueuedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ServiceId")
                        .HasColumnType("text");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("VirtualHostId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CloudServiceId");

                    b.HasIndex("DNSRecordId");

                    b.HasIndex("DefinitionId");

                    b.HasIndex("DomainId");

                    b.HasIndex("EmailId");

                    b.HasIndex("EndpointId");

                    b.HasIndex("HostId");

                    b.HasIndex("KeywordId");

                    b.HasIndex("NetRangeId");

                    b.HasIndex("ParameterId");

                    b.HasIndex("ServiceId");

                    b.HasIndex("VirtualHostId");

                    b.ToTable("TaskRecords");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.CloudService", b =>
                {
                    b.HasOne("pwnwrk.domain.Assets.Entities.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.Navigation("Domain");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.DNSRecord", b =>
                {
                    b.HasOne("pwnwrk.domain.Assets.Entities.Domain", "Domain")
                        .WithMany("DNSRecords")
                        .HasForeignKey("DomainId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Host", "Host")
                        .WithMany("AARecords")
                        .HasForeignKey("HostId");

                    b.Navigation("Domain");

                    b.Navigation("Host");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Domain", b =>
                {
                    b.HasOne("pwnwrk.domain.Assets.Entities.Domain", "RegistrationDomain")
                        .WithMany()
                        .HasForeignKey("RegistrationDomainId");

                    b.Navigation("RegistrationDomain");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Email", b =>
                {
                    b.HasOne("pwnwrk.domain.Assets.Entities.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.Navigation("Domain");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Endpoint", b =>
                {
                    b.HasOne("pwnwrk.domain.Assets.Entities.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Keyword", b =>
                {
                    b.HasOne("pwnwrk.domain.Assets.Entities.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.Navigation("Domain");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Parameter", b =>
                {
                    b.HasOne("pwnwrk.domain.Assets.Entities.Endpoint", "Endpoint")
                        .WithMany()
                        .HasForeignKey("EndpointId");

                    b.Navigation("Endpoint");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Service", b =>
                {
                    b.HasOne("pwnwrk.domain.Assets.Entities.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Host", "Host")
                        .WithMany()
                        .HasForeignKey("HostId");

                    b.Navigation("Domain");

                    b.Navigation("Host");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.VirtualHost", b =>
                {
                    b.HasOne("pwnwrk.domain.Assets.Entities.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("pwnwrk.domain.Common.Entities.Tag", b =>
                {
                    b.HasOne("pwnwrk.domain.Assets.Entities.CloudService", "CloudService")
                        .WithMany("Tags")
                        .HasForeignKey("CloudServiceId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.DNSRecord", "DNSRecord")
                        .WithMany("Tags")
                        .HasForeignKey("DNSRecordId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Domain", "Domain")
                        .WithMany("Tags")
                        .HasForeignKey("DomainId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Email", null)
                        .WithMany("Tags")
                        .HasForeignKey("EmailId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Endpoint", "Endpoint")
                        .WithMany("Tags")
                        .HasForeignKey("EndpointId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Host", "Host")
                        .WithMany("Tags")
                        .HasForeignKey("HostId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Keyword", null)
                        .WithMany("Tags")
                        .HasForeignKey("KeywordId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.NetRange", "NetRange")
                        .WithMany("Tags")
                        .HasForeignKey("NetRangeId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Parameter", null)
                        .WithMany("Tags")
                        .HasForeignKey("ParameterId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Service", "Service")
                        .WithMany("Tags")
                        .HasForeignKey("ServiceId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.VirtualHost", null)
                        .WithMany("Tags")
                        .HasForeignKey("VirtualHostId");

                    b.Navigation("CloudService");

                    b.Navigation("DNSRecord");

                    b.Navigation("Domain");

                    b.Navigation("Endpoint");

                    b.Navigation("Host");

                    b.Navigation("NetRange");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("pwnwrk.domain.Targets.Entities.Program", b =>
                {
                    b.HasOne("pwnwrk.domain.Targets.Entities.OperationalPolicy", "Policy")
                        .WithOne()
                        .HasForeignKey("pwnwrk.domain.Targets.Entities.Program", "PolicyId");

                    b.Navigation("Policy");
                });

            modelBuilder.Entity("pwnwrk.domain.Targets.Entities.ScopeDefinition", b =>
                {
                    b.HasOne("pwnwrk.domain.Targets.Entities.Program", "Program")
                        .WithMany("Scope")
                        .HasForeignKey("ProgramId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Program");
                });

            modelBuilder.Entity("pwnwrk.domain.Tasks.Entities.TaskRecord", b =>
                {
                    b.HasOne("pwnwrk.domain.Assets.Entities.CloudService", "CloudService")
                        .WithMany("Tasks")
                        .HasForeignKey("CloudServiceId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.DNSRecord", "DNSRecord")
                        .WithMany("Tasks")
                        .HasForeignKey("DNSRecordId");

                    b.HasOne("pwnwrk.domain.Tasks.Entities.TaskDefinition", "Definition")
                        .WithMany()
                        .HasForeignKey("DefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnwrk.domain.Assets.Entities.Domain", "Domain")
                        .WithMany("Tasks")
                        .HasForeignKey("DomainId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Email", null)
                        .WithMany("Tasks")
                        .HasForeignKey("EmailId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Endpoint", "Endpoint")
                        .WithMany("Tasks")
                        .HasForeignKey("EndpointId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Host", "Host")
                        .WithMany("Tasks")
                        .HasForeignKey("HostId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Keyword", "Keyword")
                        .WithMany("Tasks")
                        .HasForeignKey("KeywordId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.NetRange", "NetRange")
                        .WithMany("Tasks")
                        .HasForeignKey("NetRangeId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Parameter", null)
                        .WithMany("Tasks")
                        .HasForeignKey("ParameterId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.Service", "Service")
                        .WithMany("Tasks")
                        .HasForeignKey("ServiceId");

                    b.HasOne("pwnwrk.domain.Assets.Entities.VirtualHost", null)
                        .WithMany("Tasks")
                        .HasForeignKey("VirtualHostId");

                    b.Navigation("CloudService");

                    b.Navigation("DNSRecord");

                    b.Navigation("Definition");

                    b.Navigation("Domain");

                    b.Navigation("Endpoint");

                    b.Navigation("Host");

                    b.Navigation("Keyword");

                    b.Navigation("NetRange");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.CloudService", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.DNSRecord", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Domain", b =>
                {
                    b.Navigation("DNSRecords");

                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Email", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Endpoint", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Host", b =>
                {
                    b.Navigation("AARecords");

                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Keyword", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.NetRange", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Parameter", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.Service", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnwrk.domain.Assets.Entities.VirtualHost", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnwrk.domain.Targets.Entities.Program", b =>
                {
                    b.Navigation("Scope");
                });
#pragma warning restore 612, 618
        }
    }
}
