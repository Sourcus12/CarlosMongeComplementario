namespace CarlosMongeComplementario.Views;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;
using Services;
using CarlosMongeComplementario.Models;
public partial class ESTUDIANTE : ContentPage
{
    private ObservableCollection<Estudiante> estudiantes;
    private Estudiante estudianteSeleccionado;
    public ESTUDIANTE()
    {
        InitializeComponent();
        estudiantes = new ObservableCollection<Estudiante>();
        EstudiantesListView.ItemsSource = estudiantes;

        CargarEstudiantes();
    }

    private void CargarEstudiantes()
    {
        estudiantes.Clear();

        using var connection = new SqliteConnection($"Data Source={DatabaseService.GetDbPath()}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM ESTUDIANTES";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            estudiantes.Add(new Estudiante
            {
                COD_ESTUDIANTE = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Apellido = reader.GetString(2),
                Curso = reader.GetString(3),
                Paralelo = reader.GetString(4),
                NOTA_FINAL = reader.GetFloat(5)
            });
        }
    }

    private void OnGuardarClicked(object sender, EventArgs e)
    {
        // Validar que los campos no estén vacíos
        if (string.IsNullOrEmpty(NombreEntry.Text) || string.IsNullOrEmpty(ApellidoEntry.Text) ||
            string.IsNullOrEmpty(CursoEntry.Text) || string.IsNullOrEmpty(ParaleloEntry.Text) ||
            string.IsNullOrEmpty(NotaFinalEntry.Text))
        {
            DisplayAlert("Error", "Todos los campos deben ser completados", "OK");
            return;
        }

        // Convertir la nota final a tipo float
        float nuevaNotaFinal;
        if (!float.TryParse(NotaFinalEntry.Text, out nuevaNotaFinal))
        {
            DisplayAlert("Error", "La nota final debe ser un número válido", "OK");
            return;
        }

        // Verificar si hay un estudiante seleccionado para edición
        if (estudianteSeleccionado != null)
        {
            // Si hay un estudiante seleccionado, lo actualizamos
            using var connection = new SqliteConnection($"Data Source={DatabaseService.GetDbPath()}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            UPDATE ESTUDIANTES
            SET Nombre = @nombre,
                Apellido = @apellido,
                Curso = @curso,
                Paralelo = @paralelo,
                NOTA_FINAL = @notaFinal
            WHERE COD_ESTUDIANTE = @codEstudiante;
        ";
            command.Parameters.AddWithValue("@nombre", NombreEntry.Text);
            command.Parameters.AddWithValue("@apellido", ApellidoEntry.Text);
            command.Parameters.AddWithValue("@curso", CursoEntry.Text);
            command.Parameters.AddWithValue("@paralelo", ParaleloEntry.Text);
            command.Parameters.AddWithValue("@notaFinal", nuevaNotaFinal);
            command.Parameters.AddWithValue("@codEstudiante", estudianteSeleccionado.COD_ESTUDIANTE);

            var rowsAffected = command.ExecuteNonQuery(); // Ejecuta el UPDATE

            if (rowsAffected > 0)
            {
                // Recargar la lista de estudiantes para reflejar los cambios
                CargarEstudiantes();

                // Limpiar los Entry después de guardar
                LimpiarCampos();

                // Limpiar el estudiante seleccionado para permitir agregar uno nuevo
                estudianteSeleccionado = null;

                // Mostrar mensaje de éxito
                DisplayAlert("Éxito", "Estudiante actualizado correctamente", "OK");
            }
            else
            {
                // Si no se actualiza ninguna fila, mostrar error
                DisplayAlert("Error", "No se pudo actualizar el estudiante", "OK");
            }
        }
        else
        {
            // Si no hay estudiante seleccionado, insertamos un nuevo estudiante
            using var connection = new SqliteConnection($"Data Source={DatabaseService.GetDbPath()}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO ESTUDIANTES (Nombre, Apellido, Curso, Paralelo, NOTA_FINAL)
            VALUES (@nombre, @apellido, @curso, @paralelo, @notaFinal);
        ";
            command.Parameters.AddWithValue("@nombre", NombreEntry.Text);
            command.Parameters.AddWithValue("@apellido", ApellidoEntry.Text);
            command.Parameters.AddWithValue("@curso", CursoEntry.Text);
            command.Parameters.AddWithValue("@paralelo", ParaleloEntry.Text);
            command.Parameters.AddWithValue("@notaFinal", nuevaNotaFinal);

            var rowsAffected = command.ExecuteNonQuery(); // Ejecuta el INSERT

            if (rowsAffected > 0)
            {
                // Recargar la lista de estudiantes para reflejar el nuevo registro
                CargarEstudiantes();

                // Limpiar los Entry después de guardar
                LimpiarCampos();

                // Mostrar mensaje de éxito
                DisplayAlert("Éxito", "Estudiante agregado correctamente", "OK");
            }
            else
            {
                // Si no se inserta ninguna fila, mostrar error
                DisplayAlert("Error", "No se pudo agregar el estudiante", "OK");
            }
        }
    }
    private void LimpiarCampos()
    {
        NombreEntry.Text = string.Empty;
        ApellidoEntry.Text = string.Empty;
        CursoEntry.Text = string.Empty;
        ParaleloEntry.Text = string.Empty;
        NotaFinalEntry.Text = string.Empty;
    }

    private void OnEstudianteSelected(object sender, SelectionChangedEventArgs e)
    {
        if (EstudiantesListView.SelectedItem is Estudiante estudiante)
        {
            // Cargar los datos del estudiante seleccionado en los Entry
            estudianteSeleccionado = estudiante;
            NombreEntry.Text = estudiante.Nombre;
            ApellidoEntry.Text = estudiante.Apellido;
            CursoEntry.Text = estudiante.Curso;
            ParaleloEntry.Text = estudiante.Paralelo;
            NotaFinalEntry.Text = estudiante.NOTA_FINAL.ToString();
        }
    }

    private void OnEliminarClicked(object sender, EventArgs e)
    {
        if (EstudiantesListView.SelectedItem is Estudiante estudiante)
        {
            using var connection = new SqliteConnection($"Data Source={DatabaseService.GetDbPath()}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM ESTUDIANTES WHERE COD_ESTUDIANTE = @id";
            command.Parameters.AddWithValue("@id", estudiante.COD_ESTUDIANTE);

            command.ExecuteNonQuery();
            CargarEstudiantes();
        }
    }
}

