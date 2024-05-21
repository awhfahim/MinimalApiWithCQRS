using BubtCommunity.Persistence.IdentityMembers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BubtCommunity.Persistence;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<AppUser,AppRole,Guid>(options);