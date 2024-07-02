using System.ComponentModel.DataAnnotations;

namespace bookStoreApi.DTOs
{
    public class BookRequestDTO
    {
        
        public string Title { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Descrção precisa ter no máximo 300 caracteres.")]
        public string Description { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage ="O numero de páginas deve ser positivo.")]
        public int Pages { get; set; }
        [StringLength(100, ErrorMessage = "O nome do Autor deve ter até 100 caracteres.")]
        public string Author { get; set; } = string.Empty;

    }
}
