using Refit;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using WbNotifier.Settings;
using WbNotifier.WbApi;

namespace WbNotifier;

public class Notifier
{
    private readonly IWbSuppliersApi _wbSuppliersApi;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ChatId _chatId;
    private readonly ILogger<Notifier> _logger;

    public Notifier(
        IWbSuppliersApi wbSuppliersApi,
        ITelegramBotClient telegramBotClient,
        TelegramBotSettings telegramBotSettings,
        ILogger<Notifier> logger)
    {
        _wbSuppliersApi = wbSuppliersApi;
        _telegramBotClient = telegramBotClient;
        _chatId = new ChatId(telegramBotSettings.ChatId);
        _logger = logger;
    }

    public async Task Notify(string barcode, string caption)
    {
        GetCardsResponse? getCardsResponse = null;
        try
        {
            getCardsResponse = await _wbSuppliersApi.GetCardsAsync(new GetCardsRequest
            {
                Sort = new CardsSort
                {
                    Cursor = new CardsCursor
                    {
                        Limit = 100
                    },
                    Filter = new CardsFilter
                    {
                        TextSearch = barcode,
                        WithPhoto = -1
                    }
                }
            });
        }
        catch (ApiException e)
        {
            _logger.LogError("Error occured while calling Wb suppliers API: returned status code {statusCode} {reason} with content {content}",
                e.StatusCode, e.ReasonPhrase, e.Content);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while calling Wb suppliers API");
        }

        if (getCardsResponse == null)
        {
             return;
        }

        if (!getCardsResponse.Data.Cards.Any())
        {
            _logger.LogError("No cards found for barcode {barcode}", barcode);
            return;
        }
        
        if (getCardsResponse.Data.Cards.Count > 1)
        {
            _logger.LogError("More than one card found for barcode {barcode}", barcode);
            return;
        }

        var card = getCardsResponse.Data.Cards.Single();

        var photoUrl = card.MediaFiles.FirstOrDefault();
        if (string.IsNullOrEmpty(photoUrl))
        {
            _logger.LogError("PhotoUrl is empty for barcode {barcode}", barcode);
            return;
        }

        var vendorCode = card.VendorCode;
        if (!string.IsNullOrEmpty(vendorCode))
        {
            caption += $" {vendorCode}";
        }
        
        var colour = card.Colors.FirstOrDefault();
        if (!string.IsNullOrEmpty(colour))
        {
            caption += $", {colour}";
        }
        
        var size = card.Sizes.FirstOrDefault(s => s.Skus.Contains(barcode))?.TechSize;
        if (!string.IsNullOrEmpty(size))
        {
            caption += $", {size}";
        }

        try
        {
            await _telegramBotClient.SendPhotoAsync(_chatId, new InputOnlineFile(new Uri(photoUrl)), caption);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while sending telegram message");
        }
    }
}