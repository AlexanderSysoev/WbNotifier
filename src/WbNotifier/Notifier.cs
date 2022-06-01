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
                Id = Guid.NewGuid().ToString(),
                Jsonrpc = "2.0",
                Params = new GetCardRequestParams
                {
                    Filter = new Sort
                    {
                        Find = new List<Find>
                        {
                            new()
                            {
                                Column = "nomenclatures.variations.barcode",
                                Search = barcode
                            }
                        }
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

        if (!getCardsResponse.Result.Cards.Any())
        {
            _logger.LogError("No cards found for barcode {barcode}", barcode);
            return;
        }
        
        if (getCardsResponse.Result.Cards.Count > 1)
        {
            _logger.LogError("More than one card found for barcode {barcode}", barcode);
            return;
        }

        var card = getCardsResponse.Result.Cards.Single();
        
        string? photoUrl = null;
        string? colour = null;
        string? size = null;

        foreach (var nomenclature in card.Nomenclatures)
        {
            foreach (var var in nomenclature.Variations.Where(var => var.Barcodes.Contains(barcode)))
            {
                photoUrl = nomenclature.Addin
                    .FirstOrDefault(ai => ai.Type.Equals("фото", StringComparison.OrdinalIgnoreCase))?.Params
                    .FirstOrDefault()?.Value;
                colour = nomenclature.Addin
                    .FirstOrDefault(ai => ai.Type.Equals("основной цвет", StringComparison.OrdinalIgnoreCase))?.Params
                    .FirstOrDefault()?.Value;
                size = var.Addin
                    .FirstOrDefault(ai => ai.Type.Equals("рос. размер", StringComparison.OrdinalIgnoreCase))
                    ?.Params.FirstOrDefault()?.Value;
                break;
            }
        }

        if (string.IsNullOrEmpty(photoUrl))
        {
            _logger.LogError("PhotoUrl is empty for barcode {barcode}", barcode);
            return;
        }

        if (!string.IsNullOrEmpty(card.SupplierVendorCode))
        {
            caption += $" {card.SupplierVendorCode}";
        }
        
        if (!string.IsNullOrEmpty(colour))
        {
            caption += $", {colour}";
        }
        
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