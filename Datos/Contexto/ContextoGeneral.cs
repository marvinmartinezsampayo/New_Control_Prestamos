using Datos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Contexto
{
    public class ContextoGeneral : DbContext
    {
        public ContextoGeneral(DbContextOptions<ContextoGeneral> options) : base(options)
        {
        }

        public DbSet<DETALLE_MASTER> DETALLE_MASTER { get; set; }
        public DbSet<USUARIO> USUARIO { get; set; }
        public DbSet<ROLES_X_USUARIO> ROLES_X_USUARIO { get; set; }
        public DbSet<BARRIOS> BARRIOS { get; set; }
        public DbSet<CODIGO_ACCESO> CODIGO_ACCESO { get; set; }
        public DbSet<DOCUMENTOS> DOCUMENTOS { get; set; }
        public DbSet<DOCUMENTOS_X_SOLICITUD> DOCUMENTOS_X_SOLICITUD { get; set; }
        public DbSet<LUGARES_GEOGRAFICOS> LUGARES_GEOGRAFICOS { get; set; }
        public DbSet<SOLICITUD_PRESTAMO> SOLICITUD_PRESTAMO { get; set; }
        public DbSet<DOCUMENTOS_REQUERIDOS> DOCUMENTOS_REQUERIDOS { get; set; }
        public DbSet<AUDITORIA> AUDITORIA { get; set; }
    }
}
