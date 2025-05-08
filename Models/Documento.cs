using System;
using System.ComponentModel.DataAnnotations;

namespace Docspider.Models
{
    public class Documento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres.")]
        public string Titulo { get; set; }

        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O nome do arquivo é obrigatório.")]
        public string NomeArquivo { get; set; }

        public string CaminhoArquivo { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}