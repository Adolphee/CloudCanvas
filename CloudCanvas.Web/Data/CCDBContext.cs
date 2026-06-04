using CloudCanvas.Web.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudCanvas.Web.Data;

public class CCDBContext(DbContextOptions<CCDBContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    
}
