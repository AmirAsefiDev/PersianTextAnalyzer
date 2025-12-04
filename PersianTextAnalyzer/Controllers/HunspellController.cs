using Microsoft.AspNetCore.Mvc;
using NHunspell;
using PersianTextAnalyzer.Dto;

namespace PersianTextAnalyzer.Controllers;

[Route("api/hunspell-check")]
[ApiController]
public class HunspellController : ControllerBase
{
    private static readonly Hunspell _hunspell;

    static HunspellController()
    {
        var affPath = Path.Combine(AppContext.BaseDirectory, "Dictionaries", "fa_IR", "fa_IR.aff");
        var dicPath = Path.Combine(AppContext.BaseDirectory, "Dictionaries", "fa_IR", "fa_IR.dic");
        _hunspell = new Hunspell(affPath, dicPath);
    }

    /// <summary>
    ///     This solution isn't exact for pertain text, but it can spell simple word like "حزف" not "حذفش کردم" this word is
    ///     true but Hunspell get mistake from this
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(List<IncorrectWordDto>), StatusCodes.Status200OK)]
    public ActionResult CheckText()
    {
        var texts = new List<string>
        {
            @"سلام، امروز رفتم بیرون تا کمی هوا بخورم ولی متاسفانه هوا خیلی سردو شده بود و من کوتاهیم نکردم لباس گرم ببرم.
            وقتی رسیدم به پارک، دیدم چند نفر دارن فوتبال بازی میکنند و یه بچه کوچیکم مدام دادو بیداد می کرد.
            من روی نیمکت نشستم تا کتابمو بخونم، ولی فهمیدم که یکی از صفحه گمشش شده و باید دوباره پرینت بگیرم.
            بعد از چند دقیقه، یک آقای مسن اومد و ازم پرسید که آیا اینجوارو بلدم یا نه، چون دنبال ایستگاه اتوبوس بود.
            من هم سعی کردم کمکش کنم، اما چون خودم این محل رو خوب نشناسسیم کمی طول کشید تا مسیر درستو براش پیدا کنم.
            در آخر هم بارون شروع شد و مجبور شدم سریع برگردم خونه، چون چترمو یادمم رفته بود ببرم.",

            @"امروز صبح با عجله از خونه بیرون رفتم و حتی صبحانم رو کامل نخوردم.
            وقتی رسیدم سر کار، فهمیدم که کیفمو جاگزاشتم و مجبور شدم برگردم.
            تو راه برگشت هوا یهو تاریکو سرد شد و بارون هم شروع شد.
            آخرش که رسیدم خونه دیدم موبایلمم روی میز موندهه و با خودم نبردم."
        };

        //Predictable mistakes
        /*First text
        سردو → سرد شده / سرد بود
        کوتاهیم → کوتاهی نکردم
        بازی میکنند → بازی می‌کنند
        دادو → داد و
        گمشش → گمشده / گم شده
        اینجوارو → اینجا رو
        نشناسسیم → نمی‌شناسیم
        یادمم → یادم
        */
        /*Second text
        صبحانم → صبحانه‌ام
        جاگزاشتم → جا گذاشتم
        تاریکو → تاریک و
        موندهه → مونده
        */
        var incorrectWords = new List<IncorrectWordDto>();
        foreach (var text in texts)
        {
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (var index = 0; index < words.Length; index++)
            {
                var word = words[index];
                word = word
                    .Replace("،", "")
                    .Replace(".", "")
                    .Replace("؟", "")
                    .Trim();

                if (!_hunspell.Spell(word))
                    incorrectWords.Add(new IncorrectWordDto
                    {
                        Word = word,
                        Suggestion = _hunspell.Suggest(word)
                    });
            }
        }

        return Ok(incorrectWords);
    }
}