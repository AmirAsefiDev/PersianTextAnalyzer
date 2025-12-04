using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PersianTextAnalyzer.Dto;

namespace PersianTextAnalyzer.Controllers;

[Route("api/spaling-check")]
[ApiController]
public class SpalingController : ControllerBase
{
    private readonly string _apiKey = "JIOUEC3RHPHQ78AJ9QEEM42W9NASUWI5";
    private readonly HttpClient _httpClient;

    public SpalingController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    ///     Spaling is the Best approach for problem of finding incorrect words inside the text.
    ///     I tried it,it's exactly find the incorrect word and say alternative for that word.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(List<ResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> SpellCheck()
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
        var finalResponse = new List<ResponseDto>();
        foreach (var text in texts)
        {
            var incorrectWords = new List<IncorrectWordDto>();
            var requestBody = new
            {
                key = _apiKey,
                text,
                lang = "fa",
                session_id = "test-session"
            };
            var json = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
            var apiUrl = "https://api.sapling.ai/api/v1/spellcheck";

            var request = await _httpClient.PostAsync(apiUrl, requestContent);
            var responseContent = await request.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true
            };
            var result = JsonSerializer.Deserialize<SpalingResponse>(responseContent, options);
            foreach (var edit in result.edits)
                incorrectWords.Add(new IncorrectWordDto
                {
                    //Word = text.Substring(edit.start, edit.end - edit.start),
                    Word = edit.sentence.Substring(edit.start, edit.end - edit.start),
                    Suggestion = [edit.replacement]
                });

            finalResponse.Add(new ResponseDto
            {
                CurrentText = text,
                IncorrectWordDto = incorrectWords
            });
        }

        return Ok(finalResponse);
    }
}

internal class ResponseDto
{
    public string CurrentText { get; set; }
    public List<IncorrectWordDto> IncorrectWordDto { get; set; }
}

internal class SpalingEdit
{
    public string id { get; set; }
    public string sentence { get; set; }
    public int sentence_start { get; set; }
    public int start { get; set; }
    public int end { get; set; }
    public string replacement { get; set; }
}

internal class SpalingResponse
{
    public List<SpalingEdit> edits { get; set; }
}