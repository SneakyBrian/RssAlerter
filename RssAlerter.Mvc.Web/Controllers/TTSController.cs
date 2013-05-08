using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Speech.Synthesis;
using System.Net;

namespace RssAlerter.Mvc.Web.Controllers
{
    public class TTSController : Controller
    {
        public ActionResult Say(string text)
        {
            var speech = new SpeechSynthesizer();

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                speech.SetOutputToWaveStream(stream);
                speech.Speak(text);
                bytes = stream.ToArray();
            }

            return File(bytes, "audio/x-wav");
        }

        public ActionResult Say2(string text)
        {
            if (text.Length > 100)
                text = text.Substring(0, 100);

            var webClient = new WebClient();

            var bytes = webClient.DownloadData("http://translate.google.com/translate_tts?tl=en&q=" + HttpUtility.UrlEncode(text));

            return File(bytes, "audio/x-mpeg-3");
        }

    }
}
