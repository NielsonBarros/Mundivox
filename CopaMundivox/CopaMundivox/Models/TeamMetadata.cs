using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CopaMundivox.Models
{
    [MetadataType(typeof(TeamMetadata))]
    public partial class Team
    {
    }
        public class TeamMetadata
    {
        public int Id;

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O Nome deve ser preenchido.")]
        [MaxLength(50, ErrorMessage = "O Nome deve conter até 50 dígitos.")]
        public string Name;

        [Display(Name = "Chave")]
        public string Group;

        public int CupId;
        [Display(AutoGenerateField = false, Name = "Vencedor partida")]
        public Nullable<bool> winner;

        [Display(AutoGenerateField = false, Name = "Nome da Copa")]
        public Cup Cup;
    }
}