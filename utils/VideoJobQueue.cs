using System.Threading.Channels;
using VideoStreamingBackend.Interface;
using VideoStreamingBackend.Utils;

public class VideoJobQueue : IVideoJobQueue
{
    private readonly Channel<VideoJob>_queue = Channel.CreateUnbounded<VideoJob>();

    public void Enqueue(VideoJob job)
    {
        _queue.Writer.TryWrite(job);
    }

    public async ValueTask<VideoJob> DequeueAsync(CancellationToken token)
    {
        return await _queue.Reader.ReadAsync(token);
    }
}