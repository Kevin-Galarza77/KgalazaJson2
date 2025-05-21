using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using KgalazaJson2.Models;
using Newtonsoft.Json;

namespace KgalazaJson2.Views;

public partial class FrmBooking : ContentPage
{
    private TaskCompletionSource<bool> _taskCompletionSource;

    private string url = "http://localhost:9090/api/";
    private HttpClient httpClient = new HttpClient();
    private ObservableCollection<Booking> response;

    private int? bookingId = null;

    public FrmBooking(Booking? booking)
	{
		InitializeComponent();
        _taskCompletionSource?.SetResult(false);
        if (booking != null)
        {
            this.Title = "Modificar Reserva";
            this.bookingId = booking.bookingId;
            entryName.Text = booking.name;
            entryLastName.Text = booking.lastName;
            editorDescription.Text = booking.description;
            entryEmail.Text = booking.email;
            estadoPicker.SelectedItem = booking.statusName;
            estadoPicker.IsVisible = true;
        }
    }

    public Task<bool> ShowAsync()
    {
        _taskCompletionSource = new TaskCompletionSource<bool>();
        return _taskCompletionSource.Task;
    }

    private void OnSubmitClicked(object sender, EventArgs e)
    {
        if (bookingId == null)
        {
            createBooking();
        }
        else
        {
            updateBooking();
        }
    }

    private async void createBooking()
    {
        Booking booking = new Booking();

        booking.bookingId = null;
        booking.name = entryName.Text;
        booking.lastName = entryLastName.Text;
        booking.description = editorDescription.Text;
        booking.email = entryEmail.Text;
        booking.status = true;
        booking.deleted = false;

        var errors = ValidateBooking(booking);

        lblErrors.Text = string.Join("\n", errors);

        if (!errors.Any())
        {

            var json = JsonConvert.SerializeObject(booking);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url + "booking", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<Booking>>(jsonResponse);

            if (result != null)
            {
                if(result.status)
                {
                    _taskCompletionSource?.SetResult(true);
                    await DisplayAlert("Éxito", result.alert, "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    lblErrors.Text = string.Join("\n", result.messages);
                    await DisplayAlert("Advertencia", result.alert, "OK");
                }
            }
            else
            {
                await DisplayAlert("Advertencia", "Se produjo un error, intenta nuevamente", "OK");
            }
        }
    }

    private async void updateBooking()
    {
        Booking booking = new Booking();

        booking.bookingId = bookingId;
        booking.name = entryName.Text;
        booking.lastName = entryLastName.Text;
        booking.description = editorDescription.Text;
        booking.email = entryEmail.Text;
        booking.status = estadoPicker.SelectedItem == "ACTIVO";
        booking.deleted = false;

        var errors = ValidateBooking(booking);

        lblErrors.Text = string.Join("\n", errors);

        if (!errors.Any())
        {

            var json = JsonConvert.SerializeObject(booking);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync(url + "booking/" + booking.bookingId, content);

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<ApiResponse<Booking>>(jsonResponse);

            if (result != null)
            {
                if (result.status)
                {
                    _taskCompletionSource?.SetResult(true);
                    await DisplayAlert("Éxito", result.alert, "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    lblErrors.Text = string.Join("\n", result.messages);
                    await DisplayAlert("Advertencia", result.alert, "OK");
                }
            }
            else
            {
                await DisplayAlert("Advertencia", "Se produjo un error, intenta nuevamente", "OK");
            }
        }
    }

    private List<string> ValidateBooking(Booking booking)
    {
        var list = new List<string>();

        if (string.IsNullOrWhiteSpace(booking.name))
            list.Add("El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(booking.lastName))
            list.Add("El apellido es obligatorio.");

        if (string.IsNullOrWhiteSpace(booking.email))
            list.Add("El email es obligatorio.");
        else if (!Regex.IsMatch(booking.email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            list.Add("El email no tiene un formato válido.");

        return list;
    }

}