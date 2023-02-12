using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenAI
{
    public class Client
    {
        private readonly string _apiKey;
        private readonly string _endpoint;

        public Client(string apiKey, string endpoint = "https://api.openai.com/v1/")
        {
            _apiKey = apiKey;
            _endpoint = endpoint;
        }

        public async Task<string> GenerateText(string prompt, int length, bool nopunct, bool stop, int temperature, string model)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var payload = new
                {
                    prompt = prompt,
                    max_tokens = length,
                    n = 1,
                    stop = stop ? "stop" : null,
                    temperature = temperature,
                    model = model
                };

                var response = await client.PostAsync(_endpoint + $"engines/{model}/jobs", new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();

                var responseText = await response.Content.ReadAsStringAsync();
                dynamic responseJson = JsonConvert.DeserializeObject(responseText);
                var generatedText = responseJson.choices[0].text;

                if (nopunct)
                {
                    generatedText = generatedText.Replace(".", "").Replace(",", "").Replace("!", "").Replace("?", "");
                }

                return generatedText;
            }
        }

        public async Task<string> SummarizeText(string text, int maxtokens)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var payload = new
                {
                    prompt = "Summarize Text: " + text,
                    max_tokens = maxtokens,
                    n = 1,
                };

                var response = await client.PostAsync(_endpoint + "engines/text-curie-003/jobs", new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to summarize text: " + response.ReasonPhrase);
                }

                var responseText = await response.Content.ReadAsStringAsync();
                dynamic responseJson = JsonConvert.DeserializeObject(responseText);
                var summarizedText = responseJson.choices[0].text;

                return summarizedText;
            }
        }


        private async Task<string> CompleteCode(string text, int length, string language)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var payload = new
                {
                    prompt = "Complete this code wether you know what it's about or not, give it your best go: " + text,
                    max_tokens = length,
                    n = 1,
                    stop = "",
                    temperature = 0.5
                };

                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_endpoint + "engines/text-davinci-003/jobs"),
                    Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                };

                var response = await client.SendAsync(requestMessage);

                if (!response.IsSuccessStatusCode)
                {
                    var responseTexts = await response.Content.ReadAsStringAsync();
                    throw new Exception("Failed to complete code: " + responseTexts);
                }

                var responseText = await response.Content.ReadAsStringAsync();
                dynamic responseJson = JsonConvert.DeserializeObject(responseText);
                var completedCode = responseJson.choices[0].text;

                return completedCode;
            }
        }


        public async Task<string> AnalyzeSentiment(string text)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                    var payload = new
                    {
                        prompt = "Anazlyze Sentiments: " + text,
                    };

                    var response = await client.PostAsync(_endpoint + "engines/text-davinci-003/jobs", new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json"));

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Failed to analyze sentiment: " + response.ReasonPhrase);
                    }

                    var responseText = await response.Content.ReadAsStringAsync();
                    dynamic responseJson = JsonConvert.DeserializeObject(responseText);
                    var sentiment = responseJson.sentiment;

                    return sentiment;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error analyzing sentiment: " + ex.Message);
                    return "Error";
                }
            }
        }


        public async Task<NamedEntity[]> NamedEntityRecognition(string text)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var payload = new
                {
                    text = text,
                };

                var response = await client.PostAsync(_endpoint + "engines/text-davinci-003/jobs", new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to recognize named entities: {response.ReasonPhrase} (HTTP {(int)response.StatusCode})");
                }

                var responseText = await response.Content.ReadAsStringAsync();
                dynamic responseJson = JsonConvert.DeserializeObject(responseText);
                var entities = responseJson.entities;

                var namedEntities = new NamedEntity[entities.Count];
                for (int i = 0; i < entities.Count; i++)
                {
                    namedEntities[i] = new NamedEntity
                    {
                        Text = entities[i].text,
                        Type = entities[i].type,
                        Score = entities[i].score,
                        Start = entities[i].start,
                        End = entities[i].end,
                    };
                }

                return namedEntities;
            }
        }


        public async Task<TextClassification> ClassifyText(string text)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var data = new { text = text };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_endpoint + "engines/text-davinci-003/jobs", content);
            response.EnsureSuccessStatusCode();

            var responseText = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<dynamic>(responseText);
            var categories = responseJson.categories;

            var textClassification = new TextClassification
            {
                Text = text,
                Categories = new Category[categories.Count],
            };

            var i = 0;
            foreach (var item in categories)
            {
                textClassification.Categories[i++] = new Category
                {
                    Name = item.name,
                    Score = item.score,
                };
            }

            return textClassification;
        }



        public async Task<string> TranslateText(string text, string sourceLanguage, string targetLanguage)
        {
            var payload = new
            {
                text = $"Translate this text, from {sourceLanguage} to {targetLanguage}: " + text,
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var response = await client.PostAsync(_endpoint + "engines/text-davinci-003/jobs", CreateStringContent(payload));

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to translate text: " + response.ReasonPhrase);
                }

                var responseText = await response.Content.ReadAsStringAsync();
                var responseJson = DeserializeObject(responseText);
                var translatedText = ExtractTranslatedText(responseJson);

                return translatedText;
            }
        }

        public async Task<Dictionary<string, float>> RecognizeEmotion(string text)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var modifiedPrompt = $"Please try your best to give me a list of each emotion detected in this text, 1 emotion per line and the emotion must have a confidence score next to it: {text}";
                var request = new HttpRequestMessage(HttpMethod.Post, _endpoint + "engines/text-davinci-003/jobs");
                request.Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    prompt = modifiedPrompt
                }), System.Text.Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to recognize emotion: " + response.ReasonPhrase);
                }

                var result = await response.Content.ReadAsStringAsync();
                var emotions = JsonConvert.DeserializeObject<List<EmotionScore>>(result);
                return emotions.ToDictionary(x => x.Emotion, x => x.Score);
            }
        }

        private static HttpContent CreateStringContent(object content)
        {
            return new StringContent(JsonConvert.SerializeObject(content), System.Text.Encoding.UTF8, "application/json");
        } 

        private static dynamic DeserializeObject(string responseText)
        {
            return JsonConvert.DeserializeObject(responseText);
        }

        private static string ExtractTranslatedText(dynamic responseJson)
        {
            return responseJson.text;
        }

        public class TextClassification
        {
            public string Text { get; set; }
            public Category[] Categories { get; set; }
        }

        public class Category
        {
            public string Name { get; set; }
            public double Score { get; set; }
        }

        private class EmotionScore
        {
            public string Emotion { get; set; }
            public float Score { get; set; }
        }

        public class NamedEntity
        {
            public string Text { get; set; }
            public string Type { get; set; }
            public double Score { get; set; }
            public int Start { get; set; }
            public int End { get; set; }
        }
    }
}
