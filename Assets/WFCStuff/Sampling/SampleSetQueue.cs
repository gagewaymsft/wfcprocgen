using System.Collections.Generic;

public class SampleSetQueue
{
    private readonly Queue<TileSampleSet> sampleQueue;
    private readonly int maxQueueSize;

    public SampleSetQueue(int maxQueueSize)
    {
        this.maxQueueSize = maxQueueSize;
        sampleQueue = new Queue<TileSampleSet>();
    }

    public void AddSample(TileSampleSet sample)
    {
        if (sampleQueue.Count >= maxQueueSize)
        {
            sampleQueue.Dequeue();
        }
        sampleQueue.Enqueue(sample);
    }

    public TileSampleSet GetSample()
    {
        return sampleQueue.Dequeue();
    }

    public TileSampleSet PeekSample()
    {
          return sampleQueue.Peek();
    }

    public bool IsEmpty()
    {
        return sampleQueue.Count == 0;
    }

    public void Clear()
    {
        sampleQueue.Clear();
    }

    public List<TileSampleSet> GetSection(int start, int end)
    {
        List<TileSampleSet> section = new();
        for (int i = start; i < end; i++)
        {
            section.Add(sampleQueue.ToArray()[i]);
        }
        return section;
    }
}
