﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using pwnctl.infra.Persistence;

#nullable disable

namespace pwnctl.infra.Persistence.Migrations
{
    [DbContext(typeof(PwnctlDbContext))]
    partial class PwnctlDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.8");

            modelBuilder.Entity("pwnctl.core.Entities.Assets.DNSRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("DomainId")
                        .HasColumnType("TEXT");

                    b.Property<string>("DomainId1")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("HostId")
                        .HasColumnType("TEXT");

                    b.Property<string>("HostId1")
                        .HasColumnType("TEXT");

                    b.Property<bool>("InScope")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRoutable")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.HasIndex("DomainId1");

                    b.HasIndex("HostId");

                    b.HasIndex("HostId1");

                    b.HasIndex("Type", "Key")
                        .IsUnique();

                    b.ToTable("DNSRecords");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Domain", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("InScope")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRegistrationDomain")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRoutable")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("RegistrationDomainId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("RegistrationDomainId");

                    b.ToTable("Domains");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Endpoint", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("InScope")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRoutable")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<string>("Scheme")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServiceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("Endpoints");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Host", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("IP")
                        .HasColumnType("TEXT");

                    b.Property<bool>("InScope")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRoutable")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Version")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("IP")
                        .IsUnique();

                    b.ToTable("Hosts");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Keyword", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("InScope")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRoutable")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Word")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Word")
                        .IsUnique();

                    b.ToTable("Keywords");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.NetRange", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstAddress")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("InScope")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRoutable")
                        .HasColumnType("INTEGER");

                    b.Property<ushort>("NetPrefixBits")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FirstAddress", "NetPrefixBits")
                        .IsUnique();

                    b.ToTable("NetRanges");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Parameter", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("EndpointId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("InScope")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRoutable")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.Property<string>("UrlEncodedCsValues")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EndpointId");

                    b.HasIndex("Url", "Name", "Type")
                        .IsUnique();

                    b.ToTable("Parameters");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Service", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ApplicationProtocol")
                        .HasColumnType("TEXT");

                    b.Property<string>("DomainId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("HostId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("InScope")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRoutable")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Origin")
                        .HasColumnType("TEXT");

                    b.Property<ushort>("Port")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TransportProtocol")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.HasIndex("HostId");

                    b.HasIndex("Origin")
                        .IsUnique();

                    b.ToTable("Services");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.VirtualHost", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("InScope")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRoutable")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServiceId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.ToTable("VirtualHosts");
                });

            modelBuilder.Entity("pwnctl.core.Entities.NotificationChannel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Filter")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("ProviderId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProviderId");

                    b.ToTable("NotificationChannels");
                });

            modelBuilder.Entity("pwnctl.core.Entities.NotificationProviderSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("NotificationProviderSettings");
                });

            modelBuilder.Entity("pwnctl.core.Entities.NotificationRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Filter")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Severity")
                        .HasColumnType("TEXT");

                    b.Property<string>("ShortName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Subject")
                        .HasColumnType("TEXT");

                    b.Property<string>("Topic")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("NotificationRules");
                });

            modelBuilder.Entity("pwnctl.core.Entities.OperationalPolicy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AllowActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Blacklist")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("MaxAggressiveness")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Whitelist")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("OperationalPolicies");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Program", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Platform")
                        .HasColumnType("TEXT");

                    b.Property<int?>("PolicyId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PolicyId")
                        .IsUnique();

                    b.ToTable("Programs");
                });

            modelBuilder.Entity("pwnctl.core.Entities.ScopeDefinition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Pattern")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProgramId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProgramId");

                    b.ToTable("ScopeDefinitions");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DNSRecordId")
                        .HasColumnType("TEXT");

                    b.Property<string>("DomainId")
                        .HasColumnType("TEXT");

                    b.Property<string>("EndpointId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("HostId")
                        .HasColumnType("TEXT");

                    b.Property<string>("KeywordId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("NetRangeId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ParameterId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServiceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.Property<string>("VirtualHostId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

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

            modelBuilder.Entity("pwnctl.core.Entities.Task", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Arguments")
                        .HasColumnType("TEXT");

                    b.Property<string>("DNSRecordId")
                        .HasColumnType("TEXT");

                    b.Property<int>("DefinitionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DomainId")
                        .HasColumnType("TEXT");

                    b.Property<string>("EndpointId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FinishedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("HostId")
                        .HasColumnType("TEXT");

                    b.Property<string>("KeywordId")
                        .HasColumnType("TEXT");

                    b.Property<string>("NetRangeId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ParameterId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("QueuedAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ReturnCode")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ServiceId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("VirtualHostId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DNSRecordId");

                    b.HasIndex("DefinitionId");

                    b.HasIndex("DomainId");

                    b.HasIndex("EndpointId");

                    b.HasIndex("HostId");

                    b.HasIndex("KeywordId");

                    b.HasIndex("NetRangeId");

                    b.HasIndex("ParameterId");

                    b.HasIndex("ServiceId");

                    b.HasIndex("VirtualHostId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("pwnctl.core.Entities.TaskDefinition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Aggressiveness")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CommandTemplate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Filter")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ShortName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Subject")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TaskDefinitions");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.DNSRecord", b =>
                {
                    b.HasOne("pwnctl.core.Entities.Assets.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.HasOne("pwnctl.core.Entities.Assets.Domain", null)
                        .WithMany("DNSRecords")
                        .HasForeignKey("DomainId1");

                    b.HasOne("pwnctl.core.Entities.Assets.Host", "Host")
                        .WithMany()
                        .HasForeignKey("HostId");

                    b.HasOne("pwnctl.core.Entities.Assets.Host", null)
                        .WithMany("AARecords")
                        .HasForeignKey("HostId1");

                    b.Navigation("Domain");

                    b.Navigation("Host");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Domain", b =>
                {
                    b.HasOne("pwnctl.core.Entities.Assets.Domain", "RegistrationDomain")
                        .WithMany()
                        .HasForeignKey("RegistrationDomainId");

                    b.Navigation("RegistrationDomain");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Endpoint", b =>
                {
                    b.HasOne("pwnctl.core.Entities.Assets.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Parameter", b =>
                {
                    b.HasOne("pwnctl.core.Entities.Assets.Endpoint", "Endpoint")
                        .WithMany()
                        .HasForeignKey("EndpointId");

                    b.Navigation("Endpoint");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Service", b =>
                {
                    b.HasOne("pwnctl.core.Entities.Assets.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.HasOne("pwnctl.core.Entities.Assets.Host", "Host")
                        .WithMany()
                        .HasForeignKey("HostId");

                    b.Navigation("Domain");

                    b.Navigation("Host");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.VirtualHost", b =>
                {
                    b.HasOne("pwnctl.core.Entities.Assets.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("pwnctl.core.Entities.NotificationChannel", b =>
                {
                    b.HasOne("pwnctl.core.Entities.NotificationProviderSettings", "Provider")
                        .WithMany("Channels")
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Provider");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Program", b =>
                {
                    b.HasOne("pwnctl.core.Entities.OperationalPolicy", "Policy")
                        .WithOne()
                        .HasForeignKey("pwnctl.core.Entities.Program", "PolicyId");

                    b.Navigation("Policy");
                });

            modelBuilder.Entity("pwnctl.core.Entities.ScopeDefinition", b =>
                {
                    b.HasOne("pwnctl.core.Entities.Program", "Program")
                        .WithMany("Scope")
                        .HasForeignKey("ProgramId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Program");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Tag", b =>
                {
                    b.HasOne("pwnctl.core.Entities.Assets.DNSRecord", "DNSRecord")
                        .WithMany("Tags")
                        .HasForeignKey("DNSRecordId");

                    b.HasOne("pwnctl.core.Entities.Assets.Domain", "Domain")
                        .WithMany("Tags")
                        .HasForeignKey("DomainId");

                    b.HasOne("pwnctl.core.Entities.Assets.Endpoint", "Endpoint")
                        .WithMany("Tags")
                        .HasForeignKey("EndpointId");

                    b.HasOne("pwnctl.core.Entities.Assets.Host", "Host")
                        .WithMany("Tags")
                        .HasForeignKey("HostId");

                    b.HasOne("pwnctl.core.Entities.Assets.Keyword", null)
                        .WithMany("Tags")
                        .HasForeignKey("KeywordId");

                    b.HasOne("pwnctl.core.Entities.Assets.NetRange", "NetRange")
                        .WithMany("Tags")
                        .HasForeignKey("NetRangeId");

                    b.HasOne("pwnctl.core.Entities.Assets.Parameter", null)
                        .WithMany("Tags")
                        .HasForeignKey("ParameterId");

                    b.HasOne("pwnctl.core.Entities.Assets.Service", "Service")
                        .WithMany("Tags")
                        .HasForeignKey("ServiceId");

                    b.HasOne("pwnctl.core.Entities.Assets.VirtualHost", null)
                        .WithMany("Tags")
                        .HasForeignKey("VirtualHostId");

                    b.Navigation("DNSRecord");

                    b.Navigation("Domain");

                    b.Navigation("Endpoint");

                    b.Navigation("Host");

                    b.Navigation("NetRange");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Task", b =>
                {
                    b.HasOne("pwnctl.core.Entities.Assets.DNSRecord", "DNSRecord")
                        .WithMany("Tasks")
                        .HasForeignKey("DNSRecordId");

                    b.HasOne("pwnctl.core.Entities.TaskDefinition", "Definition")
                        .WithMany()
                        .HasForeignKey("DefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.core.Entities.Assets.Domain", "Domain")
                        .WithMany("Tasks")
                        .HasForeignKey("DomainId");

                    b.HasOne("pwnctl.core.Entities.Assets.Endpoint", "Endpoint")
                        .WithMany("Tasks")
                        .HasForeignKey("EndpointId");

                    b.HasOne("pwnctl.core.Entities.Assets.Host", "Host")
                        .WithMany("Tasks")
                        .HasForeignKey("HostId");

                    b.HasOne("pwnctl.core.Entities.Assets.Keyword", null)
                        .WithMany("Tasks")
                        .HasForeignKey("KeywordId");

                    b.HasOne("pwnctl.core.Entities.Assets.NetRange", "NetRange")
                        .WithMany("Tasks")
                        .HasForeignKey("NetRangeId");

                    b.HasOne("pwnctl.core.Entities.Assets.Parameter", null)
                        .WithMany("Tasks")
                        .HasForeignKey("ParameterId");

                    b.HasOne("pwnctl.core.Entities.Assets.Service", "Service")
                        .WithMany("Tasks")
                        .HasForeignKey("ServiceId");

                    b.HasOne("pwnctl.core.Entities.Assets.VirtualHost", null)
                        .WithMany("Tasks")
                        .HasForeignKey("VirtualHostId");

                    b.Navigation("DNSRecord");

                    b.Navigation("Definition");

                    b.Navigation("Domain");

                    b.Navigation("Endpoint");

                    b.Navigation("Host");

                    b.Navigation("NetRange");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.DNSRecord", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Domain", b =>
                {
                    b.Navigation("DNSRecords");

                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Endpoint", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Host", b =>
                {
                    b.Navigation("AARecords");

                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Keyword", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.NetRange", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Parameter", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Service", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.VirtualHost", b =>
                {
                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnctl.core.Entities.NotificationProviderSettings", b =>
                {
                    b.Navigation("Channels");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Program", b =>
                {
                    b.Navigation("Scope");
                });
#pragma warning restore 612, 618
        }
    }
}
