﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using pwnctl.infra.Persistence;

#nullable disable

namespace pwnctl.infra.Persistence.Migrations
{
    [DbContext(typeof(PwnctlDbContext))]
    [Migration("20220904160844_uniqness")]
    partial class uniqness
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.8");

            modelBuilder.Entity("pwnctl.core.Entities.Assets.DNSRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DomainId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DomainId1")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("HostId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("HostId1")
                        .HasColumnType("INTEGER");

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
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

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

                    b.Property<int?>("RegistrationDomainId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("RegistrationDomainId");

                    b.ToTable("Domains");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Endpoint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

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

                    b.Property<int>("ServiceId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Uri")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.HasIndex("Uri")
                        .IsUnique();

                    b.ToTable("Endpoints");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Host", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

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

            modelBuilder.Entity("pwnctl.core.Entities.Assets.NetRange", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

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
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EndpointId")
                        .HasColumnType("INTEGER");

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

                    b.Property<string>("UrlEncodedCsValues")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EndpointId", "Name", "Type")
                        .IsUnique();

                    b.ToTable("Parameters");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ApplicationProtocol")
                        .HasColumnType("TEXT");

                    b.Property<int?>("DomainId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("HostId")
                        .HasColumnType("INTEGER");

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
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("InScope")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRoutable")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("ServiceId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.ToTable("VirtualHosts");
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

                    b.Property<int?>("DNSRecordId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DomainId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EndpointId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("HostId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("NetRangeId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ParameterId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ServiceId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.Property<int?>("VirtualHostId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

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

                    b.Property<int?>("DNSRecordId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DefinitionId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DomainId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EndpointId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FinishedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("HostId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("NetRangeId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ParameterId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("QueuedAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ReturnCode")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ServiceId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("VirtualHostId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DNSRecordId");

                    b.HasIndex("DefinitionId");

                    b.HasIndex("DomainId");

                    b.HasIndex("EndpointId");

                    b.HasIndex("HostId");

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
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");
                });

            modelBuilder.Entity("pwnctl.core.Entities.Assets.Parameter", b =>
                {
                    b.HasOne("pwnctl.core.Entities.Assets.Endpoint", "Endpoint")
                        .WithMany()
                        .HasForeignKey("EndpointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");
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

            modelBuilder.Entity("pwnctl.core.Entities.Program", b =>
                {
                    b.Navigation("Scope");
                });
#pragma warning restore 612, 618
        }
    }
}
