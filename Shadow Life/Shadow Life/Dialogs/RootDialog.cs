using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Zork.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (String.IsNullOrEmpty(activity.Text))
            {
                var client = new HttpClient();
                var jsonHeader = new { conversation_id = activity.Conversation.Id, command = "" };
                var content = new StringContent(JsonConvert.SerializeObject(jsonHeader).ToString(), Encoding.UTF8, "application/json");
                var cortanaReply = await client.PostAsync("http://zork.southcentralus.cloudapp.azure.com/", content);
                var cortanaReplyContent = await cortanaReply.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(cortanaReplyContent);
                var reply = jsonResponse["response"].ToString();
                Activity message = activity.CreateReply(reply);
                message.Speak = reply;
                message.InputHint = InputHints.ExpectingInput;
                await context.PostAsync(message);
            }
            else
            {
                // Create LUIS HTTP request
                var client = new HttpClient();
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                var luisAppId = "67256233-3685-4fea-b72a-d216c18f45b4";
                var subscriptionKey = "031e6edfccf34b56948f881bb6deecd8";
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                // The "q" parameter contains the utterance to send to LUIS
                queryString["q"] = activity.Text;

                // These optional request parameters are set to their default values
                queryString["timezoneOffset"] = "0";
                queryString["verbose"] = "false";
                queryString["spellCheck"] = "false";
                queryString["staging"] = "false";

                // Get LUIS response
                var uri = "https://westcentralus.api.cognitive.microsoft.com/luis/v2.0/apps/" + luisAppId + "?" + queryString;
                var response = await client.GetAsync(uri);

                var responseContent = await response.Content.ReadAsStringAsync();

                // Get intent
                JObject jsonResponse = JObject.Parse(responseContent);
                var intent = jsonResponse["topScoringIntent"]["intent"];

                // Get entities
                var entities = jsonResponse["entities"];
                Dictionary<string, string> entityDict = new Dictionary<string, string>();
                foreach (var entity in entities)
                {
                    entityDict.Add(entity["type"].ToString(), entity["entity"].ToString());
                }

                // Format response
                string input = "";
                switch (intent.ToString())
                {
                    case "Attack":
                        if (entityDict.ContainsKey("creature") && entityDict.ContainsKey("item"))
                        {
                            input = $"attack {entityDict["creature"]} with {entityDict["item"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                    case "Break":
                        if (entityDict.ContainsKey("item") && entityDict.ContainsKey("object"))
                        {
                            input = $"break {entityDict["item"]} with {entityDict["object"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                    case "Climb": input = "climb";
                        break;
                    case "Close": input = "close";
                        break;
                    case "Control":
                        if (entityDict.ContainsKey("control") && entityDict.ContainsKey("item"))
                        {
                            input = $"turn {entityDict["control"]} with {entityDict["item"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                    case "Diagnostic": input = "diagnostic";
                        break;
                    case "Drink": input = "drink";
                        break;
                    case "Drop":
                        if (entityDict.ContainsKey("item"))
                        {
                            input = $"drop {entityDict["item"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                    case "Eat": input = "eat";
                        break;
                    case "Enter": input = "enter";
                        break;
                    case "Examine":
                        if (entityDict.ContainsKey("item"))
                        {
                            input = $"examine {entityDict["item"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                    case "Exit": input = "out";
                        break;
                    case "GetAll": input = "get all";
                        break;
                    case "GetItem":
                        if (entityDict.ContainsKey("item"))
                        {
                            input = $"get {entityDict["item"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                    case "Hello": input = "hello";
                        break;
                    case "Inventory": input = "inventory";
                        break;
                    case "Jump": input = "jump";
                        break;
                    case "Kill":
                        if (entityDict.ContainsKey("creature") && entityDict.ContainsKey("item"))
                        {
                            input = $"kill {entityDict["creature"]} with {entityDict["item"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                    case "Listen": input = "listen";
                        break;
                    case "Look": input = "look";
                        break;
                    case "MoveDown": input = "down";
                        break;
                    case "MoveEast": input = "east";
                        break;
                    case "MoveItem":
                        if (entityDict.ContainsKey("item"))
                        {
                            input = $"move {entityDict["item"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                    case "MoveNorth": input = "north";
                        break;
                    case "MoveNorthEast": input = "northeast";
                        break;
                    case "MoveNorthWest": input = "northwest";
                        break;
                    case "MoveSouth": input = "south";
                        break;
                    case "MoveSouthEast": input = "southeast";
                        break;
                    case "MoveSouthWest": input = "southwest";
                        break;
                    case "MoveUp": input = "up";
                        break;
                    case "MoveWest": input = "west";
                        break;
                    case "Open":
                        if (entityDict.ContainsKey("container"))
                        {
                            input = $"open {entityDict["container"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                    case "Pray": input = "pray";
                        break;
                    case "Profanity": input = "fuck";
                        break;
                    case "Put":
                        if (entityDict.ContainsKey("item") && entityDict.ContainsKey("container"))
                        {
                            input = $"put {entityDict["item"]} in {entityDict["container"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                    case "Quit": input = "quit";
                        break;
                    case "Read":
                        if (entityDict.ContainsKey("item"))
                        {
                            input = $"read {entityDict["item"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                    case "Restart": input = "restart";
                        break;
                    case "Score": input = "score";
                        break;
                    case "Shout": input = "shout";
                        break;
                    case "Smell": input = "smell";
                        break;
                    case "Tie":
                        if (entityDict.ContainsKey("item") && entityDict.ContainsKey("object"))
                        {
                            input = $"tie {entityDict["item"]} to {entityDict["object"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                    case "TurnOff":
                        if (entityDict.ContainsKey("item"))
                        {
                            input = $"turn off {entityDict["item"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                    case "TurnOn":
                        if (entityDict.ContainsKey("item"))
                        {
                            input = $"turn on {entityDict["item"]}";
                        }
                        else
                        {
                            input = activity.Text;
                        }
                        break;
                }

                var jsonHeader = new { conversation_id = activity.Conversation.Id, command = input };
                var content = new StringContent(JsonConvert.SerializeObject(jsonHeader).ToString(), Encoding.UTF8, "application/json");
                var cortanaReply = await client.PostAsync("https://zork.site/", content);
                var cortanaReplyContent = await cortanaReply.Content.ReadAsStringAsync();

                // Prepare reply
                jsonResponse = JObject.Parse(cortanaReplyContent);
                var reply = jsonResponse["response"].ToString();
                Activity message = activity.CreateReply(reply);
                message.Speak = reply;
                message.InputHint = InputHints.ExpectingInput;

                // return our reply to the user
                await context.PostAsync(message);
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}