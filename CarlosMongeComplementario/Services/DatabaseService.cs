using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;
namespace CarlosMongeComplementario.Services
{
    public static class DatabaseService
    {
        private static string DbPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DBUISRAEL.db");

        public static void InitializeDatabase()
        {
            if (!File.Exists(DbPath))
            {
                using var connection = new SqliteConnection($"Data Source={DbPath}");
                connection.Open();

                // Crear tablas
                var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS ESTUDIANTES_LOGIN (
                        ID INTEGER PRIMARY KEY,
                        USUARIO TEXT NOT NULL,
                        CONTRASEÑA TEXT NOT NULL
                    );
                    CREATE TABLE IF NOT EXISTS ESTUDIANTES (
                        COD_ESTUDIANTE INTEGER PRIMARY KEY,
                        Nombre TEXT NOT NULL,
                        Apellido TEXT NOT NULL,
                        Curso TEXT NOT NULL,
                        Paralelo TEXT NOT NULL,
                        NOTA_FINAL REAL
                    );
                ";
                command.ExecuteNonQuery();

                // Insertar datos de prueba
                command.CommandText = @"
                    INSERT INTO ESTUDIANTES_LOGIN (ID, USUARIO, CONTRASEÑA) VALUES (1, 'admin', '12345');
                    INSERT INTO ESTUDIANTES (COD_ESTUDIANTE, Nombre, Apellido, Curso, Paralelo, NOTA_FINAL) 
                    VALUES (1, 'Carlos', 'Monge', 'IA', 'A', 9.5);
                ";
                command.ExecuteNonQuery();
            }
        }

        public static string GetDbPath() => DbPath;
    }
}
