using LuftbornTask.Models;
using Microsoft.EntityFrameworkCore;

namespace LuftbornTask.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
}