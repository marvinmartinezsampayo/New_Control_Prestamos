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
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]  // No es autoincremental, ya que no es AUTO_INCREMENT en la tabla
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
        [ForeignKey("FK_TIPO_IDENTIFICACION")]
        [Column("TIPO_IDENTIFICACION")]
        public long TipoIdentificacionId { get; set; }
        public virtual DETALLE_MASTER FK_TIPO_IDENTIFICACION { get; set; }

        [Required]
        [Column("NUMERO_IDENTIFICACION")]
        public long NumeroIdentificacion { get; set; }

       
        [Required]
        [ForeignKey("FK_DEPTO")]
        [Column("ID_DEPTO_RESIDENCIA")]
        public long DepartamentoResidenciaId { get; set; }
        public virtual LUGARES_GEOGRAFICOS FK_DEPTO { get; set; }

        
        [Required]
        [ForeignKey("FK_MPIO")]
        [Column("ID_MPIO_RESIDENCIA")]
        public long MunicipioResidenciaId { get; set; }
        public virtual LUGARES_GEOGRAFICOS FK_MPIO { get; set; }

        
        [Required]
        [ForeignKey("FK_BARRIO")]
        [Column("ID_BARRIO_RESIDENCIA")]
        public long BarrioResidenciaId { get; set; }
        public virtual BARRIOS FK_BARRIO { get; set; }

        [Required] 
        [StringLength(400)]
        [Column("DIRECCION_RESIDENCIA")]
        public string DireccionResidencia { get; set; }

        
        [Required]
        [ForeignKey("FK_GENERO")]
        [Column("ID_GENERO")]
        public long GeneroId { get; set; }
        public virtual DETALLE_MASTER FK_GENERO { get; set; }

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
        public string Codigo_Acceso { get; set; }


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
        public DateTime FechaCreacion { get; set; }
    }
}
