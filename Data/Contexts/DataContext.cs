
﻿using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<UserEntity>(options)
{

    public virtual DbSet<StatusEntity> Statuses { get; set; }
    public virtual DbSet<ClientEntity> Clients { get; set; }
    public virtual DbSet<ProjectEntity> Projects { get; set; }
    public virtual DbSet<UserProfileEntity> UserProfiles { get; set; }
    public virtual DbSet<UserAddressEntity> UserAddresses { get; set; }
    public virtual DbSet<PostalCodeEntity> PostalCodes { get; set; }


}
