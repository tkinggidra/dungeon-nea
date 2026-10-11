using System;
using UnityEngine;

/*
 * AI-GENERATED IMPLEMENTATION
 *
 * This code was developed with AI assistance to support
 * supplementary features outside the core NEA objectives.
 *
 * It is primarily intended to improve the visual presentation
 * and overall user experience of the project, rather than
 * demonstrate the programming techniques being assessed.
 */

namespace Rendering
{
    [Serializable]
    public sealed class PixelationSettings
    {
        [Tooltip("Low-resolution image height. Smaller values produce larger pixels.")]
        [Range(90, 720)]
        public int ReferenceHeight = 270;

        public bool QuantizeColors = false;

        [Tooltip("Number of levels per RGB channel when colour quantisation is enabled.")]
        [Range(2, 64)]
        public int ColorSteps = 24;
    }
}
