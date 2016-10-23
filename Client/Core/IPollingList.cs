namespace Uniars.Client.Core
{
    interface IPollingList
    {
        /// <summary>
        /// Load list from server
        /// </summary>
        /// <param name="autoTriggered">If the method is invoked automatically</param>
        void LoadList(bool autoTriggered = false);
    }
}
