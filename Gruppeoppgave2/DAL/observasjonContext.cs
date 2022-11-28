using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Gruppeoppgave2.Model
{
    [ExcludeFromCodeCoverage]
    public class observasjoner
    {
        public int Id { get; set; }  
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public string Adresse { get; set; }
        public virtual Poststeder Poststed { get; set; }
        public string Når { get; set; }
        public string Beskrivelse { get; set; }

    }

    [ExcludeFromCodeCoverage]
    public class Poststeder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Postnr { get; set; }
        public String Poststed { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Brukere
    {
        public int Id { get; set; }
        public string Brukernavn { get; set; }
        public byte[] Passord { get; set; }
        public byte[] Salt { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class observasjonContext : DbContext
    {
            public observasjonContext(DbContextOptions<observasjonContext> options)
                    : base(options)
            {
                Database.EnsureCreated();
            }

        public DbSet<observasjoner> observasjoner { get; set; }
        public DbSet<Poststeder> Poststeder { get; set; }
        public DbSet<Brukere> Brukere { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

    }
}
