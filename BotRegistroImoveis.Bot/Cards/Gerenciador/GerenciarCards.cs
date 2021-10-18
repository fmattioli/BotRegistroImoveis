using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BotRegistroImoveis.Bot.Cards.Gerenciador
{
    public class GerenciarCards
    {
        public Attachment CriarAttachmentFromFile(string adaptiveName)
        {
            var cardResourcePath = GetType().Assembly.GetManifestResourceNames().First(name => name.EndsWith(adaptiveName + ".json"));
            using (var stream = GetType().Assembly.GetManifestResourceStream(cardResourcePath))
            {
                using (var reader = new StreamReader(stream))
                {
                    var adaptiveCard = reader.ReadToEnd();
                    return new Attachment()
                    {
                        ContentType = "application/vnd.microsoft.card.adaptive",
                        Content = JsonConvert.DeserializeObject(adaptiveCard),
                    };
                }
            }
        }

        public string RetornarConteudoJson(string adaptiveName)
        {
            var cardResourcePath = GetType().Assembly.GetManifestResourceNames().First(name => name.EndsWith(adaptiveName + ".json"));
            using (var stream = GetType().Assembly.GetManifestResourceStream(cardResourcePath))
            {
                using (var reader = new StreamReader(stream))
                {
                    var adaptiveCard = reader.ReadToEnd();
                    return adaptiveCard;
                }
            }
        }

        public PromptOptions RetornarAdaptiveCard(IList<string> adaptives)
        {
            var lista = new List<Attachment>();
            foreach (var item in adaptives)
            {
                lista.Add(CriarAttachmentFromFile(item));
            }

            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    AttachmentLayout = AttachmentLayoutTypes.Carousel,
                    Attachments = lista,
                    Type = ActivityTypes.Message,
                },
            };



            return opts;
        }


        public Attachment CriarAttachmentFromJson(string json)
        {
            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(json),
            };
        }

        public PromptOptions CriarAdaptiveCardBindingSemMesclagem(string json)
        {
            var lista = new List<Attachment>();
            lista.Add(CriarAttachmentFromJson(json));
            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    AttachmentLayout = AttachmentLayoutTypes.Carousel,
                    Attachments = lista,
                    Type = ActivityTypes.Message,
                },
            };

            return opts;
        }

        public PromptOptions CriarListaAdaptiveCardBindingMesclarDados(IList<string> jsons)
        {
            var lista = new List<Attachment>();
            foreach (var json in jsons)
                lista.Add(CriarAttachmentFromJson(json));
            
            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    AttachmentLayout = AttachmentLayoutTypes.Carousel,
                    Attachments = lista,
                    Type = ActivityTypes.Message,
                }
            };

            return opts;
        }



    }
}
