using System.Collections.ObjectModel; 
using KgalazaJson2.Models;
using KgalazaJson2.Views;
using Newtonsoft.Json;

namespace KgalazaJson2
{
    public partial class MainPage : ContentPage
    {
        private string url = "http://localhost:9090/api/";
        private HttpClient httpClient = new HttpClient();
        private ObservableCollection<Booking> response;

        public MainPage()
        {
            InitializeComponent();
            getBokings();
        }

        public async void getBokings()
        {
            var content = await httpClient.GetStringAsync(url + "booking/all");
            var list = JsonConvert.DeserializeObject<ApiResponse<List<Booking>>>(content);

            if (list != null)
            {
                if (list.status)
                {
                    response = new ObservableCollection<Booking>(list.data);
                    listView.ItemsSource = response;
                }
                else
                {
                    listView.ItemsSource = new ObservableCollection<Booking>([]);
                }
            } 

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var bookingPage = new FrmBooking(null);

            await Navigation.PushAsync(bookingPage);
             
            bool creado = await bookingPage.ShowAsync();

            if (creado)
            {
                getBokings();  
            }
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button?.BindingContext as Booking;

            if (item != null)
            {
                var bookingPage = new FrmBooking(item);

                await Navigation.PushAsync(bookingPage);

                bool actualizado = await bookingPage.ShowAsync();

                if (actualizado)
                {
                    getBokings();
                }
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button?.BindingContext as Booking;

            if (item != null)
            {
                bool answer = await DisplayAlert("Confirmar eliminación",
                                                 $"¿Seguro que quieres eliminar a {item.name}?",
                                                 "Sí", "No");

                if (answer)
                {
                    var httpResponse = await httpClient.DeleteAsync(url + "booking/" + item.bookingId);
                    var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

                    var response = JsonConvert.DeserializeObject<ApiResponse<List<Booking>>>(jsonResponse);

                    if (response != null)
                    {
                        if (response.status)
                        {
                            await DisplayAlert("Éxito", response.alert, "OK");
                            getBokings();
                        }
                        else
                        {
                            await DisplayAlert("Advertencia", response.alert, "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Advertencia", "Se produjo un error, intenta nuevamente", "OK");
                    }
                }
            }
        }


    }

}
