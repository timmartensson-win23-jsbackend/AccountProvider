using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<UserAccountEntity>(options)
{
    public DbSet<UserAddressEntity> UserAddresses { get; set; }
    public DbSet<UserAccountEntity> UserAccounts { get; set; }
}
