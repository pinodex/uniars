namespace Uniars.Client.Core
{
    interface IPollingList
    {
        void LoadList(bool autoTriggered = false);
    }
}
