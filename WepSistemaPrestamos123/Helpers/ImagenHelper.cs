using System;

namespace WepPrestamos.Helpers
{
    public static class ImagenHelper
    {
        public static string ObtenerFotoBase64(byte[] fotoBytes, string contentType = "image/png")
        {
            if (fotoBytes == null || fotoBytes.Length == 0)
                return null;

            var tiposPermitidos = new[] { "image/png", "image/jpeg", "image/jpg", "image/svg+xml" };

            if (!string.IsNullOrEmpty(contentType) && !Array.Exists(tiposPermitidos, t => t.Equals(contentType, StringComparison.OrdinalIgnoreCase)))
                contentType = "image/png";

            return $"data:{contentType};base64,{Convert.ToBase64String(fotoBytes)}";
        }

    }
}
