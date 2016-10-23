using System;
using System.Collections.Generic;
namespace Uniars.Client.Core
{
    interface IPickable<T>
    {
        /// <summary>
        /// Enable picker mode for page
        /// </summary>
        /// <param name="result">Callback</param>
        void EnablePicker(Action<T> result);

        /// <summary>
        /// Enable multi-picker mode for page
        /// </summary>
        /// <param name="result">Callback</param>
        void EnablePicker(Action<List<T>> result);

        /// <summary>
        /// Check if page is in picker mode
        /// </summary>
        /// <returns>true if picker mode is enabled</returns>
        bool IsPickerEnabled();
    }
}
