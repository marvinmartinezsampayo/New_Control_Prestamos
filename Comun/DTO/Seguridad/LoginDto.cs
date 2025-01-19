using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.DTO.Seguridad
{
    public class LoginDto
    {
        private string _username = string.Empty;

        [Required, Display(Name = "Usuario")]
        [StringLength(30, ErrorMessage = "El {0} debe tener entre 5 y 20 caracteres.", MinimumLength = 5)]
        [JsonPropertyName("usuario")]
        public string Usuario { get => _username; set { _username = value.Trim().ToLower(); } }


        [Required, Display(Name = "Contraseña")]
        [StringLength(50, ErrorMessage = "La {0} debe tener entre 8 y 50 caracteres.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [JsonPropertyName("contrasena")]
        public string Clave { get; set; } = string.Empty;
    }
}
