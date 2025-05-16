using System.Collections.ObjectModel; 
using KgalazaJson2.Models;
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

            if (list.data != null)
            {
                response = new ObservableCollection<Booking>(list.data);
                listView.ItemsSource = response;
            } 

        }



    }

}
