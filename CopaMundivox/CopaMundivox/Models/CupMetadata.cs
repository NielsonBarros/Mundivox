using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CopaMundivox.Models
{
    [MetadataType(typeof(CupMetadata))]
    public partial class Cup
    {
    }
    public class CupMetadata
    {

        [Display(Name = "Nome da Copa")]
        [Required(ErrorMessage = "O Nome deve ser preenchido.")]
        [MaxLength(50, ErrorMessage = "O Nome deve conter até 50 dígitos.")]
        public string Name;

        [Display(AutoGenerateField = false, Name ="Dono da Copa")]
        public int UserId;

        [Display(AutoGenerateField = false,Name ="Times Cadastrados")]
        public ICollection<Team> Team;
    }
}