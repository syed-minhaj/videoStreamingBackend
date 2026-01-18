namespace VideoStreamingBackend.Interface;
using VideoStreamingBackend.Utils;
public interface IVideoJobQueue
{
    void Enqueue(VideoJob job);
    ValueTask<VideoJob> DequeueAsync(CancellationToken token);
}
