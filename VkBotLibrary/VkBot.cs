using System.Net.Http.Headers;
using System.Text;
using BotLibrary;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model.Attachments;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;
using KeyboardBuilder = VkNet.Model.Keyboard.KeyboardBuilder;

namespace VkBotLibrary;

public class VkBot : IBot
{
    protected readonly VkBotFramework.VkBot VkBotImplementation;

    public event Action<IBot, IMessage>? OnMessageReceived;

    public event Action<IBot, IUpdate>? OnButtonClicked;

    public VkBot(string accessToken, string groupUrl)
    {
        VkBotImplementation = new VkBotFramework.VkBot(accessToken, groupUrl);

        VkBotImplementation.OnMessageReceived += (_, args) =>
        {
#warning Не понятно когда эти параметры могут иметь значение null
            if (args.Message is { PeerId: not null, FromId: not null })
            {
                if (args.Message.Payload == null)
                {
                    OnMessageReceived?.Invoke(this, new VkMessage(args.Message, VkBotImplementation.Api));
                }
                else
                {
                    OnButtonClicked?.Invoke(this, new VkMessage(args.Message, VkBotImplementation.Api));
                }

            }
        };
    }

    public void Send(long chatId, string text, Keyboard? keyboard = null, Image[]? images = null)
    {
        try
        {
            var attachments = new List<MediaAttachment>();
            if (images != null)
            {
                foreach (var image in images)
                {
                    var uploadServer = VkBotImplementation.Api.Photo.GetMessagesUploadServer(chatId);
                    var response = UploadFile(uploadServer.UploadUrl,
                        image.Data, image.Extension);
                    attachments.Add(
                        VkBotImplementation.Api.Photo.SaveMessagesPhoto(response)[0]
                    );
                }
            }

            VkBotImplementation.Api.Messages.Send(new MessagesSendParams()
            {
                PeerId = chatId,
                Message = text,
                RandomId = Environment.TickCount,
                Keyboard = keyboard == null ? null : ConvertToVkKeyboard(keyboard),
                Attachments = attachments
            });
        }
        catch (CannotSendToUserFirstlyException)
        {
            throw new BotException("Не получается отправить сообщение пользователю");
        }
    }

    public void DeleteKeyboard(long chatId, string text)
    {
        VkBotImplementation.Api.Messages.Send(new MessagesSendParams()
        {
            PeerId = chatId,
            Message = text,
            RandomId = Environment.TickCount,
            Keyboard = new KeyboardBuilder().Build()
        });
    }

    public string CreateMention(long userId, string name)
    {
        return $"[id{userId}|{name}]";
    }

    public void Start()
    {
        VkBotImplementation.Start();
    }

    private static MessageKeyboard ConvertToVkKeyboard(Keyboard keyboard)
    {
        var builder = new KeyboardBuilder(false);
        foreach (var buttonsLine in keyboard.Buttons)
        {
            foreach (var button in buttonsLine)
            {
                builder.AddButton(button.Text, button.Text, ConvertColor(button.Color));
            }
            builder.AddLine();
        }

        builder.SetInline(keyboard.IsInline);
        return builder.Build();
    }

    private static KeyboardButtonColor ConvertColor(BotLibrary.Color color)
    {
        switch (color)
        {
            case Color.Primary:
                return KeyboardButtonColor.Primary;
            case Color.Positive:
                return KeyboardButtonColor.Positive;
            case Color.Negative:
                return KeyboardButtonColor.Negative;
            case Color.Without:
                return KeyboardButtonColor.Default;
        }

        throw new Exception("Добавь новое преобразование");
    }

    static string UploadFile(string serverUrl, byte[] data, string fileExtension)
    {
        using var client = new HttpClient();
        var requestContent = new MultipartFormDataContent();
        var content = new ByteArrayContent(data);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        requestContent.Add(content, "file", $"file.{fileExtension}");
        var response = client.PostAsync(serverUrl, requestContent).Result;
        return Encoding.Default.GetString(response.Content.ReadAsByteArrayAsync().Result);
    }
}