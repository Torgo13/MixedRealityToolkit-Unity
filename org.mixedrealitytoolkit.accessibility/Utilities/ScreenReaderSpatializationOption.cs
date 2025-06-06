// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause


namespace MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Enumeration defining how screen reader audio should be spatialized.
    /// </summary>
    public enum ScreenReaderSpatializationOption
    {
        /// <summary>
        /// The screen reader will not spatialize the audio. It will be played as a
        /// 'flat stereo' presentation.
        /// </summary>
        None = 0,

        /// <summary>
        /// The screen reader will play audio at the actual location of the object.
        /// </summary>
        Full = 1,

        /// <summary>
        /// The screen reader will localize audio in the direction of the object, at a fixed
        /// distance from the user.
        /// </summary>
        Pseudo = 2
    }
}
