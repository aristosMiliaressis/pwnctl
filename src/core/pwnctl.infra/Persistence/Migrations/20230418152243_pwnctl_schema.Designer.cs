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
    [Migration("20230418152243_pwnctl_schema")]
    partial class pwnctl_schema
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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("DomainNameId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("DomainNameRecordId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("EmailId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("FoundAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("FoundByTaskId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("HttpEndpointId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("HttpHostId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("HttpParameterId")
                        .HasColumnType("uuid");

                    b.Property<bool>("InScope")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("NetworkHostId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("NetworkRangeId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("NetworkSocketId")
                        .HasColumnType("uuid");

                    b.Property<int?>("ScopeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DomainNameId");

                    b.HasIndex("DomainNameRecordId");

                    b.HasIndex("EmailId");

                    b.HasIndex("FoundByTaskId");

                    b.HasIndex("HttpEndpointId");

                    b.HasIndex("HttpHostId");

                    b.HasIndex("HttpParameterId");

                    b.HasIndex("NetworkHostId");

                    b.HasIndex("NetworkRangeId");

                    b.HasIndex("NetworkSocketId");

                    b.HasIndex("ScopeId");

                    b.ToTable("asset_records", (string)null);
                });

            modelBuilder.Entity("pwnctl.app.Notifications.Entities.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Guid>("RecordId")
                        .HasColumnType("uuid");

                    b.Property<int>("RuleId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("RecordId");

                    b.HasIndex("RuleId");

                    b.ToTable("notifications", (string)null);
                });

            modelBuilder.Entity("pwnctl.app.Notifications.Entities.NotificationRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("CheckOutOfScope")
                        .HasColumnType("boolean");

                    b.Property<string>("Filter")
                        .HasColumnType("text");

                    b.Property<string>("Template")
                        .HasColumnType("text");

                    b.Property<int>("Topic")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("notification_rules", (string)null);
                });

            modelBuilder.Entity("pwnctl.app.Operations.Entities.Operation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("PolicyId")
                        .HasColumnType("integer");

                    b.Property<int>("ScopeId")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PolicyId")
                        .IsUnique();

                    b.HasIndex("ScopeId")
                        .IsUnique();

                    b.ToTable("operations", (string)null);
                });

            modelBuilder.Entity("pwnctl.app.Operations.Entities.Policy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Blacklist")
                        .HasColumnType("text");

                    b.Property<long?>("MaxAggressiveness")
                        .HasColumnType("bigint");

                    b.Property<bool>("OnlyPassive")
                        .HasColumnType("boolean");

                    b.Property<int>("TaskProfileId")
                        .HasColumnType("integer");

                    b.Property<string>("Whitelist")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TaskProfileId");

                    b.ToTable("policies", (string)null);
                });

            modelBuilder.Entity("pwnctl.app.Scope.Entities.ScopeAggregate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("scope_aggregates", (string)null);
                });

            modelBuilder.Entity("pwnctl.app.Scope.Entities.ScopeDefinition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Pattern")
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("scope_definitions", (string)null);
                });

            modelBuilder.Entity("pwnctl.app.Scope.Entities.ScopeDefinitionAggregate", b =>
                {
                    b.Property<int>("AggregateId")
                        .HasColumnType("integer");

                    b.Property<int>("DefinitionId")
                        .HasColumnType("integer");

                    b.HasKey("AggregateId", "DefinitionId");

                    b.HasIndex("DefinitionId");

                    b.ToTable("scope_definition_aggregates", (string)null);
                });

            modelBuilder.Entity("pwnctl.app.Tagging.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid>("RecordId")
                        .HasColumnType("uuid");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RecordId", "Name")
                        .IsUnique();

                    b.ToTable("tags", (string)null);
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

                    b.Property<bool>("MatchOutOfScope")
                        .HasColumnType("boolean");

                    b.Property<int>("ProfileId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProfileId");

                    b.ToTable("task_definitions", (string)null);
                });

            modelBuilder.Entity("pwnctl.app.Tasks.Entities.TaskEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DefinitionId")
                        .HasColumnType("integer");

                    b.Property<int?>("ExitCode")
                        .HasColumnType("integer");

                    b.Property<DateTime>("FinishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("OperationId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("QueuedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("RecordId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DefinitionId");

                    b.HasIndex("OperationId");

                    b.HasIndex("RecordId");

                    b.ToTable("task_entries", (string)null);
                });

            modelBuilder.Entity("pwnctl.app.Tasks.Entities.TaskProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("task_profiles", (string)null);
                });

            modelBuilder.Entity("pwnctl.domain.Entities.DomainName", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("ParentDomainId")
                        .HasColumnType("uuid");

                    b.Property<int>("ZoneDepth")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParentDomainId");

                    b.ToTable("domain_names", (string)null);
                });

            modelBuilder.Entity("pwnctl.domain.Entities.DomainNameRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("DomainId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("HostId")
                        .HasColumnType("uuid");

                    b.Property<string>("Key")
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.HasIndex("HostId");

                    b.HasIndex("Type", "Key", "Value")
                        .IsUnique();

                    b.ToTable("domain_name_records", (string)null);
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Email", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<Guid>("DomainId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Address")
                        .IsUnique();

                    b.HasIndex("DomainId");

                    b.ToTable("emails", (string)null);
                });

            modelBuilder.Entity("pwnctl.domain.Entities.HttpEndpoint", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ParentEndpointId")
                        .HasColumnType("uuid");

                    b.Property<string>("Path")
                        .HasColumnType("text");

                    b.Property<string>("Scheme")
                        .HasColumnType("text");

                    b.Property<Guid>("SocketAddressId")
                        .HasColumnType("uuid");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ParentEndpointId");

                    b.HasIndex("SocketAddressId");

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("http_endpoints", (string)null);
                });

            modelBuilder.Entity("pwnctl.domain.Entities.HttpHost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid>("ServiceId")
                        .HasColumnType("uuid");

                    b.Property<string>("SocketAddress")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.HasIndex("Name", "SocketAddress")
                        .IsUnique();

                    b.ToTable("http_hosts", (string)null);
                });

            modelBuilder.Entity("pwnctl.domain.Entities.HttpParameter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("EndpointId")
                        .HasColumnType("uuid");

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

                    b.ToTable("http_parameters", (string)null);
                });

            modelBuilder.Entity("pwnctl.domain.Entities.NetworkHost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("IP")
                        .HasColumnType("text");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("IP")
                        .IsUnique();

                    b.ToTable("network_hosts", (string)null);
                });

            modelBuilder.Entity("pwnctl.domain.Entities.NetworkRange", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("FirstAddress")
                        .HasColumnType("text");

                    b.Property<int>("NetPrefixBits")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FirstAddress", "NetPrefixBits")
                        .IsUnique();

                    b.ToTable("network_ranges", (string)null);
                });

            modelBuilder.Entity("pwnctl.domain.Entities.NetworkSocket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<Guid?>("DomainNameId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("NetworkHostId")
                        .HasColumnType("uuid");

                    b.Property<int>("Port")
                        .HasColumnType("integer");

                    b.Property<int>("TransportProtocol")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Address")
                        .IsUnique();

                    b.HasIndex("DomainNameId");

                    b.HasIndex("NetworkHostId");

                    b.ToTable("network_sockets", (string)null);
                });

            modelBuilder.Entity("pwnctl.app.Assets.Aggregates.AssetRecord", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.DomainName", "DomainName")
                        .WithMany()
                        .HasForeignKey("DomainNameId");

                    b.HasOne("pwnctl.domain.Entities.DomainNameRecord", "DomainNameRecord")
                        .WithMany()
                        .HasForeignKey("DomainNameRecordId");

                    b.HasOne("pwnctl.domain.Entities.Email", "Email")
                        .WithMany()
                        .HasForeignKey("EmailId");

                    b.HasOne("pwnctl.app.Tasks.Entities.TaskEntry", "FoundByTask")
                        .WithMany()
                        .HasForeignKey("FoundByTaskId");

                    b.HasOne("pwnctl.domain.Entities.HttpEndpoint", "HttpEndpoint")
                        .WithMany()
                        .HasForeignKey("HttpEndpointId");

                    b.HasOne("pwnctl.domain.Entities.HttpHost", "HttpHost")
                        .WithMany()
                        .HasForeignKey("HttpHostId");

                    b.HasOne("pwnctl.domain.Entities.HttpParameter", "HttpParameter")
                        .WithMany()
                        .HasForeignKey("HttpParameterId");

                    b.HasOne("pwnctl.domain.Entities.NetworkHost", "NetworkHost")
                        .WithMany()
                        .HasForeignKey("NetworkHostId");

                    b.HasOne("pwnctl.domain.Entities.NetworkRange", "NetworkRange")
                        .WithMany()
                        .HasForeignKey("NetworkRangeId");

                    b.HasOne("pwnctl.domain.Entities.NetworkSocket", "NetworkSocket")
                        .WithMany()
                        .HasForeignKey("NetworkSocketId");

                    b.HasOne("pwnctl.app.Scope.Entities.ScopeDefinition", "Scope")
                        .WithMany()
                        .HasForeignKey("ScopeId");

                    b.OwnsOne("pwnctl.domain.ValueObjects.AssetClass", "SubjectClass", b1 =>
                        {
                            b1.Property<Guid>("AssetRecordId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("AssetRecordId");

                            b1.ToTable("asset_records");

                            b1.WithOwner()
                                .HasForeignKey("AssetRecordId");
                        });

                    b.Navigation("DomainName");

                    b.Navigation("DomainNameRecord");

                    b.Navigation("Email");

                    b.Navigation("FoundByTask");

                    b.Navigation("HttpEndpoint");

                    b.Navigation("HttpHost");

                    b.Navigation("HttpParameter");

                    b.Navigation("NetworkHost");

                    b.Navigation("NetworkRange");

                    b.Navigation("NetworkSocket");

                    b.Navigation("Scope");

                    b.Navigation("SubjectClass");
                });

            modelBuilder.Entity("pwnctl.app.Notifications.Entities.Notification", b =>
                {
                    b.HasOne("pwnctl.app.Assets.Aggregates.AssetRecord", "Record")
                        .WithMany("Notifications")
                        .HasForeignKey("RecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.app.Notifications.Entities.NotificationRule", "Rule")
                        .WithMany()
                        .HasForeignKey("RuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Record");

                    b.Navigation("Rule");
                });

            modelBuilder.Entity("pwnctl.app.Notifications.Entities.NotificationRule", b =>
                {
                    b.OwnsOne("pwnctl.app.Common.ValueObjects.ShortName", "ShortName", b1 =>
                        {
                            b1.Property<int>("NotificationRuleId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("NotificationRuleId");

                            b1.ToTable("notification_rules");

                            b1.WithOwner()
                                .HasForeignKey("NotificationRuleId");
                        });

                    b.OwnsOne("pwnctl.domain.ValueObjects.AssetClass", "SubjectClass", b1 =>
                        {
                            b1.Property<int>("NotificationRuleId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("NotificationRuleId");

                            b1.ToTable("notification_rules");

                            b1.WithOwner()
                                .HasForeignKey("NotificationRuleId");
                        });

                    b.Navigation("ShortName");

                    b.Navigation("SubjectClass");
                });

            modelBuilder.Entity("pwnctl.app.Operations.Entities.Operation", b =>
                {
                    b.HasOne("pwnctl.app.Operations.Entities.Policy", "Policy")
                        .WithOne()
                        .HasForeignKey("pwnctl.app.Operations.Entities.Operation", "PolicyId");

                    b.HasOne("pwnctl.app.Scope.Entities.ScopeAggregate", "Scope")
                        .WithOne()
                        .HasForeignKey("pwnctl.app.Operations.Entities.Operation", "ScopeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("pwnctl.app.Common.ValueObjects.ShortName", "ShortName", b1 =>
                        {
                            b1.Property<int>("OperationId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("OperationId");

                            b1.ToTable("operations");

                            b1.WithOwner()
                                .HasForeignKey("OperationId");
                        });

                    b.Navigation("Policy");

                    b.Navigation("Scope");

                    b.Navigation("ShortName");
                });

            modelBuilder.Entity("pwnctl.app.Operations.Entities.Policy", b =>
                {
                    b.HasOne("pwnctl.app.Tasks.Entities.TaskProfile", "TaskProfile")
                        .WithMany()
                        .HasForeignKey("TaskProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TaskProfile");
                });

            modelBuilder.Entity("pwnctl.app.Scope.Entities.ScopeAggregate", b =>
                {
                    b.OwnsOne("pwnctl.app.Common.ValueObjects.ShortName", "ShortName", b1 =>
                        {
                            b1.Property<int>("ScopeAggregateId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("ScopeAggregateId");

                            b1.ToTable("scope_aggregates");

                            b1.WithOwner()
                                .HasForeignKey("ScopeAggregateId");
                        });

                    b.Navigation("ShortName");
                });

            modelBuilder.Entity("pwnctl.app.Scope.Entities.ScopeDefinitionAggregate", b =>
                {
                    b.HasOne("pwnctl.app.Scope.Entities.ScopeAggregate", "Aggregate")
                        .WithMany("Definitions")
                        .HasForeignKey("AggregateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.app.Scope.Entities.ScopeDefinition", "Definition")
                        .WithMany()
                        .HasForeignKey("DefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Aggregate");

                    b.Navigation("Definition");
                });

            modelBuilder.Entity("pwnctl.app.Tagging.Entities.Tag", b =>
                {
                    b.HasOne("pwnctl.app.Assets.Aggregates.AssetRecord", "Record")
                        .WithMany("Tags")
                        .HasForeignKey("RecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Record");
                });

            modelBuilder.Entity("pwnctl.app.Tasks.Entities.TaskDefinition", b =>
                {
                    b.HasOne("pwnctl.app.Tasks.Entities.TaskProfile", "Profile")
                        .WithMany("TaskDefinitions")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("pwnctl.app.Common.ValueObjects.ShortName", "ShortName", b1 =>
                        {
                            b1.Property<int>("TaskDefinitionId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("TaskDefinitionId");

                            b1.ToTable("task_definitions");

                            b1.WithOwner()
                                .HasForeignKey("TaskDefinitionId");
                        });

                    b.OwnsOne("pwnctl.domain.ValueObjects.AssetClass", "SubjectClass", b1 =>
                        {
                            b1.Property<int>("TaskDefinitionId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("TaskDefinitionId");

                            b1.ToTable("task_definitions");

                            b1.WithOwner()
                                .HasForeignKey("TaskDefinitionId");
                        });

                    b.Navigation("Profile");

                    b.Navigation("ShortName");

                    b.Navigation("SubjectClass");
                });

            modelBuilder.Entity("pwnctl.app.Tasks.Entities.TaskEntry", b =>
                {
                    b.HasOne("pwnctl.app.Tasks.Entities.TaskDefinition", "Definition")
                        .WithMany()
                        .HasForeignKey("DefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.app.Operations.Entities.Operation", "Operation")
                        .WithMany()
                        .HasForeignKey("OperationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("pwnctl.app.Assets.Aggregates.AssetRecord", "Record")
                        .WithMany("Tasks")
                        .HasForeignKey("RecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Definition");

                    b.Navigation("Operation");

                    b.Navigation("Record");
                });

            modelBuilder.Entity("pwnctl.app.Tasks.Entities.TaskProfile", b =>
                {
                    b.OwnsOne("pwnctl.app.Common.ValueObjects.ShortName", "ShortName", b1 =>
                        {
                            b1.Property<int>("TaskProfileId")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("TaskProfileId");

                            b1.ToTable("task_profiles");

                            b1.WithOwner()
                                .HasForeignKey("TaskProfileId");
                        });

                    b.Navigation("ShortName");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.DomainName", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.DomainName", "ParentDomain")
                        .WithMany()
                        .HasForeignKey("ParentDomainId");

                    b.Navigation("ParentDomain");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.DomainNameRecord", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.DomainName", "DomainName")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.HasOne("pwnctl.domain.Entities.NetworkHost", "NetworkHost")
                        .WithMany("AARecords")
                        .HasForeignKey("HostId");

                    b.Navigation("DomainName");

                    b.Navigation("NetworkHost");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.Email", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.DomainName", "DomainName")
                        .WithMany()
                        .HasForeignKey("DomainId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DomainName");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.HttpEndpoint", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.HttpEndpoint", "ParentEndpoint")
                        .WithMany()
                        .HasForeignKey("ParentEndpointId");

                    b.HasOne("pwnctl.domain.Entities.NetworkSocket", "Socket")
                        .WithMany()
                        .HasForeignKey("SocketAddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParentEndpoint");

                    b.Navigation("Socket");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.HttpHost", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.NetworkSocket", "Socket")
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Socket");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.HttpParameter", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.HttpEndpoint", "Endpoint")
                        .WithMany("HttpParameters")
                        .HasForeignKey("EndpointId");

                    b.Navigation("Endpoint");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.NetworkSocket", b =>
                {
                    b.HasOne("pwnctl.domain.Entities.DomainName", "DomainName")
                        .WithMany()
                        .HasForeignKey("DomainNameId");

                    b.HasOne("pwnctl.domain.Entities.NetworkHost", "NetworkHost")
                        .WithMany()
                        .HasForeignKey("NetworkHostId");

                    b.Navigation("DomainName");

                    b.Navigation("NetworkHost");
                });

            modelBuilder.Entity("pwnctl.app.Assets.Aggregates.AssetRecord", b =>
                {
                    b.Navigation("Notifications");

                    b.Navigation("Tags");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("pwnctl.app.Scope.Entities.ScopeAggregate", b =>
                {
                    b.Navigation("Definitions");
                });

            modelBuilder.Entity("pwnctl.app.Tasks.Entities.TaskProfile", b =>
                {
                    b.Navigation("TaskDefinitions");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.HttpEndpoint", b =>
                {
                    b.Navigation("HttpParameters");
                });

            modelBuilder.Entity("pwnctl.domain.Entities.NetworkHost", b =>
                {
                    b.Navigation("AARecords");
                });
#pragma warning restore 612, 618
        }
    }
}