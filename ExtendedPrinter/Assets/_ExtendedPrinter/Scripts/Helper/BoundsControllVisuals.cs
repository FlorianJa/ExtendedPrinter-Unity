using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ExtendedPrinter.Scripts.Helper
{
    [Serializable]
    public class BoundsControllVisuals
    {
        [SerializeField]
        [Tooltip("Bounds control box display configuration section.")]
        private BoxDisplayConfiguration boxDisplayConfiguration;
        /// <summary>
        /// Bounds control box display configuration section.
        /// </summary>
        public BoxDisplayConfiguration BoxDisplayConfig
        {
            get => boxDisplayConfiguration;
            set => boxDisplayConfiguration = value;
        }

        [SerializeField]
        [Tooltip("This section defines the links / lines that are drawn between the corners of the control.")]
        private LinksConfiguration linksConfiguration;
        /// <summary>
        /// This section defines the links / lines that are drawn between the corners of the control.
        /// </summary>
        public LinksConfiguration LinksConfig
        {
            get => linksConfiguration;
            set => linksConfiguration = value;
        }

        [SerializeField]
        [Tooltip("Configuration of the scale handles.")]
        private ScaleHandlesConfiguration scaleHandlesConfiguration;
        /// <summary>
        /// Configuration of the scale handles.
        /// </summary>
        public ScaleHandlesConfiguration ScaleHandlesConfig
        {
            get => scaleHandlesConfiguration;
            set => scaleHandlesConfiguration = value;
        }

        [SerializeField]
        [Tooltip("Configuration of the rotation handles.")]
        private RotationHandlesConfiguration rotationHandlesConfiguration;
        /// <summary>
        /// Configuration of the rotation handles.
        /// </summary>
        public RotationHandlesConfiguration RotationHandlesConfig
        {
            get => rotationHandlesConfiguration;
            set => rotationHandlesConfiguration = value;
        }

        [SerializeField]
        [Tooltip("Configuration of the translation handles.")]
        private TranslationHandlesConfiguration translationHandlesConfiguration;
        /// <summary>
        /// Configuration of the translation handles.
        /// </summary>
        public TranslationHandlesConfiguration TranslationHandlesConfig
        {
            get => translationHandlesConfiguration;
            set => translationHandlesConfiguration = value;
        }

        [SerializeField]
        [Tooltip("Configuration for Proximity Effect to scale handles or change materials on proximity.")]
        private ProximityEffectConfiguration handleProximityEffectConfiguration;
        /// <summary>
        /// Configuration for Proximity Effect to scale handles or change materials on proximity.
        /// </summary>
        public ProximityEffectConfiguration HandleProximityEffectConfig
        {
            get => handleProximityEffectConfiguration;
            set => handleProximityEffectConfiguration = value;
        }
    }
}
