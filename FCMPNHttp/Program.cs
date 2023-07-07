using Newtonsoft.Json;
using System.Text;

public class Program
{
    /*
     * server key : https://firebase.google.com/docs/cloud-messaging/auth-server (Authorize legacy protocol send requests)
     * device token : unique device-app pairing identifier, in case of Enmeshed it is read from the debug app area
     * message : specification expected by the Enmeshed app
     */
    public static async Task Main(string[] args)
    {
        // these variables are set as environment variables in zsh, in order to use them run VS from the terminal with
        // >> open /Applications/Visual\ Studio.app 
        var serverKey = Environment.GetEnvironmentVariable("SERVER_KEY");
        var deviceToken = Environment.GetEnvironmentVariable("DEVICE_TOKEN");
        var accRef = Environment.GetEnvironmentVariable("ACC_REF");

        // preparing the request body, method #1
        var messageInformation = new
        {
            to = deviceToken,
            data = new
            {
                android_channel_id = "ENMESHED",
                content_available = "1",
                content = new
                {
                    accRef = accRef,
                    eventName = "dynamic",
                    sentAt = "2021-01-01T00:00:00.000Z",
                    payload = new
                    {
                        someProperty = "someValue"
                    }
                }
            },
            notification = new
            {
                tag = "1",
                title = "someNotificationTextTitle",
                body = "someNotificationTextBody"
            }
        };

        string jsonMessage0 = JsonConvert.SerializeObject(messageInformation);

        // preparing the request body, method #2
        string jsonMessage1 = @"{
                ""to"": """ + deviceToken + @""",
                ""data"": {
                    ""android_channel_id"": ""ENMESHED"",
                    ""content_available"": ""1"",
                    ""content"": {
                        ""accRef"": """ + accRef + @""",
                        ""eventName"": ""dynamic"",
                        ""sentAt"": ""2021-01-01T00:00:00.000Z"",
                        ""payload"": {
                            ""someProperty"": ""someValue""
                        }
                    }
                },
                ""notification"": {
                    ""tag"": ""1"",
                    ""title"": ""someNotificationTextTitle"",
                    ""body"": ""someNotificationTextBody""
                }
        }";

        var requestUri = "https://fcm.googleapis.com/fcm/send";

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + serverKey);

            var httpContent = new StringContent(jsonMessage0, Encoding.UTF8, "application/json");

            HttpResponseMessage result;

            try
            {
                result = await httpClient.PostAsync(requestUri, httpContent);
                if (result.IsSuccessStatusCode)
                {
                    Console.WriteLine("Notification has been sent successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to send notification. Status code: " + result.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notification. Error: {ex.Message}");
            }
        }
    }
}
