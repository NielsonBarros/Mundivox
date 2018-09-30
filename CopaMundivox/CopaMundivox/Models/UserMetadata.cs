using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CopaMundivox.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        [NotMapped]
        public string ConfirmPassword { get; set; }
    }
    public class UserMetadata
    {
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage ="O E-mail deve ser preenchido.")]
        public string Email;

        [Display(Name ="Senha")]
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(8, ErrorMessage = "A senha deve conter 8 dígitos, no mínimo.")]
        [MaxLength(15,ErrorMessage ="A Senha deve conter até 15 dígitos.")]
        [DataType(DataType.Password)]
        public string Password;

        [Display(Name = "Confirma Senha")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "As senhas não são iguais.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O Nome deve ser preenchido.")]
        [MaxLength(50, ErrorMessage = "O Nome deve conter até 50 dígitos.")]
        public string Name;
    }
}