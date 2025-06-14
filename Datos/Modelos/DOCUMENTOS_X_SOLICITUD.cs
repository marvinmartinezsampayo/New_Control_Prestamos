using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Table("DOCUMENTOS_X_SOLICITUD")]
    public class DOCUMENTOS_X_SOLICITUD
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Required]
        [Column("ID_SOLICITUD")]
        public long IdSolicitud { get; set; }

        [Required]
        [ForeignKey("FK_DOCUMENTO")]
        [Column("ID_DOCUMENTO")]
        public long IdDocumento { get; set; }

        [Required]
        [Column("CONTENIDO_DOC")]
        public string ContenidoDoc { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("FORMATO")]
        public string Formato { get; set; }

        [Required]
        [Column("TAMANIO")]
        public long Tamanio { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("USUARIO_CREACION")]
        public string UsuarioCreacion { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("MAQUINA_CREACION")]
        public string MaquinaCreacion { get; set; }

        [Column("FECHA_CREACION")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [MaxLength(100)]
        [Column("USUARIO_MODIFICACION")]
        public string? UsuarioModificacion { get; set; }

        [MaxLength(100)]
        [Column("MAQUINA_MODIFICACION")]
        public string? MaquinaModificacion { get; set; }

        [Column("FECHA_MODIFICACION")]
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        [Required]
        [Column("HABILITADO")]
        public bool Habilitado { get; set; }

        // Propiedades de navegación (opcional)
     
        public virtual DOCUMENTOS? FK_DOCUMENTO { get; set; }


        //[Required]
        //[ForeignKey("FK_DOCUMENTO")]
        //[Column("ID_DOCUMENTO")]
        //public long DocumentoId { get; set; }
        //public virtual DOCUMENTOS FK_DOCUMENTO { get; set; }
                
    }
}
