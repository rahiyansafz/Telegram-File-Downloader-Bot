using System.Net;

namespace TelegramFileDownloaderBot;
// From https://stackoverflow.com/a/41392145/4213397
internal class ProgressableStreamContent : HttpContent
{
    private const int _defaultBufferSize = 32 * 1024;

    private readonly HttpContent _content;

    private readonly int _bufferSize;

    private readonly IProgress<ProgressData> _progress;

    public ProgressableStreamContent(HttpContent content, IProgress<ProgressData> progress) : this(content,
        _defaultBufferSize, progress)
    {
    }

    public ProgressableStreamContent(HttpContent content, int bufferSize, IProgress<ProgressData> progress)
    {
        if (bufferSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(bufferSize));

        _content = content ?? throw new ArgumentNullException(nameof(content));
        _bufferSize = bufferSize;
        _progress = progress;

        foreach (var h in content.Headers)
            Headers.Add(h.Key, h.Value);
    }

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
        var buffer = new byte[_bufferSize];
        TryComputeLength(out long size);
        var uploaded = 0;

        using var sinput = await _content.ReadAsStreamAsync();
        while (true)
        {
            var length = await sinput.ReadAsync(buffer);
            if (length <= 0)
                break;

            uploaded += length;
            _progress?.Report(new ProgressData(size, uploaded));

            await stream.WriteAsync(buffer.AsMemory(0, length));
        }

        await stream.FlushAsync();
    }

    protected override bool TryComputeLength(out long length)
    {
        length = _content.Headers.ContentLength.GetValueOrDefault();
        return true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _content.Dispose();

        base.Dispose(disposing);
    }
}