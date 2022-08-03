using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using WebApplication1.Helpers;
using WebApplication1.Model;
using WebApplication1.Models;

namespace WebApplication1.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public Promotion PromotionData { get; set; } = default!;

    public List<SelectListItem> ListPromoType { get; } = new()
    {
        new SelectListItem { Value = "1", Text = "Simple Discount" },
        new SelectListItem { Value = "2", Text = "Complete Discount" }
    };

    public List<SelectListItem> ListValueType { get; } = new()
    {
        new SelectListItem { Value = "1", Text = "Percentage" },
        new SelectListItem { Value = "2", Text = "Amount" }
    };

    public List<Store> ListStore { get; set; } = default!;

    [BindProperty]
    public List<string> ListSelectedStore { get; set; } = default!;

    public IFormFile ItemFile { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        ApiResult result = await ApiHelper.GetApi("https://localhost:7196/generateid");

        if (!result.IsSuccess)
            return Page();

        PromotionData = new()
        {
            Id = result.ResultApi!.Replace("\"", "")!
        };

        //==============================================

        ListStore = await GenerateStore();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ApiResult result;
        string jsonData;

        ModelState.ClearValidationState(nameof(PromotionData));

        ValidateData(PromotionData);

        if (!TryValidateModel(PromotionData, nameof(PromotionData)))
        {
            ListStore = await GenerateStore();

            return Page();
        }

        PromotionData.ListSelectedStore = ListSelectedStore;
        PromotionData.ListItem = ReadUploadItemFile(ItemFile);

        GenerateFile(PromotionData);

        jsonData = JsonHelper.Serialize(PromotionData);
        result = await ApiHelper.PostApi("https://localhost:7196/add", jsonData);

        if (!result.IsSuccess)
        {
            ListStore = await GenerateStore();

            return Page();
        }

        return RedirectToPage("Index");
    }

    public IActionResult OnPostClear()
    {
        return RedirectToPage("Index");
    }

    private static List<string> ReadUploadItemFile(IFormFile uploadFile)
    {
        List<string> lstItem = new();

        using StreamReader reader = new(uploadFile.OpenReadStream());
        while (reader.Peek() >= 0)
        {
            lstItem.Add(reader.ReadLine()!);
        }

        lstItem.RemoveAt(0);

        return lstItem;
    }

    private void ValidateData(Promotion promotion)
    {
        PromotionType promoType = promotion.PromotionType;

        if (promoType.AmountTypeId == 1)
        {
            if (promoType.Amount < 0)
                ModelState.AddModelError("PromotionData.PromotionType.Amount", "Minimum percentage value is 0");
            else if (promoType.Amount > 100)
                ModelState.AddModelError("PromotionData.PromotionType.Amount", "Maximum percentage value is 100");
        }
        else if (promoType.AmountTypeId == 2)
        {
            if (promoType.Amount < 0)
                ModelState.AddModelError("PromotionData.PromotionType.Amount", "Minimum amount value is 0");
            else if (promoType.Amount > 999999999)
                ModelState.AddModelError("PromotionData.PromotionType.Amount", "Maximum amount value is 999.999.999");
        }
    }

    private static async Task<List<Store>> GenerateStore()
    {
        ApiResult result = await ApiHelper.GetApi("https://localhost:7196/getallstore");

        if (!result.IsSuccess)
            return new();

        return JsonHelper.Deserialize<List<Store>>(result.ResultApi!)!;
    }

    private void GenerateFile(Promotion promotion)
    {
        StringBuilder builder = new();
        string promotionType = promotion.PromotionType.Id == 1 ? "S" : "C";
        int promotionAmount = promotion.PromotionType.Amount;
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{promotion.Id}.txt");

        builder.AppendLine($"FHEAD|{promotion.Description}|||;");

        for (int i = 0; i < promotion.ListItem.Count; i++)
        {
            builder.AppendLine($"FITEM|{promotion.ListItem[i]}|{promotionType}|{promotionAmount}|;");
        }

        for (int i = 0; i < promotion.ListSelectedStore.Count; i++)
        {
            builder.AppendLine($"FSTORE|{promotion.ListSelectedStore[i]}|{promotion.StartDate.ToString("yyyyMMdd")}|{promotion.EndDate.ToString("yyyyMMdd")}|;");
        }

        builder.AppendLine($"FTAIL||||");

        System.IO.File.WriteAllText(filePath, builder.ToString());
    }
}