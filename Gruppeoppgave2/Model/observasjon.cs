using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Gruppeoppgave2.Model
{
    [ExcludeFromCodeCoverage]
    public class observasjon
    {
        public int Id { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string Fornavn { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string Etternavn { get; set; }
        [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{2,50}")]
        public string Adresse { get; set; }
        [RegularExpression(@"[0-9]{4}")]
        public string Postnr { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string Poststed { get; set; }
        public string Når { get; set; }
        [RegularExpression(@"{2,20}")]
        public string Beskrivelse { get; set; }
    }
}
