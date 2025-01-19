using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Table("CODIGO_ACCESO")]
    public class CODIGO_ACCESO
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // AutoIncrement
        [Column("ID")]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("CODIGO")]      
        public string Codigo { get; set; }

        [Required]
        [Column("FECHA_INICIO", TypeName = "DATE")]
        public DateTime FechaInicio { get; set; }

        [Required]
        [Column("FECHA_FIN", TypeName = "DATE")]
        public DateTime FechaFin { get; set; }

        [StringLength(200)]
        [Column("EMAIL_ASOCIADO")]
        public string EmailAsociado { get; set; }

        [StringLength(50)]
        [Column("CELULAR_ASOCIADO")]
        public string CelularAsociado { get; set; }

        [Required]
        [Column("CANTIDAD_REGISTROS")]
        public long CantidadRegistros { get; set; }

        [StringLength(300)]
        [Column("IMAGEN")]
        public string Imagen { get; set; }

        [Required]
        [Column("HABILITADO")]
        public bool Habilitado { get; set; }

        [Required] 
        [StringLength(100)]
        [Column("USUARIO_CREACION")]
        public string UsuarioCreacion { get; set; }

        [Required]
        [StringLength(100)]
        [Column("MAQUINA_CREACION")]
        public string MaquinaCreacion { get; set; }

        [Column("FECHA_CREACION")]
        public DateTime? FechaCreacion { get; set; }
    }
}
