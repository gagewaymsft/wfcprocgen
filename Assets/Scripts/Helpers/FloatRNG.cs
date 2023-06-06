using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Helpers
{
    /// <summary>
    /// Obsolete.<br/>
    /// Use UnityEngine.Random(float minInclusive, float maxInclusive)
    /// </summary>
    public class FloatRNG
    {
        [Obsolete("Obsolete. Use UnityEngine.Random(float minInclusive, float maxInclusive)")]
        public float GetRandomFloatInRange(float min, float max)
        {
            var rng = new Random();
            return (float)(rng.NextDouble() * (max - min) + min);
        }
    }
}
