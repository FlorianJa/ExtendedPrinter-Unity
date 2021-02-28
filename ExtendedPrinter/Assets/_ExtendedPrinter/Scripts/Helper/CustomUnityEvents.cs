using Assets._ExtendedPrinter.Scripts.SlicingService;
using UnityEngine.Events;

namespace Assets._ExtendedPrinter.Scripts.Helper
{
    [System.Serializable]
    public class FileSlicedEvent : UnityEvent<FileSlicedMessageArgs> { }

    [System.Serializable]
    public class StringEvent : UnityEvent<string> { }

    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }

    
}
