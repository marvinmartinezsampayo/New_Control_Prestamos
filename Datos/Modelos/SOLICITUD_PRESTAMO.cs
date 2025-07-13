using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Table("SOLICITUD_PRESTAMO")]
    public class SOLICITUD_PRESTAMO
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Required]
        [StringLength(20)]
        [Column("P_NOMBRE_SOLICITANTE")]
        public string PrimerNombreSolicitante { get; set; }

        [StringLength(20)]
        [Column("S_NOMBRE_SOLICITANTE")]
        public string SegundoNombreSolicitante { get; set; }

        [Required]
        [StringLength(20)]
        [Column("P_APELLIDO_SOLICITANTE")]
        public string PrimerApellidoSolicitante { get; set; }

        [StringLength(20)]
        [Column("S_APELLIDO_SOLICITANTE")]
        public string SegundoApellidoSolicitante { get; set; }

        [Required]
        [Column("TIPO_IDENTIFICACION")]
        public long TipoIdentificacionId { get; set; }

        [Required]
        [Column("NUMERO_IDENTIFICACION")]
        public long NumeroIdentificacion { get; set; }

        [Required]
        [Column("ID_DEPTO_RESIDENCIA")]
        public long DepartamentoResidenciaId { get; set; }

        [Required]
        [Column("ID_MPIO_RESIDENCIA")]
        public long MunicipioResidenciaId { get; set; }

        [Required]
        [Column("ID_BARRIO_RESIDENCIA")]
        public long BarrioResidenciaId { get; set; }

        [Required]
        [StringLength(400)]
        [Column("DIRECCION_RESIDENCIA")]
        public string DireccionResidencia { get; set; }

        [Required]
        [Column("ID_ESTADO")]
        public long EstadoId { get; set; }

        [Required]
        [Column("MONTO")]
        public long Monto { get; set; }


        [Required]
        [StringLength(50)]
        [Column("EMAIL")]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        [Column("CELULAR")]
        public string Celular { get; set; }

        [StringLength(100)]
        [Column("CODIGO_ACCESO")]
        public string CodigoAcceso { get; set; }

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

        [Required]
        [Column("FECHA_CREACION")]
        //[Column(TypeName = "date")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relación con CODIGO_ACCESO
        // La FK apunta al campo CODIGO de la tabla CODIGO_ACCESO, no al Id
        //public virtual CODIGO_ACCESO CodigoAccesoNavigation { get; set; }
    }
}
