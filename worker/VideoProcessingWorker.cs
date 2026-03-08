
using System.Diagnostics;
using VideoStreamingBackend.Interface;
using VideoStreamingBackend.Utils;

public class VideoProccessingWorker : BackgroundService
{
    private readonly IVideoJobQueue _queue;

    public VideoProccessingWorker(IVideoJobQueue queue)
    {
        _queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var job = await _queue.DequeueAsync(stoppingToken);
            await ProcessVideo(job);
        }
    }

    private async Task ProcessVideo(VideoJob job)
    {
        var outputDir = Path.Combine("storage", "videos", job.VideoId);

        Directory.CreateDirectory(outputDir);

        Directory.CreateDirectory(Path.Combine(outputDir, "v0"));
        Directory.CreateDirectory(Path.Combine(outputDir, "v1"));
        Directory.CreateDirectory(Path.Combine(outputDir, "v2"));
        Directory.CreateDirectory(Path.Combine(outputDir, "v3"));

        var ffmpegArgs =
            $"-y -i \"{job.InputPath}\" " +
            "-filter_complex " +
            "\"[0:v]split=4[v0][v1][v2][v3];" +
            "[v0]scale=w=640:h=360[v0out];" +
            "[v1]scale=w=854:h=480[v1out];" +
            "[v2]scale=w=1280:h=720[v2out];" +
            "[v3]scale=w=1920:h=1080[v3out]\" " +
            "-map [v0out] -c:v:0 libx264 -b:v:0 800k " +
            "-map [v1out] -c:v:1 libx264 -b:v:1 1400k " +
            "-map [v2out] -c:v:2 libx264 -b:v:2 2800k " +
            "-map [v3out] -c:v:3 libx264 -b:v:3 5000k " +
            "-map a:0 -c:a aac -b:a:0 96k " +
            "-map a:0 -c:a aac -b:a:1 128k " +
            "-map a:0 -c:a aac -b:a:2 128k " +
            "-map a:0 -c:a aac -b:a:3 192k " +
            "-f hls " +
            "-hls_time 6 " +
            "-hls_playlist_type vod " +
            "-master_pl_name master.m3u8 " +
            "-var_stream_map \"v:0,a:0 v:1,a:1 v:2,a:2 v:3,a:3\" " +
            $"{outputDir}/v%v/playlist.m3u8";


        


        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = ffmpegArgs,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Directory.GetCurrentDirectory()
            }
        };

        process.Start();

        string stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();


        //File.Delete(job.InputPath);
    }



}