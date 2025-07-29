namespace Ponche_Adess.Data
{
    using global::Ponche_Adess.Domain.Data.Models;
    using Ponche_Adess.Data;

    namespace Ponche_Adess.Data
    {
        public static class DbInitializer
        {
            public static void Seed(AppDbContext context)
            {
                if (!context.Usuarios.Any())
                {
                    var usuario = new Usuario
                    {
                        EmpleadoId = 1, // asegúrate que ese Empleado exista
                        NombreUsuario = "admin",
                        ContrasenaHash = "1234", // cambia si usas hash
                        Estado = true
                    };

                    context.Usuarios.Add(usuario);
                    context.SaveChanges();
                }
            }
        }
    }

}
