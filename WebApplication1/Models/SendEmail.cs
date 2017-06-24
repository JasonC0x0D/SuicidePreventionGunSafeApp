using System.Net.Http;
using System.Text;

namespace WebApplication1.Models
{
    public class SendEmail
    {
        public void SendAlert(string toEmailAddress, int id)
        {

            // Start creating HttpRequest to SendGrid
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Host = ("api.sendgrid.com");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer INSERT_PRIVATE_KEY_HERE");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send");
            // Http Request Body

            request.Content = new StringContent("{ \"personalizations\": [{ \"to\": [{\"email\": \"" + toEmailAddress + " \"}]}],\"from\": {\"email\": \"team@cowincrafted.com\"},\"subject\": \"Alert! Tampering Detected\",\"content\": [{\"type\": \"text/plain\", \"value\": \"Tampering was detected on your device: " + id + "\"}]}",
                                                Encoding.UTF8,
                                                "application/json");//CONTENT-TYPE header
            // Sending Request
            var response = client.SendAsync(request).Result; //Don't actually do anything with the response, but need to send the request.
        }
        public void SendRequest(string toEmailAddress, string userName)
        {

            // Start creating HttpRequest to SendGrid
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Host = ("api.sendgrid.com");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer INSERT_PRIVATE_KEY_HERE");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send");
            // Http Request Body

            request.Content = new StringContent("{ \"personalizations\": [{ \"to\": [{\"email\": \"" + toEmailAddress + " \"}]}],\"from\": {\"email\": \"team@cowincrafted.com\"},\"subject\": \"Safe Unlock Request\",\"content\": [{\"type\": \"text/plain\", \"value\": \"User: " + userName + " requested to unlock the safe.\"}]}",
                                                Encoding.UTF8,
                                                "application/json");//CONTENT-TYPE header
            // Sending Request
            var response = client.SendAsync(request).Result; //Don't actually do anything with the response, but need to send the request.
        }
    }
}
