
(function ()
{
    var audioTagSupport = !!(document.createElement('audio').canPlayType);
    var audio = document.createElement('audio');

    if (!window.tts) window.tts =
    {
        say: function (text, callback)
        {
            if (!audioTagSupport || text == '')
            {
                if (callback)
                    callback();
                return false;
            }

            //var url = 'http://translate.google.com/translate_tts?tl=en&q=' + encodeURIComponent(text);
            var url = '/TTS/Say2?text=' + encodeURIComponent(text);

            var handler = function ()
            {
                audio.removeEventListener('ended', handler);

                if (callback)
                    callback();
            };

            audio.addEventListener('ended', handler);

            audio.setAttribute('src', url);
            audio.load();
            audio.play();
        }
    };
})();
