using Microsoft.Data.Sqlite;
using Microsoft.Maui.Controls;
using System;
using CarlosMongeComplementario.Services;

namespace CarlosMongeComplementario.Views;

public partial class LOGIN : ContentPage
{
    private string _photoPath;
   
    public LOGIN()
	{
		InitializeComponent();
	}
   
    private async void CapturePhotoButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                _photoPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using (var stream = await photo.OpenReadAsync())
                using (var newStream = File.OpenWrite(_photoPath))
                    await stream.CopyToAsync(newStream);

                EvidenceImage.Source = ImageSource.FromFile(_photoPath);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo capturar la foto: {ex.Message}", "OK");
        }
    }

    private async void btnIniciar_Clicked(object sender, EventArgs e)
    {
        string username = txtUusuario.Text;
        string password = txtContrasena.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Por favor ingresa usuario y contraseña.", "OK");
            return;
        }

        // Validar usuario y contraseña desde la base de datos
        using var connection = new SqliteConnection($"Data Source={DatabaseService.GetDbPath()}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
                SELECT COUNT(*) 
                FROM ESTUDIANTES_LOGIN 
                WHERE USUARIO = @username AND CONTRASEÑA = @password;
            ";
        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@password", password);

        var result = Convert.ToInt32(command.ExecuteScalar());

        if (result > 0)
        {
            // Redirigir a la ventana principal
            await Navigation.PushAsync(new ESTUDIANTE());
        }
        else
        {
            await DisplayAlert("Error", "Usuario o contraseña incorrectos.", "OK");
        }
    }
}