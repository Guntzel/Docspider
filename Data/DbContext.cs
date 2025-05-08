using Microsoft.EntityFrameworkCore;
using Docspider.Models;

namespace Docspider.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Representa a tabela 'documentos' no banco de dados
        public DbSet<Documento> Documentos { get; set; }
    }
}