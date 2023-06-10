using System.Collections.Generic;

public class SampleSetStack
{
    private readonly Stack<TileSampleSet> sampleStack;
    private readonly int maxStackSize;

    public SampleSetStack(int maxStackSize)
    {
        this.maxStackSize = maxStackSize;
        sampleStack = new Stack<TileSampleSet>();
    }

    public void AddSample(TileSampleSet sample)
    {
        if (sampleStack.Count >= maxStackSize)
        {
            sampleStack.Pop();
        }
        sampleStack.Push(sample);
    }

    public TileSampleSet GetSample()
    {
        return sampleStack.Pop();
    }

    public TileSampleSet PeekSample()
    {
          return sampleStack.Peek();
    }

    public bool IsEmpty()
    {
        return sampleStack.Count == 0;
    }

    public void Clear()
    {
        sampleStack.Clear();
    }

    public List<TileSampleSet> GetSection(int start, int end)
    {
        List<TileSampleSet> section = new();
        for (int i = start; i < end; i++)
        {
            section.Add(sampleStack.ToArray()[i]);
        }
        return section;
    }
}
