using Microsoft.AspNetCore.Mvc;
using TextAnalysisLibrary;

namespace WebApplicationDD.Controllers
{
    [ApiController]
    [Route("api/wordcount")]
    public class TextAnalysisController : ControllerBase
    {

        [HttpPost(Name = "PostWordsCount")]
        //Вызов функции через контроллер
        public ActionResult<Dictionary<string, int>> GetWordCount([FromBody] string text)
        {
            return TextAnalysis.GetTextAnalysisParallel(text);
        }
    }
}