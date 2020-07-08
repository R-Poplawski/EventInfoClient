using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace EventInfoClient
{
    public class EventInfoAPI
    {
        public static HttpClient ApiClient { get; set; }

        public static void InitializeClient()
        {
            var handler = new WebRequestHandler();
            handler.ClientCertificates.Add(new X509Certificate2(Resources.ServerCertificate));
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ApiClient = new HttpClient(handler);
            ApiClient.BaseAddress = new Uri("https://localhost:8181/EventInfoServiceRest/webresources/EventInfo/");
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var authBytes = Encoding.ASCII.GetBytes("user1:password1");
            string authString = Convert.ToBase64String(authBytes);
            ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
        }

        public static async Task<List<EventInfo>> GetEventsForWeek(int year, int weekNumber)
        {
            using (HttpResponseMessage response = await ApiClient.GetAsync(ApiClient.BaseAddress + $"week/{year}/{weekNumber}"))
            {
                if (!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase);
                List<EventInfo> events = await response.Content.ReadAsAsync<List<EventInfo>>();
                return events;
            }
        }

        public static async Task<List<EventInfo>> GetEventsForDate(int year, int month, int day)
        {
            using (HttpResponseMessage response = await ApiClient.GetAsync($"date/{year}/{month}/{day}"))
            {
                if (!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase);
                List<EventInfo> events = await response.Content.ReadAsAsync<List<EventInfo>>();
                return events;
            }
        }

        public static async Task<EventInfo> GetEvent(long id)
        {
            using (HttpResponseMessage response = await ApiClient.GetAsync($"{id}"))
            {
                if (!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase);
                EventInfo events = await response.Content.ReadAsAsync<EventInfo>();
                return events;
            }
        }

        public static async Task<EventInfo> AddEvent(string name, string description, EventType type, DateTime date) {
            EventInfo eventInfo = new EventInfo(name, description, type, date);
            HttpContent content = new StringContent(eventInfo.ToString(), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await ApiClient.PostAsync("", content))
            {
                if (!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase);
                EventInfo events = await response.Content.ReadAsAsync<EventInfo>();
                return events;
            }
        }

        public static async Task<EventInfo> EditEvent(long id, string name, string description, EventType type, DateTime date)
        {
            EventInfo eventInfo = new EventInfo(name, description, type, date);
            HttpContent content = new StringContent(eventInfo.ToString(), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await ApiClient.PutAsync($"{id}", content))
            {
                if (!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase);
                EventInfo events = await response.Content.ReadAsAsync<EventInfo>();
                return events;
            }
        }

        public static async Task<byte[]> GetPdfForWeek(int year, int weekNumber)
        {
            byte[] response = await ApiClient.GetByteArrayAsync(ApiClient.BaseAddress + $"pdfForWeek/{year}/{weekNumber}");
            return response;
        }

        public static async Task<byte[]> GetPdfForDate(int year, int month, int day)
        {
            byte[] response = await ApiClient.GetByteArrayAsync(ApiClient.BaseAddress + $"pdfForDate/{year}/{month}/{day}");
            return response;
        }
    }
}
