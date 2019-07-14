namespace GarbageRoyale.Scripts.Localization
{
    [System.Serializable]
    public class LocalizationState
    {
        public string localizationFileName;

        public LocalizationState(string localizationFileName)
        {
            this.localizationFileName = localizationFileName;
        }

        public string LocalizationFileName
        {
            get => localizationFileName;
            set => localizationFileName = value;
        }
    }
}