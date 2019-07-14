namespace GarbageRoyale.Scripts.Localization
{
    [System.Serializable]
    public class LocalizationData
    {
        private LocalizationItem[] items;
    }

    [System.Serializable]
    public class LocalizationItem
    {
        public string key;
        public string value;
    }
}