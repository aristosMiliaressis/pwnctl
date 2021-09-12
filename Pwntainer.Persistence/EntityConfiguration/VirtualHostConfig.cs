﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pwntainer.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pwntainer.Persistence.EntityConfiguration
{
    public class VirtualHostConfig : IEntityTypeConfiguration<VirtualHost>
    {
        public void Configure(EntityTypeBuilder<VirtualHost> builder)
        {
            builder.HasKey(e => e.Name);

            builder.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId);
        }
    }
    
}
