using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using APIConnectorCore.Models;

    public class ClientContextDatabase : DbContext
    {
        public ClientContextDatabase (DbContextOptions<ClientContextDatabase> options)
            : base(options)
        {
        }

        public DbSet<APIConnectorCore.Models.Client> Client { get; set; } = default!;

public DbSet<APIConnectorCore.Models.Contract> Contract { get; set; } = default!;

public DbSet<APIConnectorCore.Models.ServiceRequest> ServiceRequest { get; set; } = default!;
    }
