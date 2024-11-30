using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models.DTOs.RuleNameDTOs
{
    public class RuleNameUpdateDTO
    {
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string Description { get; set; }

        [StringLength(500, ErrorMessage = "Json cannot exceed 500 characters.")]
        public string Json { get; set; }

        [StringLength(250, ErrorMessage = "Template cannot exceed 250 characters.")]
        public string Template { get; set; }

        [StringLength(500, ErrorMessage = "SqlStr cannot exceed 500 characters.")]
        public string SqlStr { get; set; }

        [StringLength(500, ErrorMessage = "SqlPart cannot exceed 500 characters.")]
        public string SqlPart { get; set; }
    }
}
